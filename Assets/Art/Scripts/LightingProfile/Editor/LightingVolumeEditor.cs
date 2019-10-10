using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SocialPoint.Art.LightingProfiles
{
    [CustomEditor(typeof(LightingVolume))]
    public class LightingVolumeEditor : LayoutInspector
    {
        private LightingVolume lv;
        private GUIContent content;
        private SerializedProperty profile;
        private BoxCollider bc;

        private void OnEnable()
        {
            lv = (LightingVolume)target;
            bc = lv.GetComponent<BoxCollider>();
            profile = serializedObject.FindProperty("profile");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            GUILayout.Space(4);

            content = new GUIContent("Is Global", "Check this box to mark this volume as global. This volume's Profile will be applied to the whole Scene.");
            lv.isGlobal = EditorGUILayout.Toggle(content, lv.isGlobal);

            if (lv.isGlobal)
            {
                content = new GUIContent("Blend Time", "Time to blend between two global lighting profiles.");
                lv.timeToBlend = Mathf.Clamp(EditorGUILayout.FloatField(content, lv.timeToBlend), 0.001f, 100);
                lv.timeCurve = EditorGUILayout.CurveField("  ", lv.timeCurve, Color.yellow, new Rect(0, 0, 1, 1));
            }
            else
            {
                content = new GUIContent("Blend Distance", "The distance (from the attached Collider) to start blending from. A value of 0 means there will be no blending and the Volume overrides will be applied immediatly upon entry to the attached Collider.");
                lv.blendDist = Mathf.Clamp(EditorGUILayout.FloatField(content, lv.blendDist), 0, Mathf.Infinity);
            }

            content = new GUIContent("Priority", "The volume priority in the stack. A higher value means higher priority. Negative values are supported.");
            lv.priority = EditorGUILayout.FloatField(content, lv.priority);

            EditorGUILayout.BeginHorizontal();
            {
                content = new GUIContent("Lighting Profile", "A reference to a lighting profile asset");
                EditorGUILayout.PropertyField(profile, content);

                if (GUILayout.Button("New", EditorStyles.miniButton, GUILayout.Width(60)))
                {
                    CreateLightingProfile();
                    EditorGUI.BeginChangeCheck();
                }
            }   
            EditorGUILayout.EndHorizontal();

            if (lv.profile == null)
                EditorGUILayout.HelpBox("Assign a Lighting Profile to this volume using the \"Asset\" field or create one automatically by clicking the \"New\" button.\nAssets are automatically put in a folder next to your scene file.", MessageType.Info);

            if (EditorGUI.EndChangeCheck())
                Undo.RegisterCompleteObjectUndo(lv, "Lighting Volume");

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed) EditorUtility.SetDirty(lv);

        }

        private void CreateLightingProfile()
        {
            var asset = ScriptableObject.CreateInstance<LightingProfile>();
            int index = 0;
            var name = "LIGHT_newLightingProfile";

            List<string> files = Directory.GetFiles("Assets", name + "*.asset").Select(n => Path.GetFileNameWithoutExtension(n)).ToList();

            while (files.Contains(name))
            {
                name = string.Format("LIGHT_newLightingProfile {0}", ++index);
            }

            CreateAsset(asset, name);
        }

        private void CreateAsset(LightingProfile asset, string file)
        {
            string path = Path.Combine("Assets", string.Format("{0}.asset", file));
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            lv.profile = asset;
            Selection.activeObject = asset;
        }

        void OnSceneGUI()
        {
            Handles.color = Color.yellow;
        }
    }
}


