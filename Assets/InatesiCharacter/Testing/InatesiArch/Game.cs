using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InatesiCharacter.Testing.InatesiArch.GameManager;

namespace InatesiCharacter.Testing.InatesiArch
{
    public class Game : GameBase
    {
        private Player _player;

        public Player Player { get => _player; }

        public static Player PlayerInstance { get; private set; }


        public override void ClientJoined(string name)
        {
            base.ClientJoined(name);

            var player = new Player(name);
            player.Respawn();

            _player = player;

            PlayerInstance = player;
        }
    }
}
