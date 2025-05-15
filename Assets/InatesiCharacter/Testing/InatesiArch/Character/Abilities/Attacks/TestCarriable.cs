using System.Collections;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.InatesiArch.Character.Abilities.Attacks
{
    public class TestCarriable : Carriable, IUse
    {
        TimeSince timeSinceAttack;


        public override void Enable()
        {
            //WeaponPoint.transform = ;
            //WeaponPoint.transform.GetChild(0).gameObject.SetActive(true);
            WeaponPoint.List[0].SetActive(true);
        }

        public override void Disable()
        {
            //WeaponPoint.transform = ;
            //WeaponPoint.transform.GetChild(0).gameObject.SetActive(false);
            WeaponPoint.List[0].SetActive(false);
        }            

        public bool OnUse()
        {
            return false;
        }

        public override void PrimaryAttack()
        {
            Debug.Log("babax");

            CharacterMotion.AnimatorMonitor.SetSlot0(100);
        }

        public override void Update()
        {
            timeSinceAttack += Time.deltaTime;

            if ((Input.Down("Attack") || Input.Pressed("Attack")) && timeSinceAttack > Configs.Configs.c_AttackDelay)
            {
                var casts = Physics.SphereCastAll(
                    CharacterMotion.transform.position + CharacterMotion.Up * 1, 
                                                            .5f, 
                    CharacterMotion.transform.forward, 
                    2f, 
                    CharacterMotion.RaycastLayer, 
                    QueryTriggerInteraction.Ignore
                );

                for (int i = 0; i < casts.Length; i++)
                {
                    var rb = casts[i].rigidbody;

                    if (rb != null)
                    {
                        rb.AddForce((casts[i].point - CharacterMotion.transform.position).normalized * 10, ForceMode.VelocityChange);
                    }
                }

                CharacterMotion.AnimatorMonitor.SetSlot0(0);

                timeSinceAttack = 0;
                CharacterMotion.AnimatorMonitor.SetSlot0(101);
            }

            if (timeSinceAttack > Configs.Configs.c_AttackDelay + 0.1f)
            {
                CharacterMotion.AnimatorMonitor.SetSlot0(0);
            }
        }
    }
}