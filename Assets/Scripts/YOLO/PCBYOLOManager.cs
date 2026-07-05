using System.Collections.Generic;
using UnityEngine;
using Unity.InferenceEngine;

public class PCBYOLOManager : MonoBehaviour
{
    public static PCBYOLOManager Instance;

    [Header("YOLO Model")]
    public ModelAsset modelAsset;

    [Header("Labels")]
    public TextAsset classesAsset;

    [Header("Threshold")]
    [Range(0f, 1f)] public float confidenceThreshold = 0.5f;
    [Range(0f, 1f)] public float iouThreshold = 0.5f;

    const BackendType backend = BackendType.GPUCompute;
    const int imageWidth = 640;
    const int imageHeight = 640;

    private Worker worker;
    private Tensor<float> centersToCorners;
    private string[] labels;

    public class Detection
    {
        public string className;
        public float confidence;
        public Rect box;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadLabels();
        LoadModel();
    }

    private void LoadLabels()
    {
        labels = classesAsset.text.Split(
            new[] { '\n', '\r' },
            System.StringSplitOptions.RemoveEmptyEntries
        );

        Debug.Log("Loaded labels: " + labels.Length);
    }

    private void LoadModel()
    {
        var model = ModelLoader.Load(modelAsset);

        centersToCorners = new Tensor<float>(
            new TensorShape(4, 4),
            new float[]
            {
                1, 0, 1, 0,
                0, 1, 0, 1,
                -0.5f, 0, 0.5f, 0,
                0, -0.5f, 0, 0.5f
            }
        );

        var graph = new FunctionalGraph();
        var inputs = graph.AddInputs(model);
        var output = Functional.Forward(model, inputs)[0];

        var boxCoords = output[0, 0..4, ..].Transpose(0, 1);
        var allScores = output[0, 4.., ..];

        var scores = Functional.ReduceMax(allScores, 0);
        var classIDs = Functional.ArgMax(allScores, 0);

        var boxCorners = Functional.MatMul(boxCoords, Functional.Constant(centersToCorners));
        var indices = Functional.NMS(boxCorners, scores, iouThreshold, confidenceThreshold);

        var finalBoxes = Functional.IndexSelect(boxCoords, 0, indices);
        var finalClasses = Functional.IndexSelect(classIDs, 0, indices);
        var finalScores = Functional.IndexSelect(scores, 0, indices);

        worker = new Worker(
            graph.Compile(finalBoxes, finalClasses, finalScores),
            backend
        );

        Debug.Log("YOLO model loaded and NMS graph created.");
    }

    public List<Detection> Detect(Texture2D image)
    {
        List<Detection> detections = new List<Detection>();

        RenderTexture rt = new RenderTexture(imageWidth, imageHeight, 0);
        Graphics.Blit(image, rt);

        using Tensor<float> inputTensor =
            new Tensor<float>(new TensorShape(1, 3, imageHeight, imageWidth));

        TextureConverter.ToTensor(rt, inputTensor, default);

        worker.Schedule(inputTensor);

        using var boxes = (worker.PeekOutput("output_0") as Tensor<float>).ReadbackAndClone();
        using var classIDs = (worker.PeekOutput("output_1") as Tensor<int>).ReadbackAndClone();
        using var scores = (worker.PeekOutput("output_2") as Tensor<float>).ReadbackAndClone();

        int count = boxes.shape[0];

        for (int i = 0; i < count; i++)
        {
            float cx = boxes[i, 0];
            float cy = boxes[i, 1];
            float w = boxes[i, 2];
            float h = boxes[i, 3];

            int classId = classIDs[i];
            float conf = scores[i];

            string className = "Unknown";

            if (classId >= 0 && classId < labels.Length)
                className = labels[classId].Trim();

            Rect rect = new Rect(
                cx - w / 2f,
                cy - h / 2f,
                w,
                h
            );

            detections.Add(new Detection
            {
                className = className,
                confidence = conf,
                box = rect
            });
        }
        RenderTexture.active = null;
        rt.Release();
        Destroy(rt);

        Debug.Log("YOLO detections: " + detections.Count);

        return detections;
    }

    private void OnDestroy()
    {
        worker?.Dispose();
        centersToCorners?.Dispose();
    }
}