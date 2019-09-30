using UnityEngine;
using UnityEngine.Rendering;

namespace SocialPoint.Art.LightingProfiles
{
    [CreateAssetMenu(fileName = "LIGHT_newLightingProfile.asset", menuName = "Social Point/Art/Lighting Profile", order = -10)]
    public class LightingProfile : ScriptableObject
    {
        // The next enumerator coincides with the Unity.Rendering.AmbientMode enumerator since Unity doesn't provide us of a enumerator to set the Environment Lighting Source
        // The values are Skybox = 0, Trilight = 1, Flat = 3
        public enum SourceLighting { Skybox = 0, Gradient = 1, Color = 3 }
        public enum PowerOfTwo { _16, _32, _64, _128, _256, _512, _1024, _2048, _4096 }
        public Material skybox;
        public SourceLighting sourceLighting = SourceLighting.Skybox;
        public Color ambientSkyColor;
        public Color ambientEquatorColor;
        public Color ambientGroundColor;
        public float ambientIntensity;
        public DefaultReflectionMode defaultReflectionMode = DefaultReflectionMode.Skybox;
        public int reflectionBounces;
        public float reflectionIntensity;
        public Cubemap customReflection;
        public int defaultReflectionResolution;
        public PowerOfTwo reflectionResolution = PowerOfTwo._256;
        public Color subtractiveShadowColor;
        public bool fog;
        public FogMode fogMode = FogMode.Linear;
        public Color fogColor;
        public float fogDensity;
        public float fogStartDistance;
        public float fogEndDistance;
        public float flareFadeSpeed;
        public float flareStrength;
        public float haloStrength;

        #region TEMPORAL VARIABLES TO COPY & PASTE BETWEEN PROFILES
        static private Material t_skybox;
        static private SourceLighting t_sourceLighting;
        static private Color t_ambientSkyColor;
        static private Color t_ambientEquatorColor;
        static private Color t_ambientGroundColor;
        static private float t_ambientIntensity;
        static private DefaultReflectionMode t_defaultReflectionMode;
        static private int t_reflectionBounces;
        static private float t_reflectionIntensity;
        static private Cubemap t_customReflection;
        static private int t_defaultReflectionResolution;
        static private PowerOfTwo t_reflectionResolution;
        static private Color t_subtractiveShadowColor;
        static private bool t_fog;
        static private FogMode t_fogMode;
        static private Color t_fogColor;
        static private float t_fogDensity;
        static private float t_fogStartDistance;
        static private float t_fogEndDistance;
        static private float t_flareFadeSpeed;
        static private float t_flareStrength;
        static private float t_haloStrength;
        #endregion

        public void ApplyRenderSettings()
        {
            RenderSettings.subtractiveShadowColor = subtractiveShadowColor;
            RenderSettings.skybox = skybox;
            RenderSettings.reflectionIntensity = reflectionIntensity;
            RenderSettings.reflectionBounces = reflectionBounces;
            RenderSettings.haloStrength = haloStrength;
            RenderSettings.fogStartDistance = fogStartDistance;
            RenderSettings.fogMode = fogMode;
            RenderSettings.fogEndDistance = fogEndDistance;
            RenderSettings.fogDensity = fogDensity;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fog = fog;
            RenderSettings.flareStrength = flareStrength;
            RenderSettings.flareFadeSpeed = flareFadeSpeed;
            RenderSettings.defaultReflectionResolution = defaultReflectionResolution;
            RenderSettings.defaultReflectionMode = defaultReflectionMode;
            RenderSettings.customReflection = customReflection;
            RenderSettings.ambientSkyColor = ambientSkyColor;
            RenderSettings.ambientMode = (AmbientMode)sourceLighting;
            RenderSettings.ambientIntensity = ambientIntensity;
            RenderSettings.ambientGroundColor = ambientGroundColor;
            RenderSettings.ambientEquatorColor = ambientEquatorColor;
        }

        public void CopyFromCurrentScene()
        {
            ambientEquatorColor = RenderSettings.ambientEquatorColor;
            ambientGroundColor = RenderSettings.ambientGroundColor;
            ambientIntensity = RenderSettings.ambientIntensity;
            sourceLighting = (SourceLighting)RenderSettings.ambientMode;
            ambientSkyColor = RenderSettings.ambientSkyColor;
            customReflection = RenderSettings.customReflection;
            defaultReflectionMode = RenderSettings.defaultReflectionMode;
            defaultReflectionResolution = RenderSettings.defaultReflectionResolution;
            flareFadeSpeed = RenderSettings.flareFadeSpeed;
            flareStrength = RenderSettings.flareStrength;
            fog = RenderSettings.fog;
            fogColor = RenderSettings.fogColor;
            fogDensity = RenderSettings.fogDensity;
            fogEndDistance = RenderSettings.fogEndDistance;
            fogMode = RenderSettings.fogMode;
            fogStartDistance = RenderSettings.fogStartDistance;
            haloStrength = RenderSettings.haloStrength;
            reflectionBounces = RenderSettings.reflectionBounces;
            reflectionIntensity = RenderSettings.reflectionIntensity;
            skybox = RenderSettings.skybox;
            subtractiveShadowColor = RenderSettings.subtractiveShadowColor;
        }

