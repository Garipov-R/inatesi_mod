using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace InatesiCharacter.Testing.Character.Weapons
{
    public class WeaponThirdPersonMono : MonoBehaviour
    {
        [SerializeField] private VisualEffect _ShootEffect;

        public VisualEffect ShootEffect { get => _ShootEffect; set => _ShootEffect = value; }


        public void Shoot()
        {
            if (_ShootEffect)
            {
                _ShootEffect.Play();
            }
        }
    }
}