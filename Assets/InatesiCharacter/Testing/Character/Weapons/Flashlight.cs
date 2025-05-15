using Inatesi.Inputs;
using System.Collections;
using UnityEngine;
using Input = Inatesi.Inputs.Input;

namespace InatesiCharacter.Testing.Character.Weapons
{
    public class Flashlight : WeaponBase
    {
        [SerializeField] private Light _Light;


        public override void Enable()
        {
            base.Enable();

            _Light.enabled = true;

            _Light.transform.SetParent(CameraMotion.transform);
            
            _Light.transform.localPosition = Vector3.zero;
            _Light.transform.localRotation = Quaternion.identity;   
        }

        public override void UpdateTick()
        {
            base.UpdateTick();


            if (Input.Pressed("Attack"))
            {
                if (_Light != null)
                {
                    _Light.enabled = !_Light.enabled;
                }
            }
            
        }

        public override void Disable()
        {
            if (_Light != null)
                Destroy(_Light.gameObject);


            base.Disable();
        }
    }
}