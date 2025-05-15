using InatesiCharacter.Movements.SourceEngine;
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
    public class Jump : AbilityBase
    {
        TimeSince timeSinceJumpReleased;

        public override void Update()
        {
            timeSinceJumpReleased += Time.deltaTime;

            GetCharacterVars(out CharacterVars cv);

            if (cv.Flying == true) return;


            var bhop = _CharacterMotion.MoveConfig.AutoBhop ? Inatesi.Inputs.Input.Down("Jump") : Inatesi.Inputs.Input.Pressed("Jump");
            if (bhop)
            {

                if (timeSinceJumpReleased > 0.2f && _CharacterMotion.OnGrounded == true)
                {
                    Debug.Log("Jump");
                    _CharacterMotion.AddForce(_CharacterMotion.Up * _CharacterMotion.MoveConfig.JumpForce);
                    _CharacterMotion.AnimatorMonitor.SetAbilityID(1);

                    timeSinceJumpReleased = 0;

                    cv.Jumped = true;
                }
            }

            if (timeSinceJumpReleased > 0.1f &&
                //_CharacterMotion.CheckGround() == true &&
                cv.Jumped == true)
            {
                timeSinceJumpReleased = 0;
                cv.Jumped = false;

                if (cv.Crouched == true)
                {
                    _CharacterMotion.AnimatorMonitor.SetAbilityID(4);
                }
                else
                {
                    _CharacterMotion.AnimatorMonitor.SetAbilityID(0);
                }
            }

            ApplyCharacterVars(cv);
        }
    }
}
