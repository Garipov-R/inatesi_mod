using DG.Tweening.Core.Easing;
using InatesiCharacter.Testing.LeoEcs;
using System.Collections;
using UnityEngine;
using Zenject;

namespace InatesiCharacter.Testing.LeoEcs5
{
    public class StartEcsInstaller : MonoInstaller
    {
        [SerializeField] private StartEcs _StartEcs;


        public override void InstallBindings()
        {
            // Bind the GameManager and UIManager as single instances.
            // "FromComponentInHierarchy" tells Zenject to find them in the current scene.
            //Container.Bind<StartEcs>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<StartEcs>().FromInstance(_StartEcs).AsSingle().NonLazy();


            Container.CreateSubContainer();
        }
    }
}