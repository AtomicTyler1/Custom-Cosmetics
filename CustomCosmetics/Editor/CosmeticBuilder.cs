#if UNITY_EDITOR
using Custom_Cosmetics;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CosmeticDefinition))]
public class CosmeticBuilder : Editor
{
    public override void OnInspectorGUI()
    {
        CosmeticDefinition def = (CosmeticDefinition)target;

        DrawDefaultInspector();
        EditorGUILayout.Space(10);

        if (GUILayout.Button("↺  Fetch Prefab Transforms", GUILayout.Height(25)))
        {
            FetchTransforms(def);
        }

        EditorGUILayout.Space(5);

        if (GUILayout.Button("▶  Build Cosmetic", GUILayout.Height(36)))
        {
            BuildCosmetic(def);
        }
    }

    private void FetchTransforms(CosmeticDefinition def)
    {
        Undo.RecordObject(def, "Fetch Prefab Transforms");

        if (def.headPrefab)
        {
            def.headPosition = def.headPrefab.transform.localPosition;
            def.headRotation = def.headPrefab.transform.localEulerAngles;
            def.headScale = def.headPrefab.transform.localScale;
        }

        if (def.bodyPrefab)
        {
            def.bodyPosition = def.bodyPrefab.transform.localPosition;
            def.bodyRotation = def.bodyPrefab.transform.localEulerAngles;
            def.bodyScale = def.bodyPrefab.transform.localScale;
        }

        if (def.facePrefab)
        {
            def.facePosition = def.facePrefab.transform.localPosition;
            def.faceRotation = def.facePrefab.transform.localEulerAngles;
            def.faceScale = def.facePrefab.transform.localScale;
        }

        EditorUtility.SetDirty(def);
        Debug.Log("Cosmetic Definition updated with prefab transform data.");
    }

    private static void BuildCosmetic(CosmeticDefinition def)
    {
        string bundleName = def.cosmeticId.ToLower();

        string json = JsonUtility.ToJson(def.ToSettings(), true);
        string jsonPath = "Assets/temp_metadata.json";
        File.WriteAllText(jsonPath, json);
        AssetDatabase.ImportAsset(jsonPath);

        AssetImporter.GetAtPath(jsonPath).SetAssetBundleNameAndVariant(bundleName, "");
        TagAsset(def.headPrefab, bundleName);
        TagAsset(def.bodyPrefab, bundleName);
        TagAsset(def.facePrefab, bundleName);

        string tempDir = Path.Combine(Application.temporaryCachePath, "CosmeticBuild");
        if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        Directory.CreateDirectory(tempDir);

        BuildPipeline.BuildAssetBundles(tempDir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

        string builtBundle = Path.Combine(tempDir, bundleName);
        string destination = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), $"{bundleName}.cosmetic");
        if (File.Exists(builtBundle)) File.Copy(builtBundle, destination, true);

        AssetDatabase.DeleteAsset(jsonPath);
        UntagAsset(def.headPrefab);
        UntagAsset(def.bodyPrefab);
        UntagAsset(def.facePrefab);

        EditorUtility.DisplayDialog("Done!", $"Saved to Desktop as {bundleName}.cosmetic", "OK");
    }

    private static void TagAsset(GameObject obj, string tag)
    {
        if (!obj) return;
        AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(obj)).SetAssetBundleNameAndVariant(tag, "");
    }

    private static void UntagAsset(GameObject obj)
    {
        if (!obj) return;
        AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(obj)).SetAssetBundleNameAndVariant("", "");
    }
}
#endif