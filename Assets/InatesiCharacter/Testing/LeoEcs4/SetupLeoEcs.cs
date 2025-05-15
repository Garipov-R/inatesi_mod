using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using System.Collections;
using UnityEngine;
using InatesiCharacter.Testing.LeoEcs3.Shared;
using Zenject;
using InatesiCharacter.Testing.LeoEcs3.Character.Componentts;
using InatesiCharacter.Testing.Shared.Components;
using InatesiCharacter.Testing.LeoEcs3.Shared.Systems;

namespace InatesiCharacter.Testing.LeoEcs3
{
    public class SetupLeoEcs : MonoBehaviour
    {
        [SerializeField] protected SharedData _sharedData = new SharedData();


        protected EcsWorld _world;
        protected IEcsSystems _InitSystems;
        protected IEcsSystems _UpdateSystems;
        protected IEcsSystems _FixedUpdateSystems;

        public EcsWorld World { get => _world; }
        protected SharedData SharedData { get => _sharedData; set => _sharedData = value; }


        private void Awake()
        {
            Init();
        }

        public virtual void Init()
        {
            _world = new EcsWorld();
            _InitSystems = new EcsSystems(_world, SharedData);
            _UpdateSystems = new EcsSystems(_world, SharedData);
            _FixedUpdateSystems = new EcsSystems(_world);


            _InitSystems.Init();
            _UpdateSystems.Init();
            _FixedUpdateSystems.Init();
        }

        private void Update()
        {
            _UpdateSystems.Run();
        }

        private void FixedUpdate()
        {
            _FixedUpdateSystems.Run();
        }

        private void OnDestroy()
        {
            _InitSystems.Destroy();
            _UpdateSystems.Destroy();
            _FixedUpdateSystems.Destroy();
        }
    }
}