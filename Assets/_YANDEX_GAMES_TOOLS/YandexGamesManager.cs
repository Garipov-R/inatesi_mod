#if YandexGamesPlatform_yg

using GameToolkit.Localization;
using UnityEngine;
using UnityEngine.UIElements;
using YG;


namespace Inatesi.YandexGames
{
    public class YandexGamesManager : MonoBehaviour
    {
        public static YandexGamesManager Instance;

        [SerializeField] private UIDocument _UIDocument;

        private void Awake()
        {
            Instance = this;

            Localization.Instance.LocaleChanged += Instance_LocaleChanged;

            YG2.onSwitchLang += OnSwitchLang;

            if (YG2.lang == "ru")
            {
                Localization.Instance.CurrentLanguage = Language.Russian;
            }
            else if (YG2.lang == "en")
            {
                Localization.Instance.CurrentLanguage = Language.English;
            }
            else
            {
                Localization.Instance.CurrentLanguage = Language.Russian;
            }

            YG2.GameReadyAPI();
        }

        private void OnDisable()
        {
            Localization.Instance.LocaleChanged -= Instance_LocaleChanged;
        }

        private void OnSwitchLang(string lang)
        {
            if (YG2.lang == "ru")
            {
                Localization.Instance.CurrentLanguage = Language.Russian;
            }
            else if (YG2.lang == "en")
            {
                Localization.Instance.CurrentLanguage = Language.English;
            }
            else
            {
                Localization.Instance.CurrentLanguage = Language.Russian;
            }
        }

        private void Instance_LocaleChanged(object sender, LocaleChangedEventArgs e)
        {
            if (Localization.Instance.CurrentLanguage == Language.Russian)
            {
                YG2.lang = "ru";
            }
            else if (Localization.Instance.CurrentLanguage == Language.English)
            {
                YG2.lang = "en";
            }
            else
            {
                YG2.lang = "ru";
            }

            Debug.Log(YG2.lang);
        }

        public void ShowAd()
        {
            _UIDocument.rootVisualElement.Q<VisualElement>("ad-panel").style.display = DisplayStyle.Flex;
        }

        public void CloseAd()
        {
            _UIDocument.rootVisualElement.Q<VisualElement>("ad-panel").style.display = DisplayStyle.None;
        }

        public void AdTimer(float time)
        {
            _UIDocument.rootVisualElement.Q<Label>("ad-timer").text = time.ToString("0");
        }
    }

}

#endif