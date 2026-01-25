using InatesiCharacter.Testing.Character.Data;
using InatesiCharacter.Testing.LeoEcs;
using InatesiCharacter.Testing.LeoEcs4.Events;
using System.Collections;
using UnityEngine;
using Zenject;

namespace InatesiCharacter.Testing.LeoEcs4.MonoBehaviours
{
    public class SpawnCharacter : MonoBehaviour
    {
        [SerializeField] private CharacterSO _CharacterSO;
        [SerializeField] private Transform _SpawnPoint;

        protected SetupLeoEcs _SetupLeoEcs;


        [Inject]
        protected virtual void Construct(SetupLeoEcs setupLeoEcs)
        {
            _SetupLeoEcs = setupLeoEcs;
        }

        public void Spawn()
        {
            if (_SetupLeoEcs == null)
                return;

            if (_SpawnPoint == null)
                return;

            var entity = _SetupLeoEcs.EcsWorld.NewEntity();

            var spawnComponentEventPool = _SetupLeoEcs.EcsWorld.GetPool<SpawnComponentEvent>();
            spawnComponentEventPool.Add(entity);
            ref var spawnComponentEvent = ref spawnComponentEventPool.Get(entity);
            spawnComponentEvent.data = _CharacterSO;
            spawnComponentEvent.entity = entity;
            spawnComponentEvent.position = _SpawnPoint.position;
        }
    }
}