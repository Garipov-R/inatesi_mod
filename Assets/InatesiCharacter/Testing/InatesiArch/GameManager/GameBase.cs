using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.InatesiArch.GameManager
{
    public abstract class GameBase
    {
        private bool isServer;

        public bool IsServer { get => isServer; set => isServer = value; }


        public GameBase()
        {
            Debug.Log("=== Game");
        }

        public virtual void ClientJoined(string name)
        {

        }
    }
}
