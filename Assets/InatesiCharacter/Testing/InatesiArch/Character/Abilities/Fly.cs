using InatesiCharacter.Movements.SourceEngine;
using System;
using System.Collections;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.InatesiArch.Character.Abilities
{
    public class Fly : AbilityBase
    {
        TimeSince timeSinceFlySwitch;


        public override void Update()
        {
            timeSinceFlySwitch += Time.deltaTime;

            GetCharacterVars(out CharacterVars cv);



            if (Input.Released("Jump"))
            {
                if (timeSinceFlySwitch < 0.3f)
                {
                    cv.Flying = !cv.Flying;

                    cv.Crouched = false;
                    cv.Jumped = false;

                    CharacterBase.CharacterMotion.GravityFactor = cv.Flying ? 0 : 1f;

                    if (cv.Flying == true)
                    {
                        CharacterBase.CharacterMotion.AnimatorMonitor.SetAbilityID(5);
                        CharacterBase.CharacterMotion.Velocity = Vector3.zero;
                    }
                    else
                    {
                        CharacterBase.CharacterMotion.AnimatorMonitor.SetAbilityID(0);
                    }

                    CharacterMotion.GetComponent<Collider>().isTrigger = cv.Flying;
                }

                timeSinceFlySwitch = 0;
            }

            ApplyCharacterVars(cv);


            if (cv.Flying)
            {
                var wishDir = Vector3.zero;
                if (Math.Abs(Input.GetVector("Move").y) > 0)
                {
                    var heightVector =
                        CharacterBase.CharacterMotion.transform.rotation *
                        CharacterBase.CharacterMotion.LookSource.Transform.forward * Input.GetVector("Move").y * 40f;



                    wishDir += CharacterBase.CharacterMotion.Up * heightVector.normalized.y;
                }

                if (Input.Down("Jump"))
                {
                    wishDir += CharacterBase.CharacterMotion.Up * 40;
                }

                if (Input.Down("Crouch"))
                {
                    wishDir += CharacterBase.CharacterMotion.Up * -40;
                }

                CharacterBase.CharacterMotion.Velocity = SurfPhysics.ApplyFriction(CharacterBase.CharacterMotion.Velocity, 2);
                CharacterBase.CharacterMotion.Velocity = SurfPhysics.WithAirAcceleration(wishDir, CharacterBase.CharacterMotion.Velocity);
            }

            if (CharacterBase.CharacterMotion.InputVector.y != 0 || CharacterBase.CharacterMotion.InputVector.x != 0f)
            {
                timeSinceFlySwitch = 1;
            }




            return;


            if (Input.Released("Jump"))
            {
                if (timeSinceFlySwitch < 0.3f)
                {
                    cv.Flying = !cv.Flying;

                    cv.Crouched = false;
                    cv.Jumped = false;

                    CharacterBase.CharacterMotion.GravityMagnitude = cv.Flying ? 0 : 9.81f;
                    CharacterBase.CharacterMotion.SpeedMove =
                        cv.Flying ? CharacterBase.CharacterMotion.SpeedMove * 2 : CharacterBase.CharacterMotion.SpeedMove / 2;

                    if (cv.Flying == true)
                    {
                        CharacterBase.CharacterMotion.AnimatorMonitor.SetAbilityID(5);
                    }
                    else
                    {
                        CharacterBase.CharacterMotion.AnimatorMonitor.SetAbilityID(0);
                    }
                }

                timeSinceFlySwitch = 0;
            }

            ApplyCharacterVars(cv);

            var inputVector3 = CharacterBase.CharacterMotion.InputVector3;

            if (cv.Flying)
            {
                if (Math.Abs(Input.GetVector("Move").y) > 0)
                {
                    var heightVector =
                        CharacterBase.CharacterMotion.transform.rotation *
                        CharacterBase.CharacterMotion.LookSource.Transform.forward * Input.GetVector("Move").y;

                    inputVector3 = new Vector3(inputVector3.x, heightVector.y, inputVector3.z);
                }
                else
                {
                    inputVector3 = new Vector3(inputVector3.x, 0, inputVector3.z);
                }

                if (Input.Down("Jump"))
                {
                    inputVector3 = new Vector3(inputVector3.x, 1, inputVector3.z);
                }

                if (Input.Down("Crouch"))
                {
                    inputVector3 = new Vector3(inputVector3.x, -1, inputVector3.z);
                }
            }
            else
            {
                inputVector3 = new Vector3(inputVector3.x, 0, inputVector3.z);
            }

            CharacterBase.CharacterMotion.InputVector3 = inputVector3;

            if (CharacterBase.CharacterMotion.InputVector.y != 0 || CharacterBase.CharacterMotion.InputVector.x != 0f)
            {
                timeSinceFlySwitch = 1;
            }
        }
    }
}