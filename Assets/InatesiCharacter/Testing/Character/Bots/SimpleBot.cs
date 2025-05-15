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

            
            var render = GameObject.GetComponentInChildren<Renderer>();

            for (int i = 0; i < transform.childCount; i++)
            {
                Debug.Log(transform.GetChild(i).name);
            }

            if (render != null)
            {
                render.material.SetColor("_Color", Color.red);
            }
        }

        public override void UpdateTick()
        {
            if (Target == null || GameSettings.IsPause)
            {
                CharacterMotion.Move(Vector2.zero);
                return;
            }

            if (_damagedSinceTime >= 0)
            {
                _damagedSinceTime -= Time.deltaTime;
            }

            var buildPath = BuildPath(Target.transform.position, Transform.position, out Vector3 movePosition);
            if (buildPath)
            {
                SetWalkPoint(movePosition);
            }

            bool obstacle = NavMeshPath.corners != null && NavMeshPath.corners.Length > 1;
            var move = movePosition - Transform.position;
            move = Vector2.ClampMagnitude(new Vector2(move.x, move.z), 1);
            //move.Normalize();
            _move = move;

            var dot = Vector3.Dot(Transform.forward, (Target.transform.position - Transform.position).normalized) > .9f;

            if (Vector3.Distance(transform.position, Target.transform.position) < _stopDistance + 10)
            {
                _attackSinceTime -= Time.deltaTime;
            }

            if (Vector3.Distance(transform.position, Target.transform.position) < (CharacterMotion.Radius / 2) + _stopDistance && dot)
            {
                move = Vector3.zero;

                Attack();
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

            base.Attack();
        }
    }
}