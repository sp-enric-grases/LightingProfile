using UnityEditor;
using UnityEngine;

namespace SocialPoint.Art.LightingProfiles
{
    public class LightingVolumeCreator : MonoBehaviour
    {
        [MenuItem("GameObject/3D Object/Volume Lighting", false, 200)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Lighting Volume");
            go.AddComponent<LightingVolume>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
    }
}