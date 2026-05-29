using UnityEngine;

namespace Custom_Cosmetics
{
    [CreateAssetMenu(fileName = "NewCosmetic", menuName = "Custom Cosmetics/Cosmetic Definition")]
    public class CosmeticDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string cosmeticId = "my-cosmetic";

        [Header("Head")]
        public GameObject headPrefab;
        public Vector3 headPosition = Vector3.zero;
        public Vector3 headRotation = Vector3.zero;
        public Vector3 headScale = Vector3.one;

        [Header("Body")]
        public GameObject bodyPrefab;
        public Vector3 bodyPosition = Vector3.zero;
        public Vector3 bodyRotation = Vector3.zero;
        public Vector3 bodyScale = Vector3.one;

        [Header("Face")]
        public GameObject facePrefab;
        public Vector3 facePosition = Vector3.zero;
        public Vector3 faceRotation = Vector3.zero;
        public Vector3 faceScale = Vector3.one;

        [System.Serializable]
        public class CosmeticSettings
        {
            public string id;
            public string headName, bodyName, faceName;
            public Vector3 headPos, headRot, headScale;
            public Vector3 bodyPos, bodyRot, bodyScale;
            public Vector3 facePos, faceRot, faceScale;
        }

        public CosmeticSettings ToSettings()
        {
            return new CosmeticSettings
            {
                id = cosmeticId,
                headName = headPrefab ? headPrefab.name : "",
                headPos = headPosition,
                headRot = headRotation,
                headScale = headScale,
                bodyName = bodyPrefab ? bodyPrefab.name : "",
                bodyPos = bodyPosition,
                bodyRot = bodyRotation,
                bodyScale = bodyScale,
                faceName = facePrefab ? facePrefab.name : "",
                facePos = facePosition,
                faceRot = faceRotation,
                faceScale = faceScale
            };
        }
    }
}