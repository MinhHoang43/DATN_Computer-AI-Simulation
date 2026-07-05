using System.Collections.Generic;
using UnityEngine;

public class PCBTextureDatabase : MonoBehaviour
{
    public static PCBTextureDatabase Instance;

    [Header("Resources Settings")]
    [Tooltip("Root folder inside Assets/Resources.")]
    public string resourcesRoot = "PCBImages";

    [Tooltip("Folder names under Resources/PCBImages. These names are used as Ground Truth class names.")]
    public string[] classFolders =
    {
        "OK",
        "MissingUSB",
        "MissingCP2102",
        "MissingESP32",
        "MissingCapacitor",
        "MissingResistor"
    };

    [Header("Loaded PCB Textures")]
    public Texture2D[] allTextures;

    private readonly Dictionary<Texture2D, string> textureToClass =
        new Dictionary<Texture2D, string>();

    private void Awake()
    {
        Instance = this;
        LoadTextures();
    }

    private void LoadTextures()
    {
        textureToClass.Clear();

        List<Texture2D> loadedTextures = new List<Texture2D>();
        HashSet<Texture2D> uniqueTextures = new HashSet<Texture2D>();

        foreach (string classFolder in classFolders)
        {
            if (string.IsNullOrWhiteSpace(classFolder))
                continue;

            LoadTexturesFromClassFolder(
                classFolder.Trim(),
                loadedTextures,
                uniqueTextures
            );
        }

        allTextures = loadedTextures.ToArray();

        Debug.Log("Loaded PCB textures total: " + allTextures.Length);

        if (allTextures.Length == 0)
        {
            Debug.LogWarning(
                "Không tìm thấy ảnh PCB. Kiểm tra lại thư mục: Assets/Resources/" +
                resourcesRoot +
                "/<ClassName>"
            );
        }
    }

    private void LoadTexturesFromClassFolder(
        string classFolder,
        List<Texture2D> loadedTextures,
        HashSet<Texture2D> uniqueTextures
    )
    {
        string[] candidateFolders = GetCandidateFolders(classFolder);
        bool loadedThisClass = false;

        foreach (string candidateFolder in candidateFolders)
        {
            string folderPath = resourcesRoot + "/" + candidateFolder;
            Texture2D[] texturesInClass = Resources.LoadAll<Texture2D>(folderPath);

            if (texturesInClass == null || texturesInClass.Length == 0)
                continue;

            string groundTruthClass = NormalizeGroundTruthName(candidateFolder);

            foreach (Texture2D texture in texturesInClass)
            {
                if (texture == null)
                    continue;

                if (uniqueTextures.Contains(texture))
                    continue;

                uniqueTextures.Add(texture);
                loadedTextures.Add(texture);

                if (!textureToClass.ContainsKey(texture))
                    textureToClass.Add(texture, groundTruthClass);
            }

            loadedThisClass = true;

            Debug.Log(
                "Loaded " +
                texturesInClass.Length +
                " PCB textures from: Assets/Resources/" +
                folderPath +
                " | Ground Truth: " +
                groundTruthClass
            );

            // Nếu đã load được bằng một tên folder thì dừng để tránh trùng ảnh
            // trong trường hợp project có nhiều biến thể tên folder.
            break;
        }

        if (!loadedThisClass)
        {
            Debug.LogWarning(
                "Không load được ảnh cho class/folder: " +
                classFolder +
                ". Đã thử các tên folder: " +
                string.Join(", ", candidateFolders)
            );
        }
    }

    private string[] GetCandidateFolders(string classFolder)
    {
        List<string> candidates = new List<string>();

        string trimmed = classFolder.Trim();
        string compact = RemoveSeparators(trimmed);

        AddCandidate(candidates, trimmed);
        AddCandidate(candidates, compact);
        AddCandidate(candidates, trimmed.Replace(" ", "_"));
        AddCandidate(candidates, trimmed.Replace("_", " "));
        AddCandidate(candidates, trimmed.Replace("-", "_"));
        AddCandidate(candidates, trimmed.Replace("-", " "));

        // Hỗ trợ cả 3 kiểu tên folder:
        // MissingUSB, Missing_USB, Missing USB
        if (compact.StartsWith("Missing") && compact.Length > "Missing".Length)
        {
            string suffix = compact.Substring("Missing".Length);
            AddCandidate(candidates, "Missing" + suffix);
            AddCandidate(candidates, "Missing_" + suffix);
            AddCandidate(candidates, "Missing " + suffix);
        }

        return candidates.ToArray();
    }

    private void AddCandidate(List<string> candidates, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        value = value.Trim();

        if (!candidates.Contains(value))
            candidates.Add(value);
    }

    private string RemoveSeparators(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return value
            .Trim()
            .Replace("_", "")
            .Replace(" ", "")
            .Replace("-", "");
    }

    private string NormalizeGroundTruthName(string value)
    {
        return PCBInfo.ToDisplayClassName(value);
    }

    public Texture2D GetRandomTexture()
    {
        if (allTextures == null || allTextures.Length == 0)
        {
            Debug.LogWarning("Không tìm thấy ảnh PCB trong Resources/" + resourcesRoot + ".");
            return null;
        }

        int index = Random.Range(0, allTextures.Length);
        return allTextures[index];
    }

    public string GetClassName(Texture2D texture)
    {
        if (texture == null)
            return "Unknown";

        if (textureToClass.TryGetValue(texture, out string className))
            return className;

        return "Unknown";
    }
}
