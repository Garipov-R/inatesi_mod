using InatesiCharacter.Testing.Shared;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
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
        [SerializeField] private string _graphicSettingDropdown;
        [SerializeField] private string _vSyncToggle;
        [SerializeField] private string _musicSlider = "music-volume-slider";
        [SerializeField] private string _sfxVolumeSider = "sfx-volume-slider";
        [SerializeField] private string _mouseSensetiveSider = "mouse-sensetive-slider";
        [SerializeField] private string _fovSider = "fov-slider";
        [SerializeField] private AudioMixer _AudioMixer;


        public UIDocument Document { get => _document; set => _document = value; }


        private void Awake()
        {
            if (_document == null) return;

            _document.rootVisualElement.Q<Button>(_continueButtonName).clicked += ContinueClicked;
            _document.rootVisualElement.Q<Button>(_backButtonName).clicked += OpenButtonsPanelButtonClicked;
            _document.rootVisualElement.Q<Button>(_settingsButtonName).clicked += SettingsButtonClicked;

            _document.rootVisualElement.Q<DropdownField>(_graphicSettingDropdown).choices = new();
            _document.rootVisualElement.Q<DropdownField>(_graphicSettingDropdown).choices.Clear();
            foreach (var qualityName in QualitySettings.names)
            {
                _document.rootVisualElement.Q<DropdownField>(_graphicSettingDropdown).choices.Add(qualityName);
            }
            _document.rootVisualElement.Q<DropdownField>(_graphicSettingDropdown).index = QualitySettings.GetQualityLevel();
            _document.rootVisualElement.Q<DropdownField>(_graphicSettingDropdown).RegisterValueChangedCallback((e) => 
            {
                int indexQuaility = 0;
                foreach (var qualityName in QualitySettings.names)
                {
                    if (qualityName == e.newValue)
                    {
                        QualitySettings.SetQualityLevel(indexQuaility);
                    }

                    indexQuaility++;
                }

            });

            _document.rootVisualElement.Q<Toggle>(_vSyncToggle).value = QualitySettings.vSyncCount == 0 ? false : true;
            _document.rootVisualElement.Q<Toggle>(_vSyncToggle).RegisterValueChangedCallback(e => 
            {
                QualitySettings.vSyncCount = e.newValue == true ? 1 : 0;
            });


            _document.rootVisualElement.Q<Slider>(_musicSlider).RegisterValueChangedCallback(e => 
            {
                _AudioMixer.SetFloat("music", Mathf.Log10(e.newValue) * 20);
                //e.newValue
                Debug.Log(e.newValue);
            });

            _document.rootVisualElement.Q<Slider>(_sfxVolumeSider).RegisterValueChangedCallback(e => 
            {
                _AudioMixer.SetFloat("master", Mathf.Log10(e.newValue) * 20);
                //e.newValue
            });

            _document.rootVisualElement.Q<Slider>(_mouseSensetiveSider).value = G.GameSettingsValue.MouseSens;
            _document.rootVisualElement.Q<Slider>(_mouseSensetiveSider).RegisterValueChangedCallback(e => 
            {
                G.GameSettingsValue.MouseSens = e.newValue;
                //e.newValue
            });

            _document.rootVisualElement.Q<Slider>(_fovSider).value = G.GameSettingsValue.Fov;
            _document.rootVisualElement.Q<Slider>(_fovSider).RegisterValueChangedCallback(e => 
            {
                G.GameSettingsValue.Fov = e.newValue;
                //e.newValue
            });


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