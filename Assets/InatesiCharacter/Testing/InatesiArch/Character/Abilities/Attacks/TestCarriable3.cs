using System.Collections;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.InatesiArch.Character.Abilities.Attacks
{
    public class TestCarriable3 : Carriable, IUse
    {
        TimeSince timeSinceAttack;
        private Rigidbody _currentRigidbody;


        public override void Enable()
        {
            //WeaponPoint.transform = ;
            //WeaponPoint.transform.GetChild(0).gameObject.SetActive(true);
            WeaponPoint.List[2].SetActive(true);
        }

        public override void Disable()
        {
            //WeaponPoint.transform = ;
            //WeaponPoint.transform.GetChild(0).gameObject.SetActive(false);
            WeaponPoint.List[2].SetActive(false);

            CharacterVars.RiggingTest.Rig.weight = 0;
        }

        public bool OnUse()
        {
            return false;
        }

        public override void Update()
        {
            timeSinceAttack += Time.deltaTime;

            if (Input.Pressed("Secondary Attack") && timeSinceAttack > 0.1f)
            {
                RaycastHit hit;
                var isHit = Physics.Raycast(
                    CharacterMotion.LookSource.LookPosition(),
                    CharacterMotion.LookSource.Transform.forward,
                    out hit,
                    15f,
                    LayerMask.NameToLayer("Everything"),
                    QueryTriggerInteraction.Ignore
                );

                if (isHit)
                {
                    if (hit.rigidbody != null)
                    {
                        _currentRigidbody = hit.rigidbody;
                    }
                }

                CharacterMotion.AnimatorMonitor.SetSlot0(100);
            }

            if (Input.Down("Secondary Attack") && timeSinceAttack > 0.1f)
            {
                if (_currentRigidbody != null)
                {
                    _currentRigidbody.position = 
                        CharacterMotion.LookSource.LookPosition() + CharacterMotion.LookSource.Transform.forward * 5f;
                    //_currentRigidbody.rotation = CharacterMotion.LookSource.Transform.rotation;
                    _currentRigidbody.centerOfMass = Vector3.zero;
                    _currentRigidbody.linearVelocity = Vector3.zero;
                }

                Rigging();
            }

            if (Input.Released("Secondary Attack"))
            {
                _currentRigidbody = null;
                CharacterVars.RiggingTest.Rig.weight = 0;
                CharacterMotion.AnimatorMonitor.SetSlot0(0);
            }

            if (Input.Pressed("Attack"))
            {
                if (_currentRigidbody != null)
                {
                    _currentRigidbody.AddForce(CharacterMotion.LookSource.Transform.forward * 30f, ForceMode.VelocityChange);
                    timeSinceAttack = 0;
                    _currentRigidbody = null;
                }
            }
        }

        private void Rigging()
        {
            CharacterVars.RiggingTest.Rig.weight = 1;
            CharacterVars.RiggingTest.SpineRig.transform.rotation =
                Quaternion.LookRotation(
                    CharacterMotion.LookSource.Transform.forward//,
                                                                //CharacterMotion.Up
                );

            CharacterVars.RiggingTest.RHandRig.transform.rotation =
                    Quaternion.LookRotation(
                        CharacterMotion.LookSource.Transform.right
                    //characterComponent.characterMotionTest.Up
                    );

            CharacterVars.RiggingTest.LHandRig.transform.rotation =
                Quaternion.LookRotation(
                    CharacterMotion.LookSource.Transform.right
                //characterComponent.characterMotionTest.Up
                );
        }
    }
}