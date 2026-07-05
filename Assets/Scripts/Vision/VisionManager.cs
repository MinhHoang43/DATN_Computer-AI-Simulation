using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class VisionManager : MonoBehaviour
{
    public static VisionManager Instance;

    [Header("Inspection")]
    public Transform inspectionPoint;
    public float stopDistance = 0.05f;

    [Header("Timing")]
    public float settleTime = 0.5f;
    public float inspectionTime = 1.5f;

    [Header("Capture")]
    public CameraCapture cameraCapture;

    private bool isInspecting = false;

    private void Awake()
    {
        Instance = this;
    }

    public bool StartInspection(PCBMove pcb)
    {
        if (isInspecting || pcb == null)
            return false;

        StartCoroutine(ProcessPCB(pcb));
        return true;
    }

    private IEnumerator ProcessPCB(PCBMove pcb)
    {
        isInspecting = true;

        pcb.StopMove();
        UnityEngine.Debug.Log("PCB dừng tại vị trí kiểm tra.");

        yield return new WaitForSeconds(settleTime);

        Texture2D image = null;

        if (cameraCapture != null)
        {
            image = cameraCapture.Capture();
            UnityEngine.Debug.Log("Camera đã chụp ảnh PCB.");
        }
        else
        {
            UnityEngine.Debug.LogWarning("VisionManager thiếu CameraCapture.");
        }

        if (image != null && PCBYOLOManager.Instance != null)
        {
            PCBInfo info = pcb.GetComponent<PCBInfo>();

            Stopwatch sw = Stopwatch.StartNew();
            List<PCBYOLOManager.Detection> detections = PCBYOLOManager.Instance.Detect(image);
            sw.Stop();

            float inferenceTimeMs = sw.ElapsedMilliseconds;

            Texture2D displayImage = image;
            if (detections.Count > 0)
            {
                displayImage = BoundingBoxRenderer.DrawBoxes(image, detections);
            }

            PCBYOLOManager.Detection bestDetection = GetBestDetection(detections);

            if (info != null)
            {
                if (bestDetection == null)
                {
                    // YOLO không phát hiện lỗi nào => Prediction = OK.
                    // Nếu Ground Truth không phải OK thì EvaluatePrediction() sẽ trả Wrong.
                    info.SetPrediction("OK", 1f, false);
                    UnityEngine.Debug.Log("PCBInfo -> OK / No detection");
                }
                else
                {
                    info.SetPrediction(bestDetection.className, bestDetection.confidence, true);
                    UnityEngine.Debug.Log(
                        "PCBInfo -> " + info.defectClass +
                        " | Conf = " + info.confidence.ToString("F2")
                    );
                }

                info.EvaluatePrediction();

                UnityEngine.Debug.Log(
                    "GROUND_TRUTH_VS_PREDICTION | Ground Truth: " +
                    info.groundTruth +
                    " | Prediction: " +
                    info.defectClass +
                    " | Correct: " +
                    info.isCorrect
                );
            }

            if (detections.Count == 0)
            {
                UnityEngine.Debug.Log("Kết quả YOLO: OK / No object detected");
            }
            else
            {
                UnityEngine.Debug.Log("Kết quả YOLO: NG / Object detected");

                foreach (var d in detections)
                {
                    UnityEngine.Debug.Log(
                        PCBInfo.ToDisplayClassName(d.className) +
                        " | Conf: " +
                        (d.confidence * 100f).ToString("F1") +
                        "%" +
                        " | Box: " +
                        d.box
                    );
                }
            }

            if (CameraPreviewUI.Instance != null)
            {
                CameraPreviewUI.Instance.ShowImage(displayImage);
                CameraPreviewUI.Instance.ShowResult(detections, inferenceTimeMs, info);
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("Không thể chạy YOLO.");
        }

        yield return new WaitForSeconds(inspectionTime);

        pcb.MarkInspected();
        pcb.ResumeMove();

        UnityEngine.Debug.Log("PCB kiểm tra xong, chạy tiếp.");

        isInspecting = false;
    }

    private PCBYOLOManager.Detection GetBestDetection(List<PCBYOLOManager.Detection> detections)
    {
        if (detections == null || detections.Count == 0)
            return null;

        PCBYOLOManager.Detection best = detections[0];

        for (int i = 1; i < detections.Count; i++)
        {
            if (detections[i].confidence > best.confidence)
                best = detections[i];
        }

        return best;
    }
}
