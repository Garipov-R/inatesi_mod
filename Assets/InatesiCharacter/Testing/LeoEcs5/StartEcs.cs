using InatesiCharacter.Testing.LeoEcs5.Components;
using InatesiCharacter.Testing.LeoEcs5.PoolSystems;
using InatesiCharacter.Testing.LeoEcs5.Systems;
using Leopotam.EcsLite;
using Leopotam.EcsLite.ExtendedSystems;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5
{
    public class StartEcs : MonoBehaviour
    {
        [SerializeField] private SharedData _SharedData;

        protected EcsWorld _ecsWorld;
        protected IEcsSystems _InitSystems;
        protected IEcsSystems _UpdateSystems;
        protected IEcsSystems _FixedUpdateSystems;
        protected IEcsSystems _LateUpdateSystems;

        public EcsWorld EcsWorld { get => _ecsWorld; set => _ecsWorld = value; }


        private void Awake()
        {
            
        }

        private void Start()
        {
            _ecsWorld = new EcsWorld();
            _InitSystems = new EcsSystems(_ecsWorld, _SharedData);
            _UpdateSystems = new EcsSystems(_ecsWorld, _SharedData);
            _FixedUpdateSystems = new EcsSystems(_ecsWorld, _SharedData);
            _LateUpdateSystems = new EcsSystems(_ecsWorld, _SharedData);

            _UpdateSystems
                .Add(new PlayerWeaponActionsSystem())
                .Add(new PlayerSystem())
                .Add(new BotsSystem())
                .Add(new HitEffectsSystem())
                .Add(new ObjectPoolManagerSystem())

                .DelHere<DamageComponent>()
                .DelHere<ParticleEvent>()
                .DelHere<ObjectPoolSendEvent>()
            ;

            _FixedUpdateSystems
                .Add(new PlayerFixedUpdateSystem())
            ;
            _InitSystems.Init();
            _UpdateSystems.Init();
            _FixedUpdateSystems.Init();
            _LateUpdateSystems.Init();
        }

        private void Update()
        {
            if (_UpdateSystems == null) return;

            _UpdateSystems.Run();
        }

        private void FixedUpdate()
        {
            if (_FixedUpdateSystems == null) return;

            _FixedUpdateSystems.Run();
        }

        private void LateUpdate()
        {
            if (_LateUpdateSystems == null) return;

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