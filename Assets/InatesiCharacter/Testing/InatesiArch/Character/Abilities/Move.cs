using InatesiCharacter.SuperCharacter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Input = Inatesi.Inputs.Input;


namespace InatesiCharacter.Testing.InatesiArch.Character.Abilities
{
    public class Move : AbilityBase
    {
        public override void Update()
        {
            var move = Input.GetVector("Move");

            if (move.magnitude > 0.3f)
            {
                _CharacterMotion.Move(Input.GetVector("Move"));
            }
            else
            {
                _CharacterMotion.Move(Vector2.zero);
            }
        }
    }
}
