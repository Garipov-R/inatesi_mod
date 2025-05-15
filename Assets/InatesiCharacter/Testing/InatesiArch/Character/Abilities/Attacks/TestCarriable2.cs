using System;
using System.Collections;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.InatesiArch.Character.Abilities.Attacks
{
    [Serializable]
    public class TestCarriable2 : Carriable, IUse
    {
        [SerializeField] private float _AttackDelay = 2f;

        TimeSince timeSinceAttack;


        public override void Enable()
        {
            //WeaponPoint.transform = ;
            //WeaponPoint.transform.GetChild(0).gameObject.SetActive(true);
            WeaponPoint.List[1].SetActive(true);
        }

        public override void Disable()
        {
            //WeaponPoint.transform = ;
            //WeaponPoint.transform.GetChild(0).gameObject.SetActive(false);
            WeaponPoint.List[1].SetActive(false);

            CharacterVars.RiggingTest.Rig.weight = 0;

        }

        public override void Update()
        {
            timeSinceAttack += Time.deltaTime;


            if (Input.Pressed("Attack") && timeSinceAttack > _AttackDelay)
            {
                CharacterMotion.AnimatorMonitor.SetSlot0(0);

                timeSinceAttack = 0;
                CharacterMotion.AnimatorMonitor.SetSlot0(102);

                Rigging();



                RaycastHit hit;
                var isHit = Physics.Raycast(
                    CharacterMotion.LookSource.LookPosition(),
                    CharacterMotion.LookSource.Transform.forward,
                    out hit,
                    100f,
                    LayerMask.NameToLayer("Everything"),
                    QueryTriggerInteraction.Ignore
                );                                       

                if (isHit)
                {
                    if (hit.rigidbody != null)
                    {
                        hit.rigidbody.AddForce(CharacterMotion.LookSource.Transform.forward * 30f, ForceMode.VelocityChange);
                    }
                }
            }

            if (Input.Down("Secondary Attack"))
            {
                Rigging();

                CharacterMotion.AnimatorMonitor.SetSlot0(102);
            }

            if (Input.Released("Secondary Attack"))
            {
                CharacterVars.RiggingTest.Rig.weight = 0;
                CharacterMotion.AnimatorMonitor.SetSlot0(0);
            }

            if (Input.Down("Secondary Attack") == false && timeSinceAttack > _AttackDelay)
            {
                CharacterVars.RiggingTest.Rig.weight = 0;
                CharacterMotion.AnimatorMonitor.SetSlot0(0);
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


        public bool OnUse()
        {
            throw new NotImplementedException();
        }
    }
}