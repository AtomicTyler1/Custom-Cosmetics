using System.Linq;
using UnityEngine;

namespace Custom_Cosmetics.Helpers
{
    public static class CreateCostumeType
    {
        public static PlayerCostumeManager.Costume CreateCostume(string name, ref GameObject? head, ref GameObject? body, ref GameObject? face)
        {
            PlayerCostumeManager.Part? headPart = null;
            if (head != null)
                headPart = new PlayerCostumeManager.Part
                {
                    frameObjects = new[] { head },
                    partType = PlayerCostumeManager.PartType.CostumeHead
                };

            PlayerCostumeManager.Part? bodyPart = null;
            if (body != null)
                bodyPart = new PlayerCostumeManager.Part
                {
                    frameObjects = new[] { body },
                    partType = PlayerCostumeManager.PartType.CostumeBody
                };

            PlayerCostumeManager.Part? facePart = null;
            if (face != null)
                facePart = new PlayerCostumeManager.Part
                {
                    frameObjects = new[] { face },
                    partType = PlayerCostumeManager.PartType.CostumeFace
                };

            var expression = new PlayerCostumeManager.Expression
            {
                expressionClip = PlayerCostumeManager.ExpressionClip.Idle,
                parts = new[] { headPart, bodyPart, facePart }
                    .Where(p => p != null)
                    .ToArray()!
            };

            CostumeObject costumeObj = ScriptableObject.CreateInstance<CostumeObject>();
            costumeObj.name = name.ToUpper();
            costumeObj.costumeName = name;
            costumeObj.startsUnlocked = true;

            var globalData = Aggro.Core.GlobalScriptableObject<CosmeticGlobalData>.instance;
            if (globalData != null && globalData.costumes != null && globalData.costumes.Length > 0)
            {
                costumeObj.costumeTextures = globalData.costumes[0].costumeTextures;
            }

            Plugin.AddToGlobalRegistry(costumeObj);

            return new PlayerCostumeManager.Costume
            {
                costumeObject = costumeObj,
                expressions = new[] { expression },
                name = $"custom-costume-{name}"
            };
        }
    }
}