using InatesiCharacter.Testing.LeoEcs5.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Utility
{
    public class GameLogicBase : MonoBehaviour, IGameLogic
    {
        public virtual bool PlayerAlive { get; set; }

        public virtual void GameOver()
        {
            
        }

        public virtual void OnPlayerCollision(CollisionComponentEvent collisionEvent)
        {

        }

        public virtual void OnPlayerDead()
        {
        }

        public virtual void Pause()
        {
        }

        public virtual void RestartGame()
        {
        }

        public virtual void StartGame()
        {
        }

        public virtual void UpdateUI()
        {

        }
    }
}
