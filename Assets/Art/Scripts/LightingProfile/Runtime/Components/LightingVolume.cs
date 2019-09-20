using UnityEngine;

namespace SocialPoint.Art.LightingProfiles
{
    [AddComponentMenu("Rendering/Lighting Volume", -10)]
    [RequireComponent(typeof(BoxCollider))]
    [DisallowMultipleComponent]
    public class LightingVolume : MonoBehaviour
    {
        public bool isGlobal = false;
        public float timeToBlend = 1;
        //public AnimationCurve timeCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float blendDistance = 0f;
        public float priority = 0f;
        public LightingProfile profile;

        //public LightingProfile instance
        //{
        //    get
        //    {
        //        if(internalProfile == null)
        //        {
        //            if(profile != null)
        //                internalProfile = ScriptableObject.Instantiate<LightingProfile>(profile);
        //            else
        //                internalProfile = ScriptableObject.CreateInstance<LightingProfile>();
        //        }

        //        return internalProfile;
        //    }
        //    set
        //    {
        //        internalProfile = value;
        //    }
        //}
        //private bool isRegistered = false;

        //internal LightingProfile profileRef
        //{
        //    get
        //    {
        //        return internalProfile == null ? profile : internalProfile;
        //    }
        //}

        //private int previousLayer;
        //private float previousPriority;
        public BoxCollider boxCollider;
        public BoxCollider blendCollider;
        private LightingProfile internalProfile;

        private void Start()
        {
            //LightingLayer.Instance.Register(this);
            //isRegistered = true;
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;

            CreateBlendCollider();
        }

        private void CreateBlendCollider()
        {
            if (isGlobal) return;
            blendCollider = gameObject.AddComponent<BoxCollider>();
            blendCollider.size = new Vector3(GetFactor(transform.lossyScale.x, blendDistance), GetFactor(transform.lossyScale.y, blendDistance), GetFactor(transform.lossyScale.z, blendDistance));
        }

        private float GetFactor(float lossyScale, float blend)
        {
            return (lossyScale + blend * 2) / lossyScale;
        }

        public bool HasInstantiatedProfile()
        {
            return internalProfile != null;
        }
        
        void OnEnable()
        {
            //if (!isRegistered)
                LightingLayer.Instance.Register(this);
            //previousLayer = gameObject.layer;
            //isRegistered = false;
        }

        void OnDisable()
        {
            LightingLayer.Instance.Unregister(this);
        }
        
        void Update()
        {
            //if(priority != previousPriority)
            //{
            //    LightingBlendingManager.instance.SetDirty();
            //    previousPriority = priority;
            //}
        }

        void OnDrawGizmos()
        {
            if(isGlobal) return;

            Gizmos.color = new Color(1, 1, 0, 0.5f);
            var scale = transform.lossyScale;
            var invScale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, scale);

            boxCollider = GetComponent<BoxCollider>();
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size + invScale * blendDistance * 2f);
        }
    }
}
