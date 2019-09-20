using UnityEngine;
using UnityEditor;

namespace SocialPoint.Art.LightingProfiles
{
    [CustomEditor(typeof(LightingLayer))]
    public class LightingLayerEditor : LayoutInspector
    {
        private LightingLayer ll;
        private GUIContent content;
        private float labelWidth;

        private void OnEnable()
        {
            ll = (LightingLayer)target;
            labelWidth = EditorGUIUtility.labelWidth;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            GUILayout.Space(2);
            Section("Lighting Blending Settings", SectionBlendings);
            Section("Optimization", SectionOptimization);
            Section("Status", SectionStatus);
            if (ll.volumes.Count > 0)
                Section("Volumes", SectionVolumes);

            if (EditorGUI.EndChangeCheck())
                Undo.RegisterCompleteObjectUndo(ll, "Lighting Layer");

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed) EditorUtility.SetDirty(ll);
        }

        private void SectionBlendings()
        {
            ll.blendAllSettings = EditorGUILayout.Toggle("Blend All Settings", ll.blendAllSettings);

            if (ll.blendAllSettings == true)
            {
                EditorGUILayout.HelpBox("Uncheck this box if you want to blend effects separately. Blendings settings separately can help you increase performance.", MessageType.Info);
            }
            else
            {
                EditorGUI.indentLevel++;
                GUILayout.Space(2);
                EditorGUIUtility.labelWidth = 250;

                content = new GUIContent("Switch Skybox", "Two different skyboxes cannot be blended. They will be switched to the one with more priority.");
                ll.switchSkybox = EditorGUILayout.Toggle(content, ll.switchSkybox);

                content = new GUIContent("Interpolate Environment Lighting", "Check this box if you want to interpolate the environment lighting.");
                ll.useEnvLighting = EditorGUILayout.Toggle(content, ll.useEnvLighting);

                content = new GUIContent("Interpolate Environment Reflections", "Check this box if you want to interpolate the environment reflections.");
                ll.useEnvReflection = EditorGUILayout.Toggle(content, ll.useEnvReflection);

                content = new GUIContent("Interpolate Mixed Lighting", "Check this box if you want to interpolate the mixed lighting.");
                ll.useMixedLighting = EditorGUILayout.Toggle(content, ll.useMixedLighting);

                content = new GUIContent("Interpolate Fog Settings", "Check this box if you want to interpolate the fog settings.");
                ll.useFog = EditorGUILayout.Toggle(content, ll.useFog);

                content = new GUIContent("Interpolate Halo Settings", "Check this box if you want to interpolate the halo settings.");
                ll.useHalo = EditorGUILayout.Toggle(content, ll.useHalo);

                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUI.indentLevel--;
            }
        }

        private void SectionOptimization()
        {
            content = new GUIContent("Frame Skip", "This allows you to save performance by not updating the lighting every frame.");
            ll.frameSkip = EditorGUILayout.IntSlider(content, ll.frameSkip, 0, 10);
        }

        private void SectionStatus()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Current Lighting Profile", GUILayout.Width(250));
                GUILayout.FlexibleSpace();
                GUILayout.Label(ll.currentProfileName);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Desire Lighting Profile", GUILayout.Width(250));
                GUILayout.FlexibleSpace();
                GUILayout.Label(ll.desiredProfileName);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2);
            
            EditorGUILayout.Slider("Blend", ll.blend, 0, 1);
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(2);
            ll.showDebugLines = EditorGUILayout.Toggle("Show Debug Lines", ll.showDebugLines);

            if (ll.showDebugLines == true)
            {
                EditorGUILayout.HelpBox("Check this box only with debug purposes. For performance reasons, it is fully recomendable to uncheck it once you're sure everything is working properly.", MessageType.Info);
            }
        }

        private void SectionVolumes()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("NAME", GUILayout.Width(180));
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("TYPE", GUILayout.Width(60));
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("BLEND/TIME", GUILayout.Width(100));
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("PRIOR", GUILayout.Width(60));
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
            DrawSeparator(Color.grey, 1, 4, 28);

            for (int i = 0; i < ll.volumes.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(ll.volumes[i].gameObject.name, GUILayout.Width(180));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField((ll.volumes[i].isGlobal ? "Global" : "Blend"), GUILayout.Width(60));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField((ll.volumes[i].isGlobal ? ll.volumes[i].timeToBlend.ToString() : ll.volumes[i].blendDistance.ToString()), GUILayout.Width(100));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(ll.volumes[i].priority.ToString(), GUILayout.Width(60));
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}