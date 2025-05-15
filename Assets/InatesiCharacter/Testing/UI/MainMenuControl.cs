using InatesiCharacter.Testing.Shared;
using InatesiCharacter.Testing.Stuff;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.UI
{
    public class MainMenuControl : MonoBehaviour
    {
        [Header("Scenes")]
        [SerializeField] private string _LoadGameScene = "PresentationSceneLeoEcs";
        [SerializeField] private string _MainMenuScene = "MainMenu";
        [SerializeField] private string _ZombieWaveScene = "ZombieWave";
        [SerializeField] private string _FreeGameScene = "FreeGame";
        [Space(10)]
        [SerializeField] private UIDocument _MainMenuDocument;

        private VisualElement _rootElement;
        private VisualElement _ButtonsContainer;
        private VisualElement _SettingsContainer;
        private Button _StartSinglePlayerButton;
        private Button _StartServerClientPlayerButton;
        private Button _StartClientPlayerButton;
        private Button _GoMainMenuButton;
        private Button _ExitGameButton;
        private Button _ContinueButton;
        private Button _SettingsButton;
        private Button _ChangeLocalizationButton;
        private Button _ZombieWaveButton;
        private Button _FreeGameButton;
        private Button _HomeMenuButton;
        private VisualElement _LoadPanel;
        private Slider _MouseSensSlider;
        private Label _MouseSensLabel;
        private Slider _FovSlider;
        private Label _FovLabel;

        private const string c_startSinglePlayer = "start-single-player";
        private const string c_startServerClientPlayer = "start-server-client-player";
        private const string c_StartClientPlayer = "start-client-player";
        private const string c_GoMainMenu = "go-main-menu";
        private const string c_Exit = "exit-game";
        private const string c_Continue = "continue";
        private const string c_Settings = "settings";
        private const string c_ChangeLocalization = "change-localization";
        private const string c_ZombieWave = "zombie-wave";
        private const string c_FreeGame = "free-game";
        private const string c_SettingsContainer = "settings-container";
        private const string c_ButtonContainer = "buttons-container";
        private const string c_HomeMenuButton = "home-menu-button";
        private const string c_LoadPanel = "load-panel";
        private const string c_MouseSensSlider = "mouse-sens-slider";
        private const string c_MouseSensText = "mouse-sensivity-value-text";
        private const string c_FovSlider = "fov-slider";
        private const string c_FovText = "fov-value-text";

        private Dictionary<string, Button> _ButtonDictionary = new Dictionary<string, Button>();


        private void Awake()
        {
            _rootElement = _MainMenuDocument.rootVisualElement;

            _StartSinglePlayerButton = _rootElement.Q("root").Q(c_ButtonContainer).Q<Button>(c_startSinglePlayer);
            _StartServerClientPlayerButton = _rootElement.Q("root").Q(c_ButtonContainer).Q<Button>(c_startServerClientPlayer);
            _StartClientPlayerButton = _rootElement.Q("root").Q(c_ButtonContainer).Q<Button>(c_StartClientPlayer);
            _GoMainMenuButton = _rootElement.Q("root").Q(c_ButtonContainer).Q<Button>(c_GoMainMenu);
            _ExitGameButton = _rootElement.Q("root").Q(c_ButtonContainer).Q<Button>(c_Exit);
            _ContinueButton = _rootElement.Q("root").Q(c_ButtonContainer).Q<Button>(c_Continue);
            _SettingsButton = _rootElement.Q("root").Q(c_ButtonContainer).Q<Button>(c_Settings);
            _ChangeLocalizationButton = _rootElement.Q("root").Q(c_ButtonContainer).Q<Button>(c_ChangeLocalization);
            _ZombieWaveButton = _rootElement.Q("root").Q(c_ButtonContainer).Q<Button>(c_ZombieWave);
            _FreeGameButton = _rootElement.Q("root").Q(c_ButtonContainer).Q<Button>(c_FreeGame);
            _ButtonsContainer = _rootElement.Q("root").Q(c_ButtonContainer);
            _SettingsContainer = _rootElement.Q("root").Q(c_SettingsContainer);
            _HomeMenuButton = _rootElement.Q("root").Q<Button>(c_HomeMenuButton);
            _LoadPanel = _rootElement.Q("root").Q<VisualElement>(c_LoadPanel);
            _MouseSensSlider = _rootElement.Q("root").Q<Slider>(c_MouseSensSlider);
            _MouseSensLabel = _rootElement.Q("root").Q<Label>(c_MouseSensText);
            _FovSlider = _rootElement.Q("root").Q<Slider>(c_FovSlider);
            _FovLabel = _rootElement.Q("root").Q<Label>(c_FovText);

            _ButtonDictionary.Add(c_startSinglePlayer, _StartSinglePlayerButton);
            _ButtonDictionary.Add(c_startServerClientPlayer, _StartServerClientPlayerButton);
            _ButtonDictionary.Add(c_StartClientPlayer, _StartClientPlayerButton);
            _ButtonDictionary.Add(c_GoMainMenu, _GoMainMenuButton);
            _ButtonDictionary.Add(c_Exit, _ExitGameButton);
            _ButtonDictionary.Add(c_Continue, _ContinueButton);

            _GoMainMenuButton.clicked += GoMainMenu;
            _ExitGameButton.clicked += ExitGame;
            _StartSinglePlayerButton.clicked += StartSinglePlayer;
            _ContinueButton.clicked += ContinueButton_clicked;
            _SettingsButton.clicked += SettingsButton_clicked;
            _ChangeLocalizationButton.clicked += ChangeLocalizationButton_clicked;
            _ZombieWaveButton.clicked += StartZombieWave;
            _FreeGameButton.clicked += StartFreeGame;
            _HomeMenuButton.clicked += HomeMenuButton_clicked;
            _MouseSensSlider.RegisterCallback<ChangeEvent<float>>((evt) =>
            {
                GameSettings.GameSettingsValue.MouseSens = evt.newValue;
                _MouseSensLabel.text = GameSettings.GameSettingsValue.MouseSens.ToString("0.00");
            });
            _MouseSensSlider.value = GameSettings.GameSettingsValue.MouseSens;
            _FovSlider.RegisterCallback<ChangeEvent<float>>((evt) =>
            {
                GameSettings.GameSettingsValue.Fov = evt.newValue;
                _FovLabel.text = GameSettings.GameSettingsValue.Fov.ToString("0");
            });
            _FovSlider.value = GameSettings.GameSettingsValue.Fov;
            _MouseSensLabel.text = GameSettings.GameSettingsValue.MouseSens.ToString("0.00");
            _FovLabel.text = GameSettings.GameSettingsValue.Fov.ToString("0");

            SetActiveButton(_ContinueButton, false);
            SetActiveButton(_StartServerClientPlayerButton, false);
            SetActiveButton(_StartClientPlayerButton, false);
            SetActiveButton(_GoMainMenuButton, false);
            SetActiveButton(_ExitGameButton, false);
            SetActiveButton(_StartSinglePlayerButton, false);
            SetActiveButton(_SettingsButton, true);
            SetActiveButton(_HomeMenuButton, false);

            GameToolkit.Localization.Localization.Instance.LocaleChanged += LocaleChanged;

            UpdateLocalizationFlag();

            SetActivePanel(c_ButtonContainer);

            _LoadPanel.style.display = DisplayStyle.None;
        }


        private void Update()
        {
            if (SceneManager.GetActiveScene().name == _MainMenuScene) return;
            if (Input.GetKeyDown(Key.Escape)) 
            {
                ToggleVisibleRootPanel();

                GameSettings.SetPause(!GameSettings.IsPause);
            }
        }

        private void StartSinglePlayer()
        {
            SceneManager.LoadScene(_LoadGameScene, LoadSceneMode.Single);
            SetVisibleRootPanel(false);

            SetActiveButton(_ContinueButton, true);
            SetActiveButton(_GoMainMenuButton, true);
            SetActiveButton(_StartSinglePlayerButton, false);

            GameSettings.SetPause(false);
        }

        private void StartFreeGame()
        {
            LoadScene(_FreeGameScene);

            SetActiveButton(_ContinueButton, true);
            SetActiveButton(_GoMainMenuButton, true);
            SetActiveButton(_StartSinglePlayerButton, false);
            SetActiveButton(_ZombieWaveButton, false);
            SetActiveButton(_FreeGameButton, false);

            GameSettings.SetPause(false);
        }

        private void SettingsButton_clicked()
        {
            //GameToolkit.Localization.Localization.Instance.SetSystemLanguage();
            SetActivePanel(c_SettingsContainer);
            SetActiveButton(_HomeMenuButton, true);
        }

        private void HomeMenuButton_clicked()
        {
            //GameToolkit.Localization.Localization.Instance.SetSystemLanguage();
            SetActivePanel(c_ButtonContainer);
            SetActiveButton(_HomeMenuButton, false);
        }

        private void SetActivePanel(string namePanel)
        {
            _ButtonsContainer.style.display = namePanel == _ButtonsContainer.name ? DisplayStyle.Flex : DisplayStyle.None;
            _SettingsContainer.style.display = namePanel == _SettingsContainer.name ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void GoMainMenu()
        {
            LoadScene(_MainMenuScene);

            SetActiveButton(_ContinueButton, false);
            SetActiveButton(_StartServerClientPlayerButton, false);
            SetActiveButton(_StartClientPlayerButton, false);
            SetActiveButton(_GoMainMenuButton, false);
            SetActiveButton(_ExitGameButton, false);
            SetActiveButton(_StartSinglePlayerButton, false);

            SetActiveButton(_ZombieWaveButton, true);
            SetActiveButton(_FreeGameButton, true);
        }

        private void StartZombieWave()
        {
            LoadScene(_ZombieWaveScene);

            SetActiveButton(_ContinueButton, true);
            SetActiveButton(_GoMainMenuButton, true);
            SetActiveButton(_StartSinglePlayerButton, false);
            SetActiveButton(_ZombieWaveButton, false);
            SetActiveButton(_FreeGameButton, false);

            GameSettings.SetPause(false);
        }

        private void ExitGame()
        {
            Application.Quit();
        }


        private void ContinueButton_clicked()
        {
            SetVisibleRootPanel(false);
            GameSettings.SetPause(false);
        }

        private void ToggleVisibleRootPanel()
        {
            SetVisibleRootPanel(!GameSettings.IsPause);
        }

        private void SetVisibleRootPanel(bool state)
        {
            _rootElement.Q<VisualElement>("root").visible = state;
        }

        private void SetActiveButton(Button button, bool active) 
        {
            if (button == null) return;
            button.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void LocaleChanged(object sender, GameToolkit.Localization.LocaleChangedEventArgs e)
        {

        }

        private void ChangeLocalizationButton_clicked()
        {
            if (GameToolkit.Localization.Language.Russian == GameToolkit.Localization.Localization.Instance.CurrentLanguage)
            {
                GameToolkit.Localization.Localization.Instance.CurrentLanguage = GameToolkit.Localization.Language.English;
            }
            else
            {
                GameToolkit.Localization.Localization.Instance.CurrentLanguage = GameToolkit.Localization.Language.Russian;
            }

            UpdateLocalizationFlag();
        }

        private void UpdateLocalizationFlag()
        {
            var usa = _ChangeLocalizationButton.Q<VisualElement>("usa");
            var ru = _ChangeLocalizationButton.Q<VisualElement>("ru");

            if (GameToolkit.Localization.Language.Russian != GameToolkit.Localization.Localization.Instance.CurrentLanguage)
            {
                usa.style.display = DisplayStyle.Flex;
                ru.style.display = DisplayStyle.None;
            }
            else
            {
                usa.style.display = DisplayStyle.None;
                ru.style.display = DisplayStyle.Flex;
            }
        }

        public void SetActiveLoadPanel(bool active)
        {
            _LoadPanel.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
        }


        #region Scene Managment


        /*private void LoadScene(string nameScene)
        {
            SceneManager.LoadScene(nameScene, LoadSceneMode.Single);
            SetVisibleRootPanel(false);

            SetActiveButton(_ContinueButton, true);
            SetActiveButton(_GoMainMenuButton, true);
            SetActiveButton(_StartSinglePlayerButton, false);

            GameSettings.SetPause(false);
        }*/


        public bool LoadScene(string nameScene)
        {
            SetActiveLoadPanel(true);

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nameScene, LoadSceneMode.Single);
            StartCoroutine(LoadSceneAsync(asyncOperation));

            return true;
        }

        public bool LoadScene(int sceneId)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Single);
            StartCoroutine(LoadSceneAsync(asyncOperation));

            return true;
        }

        private IEnumerator LoadSceneAsync(AsyncOperation asyncOperation)
        {
            yield return new WaitForSeconds(1f);

            while (asyncOperation.isDone == false) 
            {
                float progressValue = Mathf.Clamp01(asyncOperation.progress / .9f);


                yield return null;
            }

            SetActiveLoadPanel(false);

            if (SceneManager.GetActiveScene().name != _MainMenuScene) SetVisibleRootPanel(false);

        }

        #endregion
    }
}