using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace InatesiCharacter.Testing.Character.Bot3
{
    public class BotBehaviourBase : MonoBehaviour
    {
        [SerializeField] private int _Health = 1;
        [SerializeField] private Animator _Animator;

        [Header("Effects")]
        [SerializeField] private VisualEffect _DamageVisualEffect;

        public int Health { get => _Health; set => _Health = value; }
        public VisualEffect DamageVisualEffect { get => _DamageVisualEffect; set => _DamageVisualEffect = value; }
        public Animator Animator { get => _Animator; set => _Animator = value; }


        public void Damage()
        {
            _Animator.SetTrigger("damage");
        }

        public void Die()
        {
        }
    }
}