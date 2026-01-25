using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InatesiCharacter.Testing.Character.Bots
{
    public class SimpleBot : BotBehaviorBase2
    {
        [Space(20)]
        [SerializeField] private float _damageStayTime = .4f;
        [SerializeField] protected float _AttackDelay = 1f;
        [SerializeField] private List<AudioClip> _AttackClips = new();

        protected float _attackSinceTime = 0;
        protected float _damagedSinceTime = 0;
        protected Vector2 _move = Vector2.zero;

        public override void Enabled()
        {
            _attackSinceTime = _AttackDelay;
        }

        public override void Damage()
        {
            //base.Damage();
            _attackSinceTime = _AttackDelay;
            _damagedSinceTime = _damageStayTime;
        }

        public override void Died()
        {
            base.Died();

            
        }

        public override void UpdateTick()
        {
            base.UpdateTick();

            if (Target == null || GameSettings.IsPause)
            {
                CharacterMotion.Move(Vector2.zero);
                return;
            }

            if (_damagedSinceTime >= 0)
            {
                _damagedSinceTime -= Time.deltaTime;
            }


            var move = _walkPoint - Transform.position;
            move = Vector2.ClampMagnitude(new Vector2(move.x, move.z), 1);

            var dot = Vector3.Dot(Transform.forward, (Target.transform.position - Transform.position).normalized) > .2f;

            if (Vector3.Distance(transform.position, Target.transform.position) < _stopDistance + 10)
            {
                CharacterMotion.AnimatorMonitor.SetSlot0(0);
                _attackSinceTime -= Time.deltaTime;
            }

            if (Vector3.Distance(transform.position, Target.transform.position) < (CharacterMotion.Radius) + _stopDistance && dot)
            {
                Attack();
            }

            if (
                _walkPointSet == false && 
                Vector3.Distance(transform.position, _walkPoint) < (CharacterMotion.Radius)
            )
            {
                move = Vector3.zero;
            }

            if (_walkPointSet == false)
            {
                move = Vector3.zero;
            }

            if (_damagedSinceTime > 0)
            {
                CharacterMotion.Move(Vector2.zero);
            }
            else
            {
                CharacterMotion.Move(move);
            }





            /*for (int i = 0; i < NavMeshPath.corners.Length - 1; i++)
            {
                if (i == NavMeshPath.corners.Length) continue;
                Debug.DrawLine(NavMeshPath.corners[i], NavMeshPath.corners[i + 1], Color.red);
            }*/
        }

        protected override void Attack()
        {
            if (_attackSinceTime > 0)
                return;

            _attackSinceTime = _AttackDelay;

            if (_AttackClips != null && _AttackClips.Count > 0)
            {
                CharacterMotion.AudioSource.PlayOneShot(_AttackClips[Random.Range(0, _AttackClips.Count)]);
            }

            CharacterMotion.AnimatorMonitor.SetSlot0(101);

            base.Attack();
        }
    }
}