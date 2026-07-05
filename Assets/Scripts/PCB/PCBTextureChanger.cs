using UnityEngine;

public class PCBTextureChanger : MonoBehaviour
{
    private MeshRenderer textureRenderer;

    void Awake()
    {
        Transform texObj = transform.Find("PCB_Texture");

        if (texObj == null)
        {
            Debug.LogError("Không tìm thấy object con PCB_Texture.");
            return;
        }

        textureRenderer = texObj.GetComponent<MeshRenderer>();

        if (textureRenderer == null)
        {
            Debug.LogError("PCB_Texture không có MeshRenderer.");
        }
    }

    public void SetTexture(Texture2D texture)
    {
        if (textureRenderer == null || texture == null)
            return;

        textureRenderer.material.mainTexture = texture;

        Debug.Log("Đã đổi texture thành: " + texture.name);
    }
}