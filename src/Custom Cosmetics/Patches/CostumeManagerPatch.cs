using HarmonyLib;

namespace Custom_Cosmetics.Patches
{
    [HarmonyPatch]
    public static class CostumeManagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerCostumeManager), nameof(PlayerCostumeManager.OnEntityCreated))]
        public static void RestoreSavedCostume(PlayerCostumeManager __instance)
        {
            string savedName = Plugin.SavedCostumeName.Value;
            if (string.IsNullOrEmpty(savedName)) return;

            for (int i = 0; i < __instance.costumes.Length; i++)
            {
                if (__instance.costumes[i].name == savedName)
                {
                    __instance.currentCostumeID = i;
                    return;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerCostumeManager), nameof(PlayerCostumeManager.GetIndexFromCostumeObject))]
        public static void FallbackToNameMatch(PlayerCostumeManager __instance, CostumeObject costumeObject, ref int __result)
        {
            if (__result != -1) return;
            if (costumeObject == null) return;

            for (int i = 0; i < __instance.costumes.Length; i++)
            {
                if (__instance.costumes[i].costumeObject != null &&
                    __instance.costumes[i].costumeObject.name == costumeObject.name)
                {
                    __result = i;
                    return;
                }
            }
        }
    }
}