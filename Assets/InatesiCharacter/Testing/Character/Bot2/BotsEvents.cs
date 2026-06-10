using InatesiCharacter.Testing.Character.Bots;
using InatesiCharacter.Testing.LeoEcs5;
using InatesiCharacter.Testing.LeoEcs5.Components;
using InatesiCharacter.Testing.LeoEcs5.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InatesiCharacter.Testing.Character.Bot2
{
    public class BotsEvents : MonoBehaviour, IBotObserver
    {
        [SerializeField] private List<GameObject> _botTests = new List<GameObject>();
        [SerializeField] private UnityEvent _OnAllDeathBot;
        [SerializeField] private UnityEvent _OnDeathBot;
        [SerializeField] private Transform _SpawnPoint;

        private int _countDeath;
        [Zenject.Inject] private StartEcs _StartEcs;


        public void DeathBot(GameObject botGameObject)
        {
            _OnDeathBot?.Invoke();

            _botTests.ForEach (bot => 
            {
                if (bot.gameObject == botGameObject)
                {
                    Debug.Log(_countDeath);
                    _countDeath++;
                }
                else
                {
                    return;
                }
            });

            

            if (_countDeath >= _botTests.Count)
            {
                _OnAllDeathBot?.Invoke();
            }
        }

        public void SpawnBot()
        {
            if (_StartEcs == null)
                return;

            if (_SpawnPoint == null)
                return;

            if (_botTests == null && _botTests.Count == 0)
                return;

            ref var component = ref ECSHelper.Create<BotInitEvent>(_StartEcs.EcsWorld);
            component.prefab = _botTests[0];
            component.position = _SpawnPoint.position;
            component.rotation = _SpawnPoint.rotation;
        }

        public void InvokeAllDeathBot()
        {
            _OnAllDeathBot?.Invoke();
        }
    }
}