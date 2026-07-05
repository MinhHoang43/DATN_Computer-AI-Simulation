using System.Collections.Generic;
using UnityEngine;

public static class BoundingBoxRenderer
{
    public static Texture2D DrawBoxes(Texture2D source, List<PCBYOLOManager.Detection> detections)
    {
        Texture2D result = Object.Instantiate(source);

        if (detections == null || detections.Count == 0)
        {
            result.Apply();
            return result;
        }

        foreach (var det in detections)
        {
            DrawRect(result, det.box, Color.red, 3);
        }

        result.Apply();
        return result;
    }

    private static void DrawRect(Texture2D tex, Rect rect, Color color, int thickness)
    {
        int xMin = Mathf.Clamp(Mathf.RoundToInt(rect.xMin), 0, tex.width - 1);
        int xMax = Mathf.Clamp(Mathf.RoundToInt(rect.xMax), 0, tex.width - 1);

        int yMin = Mathf.Clamp(Mathf.RoundToInt(tex.height - rect.yMax), 0, tex.height - 1);
        int yMax = Mathf.Clamp(Mathf.RoundToInt(tex.height - rect.yMin), 0, tex.height - 1);

        for (int t = 0; t < thickness; t++)
        {
            for (int x = xMin; x <= xMax; x++)
            {
                tex.SetPixel(x, Mathf.Clamp(yMin + t, 0, tex.height - 1), color);
                tex.SetPixel(x, Mathf.Clamp(yMax - t, 0, tex.height - 1), color);
            }

            for (int y = yMin; y <= yMax; y++)
            {
                tex.SetPixel(Mathf.Clamp(xMin + t, 0, tex.width - 1), y, color);
                tex.SetPixel(Mathf.Clamp(xMax - t, 0, tex.width - 1), y, color);
            }
        }
    }
}