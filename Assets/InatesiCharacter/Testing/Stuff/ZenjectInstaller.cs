using InatesiCharacter.Testing.LeoEcs3;
using Leopotam.EcsLite;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;


namespace InatesiCharacter.Testing.Stuff
{
    public class ZenjectInstaller : MonoInstaller
    {
        [SerializeField] private SetupLeoEcs _SetupLeoEcs;

        public override void InstallBindings()
        {
            //Container.BindInstance(_SetupLeoEcs).AsSingle().NonLazy();
            Container.Bind<SetupLeoEcs>().FromInstance(_SetupLeoEcs).AsSingle().NonLazy();


            Container.Bind<string>().FromInstance("Hello World!");
            Container.Bind<Services>().AsSingle().NonLazy();

            Container.CreateSubContainer();


            //Container.Bind<IInitializable>().FromInstance(_SetupLeoEcs).AsSingle().NonLazy();
            //Container.Bind<SetupLeoEcs>().FromInstance(_SetupLeoEcs).AsSingle().NonLazy();
        }
    }

    [System.Serializable]
    public class Services
    {
        public Services(string message)
        {
            Debug.Log(message);
        }
    }
}