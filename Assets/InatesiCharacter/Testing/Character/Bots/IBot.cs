using InatesiCharacter.SuperCharacter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace InatesiCharacter.Testing.Character.Bots
{
    public interface IBot
    {
        public abstract float StopDistance { get; set; }
        public abstract GameObject GameObject { get; set; }
        public abstract Transform Transform { get; set; }
        public abstract NavMeshPath NavMeshPath { get; set; }
        public abstract CharacterMotionBase CharacterMotion { get; set; }
        public abstract GameObject Target { get; set; }
        public abstract bool IsEnabled { get; set; }


        public abstract void Enabled();
        public abstract void UpdateTick();

        public abstract Vector2 PathInput(Vector3 playerPosition, Vector3 botPosition, NavMeshPath navMeshPath, float distance = 2);
        public abstract void Damage();
        public abstract void Died();
    }
}
