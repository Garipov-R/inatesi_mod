using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InatesiCharacter.Testing.Character.InteractionSystem
{
    public class GameObjectTimer : MonoBehaviour
    {
        [SerializeField] private float _returnTime = 30f;

        private Dictionary<GameObject, float> _TimerGameObjects = new Dictionary<GameObject, float>();

        public float ReturnTime { get => _returnTime; set => _returnTime = value; }


        private void Update()
        {
            foreach (var item in _TimerGameObjects)
            {
                if (item.Key == null)
                {
                    continue;
                }

                if (item.Key.activeSelf == false)
                {
                    continue;
                }

                if (_TimerGameObjects.TryGetValue(item.Key, out float time))
                {
                    time -= Time.deltaTime;

                    if (time <= _returnTime)
                    {
                        item.Key.SetActive(true);
                    }

                    _TimerGameObjects[item.Key] = time;
                }
            }
        }

        public void AddGameObject(GameObject addGameObject)
        {
            _TimerGameObjects.Add(addGameObject, _returnTime);
        }
    }
}