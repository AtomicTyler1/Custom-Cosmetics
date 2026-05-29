using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

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

            var costume = __instance.costumeManager.costumes[index];
            bool isCustom = costume.name.StartsWith("custom-costume-");

            if (isCustom)
            {
                __instance.NetworkcurrentCostumeID = index;
                __instance.costumeManager.currentCostumeID = index;
                __instance.costumeManager.UpdateCostume();

                Plugin.SavedCostumeName.Value = costume.name;

                return false;
            }

            Plugin.SavedCostumeName.Value = costume.name;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerCostumeManagerNetwork), nameof(PlayerCostumeManagerNetwork.InitializeCostumeFromSaveData))]
        public static bool SkipIfCustomCostumeSaved(PlayerCostumeManagerNetwork __instance)
        {
            string savedName = Plugin.SavedCostumeName.Value;
            if (string.IsNullOrEmpty(savedName) || !savedName.StartsWith("custom-costume-"))
                return true;

            for (int i = 0; i < __instance.costumeManager.costumes.Length; i++)
            {
                if (__instance.costumeManager.costumes[i].name == savedName)
                {
                    __instance.NetworkcurrentCostumeID = i;
                    __instance.costumeManager.currentCostumeID = i;
                    return false;
                }
            }

            return true;
        }
    }
}
