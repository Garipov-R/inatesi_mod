using GameToolkit.Localization;
using Inatesi.Game;
using InatesiCharacter.Testing.LeoEcs5;
using InatesiCharacter.Testing.LeoEcs5.Components;
using InatesiCharacter.Testing.LeoEcs5.Utility;
using InatesiCharacter.Testing.Shared;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject.SpaceFighter;


namespace InatesiCharacter.Template
{
    public class TemplateGameManager : GameLogicBase
    {
        [Zenject.Inject] private StartEcs _StartEcs;

        private float _DeadTimeSince;

        [Zenject.Inject]
        private void s(StartEcs startEcs)
        {
            _StartEcs = startEcs;
            Debug.Log(123);
        }

        private void Start()
        {
            //ECSHelper.Create<PlayerInitEvent>(_StartEcs.EcsWorld);
            G.SetVisibleCursor(false);

            Localization.Instance.CurrentLanguage = Language.Russian;

            Invoke(nameof(StartGame), .2f);
            //StartGame();
        }

        private void Update()
        {
            //ECSHelper.Create<PlayerInitEvent>(_StartEcs.EcsWorld);
        }

        public override void StartGame()
        {
            Debug.Log("start game");
            ECSHelper.Create<PlayerInitEvent>(_StartEcs.EcsWorld);

        }

        public override void OnPlayerSpawn()
        {

        }
    }
}

