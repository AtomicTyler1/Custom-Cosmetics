using UnityEngine;

namespace Custom_Cosmetics
{
    public class CustomCostumeData
    {
        public string Name { get; }

        public GameObject? HeadObject { get; }
        public GameObject? BodyObject { get; }
        public GameObject? FaceObject { get; }

        public Vector3 HeadPosition { get; }
        public Quaternion HeadRotation { get; }
        public Vector3 HeadScale { get; }

        public Vector3 BodyPosition { get; }
        public Quaternion BodyRotation { get; }
        public Vector3 BodyScale { get; }

        public Vector3 FacePosition { get; }
        public Quaternion FaceRotation { get; }
        public Vector3 FaceScale { get; }

        public CustomCostumeData(CosmeticDefinition.CosmeticSettings settings, AssetBundle bundle)
        {
            Name = settings.id;

            if (!string.IsNullOrEmpty(settings.headName))
                HeadObject = bundle.LoadAsset<GameObject>(settings.headName);

            if (!string.IsNullOrEmpty(settings.bodyName))
                BodyObject = bundle.LoadAsset<GameObject>(settings.bodyName);

            if (!string.IsNullOrEmpty(settings.faceName))
                FaceObject = bundle.LoadAsset<GameObject>(settings.faceName);

            HeadPosition = settings.headPos;
            HeadRotation = Quaternion.Euler(settings.headRot);
            HeadScale = settings.headScale;

            BodyPosition = settings.bodyPos;
            BodyRotation = Quaternion.Euler(settings.bodyRot);
            BodyScale = settings.bodyScale;

            FacePosition = settings.facePos;
            FaceRotation = Quaternion.Euler(settings.faceRot);
            FaceScale = settings.faceScale;
        }
    }
}