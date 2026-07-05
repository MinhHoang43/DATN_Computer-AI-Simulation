using UnityEngine;

public class PCBSpawner : MonoBehaviour
{
    [Header("PCB Prefab")]
    public GameObject pcbPrefab;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    [Header("Spawn Settings")]
    public float spawnInterval = 8f;
    public bool spawnOnStart = true;

    private float timer = 0f;

    void Start()
    {
        if (spawnOnStart)
        {
            SpawnPCB();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnPCB();
            timer = 0f;
        }
    }

    void SpawnPCB()
    {
        if (pcbPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("PCBSpawner thiếu PCB Prefab hoặc Spawn Point.");
            return;
        }

        GameObject pcb = Instantiate(pcbPrefab, spawnPoint.position, spawnPoint.rotation);

        PCBTextureChanger changer = pcb.GetComponent<PCBTextureChanger>();
        PCBInfo pcbInfo = pcb.GetComponent<PCBInfo>();

        if (changer != null && PCBTextureDatabase.Instance != null)
        {
            Texture2D randomTexture = PCBTextureDatabase.Instance.GetRandomTexture();
            changer.SetTexture(randomTexture);

            string groundTruth = PCBTextureDatabase.Instance.GetClassName(randomTexture);

            if (pcbInfo != null)
            {
                pcbInfo.SetGroundTruth(groundTruth);
            }
            else
            {
                Debug.LogWarning("PCB mới spawn thiếu PCBInfo.");
            }

            if (randomTexture != null)
            {
                Debug.Log(
                    "Spawn PCB texture: " +
                    randomTexture.name +
                    " | Ground Truth: " +
                    groundTruth
                );
            }
        }
        else
        {
            Debug.LogWarning("Thiếu PCBTextureChanger hoặc PCBTextureDatabase.");
        }
    }
}
