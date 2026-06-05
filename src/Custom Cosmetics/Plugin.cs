using Aggro.Core;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Custom_Cosmetics.Helpers;
using HarmonyLib;
using Steamworks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Custom_Cosmetics
{
    [BepInAutoPlugin]
    public partial class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log { get; private set; } = null!;
        public const string CosmeticExtension = ".cosmetic";
        private Harmony harmony = null!;

        public static List<CustomCostumeData> customCostumes { get; private set; } = new();

        internal static ConfigEntry<string> SavedCostumeName { get; private set; } = null!;
        public static ConfigEntry<bool> CosmeticDevTools;

        public static ConfigEntry<float> SnapStepPos;
        public static ConfigEntry<float> SnapStepRot;

        public static CosmeticDevUI devUI;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo($"Plugin {Name} is loaded!");

            CosmeticDevTools = Config.Bind("Devtools", "Cosmetic Dev Tools", false, "Brings up UI in the cosmetic selector to reposition ");
            SnapStepPos = Config.Bind("Devtools", "Position Snap Step", 0.001f, "How much to move per click.");
            SnapStepRot = Config.Bind("Devtools", "Rotation Snap Step", 1.0f, "How many degrees to rotate per click.");

            SavedCostumeName = Config.Bind("Persistence", "LastCostumeName", "",
                "Internal name of the last selected costume. You do not need to set this.");

            harmony = new Harmony(Id);
            harmony.PatchAll();

            GameObject toolObj = new GameObject("CosmeticDevUI");
            devUI = toolObj.AddComponent<CosmeticDevUI>();
            toolObj.hideFlags = HideFlags.HideAndDontSave;

            LoadAllCosmetics();
        }

        private void LoadAllCosmetics()
        {
            string pluginsFolder = Paths.PluginPath;

            string[] allFiles = Directory.GetFiles(pluginsFolder, "*" + CosmeticExtension, SearchOption.AllDirectories);

            if (allFiles.Length == 0)
            {
                Log.LogInfo("No .cosmetic files found in plugins folder.");
                return;
            }
            System.Array.Sort(allFiles);

            foreach (string filePath in allFiles)
            {
                TryLoadCosmeticBundle(filePath);
            }
        }

        private void TryLoadCosmeticBundle(string filePath)
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
            if (bundle == null) return;

            TextAsset metadataAsset = bundle.LoadAsset<TextAsset>("temp_metadata");

            if (metadataAsset == null)
            {
                Log.LogWarning($"Bundle '{Path.GetFileName(filePath)}' has no metadata. Export it with the new builder.");
                bundle.Unload(true);
                return;
            }

            try
            {
                var settings = JsonUtility.FromJson<CosmeticDefinition.CosmeticSettings>(metadataAsset.text);
                var data = new CustomCostumeData(settings, bundle);
                customCostumes.Add(data);
                Log.LogInfo($"Successfully registered: {data.Name}");
            }
            catch (System.Exception e)
            {
                Log.LogError($"Failed to parse cosmetic metadata: {e.Message}");
                bundle.Unload(true);
            }
        }

        public static void AddToGlobalRegistry(CostumeObject customObject)
        {
            var globalData = GlobalScriptableObject<CosmeticGlobalData>.instance;
            if (globalData == null) return;

            if (!globalData.costumes.Contains(customObject))
            {
                var list = globalData.costumes.ToList();
                list.Add(customObject);
                globalData.costumes = list.ToArray();
            }
        }
    }
}