        #region COPY & PASTE BETWEEN PROFILES
        public void CopyEnvironmentSettings()
        {
            t_skybox = skybox;
            t_sourceLighting = sourceLighting;
            t_ambientSkyColor = ambientSkyColor;
            t_ambientEquatorColor = ambientEquatorColor;
            t_ambientGroundColor = ambientGroundColor;
            t_ambientIntensity = ambientIntensity;
            t_defaultReflectionMode = defaultReflectionMode;
            t_defaultReflectionResolution = defaultReflectionResolution;
            t_customReflection = customReflection;
            t_reflectionIntensity = reflectionIntensity;
            t_reflectionBounces = reflectionBounces;
        }

        public void PasteEnvironmentSettings()
        {
            skybox = t_skybox;
            sourceLighting = t_sourceLighting;
            ambientSkyColor = t_ambientSkyColor;
            ambientEquatorColor = t_ambientEquatorColor;
            ambientGroundColor = t_ambientGroundColor;
            ambientIntensity = t_ambientIntensity;
            defaultReflectionMode = t_defaultReflectionMode;
            defaultReflectionResolution = t_defaultReflectionResolution;
            customReflection = t_customReflection;
            reflectionIntensity = t_reflectionIntensity;
            reflectionBounces = t_reflectionBounces;
        }

        public void CopyLightingSettings()
        {
            t_subtractiveShadowColor = subtractiveShadowColor;
        }

        public void PasteLightingSettings()
        {
            subtractiveShadowColor = t_subtractiveShadowColor;
        }

        public void CopyOtherSettings()
        {
            t_haloStrength = haloStrength;
            t_fogStartDistance = fogStartDistance;
            t_fogMode = fogMode;
            t_fogEndDistance = fogEndDistance;
            t_fogDensity = fogDensity;
            t_fogColor = fogColor;
            t_fog = fog;
            t_flareStrength = flareStrength;
            t_flareFadeSpeed = flareFadeSpeed;
        }

        public void PasteOtherSettings()
        {
            haloStrength = t_haloStrength;
            fogStartDistance = t_fogStartDistance;
            fogMode = t_fogMode;
            fogEndDistance = t_fogEndDistance;
            fogDensity = t_fogDensity;
            fogColor = t_fogColor;
            fog = t_fog;
            flareStrength = t_flareStrength;
            flareFadeSpeed = t_flareFadeSpeed;
        }
        #endregion

        public void Lerp(LightingProfile current, LightingProfile source, float blend, bool switchSkybox, bool useEnvLighting, bool useEnvReflection, bool useMixedLighting, bool useFog, bool useHalo)
        {
            if (switchSkybox)       SetSkybox(current, source);
            if (useEnvLighting)     SetEnvironmentLighting(current, source, blend);
            if (useEnvReflection)   SetEnvironmentReflections(current, source, blend);
            if (useFog)             SetFog(current, source, blend);
            if (useHalo)            SetHalo(current, source, blend);
            if (useMixedLighting)   SetMixedLighting(current, source, blend);
        }

        private void SetSkybox(LightingProfile current, LightingProfile source)
        {
            if (skybox != source.skybox)
            {
                skybox = source.skybox;
                Debug.Log("Changing skybox...");
            }
        }

        private void SetEnvironmentLighting(LightingProfile current, LightingProfile source, float blend)
        {
            if (sourceLighting != source.sourceLighting)
            {
                sourceLighting = source.sourceLighting;
                Debug.Log("Changing Source Environment Lighting...");
            }

            ambientEquatorColor = Color.Lerp(current.ambientEquatorColor, source.ambientEquatorColor, blend);
            ambientGroundColor = Color.Lerp(current.ambientGroundColor, source.ambientGroundColor, blend);
            ambientIntensity = Mathf.Lerp(current.ambientIntensity, source.ambientIntensity, blend);
            ambientSkyColor = Color.Lerp(current.ambientSkyColor, source.ambientSkyColor, blend);
        }

        private void SetEnvironmentReflections(LightingProfile current, LightingProfile source, float blend)
        {
            if (customReflection != source.customReflection) customReflection = source.customReflection;
            if (defaultReflectionMode != source.defaultReflectionMode) defaultReflectionMode = source.defaultReflectionMode;
            if (defaultReflectionResolution != source.defaultReflectionResolution) defaultReflectionResolution = source.defaultReflectionResolution;
            if (reflectionBounces != source.reflectionBounces) reflectionBounces = source.reflectionBounces;

            reflectionIntensity = Mathf.Lerp(current.reflectionIntensity, source.reflectionIntensity, blend);
        }

        private void SetFog(LightingProfile current, LightingProfile source, float blend)
        {
            fog = source.fog;
            fogMode = source.fogMode;
            fogColor = Color.Lerp(current.fogColor, source.fogColor, blend);
            fogDensity = Mathf.Lerp(current.fogDensity, source.fogDensity, blend);
            fogEndDistance = Mathf.Lerp(current.fogEndDistance, source.fogEndDistance, blend);
            fogStartDistance = Mathf.Lerp(current.fogStartDistance, source.fogStartDistance, blend);
        }

        private void SetMixedLighting(LightingProfile current, LightingProfile source, float blend)
        {
            subtractiveShadowColor = Color.Lerp(current.subtractiveShadowColor, source.subtractiveShadowColor, blend);
        }

        private void SetHalo(LightingProfile current, LightingProfile source, float blend)
        {
            flareFadeSpeed = Mathf.Lerp(current.flareFadeSpeed, source.flareFadeSpeed, blend);
            flareStrength = Mathf.Lerp(current.flareStrength, source.flareStrength, blend);
            haloStrength = Mathf.Lerp(current.haloStrength, source.haloStrength, blend);
        }
    }
}
