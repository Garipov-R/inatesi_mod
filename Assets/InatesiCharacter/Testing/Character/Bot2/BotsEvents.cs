using InatesiCharacter.Testing.Character.Bots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InatesiCharacter.Testing.Character.Bot2
{
    public class BotsEvents : MonoBehaviour, IBotObserver
    {
        [SerializeField] private List<BotTest> _botTests = new List<BotTest>();
        [SerializeField] private UnityEvent _OnAllDeathBot;

        private int _countDeath;


        public void DeathBot(GameObject botGameObject)
        {
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

        public void InvokeAllDeathBot()
        {
            _OnAllDeathBot?.Invoke();
        }
    }
}