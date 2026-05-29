using Custom_Cosmetics.Helpers;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace Custom_Cosmetics.Patches
{
    [HarmonyPatch]
    public static class PlaceCostumeOntoPlayer
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerCostumeManager), nameof(PlayerCostumeManager.OnEntityCreated))]
        public static void UpdateCostumePrefix(PlayerCostumeManager __instance)
        {
            var headParent = __instance.costumes[1].expressions[0].parts[0].frameObjects[0].transform.parent.parent;
            var bodyParent = __instance.costumes[1].expressions[0].parts[0].frameObjects[0].transform.parent.parent.parent.parent.parent.Find("COSTUME-BODY");
            var faceParent = __instance.costumes[1].expressions[0].parts[0].frameObjects[0].transform.parent.parent.parent.Find("COSTUME-FACE");

            foreach (var costume in Plugin.customCostumes)
            {
                string costumeId = costume.Name;

                bool alreadyAdded = __instance.costumes.Any(c => c.name == "custom-costume-" + costumeId);
                if (alreadyAdded) continue;

                GameObject? headInstance = AttachPart(
                    costume.HeadObject, headParent,
                    $"custom-costume-{costumeId}-head",
                    costume.HeadPosition, costume.HeadRotation, costume.HeadScale);

                GameObject? bodyInstance = AttachPart(
                    costume.BodyObject, bodyParent,
                    $"custom-costume-{costumeId}-body",
                    costume.BodyPosition, costume.BodyRotation, costume.BodyScale);

                GameObject? faceInstance = AttachPart(
                    costume.FaceObject, faceParent,
                    $"custom-costume-{costumeId}-face",
                    costume.FacePosition, costume.FaceRotation, costume.FaceScale);

                var builtCostume = CreateCostumeType.CreateCostume(
                    costumeId,
                    ref headInstance,
                    ref bodyInstance,
                    ref faceInstance);

                AddCostumeToArray.AddCostume(__instance, builtCostume);

                headInstance?.SetActive(false);
                bodyInstance?.SetActive(false);
                faceInstance?.SetActive(false);
            }

            __instance.SetUpUnlockedIndicies();

            var indices = __instance.unlockedCostumeIndicies.ToList();
            for (int i = 0; i < __instance.costumes.Length; i++)
            {
                if (!indices.Contains(i) && __instance.costumes[i].name.StartsWith("custom-costume-"))
                    indices.Add(i);
            }
            __instance.unlockedCostumeIndicies = indices.ToArray();
        }

        private static GameObject? AttachPart(GameObject? prefab, Transform? parent, string instanceName, Vector3 localPos, Quaternion localRot, Vector3 localScale)
        {
            if (prefab == null || parent == null) return null;

            Transform? existing = parent.Find(instanceName);
            if (existing != null) return existing.gameObject;

            GameObject instance = GameObject.Instantiate(prefab, parent);
            instance.name = instanceName;

            Shader gameShader = Shader.Find("shader-main");

            if (gameShader != null)
            {
                Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in renderers)
                {
                    foreach (Material mat in renderer.materials)
                    {
                        mat.shader = gameShader;
                    }
                }
            }
            else
            {
                Plugin.Log.LogWarning("Could not find shader 'shader-main'.");
            }

            instance.transform.localPosition = localPos;
            instance.transform.localRotation = localRot;
            instance.transform.localScale = localScale;

            return instance;
        }
    }
}