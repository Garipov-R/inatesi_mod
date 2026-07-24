#if YandexGamesPlatform_yg

using InatesiCharacter.Testing.LeoEcs5.Utility;
using InatesiCharacter.Testing.Shared;
using System.Collections;
using UnityEngine;
using YG;

namespace Inatesi.YandexGames
{
    public class YandexGamesGameLogic : GameLogicBase
    {
        private void Start()
        {
            YG2.GameplayStart();

            StartCoroutine(AdTimer());

            YG2.onOpenAnyAdv += OnOpenAdv;
            YG2.onCloseAnyAdv += OnCloseAdv;
            YG2.onErrorAnyAdv += OnCloseAdv;
        }

        private void OnDestroy()
        {
            YG2.onOpenAnyAdv -= OnOpenAdv;
            YG2.onCloseAnyAdv -= OnCloseAdv;
            YG2.onErrorAnyAdv -= OnCloseAdv;
        }

        private void OnOpenAdv()
        {
            YandexGamesManager.Instance.ShowAd();
            G.SetPause(true);
        }

        private void OnCloseAdv()
        {
            YandexGamesManager.Instance.CloseAd();
            G.SetPause(false);
            YG2.PauseGameNoEditEventSystem(false);
            YG2.PauseGame(false, false, false, false, false); 
        }

        private IEnumerator AdTimer()
        {
            yield return new WaitForSeconds(50f);

            while (YG2.isTimerAdvCompleted == false)
            {
                yield return null;
            }

            OnOpenAdv();

            float timer = 2;

            while (timer > 0)
            {
                YandexGamesManager.Instance.AdTimer(timer);

                timer -= Time.unscaledDeltaTime;

                yield return null;
            }

            YG2.InterstitialAdvShow();

            StartCoroutine(AdTimer());
        }

        private void OnDisable()
        {
            YG2.GameplayStop();
        }
    }
}

#endif