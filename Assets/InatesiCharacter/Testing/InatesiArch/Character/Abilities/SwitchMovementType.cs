using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.InatesiArch.Character.Abilities
{
    public class SwitchMovementType : AbilityBase
    {
        public override void Update()
        {
            if (Input.Pressed("Change"))
            {
                _CharacterMotion.SetMovementType();
            }
        }
    }
}
