using InatesiCharacter.Template;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.Character.Customize
{
    public class CharacterCustomizeUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _UIDocument;
        [SerializeField] private MainMenuUI _MainMenuUI;

        private string _namePanel = "character-customize-panel";
        private VisualElement _root;
        private bool _active;

        private void Awake()
        {
            _root =  _UIDocument.rootVisualElement.Q<VisualElement>(_namePanel);
            _UIDocument.rootVisualElement.Q<Button>("character-customize-button").clicked += CharacterCustomizeUI_clicked;
        }

        private void CharacterCustomizeUI_clicked()
        {
            OpenPanel();
            _MainMenuUI.SetActiveElement(_MainMenuUI.BackButtonName, _active);
            _MainMenuUI.Document.rootVisualElement.Q<Button>(_MainMenuUI.BackButtonName).clicked += BackClicked;
        }

        private void BackClicked()
        {
            ClosePanel();
        }

        public void OpenPanel()
        {
            SetActivePanel(true);
        }

        public void ClosePanel()
        {
            SetActivePanel(false);
        }

        public void TogglePanel()
        {
            if (_UIDocument == null)
                return;

            SetActivePanel(!_active);
        }

        public void SetActivePanel(bool state)
        {
            if (_UIDocument == null)
                return;

            _root.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
            _active = _root.style.display == DisplayStyle.Flex;
        }
    }
}