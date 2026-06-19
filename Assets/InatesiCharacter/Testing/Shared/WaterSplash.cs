using InatesiCharacter.Testing.LeoEcs5.Utility;
using InatesiCharacter.Testing.LeoEcs5;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using InatesiCharacter.Testing.LeoEcs5.PoolSystems;
using Zenject;
using System.Linq;

namespace InatesiCharacter.Testing.Shared
{
    public class WaterSplash : MonoBehaviour
    {
        [Header("Effects")]
        [SerializeField] private VisualEffect _SplashVfx;
        [Inject] private StartEcs _StartEcs;

        private List<GameObject> _enteredGameObjects = new();
        private Dictionary<GameObject, Vector3> _EnteredGameObject = new Dictionary<GameObject, Vector3>();
        private float _TimeSince;
        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (LayerMask.LayerToName(other.gameObject.layer) == "Camera")
            {
                return;
            }

            if (_EnteredGameObject.ContainsKey(other.gameObject) == false)
            {
                _EnteredGameObject.Add(other.gameObject, other.transform.position); 
                SpawnSplash(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_EnteredGameObject.ContainsKey(other.gameObject) == true)
            {
                if (_EnteredGameObject.TryGetValue(other.gameObject, out Vector3 oldPosition) == true)
                {
                    if (Vector3.Distance(other.transform.position, oldPosition) > 1.4f)
                    {
                        SpawnSplash(other);
                        _EnteredGameObject[other.gameObject] = other.transform.position;
                    }
                }
                _EnteredGameObject.Remove(other.gameObject);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (_EnteredGameObject.TryGetValue(other.gameObject, out Vector3 oldPosition) == true)
            {
                var positionBound = _collider.bounds.ClosestPoint(new Vector3(other.transform.position.x, _collider.bounds.extents.y, other.transform.position.z));
                Debug.DrawLine(positionBound, other.transform.position, Color.red, 2f);
                if (Vector3.Distance(other.transform.position, oldPosition) > 1.4f && Vector3.Distance(other.transform.position, positionBound) < 1.4f)
                {
                    SpawnSplash(other);
                    _EnteredGameObject[other.gameObject] = other.transform.position;
                }
                else
                {
                    //_EnteredGameObject[other.gameObject] = other.transform.position;
                }
            }
        }

        private void SpawnSplash(Collider other)
        {
            if (_SplashVfx != null)
            {
                ref var sendEvent = ref ECSHelper.Create<ObjectPoolSendEvent>(_StartEcs.EcsWorld);
                sendEvent.parent = gameObject.transform;
                sendEvent.objectToSpawn = _SplashVfx.gameObject;
                
                sendEvent.position = _collider.bounds.ClosestPoint(new Vector3(other.transform.position.x, _collider.bounds.extents.y, other.transform.position.z));
                sendEvent.poolType = PoolType.Particle;
                //other.ClosestPointOnBounds(other.transform.position);
            }
        }
    }
}