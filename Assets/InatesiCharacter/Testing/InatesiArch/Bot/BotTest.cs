using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace InatesiCharacter.Testing.InatesiArch.Bot
{
    public class BotTest : MonoBehaviour
    {
        [SerializeField] private float _HP = 1;
        [SerializeField] private SuperCharacter.CharacterMotion _CharacterMotion;
        [SerializeField] private float _Timer = 5f;

        private UnityEngine.AI.NavMeshAgent _NavMeshAgent;

        private float _amountHp;

        public NavMeshAgent NavMeshAgent { get => _NavMeshAgent; set => _NavMeshAgent = value; }
        public float AmountHp { get => _amountHp; set => _amountHp = value; }

        private void Awake()
        {
            _amountHp = _HP;

            _NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }



        public void Damage(float amount)
        {
            if (_amountHp <= 0)
                return;

            _amountHp -= amount;

            if (_amountHp <= 0)
            {
                if (_CharacterMotion != null)
                {
                    _CharacterMotion.RagdollMonitor.EnableRagdoll();
                    _NavMeshAgent.enabled = false;
                    StartCoroutine(Timer());
                }
            }
        }

        IEnumerator Timer()
        {
            yield return new WaitForSeconds(_Timer);

            _CharacterMotion.RagdollMonitor.DisableRagdoll();

            _amountHp = _HP;
            _NavMeshAgent.enabled = true;
        }
    }
}