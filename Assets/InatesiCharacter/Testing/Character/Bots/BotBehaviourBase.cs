using InatesiCharacter.SuperCharacter;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.AI;

namespace InatesiCharacter.Testing.Character.Bots
{
    public class BotBehaviorBase : MonoBehaviour
    {
        [SerializeField] private Animator _Animator;
        [SerializeField] private NavMeshAgent _NavMeshAgent;
        [SerializeField] private float _range = 1f;

        private Vector2 _SmoothDeltaPosition;
        private Vector2 _Velocity;

        private void Awake()
        {
            _NavMeshAgent.updatePosition = false;
            _NavMeshAgent.updateRotation = true;
        }

        private void OnAnimatorMove()
        {
            Vector3 rootPosition = _Animator.rootPosition;
            rootPosition.y = _NavMeshAgent.nextPosition.y;
            transform.position = rootPosition;
            _NavMeshAgent.nextPosition = rootPosition;
        }

        void Update()
        {
            Vector3 point = Vector3.zero;
            if (_NavMeshAgent.remainingDistance <= _NavMeshAgent.stoppingDistance) //done with path
            {
                if (RandomPoint(transform.position, _range, out point)) //pass in our centre point and radius of area
                {
                    Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                    _NavMeshAgent.SetDestination(point);
                }
            }

            if (_Animator) 
            {
                Vector3 movement = _NavMeshAgent.nextPosition - transform.position;


                movement.y = 0f;

                float moveAmount = _Velocity.magnitude;
                moveAmount = Mathf.Clamp01(Mathf.Abs(movement.x) + Mathf.Abs(movement.z));
                _Animator.SetFloat("ForwardMovement", moveAmount + 1);

                if (moveAmount > 0)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(movement);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 500);
                }
            }
        }

        bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
            {
                //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
                //or add a for loop like in the documentation
                result = hit.position;
                return true;
            }

            result = Vector3.zero;
            return false;
        }
    }
}