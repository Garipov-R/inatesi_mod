using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditor.UIElements;
using GameToolkit.Localization;

namespace InatesiCharacter.Testing.UI.Editor
{
    public class TranslateLabelTextHelper : EditorWindow
    {
        [SerializeField] private string _ParentGameObjectName  = "localize";
        [SerializeField] private List<LocalizedText> _localizedTextBehaviours = new();
        [SerializeField] private UIDocument _Uidocument;


        [MenuItem("Tools/inatesi/translate helper")]
        public static void ShowWindow()
        {
            TranslateLabelTextHelper wnd = GetWindow<TranslateLabelTextHelper>();
            wnd.titleContent = new GUIContent("Toolkit Window");
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;
            SerializedObject serializedWin = new SerializedObject(this);


            SerializedProperty prefabProp = serializedWin.FindProperty(nameof(_localizedTextBehaviours));
            PropertyField objectField = new PropertyField(prefabProp);
            root.Add(objectField);

            SerializedProperty uiDocProp = serializedWin.FindProperty(nameof(_Uidocument));
            root.Add(new PropertyField(uiDocProp));

            SerializedProperty nameprop = serializedWin.FindProperty(nameof(_ParentGameObjectName));
            root.Add(new PropertyField(nameprop));


            Button button = new Button();
            button.text = "Apply";
            button.clicked += ApplyClick;
            root.Add(button);

            rootVisualElement.Bind(serializedWin);
            Repaint();
        }


        private void ApplyClick()
        {
            if (_Uidocument == null)
                return;

            var parent = new GameObject(_ParentGameObjectName);

            foreach (var item in _localizedTextBehaviours)
            {
                if (item == null) continue;

                var newText = new GameObject(item.name);
                var localizeTextBeh = newText.AddComponent<LocalizedTextBehaviour>();
                var labelText = newText.AddComponent<TranlateLabelText>();
                labelText.UIDocument = _Uidocument;
                labelText.ElementPath = item.name;
                localizeTextBeh.LocalizedAsset = item;
                localizeTextBeh.Component = labelText;
                localizeTextBeh.Property = "Text";

                newText.transform.SetParent(parent.transform);
            }
        }
    }
}