using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using System.Linq;

namespace Custom_Cosmetics.Patches
{
    [HarmonyPatch]
    public static class PlayerCostumeManagerNetworkPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerCostumeManagerNetwork), nameof(PlayerCostumeManagerNetwork.SetCostumeIndex))]
        public static bool SetCostumeIndexPrefix(PlayerCostumeManagerNetwork __instance, int index)
        {
            if (index < 0 || index >= __instance.costumeManager.costumes.Length)
            {
                Plugin.Log.LogWarning($"SetCostumeIndex called with out-of-range index {index}, ignoring.");
                return false;
            }

            EnsureUnlockedIndices(__instance.costumeManager);

            var costume = __instance.costumeManager.costumes[index];
            bool isCustom = costume.name.StartsWith("custom-costume-");

            if (isCustom)
            {
                __instance.NetworkcurrentCostumeID = index;
                __instance.costumeManager.currentCostumeID = index;
                __instance.costumeManager.UpdateCostume();

                Plugin.SavedCostumeName.Value = costume.name;

                return true;
            }

            Plugin.SavedCostumeName.Value = costume.name;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerCostumeManagerNetwork), nameof(PlayerCostumeManagerNetwork.InitializeCostumeFromSaveData))]
        public static bool SkipIfCustomCostumeSaved(PlayerCostumeManagerNetwork __instance)
        {
            EnsureUnlockedIndices(__instance.costumeManager);

            string savedName = Plugin.SavedCostumeName.Value;
            if (string.IsNullOrEmpty(savedName) || !savedName.StartsWith("custom-costume-"))
                return true;

            for (int i = 0; i < __instance.costumeManager.costumes.Length; i++)
            {
                if (__instance.costumeManager.costumes[i].name == savedName)
                {
                    __instance.NetworkcurrentCostumeID = i;
                    __instance.costumeManager.currentCostumeID = i;

                    // Allow vanilla method to run so it finishes setting up internal UI/network states safely
                    return true;
                }
            }

            return true;
        }

        // Helper method to make sure the manager is fully aware of custom indexes
        private static void EnsureUnlockedIndices(PlayerCostumeManager manager)
        {
            if (manager == null || manager.costumes == null) return;

            var indices = manager.unlockedCostumeIndicies?.ToList() ?? new List<int>();
            bool changed = false;

            for (int i = 0; i < manager.costumes.Length; i++)
            {
                if (manager.costumes[i].name != null && manager.costumes[i].name.StartsWith("custom-costume-"))
                {
                    if (!indices.Contains(i))
                    {
                        indices.Add(i);
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                manager.unlockedCostumeIndicies = indices.ToArray();
            }
        }
    }
}