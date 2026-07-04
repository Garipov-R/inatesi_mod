using InatesiCharacter.Testing.Shared;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace InatesiCharacter.Template
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;
        [SerializeField] private string _rootPanelName = "root-panel";
        [SerializeField] private string _continueButtonName = "continue-button";
        [SerializeField] private string _backButtonName = "back-button";
        [SerializeField] private string _settingsButtonName = "settings-button";
        [SerializeField] private string _exitButtonName = "exit-button";

        [Header("Panels")]
        [SerializeField] private string _buttonsPanelName = "buttons-panel";
        [SerializeField] private string _settingsPanelName = "settings-panel";


        [Header("Loading Panel")]
        [SerializeField] private string _loadingPanelName = "loading-panel";
        [SerializeField] private string _loadingProgressTextName = "loading-progress-text";

        [Header("Settings")]
        [SerializeField] private string _graphicSettingDropdown = "graphic-settings-dropdown";
        [SerializeField] private string _vSyncToggle = "vSync-toggle";
        [SerializeField] private string _musicSlider = "music-volume-slider";
        [SerializeField] private string _sfxVolumeSider = "sfx-volume-slider";
        [SerializeField] private string _mouseSensetiveSider = "mouse-sensetive-slider";
        [SerializeField] private string _fovSider = "fov-slider";
        [SerializeField] private string _mobileInputButtonName = "open-menu";
        [SerializeField] private string _postProcessingToggleName = "post-processing-toggle";
        [SerializeField] private string _virtualInputToggle = "virtual-input-toggle";
        [SerializeField] private AudioMixer _AudioMixer;
        [SerializeField] private GameObject _virtualInput;
        [SerializeField] private Volume _PostProcessing;


        public UIDocument Document { get => _document; set => _document = value; }
        public string BackButtonName { get => _backButtonName; set => _backButtonName = value; }
        public string ContinueButtonName { get => _continueButtonName; set => _continueButtonName = value; }
        public string ExitButtonName { get => _exitButtonName; set => _exitButtonName = value; }

        private void Awake()
        {
            if (_document == null) return;

            _document.rootVisualElement.Q<Button>(_continueButtonName).clicked += ContinueClicked;
            _document.rootVisualElement.Q<Button>(BackButtonName).clicked += OpenButtonsPanelButtonClicked;
            _document.rootVisualElement.Q<Button>(_settingsButtonName).clicked += SettingsButtonClicked;
            _document.rootVisualElement.Q<Button>(_mobileInputButtonName).clicked += MobileInputClicked;

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

            _document.rootVisualElement.Q<Toggle>(_virtualInputToggle).RegisterValueChangedCallback(e =>
            {
                SetActiveVirtualInput(e.newValue);
            });

            _document.rootVisualElement.Q<Toggle>(_postProcessingToggleName).value = _PostProcessing.enabled;
            _document.rootVisualElement.Q<Toggle>(_postProcessingToggleName).RegisterValueChangedCallback(e =>
            {
                if (_PostProcessing)
                {
                    _PostProcessing.enabled = !_PostProcessing.enabled;
                }
            });


            SetActivePanel(_buttonsPanelName);
            SetRootPanel(false);    
        }



        public void SetActiveVirtualInput(bool state)
        {
            _virtualInput?.SetActive(state);
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
            SetActiveElement(BackButtonName, true);
        }

        private void OpenButtonsPanelButtonClicked()
        {
            if (_document == null) return;

            SetActivePanel(_buttonsPanelName);
            SetActiveElement(BackButtonName, false);
        }

        private void ExitButtonClicked()
        {
            if (_document == null) return;
        }

        private void MobileInputClicked()
        {
            G.SetPause(!G.IsPause);
            G.SetVisibleCursor(G.IsPause);
            ToggleRootPanel();
        }

        #endregion

        public void SetRootPanel(bool state)
        {
            if (_document == null) return;

            _document.rootVisualElement.Q<VisualElement>(_rootPanelName).style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
            SetActivePanel(_buttonsPanelName);
            SetActiveElement(BackButtonName, false);
        }

        public void ToggleRootPanel()
        {
            if (_document == null) return;

            _document.rootVisualElement.Q<VisualElement>(_rootPanelName).style.display = G.IsPause ? DisplayStyle.Flex : DisplayStyle.None;

            //_document.rootVisualElement.Q<VisualElement>(_rootPanelName).style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        }


        public void SetActivePanel(string panelName)
        {
            if (_document == null) return;

            _document.rootVisualElement.Q<VisualElement>(_buttonsPanelName).style.display = panelName == _buttonsPanelName ? DisplayStyle.Flex : DisplayStyle.None;
            _document.rootVisualElement.Q<VisualElement>(_settingsPanelName).style.display = panelName == _settingsPanelName ? DisplayStyle.Flex : DisplayStyle.None;
            _document.rootVisualElement.Q<VisualElement>(_loadingPanelName).style.display = panelName == _loadingPanelName ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetActiveElement(string elementName, bool active)
        {
            if (_document == null) return;

            _document.rootVisualElement.Q<VisualElement>(elementName).style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
        }


        #region Loading

        public void LoadScene(string nameScene)
        {
            SetActiveElement(_loadingPanelName, true);
            _document.rootVisualElement.Q<Label>(_loadingProgressTextName).text = $"{0}%";
            StartCoroutine(LoadSceneAsyncCoroutine(nameScene));
        }

        private IEnumerator LoadSceneAsyncCoroutine(string nameScene)
        {
            yield return new WaitForSeconds(0.5f);

            AsyncOperation operation = SceneManager.LoadSceneAsync(nameScene);

            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);

                //progressBar.value = progress;
                //progressBar.title = $"{(progress * 100):F0}%";
                _document.rootVisualElement.Q<Label>(_loadingProgressTextName).text = $"{(progress * 100):F0}%";

                if (operation.progress >= 0.9f)
                {
                    yield return new WaitForSeconds(0.1f);
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

            LoadedScene();
        }

        public virtual void LoadedScene()
        {
            SetActiveElement(_loadingPanelName, false);
        }

        #endregion
    }
}