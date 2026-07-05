using System.IO;
using UnityEngine;

public class CameraCapture : MonoBehaviour
{
    [Header("Camera")]
    public Camera visionCamera;

    [Header("Capture Size")]
    public int imageWidth = 640;
    public int imageHeight = 640;

    [Header("Dataset Save Mode")]
    public bool saveDataset = true;

    // Thay đường dẫn này thành thư mục bạn muốn
    public string saveFolder = @"D:\3d unity\picture";

    private int captureIndex = 0;

    public Texture2D Capture()
    {
        if (visionCamera == null)
        {
            Debug.LogWarning("CameraCapture thiếu VisionCamera.");
            return null;
        }

        RenderTexture renderTexture = new RenderTexture(imageWidth, imageHeight, 24);
        Texture2D texture = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);

        visionCamera.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;

        visionCamera.Render();

        texture.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        texture.Apply();

        visionCamera.targetTexture = null;
        RenderTexture.active = null;

        renderTexture.Release();
        Destroy(renderTexture);

        if (saveDataset)
        {
            SaveTexture(texture);
        }

        return texture;
    }

    void SaveTexture(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();

        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }

        string fileName = "PCB_Capture_" + captureIndex.ToString("0000") + ".png";

        string filePath = Path.Combine(saveFolder, fileName);

        File.WriteAllBytes(filePath, bytes);

        captureIndex++;

        Debug.Log("Đã lưu ảnh: " + filePath);
    }
}