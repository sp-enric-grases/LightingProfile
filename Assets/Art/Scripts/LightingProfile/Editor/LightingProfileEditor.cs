using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace SocialPoint.Art.LightingProfiles
{
    public delegate void CopySettings();
    public delegate void PasteSettings();

    [CustomEditor(typeof(LightingProfile))]
    public class LightingProfileEditor : LayoutInspector
    {
        private readonly Color separator = new Color(0.35f, 0.35f, 0.35f);

        private bool foldSectionEnvironment = true;
        private bool foldSectionLighting = true;
        private bool foldSectionOtherSettings = true;
        
        private LightingProfile lp;
        private SerializedProperty skybox;
        private SerializedProperty sourceLighting;
        private SerializedProperty defaultReflectionMode;
        private SerializedProperty customReflection;
        private SerializedProperty reflectionResolution;
        private SerializedProperty fogMode;

        private void OnEnable()
        {
            lp = (LightingProfile)target;
            skybox = serializedObject.FindProperty("skybox");
            sourceLighting = serializedObject.FindProperty("sourceLighting");
            defaultReflectionMode = serializedObject.FindProperty("defaultReflectionMode");
            customReflection = serializedObject.FindProperty("customReflection");
            reflectionResolution = serializedObject.FindProperty("reflectionResolution");
            fogMode = serializedObject.FindProperty("fogMode");
        }

        public override void OnInspectorGUI()
        {
            Title("Lighting Profile Settings");

            DrawSeparator(EditorGUIUtility.isProSkin ? separator : Color.black);
            FoldSection("Environment", SectionEnvironment, ref foldSectionEnvironment);
            DrawSeparator(EditorGUIUtility.isProSkin ? separator : Color.black);
            FoldSection("Mixed Lighting", SectionLighting, ref foldSectionLighting);
            DrawSeparator(EditorGUIUtility.isProSkin ? separator : Color.black);
            FoldSection("Other Settings", SectionOtherSettings, ref foldSectionOtherSettings);
            DrawSeparator(EditorGUIUtility.isProSkin ? separator : Color.black);
            SectionApplySettings();

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void SectionEnvironment()
        {
            EditorGUILayout.PropertyField(skybox);
            SectionEnvironmentLighting();
            SectionEnvironmentReflections();
            SectionButtons(lp.CopyEnvironmentSettings, lp.PasteEnvironmentSettings);
        }

        private void SectionEnvironmentLighting()
        {
            GUILayout.Space(6);
            GUILayout.Label("Environment Lighting");
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(sourceLighting, new GUIContent("Source"));

                if (lp.sourceLighting == LightingProfile.SourceLighting.Gradient)
                {
                    lp.ambientSkyColor = EditorGUILayout.ColorField(new GUIContent("Sky Color"), lp.ambientSkyColor, true, true, true);
                    lp.ambientEquatorColor = EditorGUILayout.ColorField(new GUIContent("Equator Color"), lp.ambientEquatorColor, true, true, true);
                    lp.ambientGroundColor = EditorGUILayout.ColorField(new GUIContent("Ground Color"), lp.ambientGroundColor, true, true, true);
                }
                else if (lp.sourceLighting == LightingProfile.SourceLighting.Skybox)
                {
                    lp.ambientIntensity = EditorGUILayout.Slider(new GUIContent("Intensity Multiplier"), lp.ambientIntensity, 0, 8);
                }
                else if (lp.sourceLighting == LightingProfile.SourceLighting.Color)
                {
                    lp.ambientSkyColor = EditorGUILayout.ColorField(new GUIContent("Ambient Color"), lp.ambientSkyColor, true, true, true);
                }

                EditorGUI.indentLevel--;
            }
        }

        private void SectionEnvironmentReflections()
        {
            GUILayout.Space(6);
            GUILayout.Label("Environment Reflections");
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(defaultReflectionMode, new GUIContent("Source"));

                if (lp.defaultReflectionMode == DefaultReflectionMode.Custom)
                {
                    EditorGUILayout.PropertyField(customReflection, new GUIContent("Cubemmap"));
                }
                else
                {
                    EditorGUILayout.PropertyField(reflectionResolution, new GUIContent("Resolution"));
                    lp.defaultReflectionResolution = (int)lp.reflectionResolution;
                }

                lp.reflectionIntensity = EditorGUILayout.Slider(new GUIContent("Intensity Multiplier"), lp.reflectionIntensity, 0, 1);
                lp.reflectionBounces = EditorGUILayout.IntSlider(new GUIContent("Bounces"), lp.reflectionBounces, 1, 5);

                EditorGUI.indentLevel--;
            }
        }

        private void SectionLighting()
        {
            lp.subtractiveShadowColor = EditorGUILayout.ColorField("Realtime Shadow Color", lp.subtractiveShadowColor);
            SectionButtons(lp.CopyLightingSettings, lp.PasteLightingSettings);
        }

        private void SectionOtherSettings()
        {
            lp.fog = EditorGUILayout.Toggle("Fog", lp.fog);
            if (lp.fog) SectionFog();
            SectionHalo();
            SectionButtons(lp.CopyOtherSettings, lp.PasteOtherSettings);
        }

        private void SectionFog()
        {
            EditorGUI.indentLevel++;
            {
                lp.fogColor = EditorGUILayout.ColorField("Color", lp.fogColor);
                EditorGUILayout.PropertyField(fogMode, new GUIContent("Mode"));
                if (lp.fogMode == FogMode.Linear)
                {
                    lp.fogStartDistance = EditorGUILayout.FloatField("Start", lp.fogStartDistance);
                    lp.fogEndDistance = EditorGUILayout.FloatField("End", lp.fogEndDistance);
                }
                else
                {
                    lp.fogDensity = Mathf.Clamp01(EditorGUILayout.FloatField("Density", lp.fogDensity));
                }
            }
            EditorGUI.indentLevel--;
            GUILayout.Space(8);
        }

        private void SectionHalo()
        {
            lp.haloStrength = EditorGUILayout.Slider(new GUIContent("Halo Strength"), lp.haloStrength, 0, 1);
            lp.flareFadeSpeed = EditorGUILayout.FloatField("Flare Fade Speed", lp.flareFadeSpeed);
            lp.flareStrength = EditorGUILayout.Slider(new GUIContent("Flare Strength"), lp.flareStrength, 0, 1);
        }

        private void SectionButtons(CopySettings OnCopySettings, PasteSettings OnPasteSettings)
        {
            GUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Copy Settings", GUILayout.Height(24)))
                {
                    OnCopySettings();
                }
                GUILayout.Space(12);
                if (GUILayout.Button("Paste Settings", GUILayout.Height(24)))
                {
                    OnPasteSettings();
                }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void SectionApplySettings()
        {
            GUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Copy FROM current scene)", GUILayout.Height(40), GUILayout.MinWidth(180)))
            {
                Undo.RecordObject(lp, "Copy lighting from scene to profile");
                lp.CopyFromCurrentScene();
                EditorUtility.SetDirty(lp);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                // QGM: I think is not necessary.
                //serializedObject.Update();
            }

            if (GUILayout.Button("Apply TO current scene", GUILayout.Height(40), GUILayout.MinWidth(180)))
            {
                // Unity leaves us no other way than to do this dirty trick.
                Object renderSettings = Object.FindObjectOfType(typeof(RenderSettings));
                Undo.RecordObject(renderSettings, "Apply lighting from profile to scene");
                lp.Apply();
                // QGM: I think is not necessary.
                //serializedObject.Update();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            // TODO: automatize the process of preserving the Unity-generated bake assets.
            EditorGUILayout.HelpBox("IMPORTANT: Unity removes the baked assets when re-baking. Make sure you keep copies of those assets or they could get deleted unintentionally!", MessageType.Warning);

            if (RenderSettings.sun != null)
            {
                EditorGUILayout.HelpBox("The Profile will not preserve the reference to the Sun object, because the object may not exist when the profile is applied in a different scene. Consider not setting that reference or implement a workaround for it", MessageType.Warning);
            }
        }
    }
}


