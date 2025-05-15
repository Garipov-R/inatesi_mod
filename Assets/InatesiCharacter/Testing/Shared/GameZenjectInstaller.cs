using System.Collections;
using UnityEngine;
using Zenject;

namespace InatesiCharacter.Testing.Shared
{
    public class GameZenjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            //Container.Bind<GameSettings>().
        }
    }
}