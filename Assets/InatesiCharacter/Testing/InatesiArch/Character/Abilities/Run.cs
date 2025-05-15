using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.InatesiArch.Character.Abilities
{
    public class Run : AbilityBase
    {
        public override void Update()
        {
            if (Input.Pressed("Run"))
            {
                _CharacterMotion.SpeedMove = _CharacterMotion.SpeedMove * 1.5f;
            }

            if (Input.Released("Run"))
            {
                _CharacterMotion.SpeedMove = _CharacterMotion.SpeedMove / 1.5f;
            }
        }
    }
}
