using InatesiCharacter.Testing.LeoEcs;
using InatesiCharacter.Testing.Shared;
using Leopotam.EcsLite;
using System.Collections;
using UnityEngine;
using Zenject;


namespace InatesiCharacter.Testing.Stuff
{
    public class ZenjectInstaller : MonoInstaller
    {
        [SerializeField] private SetupLeoEcs _SetupLeoEcs;

        public override void InstallBindings()
        {
            Container.Bind<SetupLeoEcs>().FromInstance(_SetupLeoEcs).AsSingle().NonLazy();

            Container.CreateSubContainer();
        }
    }
}