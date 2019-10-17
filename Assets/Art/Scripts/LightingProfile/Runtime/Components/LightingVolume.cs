using System.Collections;
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
        public AnimationCurve timeCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float blendDist = 0f;
        public float priority = 0f;
        public float blend = 0;
        public LightingProfile profile;

        public BoxCollider boxCol;
        public BoxCollider blendCol;

        private void Start()
        {
            boxCol = GetComponent<BoxCollider>();
            boxCol.isTrigger = true;

            CreateBlendCollider();
        }

        private void CreateBlendCollider()
        {
            if (isGlobal) return;

            blendCol = gameObject.AddComponent<BoxCollider>();
            Vector3 scale = transform.lossyScale;
            blendCol.size = new Vector3(GetFactor(scale.x, boxCol.size.x, blendDist), GetFactor(scale.y, boxCol.size.y, blendDist), GetFactor(scale.z, boxCol.size.z, blendDist));
        }

        private float GetFactor(float lossyScale, float colScale, float blend)
        {
            return (lossyScale * colScale + blend * 2 ) / lossyScale;
        }

        void OnEnable()
        {
            StartCoroutine(Register());
        }

        private IEnumerator Register()
        {
            yield return new WaitForEndOfFrame();
            try { LightingLayer.Instance.Register(this); }
            catch { ShowLightingLayerError(); }
        }

        void OnDisable()
        {
            try { LightingLayer.Instance.Unregister(this); }
            catch { ShowLightingLayerError(); }
        }

        private void ShowLightingLayerError()
        {
            Debug.LogWarning("No Lighting Layer detected in the scene. Please ensure you have at least one gameobject with a Lighting Layer component.");
        }

        void OnDrawGizmos()
        {
            if(isGlobal) return;

            Gizmos.color = new Color(1, 1, 0, 0.5f);
            var scale = transform.lossyScale;
            var invScale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, scale);

            boxCol = GetComponent<BoxCollider>();
            Gizmos.DrawCube(boxCol.center, boxCol.size);
            Gizmos.DrawWireCube(boxCol.center, boxCol.size + invScale * blendDist * 2f);
        }
    }
}
