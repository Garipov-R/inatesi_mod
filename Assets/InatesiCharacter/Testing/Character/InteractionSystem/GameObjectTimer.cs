using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InatesiCharacter.Testing.Character.InteractionSystem
{
    public class GameObjectTimer : MonoBehaviour
    {
        [SerializeField] private float _returnTime = 30f;

        private Dictionary<GameObject, float> _TimerGameObjects = new Dictionary<GameObject, float>();
        private List<GameObject> _GameObjects = new();

        public float ReturnTime { get => _returnTime; set => _returnTime = value; }


        private void Update()
        {
            if (_TimerGameObjects == null)
            {
                return;
            }

            if (_TimerGameObjects.Count == 0)
            {
                return;
            }

            if (_GameObjects == null)
            {
                return;
            }

            if (_GameObjects.Count == 0)
            {
                return;
            }

            foreach (var item in _GameObjects)
            {
                if (item == null)
                {
                    continue;
                }

                if (item.activeSelf == true)
                {
                    continue;
                }

                if (_TimerGameObjects.TryGetValue(item, out float time))
                {
                    if (time <= 0)
                    {
                        item.SetActive(true);
                    }

                    time -= Time.deltaTime;

                    _TimerGameObjects[item] = time;
                }
            }
        }

        public void AddGameObject(GameObject addGameObject)
        {
            if (_GameObjects.Contains(addGameObject) == false)
            {
                _GameObjects.Add(addGameObject);
            }

            if (_TimerGameObjects.ContainsKey(addGameObject) == true)
            {
                _TimerGameObjects[addGameObject] = _returnTime;
                Debug.Log(_TimerGameObjects[addGameObject]);
            }
            else
            {
                _TimerGameObjects.Add(addGameObject, _returnTime);
            }
        }
    }
}