using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.SurfaceManagers.Editors
{
    [CustomEditor(typeof(SurfaceDataSO))]
    public class SurfaceDataEditor : Editor
    {
        [SerializeField] private VisualTreeAsset _VisualTreeAsset;


        public override VisualElement CreateInspectorGUI()
        {
            if (_VisualTreeAsset == null) return null;

            var root = _VisualTreeAsset.CloneTree();

            var t = target as SurfaceDataSO;


            serializedObject.ApplyModifiedProperties();
            //RegisteredMaterialPropertyDrawer;

            return root;
        }
    }
}