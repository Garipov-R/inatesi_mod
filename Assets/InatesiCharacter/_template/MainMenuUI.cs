using InatesiCharacter.Testing.Shared;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Template
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;
        [SerializeField] private string _rootPanelName;
        [SerializeField] private string _continueButtonName;
        [SerializeField] private string _backButtonName;
        [SerializeField] private string _settingsButtonName;
        [SerializeField] private string _buttonsPanelName;
        [SerializeField] private string _settingsPanelName;


        public UIDocument Document { get => _document; set => _document = value; }


        private void Awake()
        {
            if (_document == null) return;

            _document.rootVisualElement.Q<Button>(_continueButtonName).clicked += ContinueClicked;
            _document.rootVisualElement.Q<Button>(_backButtonName).clicked += OpenButtonsPanelButtonClicked;
            _document.rootVisualElement.Q<Button>(_settingsButtonName).clicked += SettingsButtonClicked;

            SetActivePanel(_buttonsPanelName);
            SetRootPanel(false);    
        }

        
        #region buttons events

        private void ContinueClicked()
        {
            if (_document == null) return;

            G.SetPause(false);
            G.SetVisibleCursor(false);
            SetRootPanel(G.IsPause);
        }

        private void SettingsButtonClicked()
        {
            if (_document == null) return;

            SetActivePanel(_settingsPanelName);
        }

        private void OpenButtonsPanelButtonClicked()
        {
            if (_document == null) return;

            SetActivePanel(_buttonsPanelName);
        }

        private void ExitButtonClicked()
        {
            if (_document == null) return;
        }

        #endregion

        public void SetRootPanel(bool state)
        {
            if (_document == null) return;

            _document.rootVisualElement.Q<VisualElement>(_rootPanelName).style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
            SetActivePanel(_buttonsPanelName);
        }

        public void ToggleRootPanel()
        {
            if (_document == null) return;
            //_document.rootVisualElement.Q<VisualElement>(_rootPanelName).style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        }


        private void SetActivePanel(string panelName)
        {
            if (_document == null) return;

            _document.rootVisualElement.Q<VisualElement>(_buttonsPanelName).style.display = panelName == _buttonsPanelName ? DisplayStyle.Flex : DisplayStyle.None;
            _document.rootVisualElement.Q<VisualElement>(_settingsPanelName).style.display = panelName == _settingsPanelName ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}