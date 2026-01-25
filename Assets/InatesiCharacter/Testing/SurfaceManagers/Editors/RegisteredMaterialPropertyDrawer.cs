using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.SurfaceManagers.Editors
{
    [CustomPropertyDrawer(typeof(RegisteredMaterial))]
    public class RegisteredMaterialPropertyDrawer : PropertyDrawer
    {
        [SerializeField] private VisualTreeAsset _VisualTreeAsset;

        private SurfaceDataSO _SurfaceManagerSO;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (_VisualTreeAsset == null) return null;

            var root = _VisualTreeAsset.CloneTree();
            SerializedProperty surfaceIndex = property.FindPropertyRelative("surfaceIndex");
            SerializedProperty texture = property.FindPropertyRelative("texture");
            var t = property.serializedObject.targetObject as SurfaceDataSO;

            root.Q<DropdownField>("dropdown").choices = new();
            foreach (var i in t.GetAllSurfaceNames())
            {
                root.Q<DropdownField>("dropdown").choices.Add(i);
            }

            root.Q<DropdownField>("dropdown").RegisterValueChangedCallback((e) => 
            {
                for (int i = 0; i <  t.GetAllSurfaceNames().Length; i++)
                {
                    if (t.GetAllSurfaceNames()[i] == e.newValue)
                    {
                        surfaceIndex.intValue = i; 
                        property.serializedObject.ApplyModifiedProperties();
                        break;
                    }
                }
            });

            root.Q<DropdownField>("dropdown").value = t.GetAllSurfaceNames()[surfaceIndex.intValue];

            var tt = property.serializedObject.context as SurfaceDataEditor;

            root.Q<VisualElement>("selected-texture").style.display = DisplayStyle.None;
            root.Q<ObjectField>("texture").RegisterValueChangedCallback(e => 
            {
                root.Q<VisualElement>("selected-texture").style.backgroundImage = new StyleBackground((Texture2D)texture.objectReferenceValue);
                root.Q<VisualElement>("selected-texture").style.display = DisplayStyle.None;
                property.serializedObject.ApplyModifiedProperties();
            });

            root.Q<ObjectField>("texture").RegisterCallback<ClickEvent>((x) => {
                root.Q<VisualElement>("selected-texture").style.backgroundImage = new StyleBackground((Texture2D)texture.objectReferenceValue);
                root.Q<VisualElement>("selected-texture").style.display = root.Q<VisualElement>("selected-texture").style.display == DisplayStyle.Flex ? DisplayStyle.None : DisplayStyle.Flex;
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.UpdateIfRequiredOrScript(); 
                property.serializedObject.Update(); 
            });

            return root;
        }
    }
}