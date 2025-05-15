using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Character.Bots
{
    public class ShooterBot : SimpleBot
    {
        [SerializeField] protected GameObject _Projectile;

        public override void Enabled()
        {
            _attackSinceTime = 3;
        }

        protected override void Attack()
        {
            if (_attackSinceTime > 0)
                return;


            if (_Projectile == null) return;
            if (Target == null) return;


            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 toOther = Vector3.Normalize(Target.transform.position - transform.position);
            var dot = Vector3.Dot(forward, toOther);

            //if (dot < 0) return;

            var position = CharacterMotion.transform.position + toOther + CharacterMotion.Up * CharacterMotion.Height;

            var projectile = Instantiate(_Projectile, position, Quaternion.identity);
            
            if (projectile.TryGetComponent(out Rigidbody component))
            {
                var ballistic = Inatesi.Utilities.MathUtility.Ballistic(Target.transform.position, position, 40f, Physics.gravity.y);
                projectile.transform.rotation = Inatesi.Utilities.MathUtility.LookTarget(Target.transform, transform);
                projectile.transform.eulerAngles = new Vector3(-40f, projectile.transform.eulerAngles.y, projectile.transform.eulerAngles.z);
                component.AddForce((projectile.transform.forward) * ballistic, ForceMode.Impulse);
            }

            base.Attack();

            _attackSinceTime = _AttackDelay;
        }

        public override void UpdateTick()
        {
            if (_damagedSinceTime >= 0)
            {
                _damagedSinceTime -= Time.deltaTime;
            }

            if (_attackSinceTime >= 0)
                _attackSinceTime -= Time.deltaTime;


            if (Target == null)
            {
                CharacterMotion.Move(Vector2.zero);
                return;
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

            bool cast;
            cast = Physics.SphereCast(
                    transform.position + (CharacterMotion.Up * (CharacterMotion.Height / 2)),
                    CharacterMotion.Radius,
                    (Target.transform.position + (CharacterMotion.Up * (CharacterMotion.Height / 2)) - Transform.position).normalized,
                    out RaycastHit hitInfo,
                    100f,
                    LayerMask.NameToLayer("Everything"),
                    QueryTriggerInteraction.Ignore
                );

            if (cast)
            {
                cast = hitInfo.transform == Target.transform;
            }
            else
            {
                cast = false;
            }

            if (Vector3.Distance(transform.position, Target.transform.position) < (CharacterMotion.Radius / 2) + _stopDistance && dot && cast)
            {
                move = Vector3.zero;
            }

            if (cast && _attackSinceTime < 0)
            {
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
    }
}