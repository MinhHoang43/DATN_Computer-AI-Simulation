using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CameraPreviewUI : MonoBehaviour
{
    public static CameraPreviewUI Instance;

    [Header("UI")]
    public RawImage cameraImage;

    [Header("Texts")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI infoText;

    private int totalCount = 0;
    private int correctCount = 0;
    private int wrongCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowImage(Texture2D image)
    {
        if (cameraImage == null || image == null)
            return;

        cameraImage.texture = image;
    }

    public void ShowWaiting()
    {
        if (statusText != null)
        {
            statusText.text = "Status : Waiting...";
            statusText.color = Color.white;
        }

        if (infoText != null)
        {
            infoText.text =
                "Ground Truth : -\n" +
                "Prediction : -\n" +
                "Confidence : -\n" +
                "Objects : -\n" +
                "Time : -\n" +
                "Result : -\n\n" +
                "Total : " + totalCount + "\n" +
                "Correct : " + correctCount + "\n" +
                "Wrong : " + wrongCount + "\n" +
                "Accuracy : " + GetAccuracyText();
        }
    }

    public void ShowResult(List<PCBYOLOManager.Detection> detections, float inferenceTimeMs)
    {
        ShowResult(detections, inferenceTimeMs, null);
    }

    public void ShowResult(
        List<PCBYOLOManager.Detection> detections,
        float inferenceTimeMs,
        PCBInfo pcbInfo
    )
    {
        if (detections == null)
            return;

        string prediction = "OK";
        float confidence = 1f;
        int objectCount = detections.Count;

        // Quan trọng: UI phải lấy prediction đã được đánh giá từ PCBInfo,
        // không tự suy lại từ detections[0], để tránh lệch giữa text Prediction và Result.
        if (pcbInfo != null && pcbInfo.inspected)
        {
            prediction = pcbInfo.defectClass;
            confidence = pcbInfo.confidence;
        }
        else if (detections.Count > 0)
        {
            PCBYOLOManager.Detection best = GetBestDetection(detections);
            prediction = PCBInfo.ToDisplayClassName(best.className);
            confidence = best.confidence;
        }

        bool? correct = null;

        if (pcbInfo != null && pcbInfo.evaluated)
        {
            correct = pcbInfo.isCorrect;

            totalCount++;

            if (pcbInfo.isCorrect)
                correctCount++;
            else
                wrongCount++;
        }

        if (statusText != null)
        {
            // Status là phán định của camera/YOLO.
            // Prediction = OK          => Status : OK (xanh)
            // Prediction = Missing...  => Status : NG (đỏ)
            bool predictionIsOK = PCBInfo.ClassMatches(prediction, "OK");

            statusText.text = predictionIsOK ? "Status : OK" : "Status : NG";
            statusText.color = predictionIsOK ? Color.green : Color.red;
        }

        if (infoText != null)
        {
            string groundTruth =
                pcbInfo != null && !string.IsNullOrWhiteSpace(pcbInfo.groundTruth)
                    ? pcbInfo.groundTruth
                    : "-";

            string correctText = "-";

            if (correct.HasValue)
                correctText = correct.Value ? "Correct" : "Wrong";

            infoText.text =
                "Ground Truth : " + groundTruth + "\n" +
                "Prediction : " + prediction + "\n" +
                "Confidence : " + (confidence * 100f).ToString("F1") + " %\n" +
                "Objects : " + objectCount + "\n" +
                "Time : " + inferenceTimeMs.ToString("F1") + " ms\n" +
                "Result : " + correctText + "\n\n" +
                "Total : " + totalCount + "\n" +
                "Correct : " + correctCount + "\n" +
                "Wrong : " + wrongCount + "\n" +
                "Accuracy : " + GetAccuracyText();
        }
    }

    private PCBYOLOManager.Detection GetBestDetection(List<PCBYOLOManager.Detection> detections)
    {
        PCBYOLOManager.Detection best = detections[0];

        for (int i = 1; i < detections.Count; i++)
        {
            if (detections[i].confidence > best.confidence)
                best = detections[i];
        }

        return best;
    }

    private string GetAccuracyText()
    {
        if (totalCount <= 0)
            return "-";

        float accuracy = (float)correctCount / totalCount * 100f;
        return accuracy.ToString("F2") + " %";
    }

    public void ResetStatistics()
    {
        totalCount = 0;
        correctCount = 0;
        wrongCount = 0;
        ShowWaiting();
    }
}
