using InatesiCharacter.Testing.LeoEcs5.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InatesiCharacter.Testing.LeoEcs5.Utility
{
    public interface IGameLogic
    {
        public bool PlayerAlive { get; set; }

        public void StartGame();

        public void GameOver();

        public void Pause();

        public void RestartGame();

        public void OnPlayerDead();

        public void OnPlayerCollision(CollisionComponentEvent collisionEvent);

        public void UpdateUI();
    }
}
