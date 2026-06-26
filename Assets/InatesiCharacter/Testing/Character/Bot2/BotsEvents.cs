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
        [SerializeField] private List<GameObject> _botsOnScene = new List<GameObject>();
        [SerializeField] private List<GameObject> _botPrefabs = new List<GameObject>();
        [SerializeField] private UnityEvent _OnAllDeathBot;
        [SerializeField] private UnityEvent _OnAnyDeathBotEvent;
        [SerializeField] private UnityEvent<GameObject> _OnDeathBotEvent;
        [SerializeField] private List<Transform> _SpawnPoints = new List<Transform>();

        private int _countDeath;
        [Zenject.Inject] private StartEcs _StartEcs;


        public void DeathBot(GameObject botGameObject)
        {
            _OnAnyDeathBotEvent?.Invoke();
            _OnDeathBotEvent?.Invoke(botGameObject);

            foreach (var bot in _botsOnScene)
            {
                if (bot == botGameObject)
                {
                    _countDeath++;
                }
                else
                {
                    return;
                }
            }

            if (_countDeath >= _botsOnScene.Count)
            {
                _OnAllDeathBot?.Invoke();
            }
        }

        public void SpawnBot()
        {
            if (_StartEcs == null)
                return;

            if (_SpawnPoints == null)
                return;

            if (_SpawnPoints.Count == 0) 
                return;

            if (_botPrefabs == null && _botPrefabs.Count == 0)
                return;

            ref var component = ref ECSHelper.Create<BotInitEvent>(_StartEcs.EcsWorld);
            component.prefab = _botPrefabs[Random.Range(0, _botPrefabs.Count)];
            var spawnPoint = _SpawnPoints[Random.Range(0, _SpawnPoints.Count)];
            component.position = spawnPoint.position;
            component.rotation = spawnPoint.rotation;
        }

        public void SpawnBot(GameObject targetBotGameObject)
        {
            if (_StartEcs == null)
                return;

            if (_SpawnPoints == null)
                return;

            if (_SpawnPoints.Count == 0)
                return;

            if (_botPrefabs == null && _botPrefabs.Count == 0)
                return;


            GameObject prefab = null;
            var spawnPoint = _SpawnPoints[Random.Range(0, _SpawnPoints.Count)];
            foreach (var botPrefab in _botPrefabs)
            {
                var nameBot = targetBotGameObject.name.Replace("(Clone)", "");
                if (botPrefab.name == nameBot)
                {
                    prefab = botPrefab;
                }
            }

            if (prefab != null)
            {
                ref var component = ref ECSHelper.Create<BotInitEvent>(_StartEcs.EcsWorld);
                component.prefab = prefab;
                component.position = spawnPoint.position;
                component.rotation = spawnPoint.rotation;
            }
        }

        public void InvokeAllDeathBot()
        {
            _OnAllDeathBot?.Invoke();
        }
    }
}