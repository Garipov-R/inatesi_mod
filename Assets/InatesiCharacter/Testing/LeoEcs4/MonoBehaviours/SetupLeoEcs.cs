using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using System.Collections;
using UnityEngine;
using InatesiCharacter.Testing.LeoEcs.Shared;
using Zenject;
using InatesiCharacter.Testing.LeoEcs.Character.Componentts;
using InatesiCharacter.Testing.Shared.Components;
using InatesiCharacter.Testing.LeoEcs.Shared.Systems;

namespace InatesiCharacter.Testing.LeoEcs
{
    public class SetupLeoEcs : MonoBehaviour
    {
        [SerializeField] protected SharedData _sharedData = new SharedData();


        protected EcsWorld _ecsWorld;
        protected IEcsSystems _InitSystems;
        protected IEcsSystems _UpdateSystems;
        protected IEcsSystems _FixedUpdateSystems;
        protected IEcsSystems _LateUpdateSystems;

        public EcsWorld EcsWorld { get => _ecsWorld; }
        protected SharedData SharedData { get => _sharedData; set => _sharedData = value; }


        private void Awake()
        {
            Init();
        }

        public virtual void Init()
        {
            _ecsWorld = new EcsWorld();
            _InitSystems = new EcsSystems(_ecsWorld, SharedData);
            _UpdateSystems = new EcsSystems(_ecsWorld, SharedData);
            _FixedUpdateSystems = new EcsSystems(_ecsWorld);
            _LateUpdateSystems = new EcsSystems(_ecsWorld);


            _InitSystems.Init();
            _UpdateSystems.Init();
            _FixedUpdateSystems.Init();
            _LateUpdateSystems.Init();
        }

        private void Update()
        {
            _UpdateSystems.Run();
        }

        private void FixedUpdate()
        {
            _FixedUpdateSystems.Run();
        }

        private void LateUpdate()
        {
            _LateUpdateSystems.Run();
        }

        private void OnDestroy()
        {
            _InitSystems.Destroy();
            _UpdateSystems.Destroy();
            _FixedUpdateSystems.Destroy();
            _LateUpdateSystems.Destroy();
        }
    }
}