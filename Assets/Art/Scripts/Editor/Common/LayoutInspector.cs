using UnityEngine;
using UnityEditor;

namespace SocialPoint.Art
{
    public delegate void DrawSection();

    public class LayoutInspector : Editor
    {
        private GUIStyle GetGUIStyle(Color color)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = color;
            style.fontStyle = FontStyle.Bold;

            return style;
        }

        private GUIStyle FoldOut()
        {
            GUIStyle style = new GUIStyle(EditorStyles.foldout);
            style.fontStyle = FontStyle.Bold;

            return style;
        }

        public virtual void Header(string title)
        {
            GUILayout.Space(3);
            GUI.color = new Color(0.75f, 0.75f, 0.75f);
            EditorGUILayout.BeginVertical("Box");
            //GUIStyle font = new GUIStyle { fontSize = 12, fontStyle = FontStyle.Bold };
            GUILayout.Space(1);
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            GUI.color = Color.white;
            GUILayout.Space(3);
            EditorGUI.indentLevel++;
        }

        public virtual void Section(string title, DrawSection OnDrawSection)
        {
            Header(title);
            OnDrawSection();
            Footer();
        }

        public virtual void FoldSection(string title, DrawSection OnDrawSection, ref bool foldSection)
        {
            foldSection = EditorGUILayout.Foldout(foldSection, title, FoldOut());

            if (foldSection)
                OnDrawSection();
        }

        public void DrawSeparator(Color color, int thickness = 1, int topPadding = 6, int sidePadding = 4)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(topPadding + thickness));
            r.x = sidePadding;
            r.y += topPadding / 2;
            r.width = EditorGUIUtility.currentViewWidth - (sidePadding * 2);
            r.height = thickness;
            EditorGUI.DrawRect(r, color);
        }

        public virtual void Footer()
        {
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        public void Title (string title, bool bold = true, int fontSize = 14)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            if (bold) style.fontStyle = FontStyle.Bold;
            style.fontSize = fontSize;

            GUILayout.Label(title, style);
        }
    }
}