using UnityEngine;

public class PCBInfo : MonoBehaviour
{
    [Header("Inspection Result")]
    public bool inspected = false;
    public bool isNG = false;

    [Tooltip("Prediction result returned by YOLO.")]
    public string defectClass = "Unknown";

    [Range(0, 1)]
    public float confidence = 0f;

    [Header("Ground Truth Evaluation")]
    [Tooltip("True class of this PCB, assigned when the PCB is spawned from a Resources folder.")]
    public string groundTruth = "Unknown";

    [Tooltip("True after YOLO prediction has been compared with Ground Truth.")]
    public bool evaluated = false;

    [Tooltip("True if Ground Truth and YOLO prediction match.")]
    public bool isCorrect = false;

    public void SetGroundTruth(string className)
    {
        groundTruth = ToDisplayClassName(className);

        // Reset state for each newly spawned PCB.
        inspected = false;
        isNG = false;
        defectClass = "Unknown";
        confidence = 0f;
        evaluated = false;
        isCorrect = false;
    }

    public void SetPrediction(string className, float predictionConfidence, bool predictionIsNG)
    {
        defectClass = ToDisplayClassName(className);
        confidence = Mathf.Clamp01(predictionConfidence);
        isNG = predictionIsNG;
        inspected = true;
    }

    public void EvaluatePrediction()
    {
        evaluated = true;

        string gtKey = NormalizeClassName(groundTruth);
        string predKey = NormalizeClassName(defectClass);

        if (string.IsNullOrEmpty(gtKey) || gtKey == "unknown")
        {
            isCorrect = false;
        }
        else
        {
            isCorrect = gtKey == predKey;
        }

        Debug.Log(
            "EVALUATE | Ground Truth = " + groundTruth +
            " | Prediction = " + defectClass +
            " | GT Key = " + gtKey +
            " | Pred Key = " + predKey +
            " | Correct = " + isCorrect
        );
    }

    public static bool ClassMatches(string a, string b)
    {
        return NormalizeClassName(a) == NormalizeClassName(b);
    }

    public static string ToDisplayClassName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Unknown";

        string key = NormalizeClassName(value);

        switch (key)
        {
            case "ok":
            case "pcbok":
                return "OK";

            case "missingusb":
                return "Missing_USB";

            case "missingcp2102":
                return "Missing_CP2102";

            case "missingesp32":
                return "Missing_ESP32";

            case "missingcapacitor":
                return "Missing_Capacitor";

            case "missingresistor":
                return "Missing_Resistor";

            default:
                return value.Trim().Replace(" ", "_").Replace("-", "_");
        }
    }

    public static string NormalizeClassName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        string normalized = "";

        foreach (char c in value.Trim().ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(c))
                normalized += c;
        }

        return normalized;
    }
}
