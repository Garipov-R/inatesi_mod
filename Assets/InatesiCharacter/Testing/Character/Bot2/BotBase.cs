using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace InatesiCharacter.Testing.Character.Bot2
{
    public class BotBase : MonoBehaviour
    {
        [SerializeField] private Animator _Animator;
        [Header("Movement Settings")]
        public float moveSpeed = 3.5f;
        public float turnSpeed = 120f;

        [Header("Target")]
        public Transform target;
        public float updateTargetInterval = 0.5f; // How often to update path

        private NavMeshAgent agent;
        private float lastUpdateTime;


        private void Start()
        {
            // Get or add NavMeshAgent component
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
                agent = gameObject.AddComponent<NavMeshAgent>();

            // Configure agent
            agent.speed = moveSpeed;
            agent.angularSpeed = turnSpeed;
            agent.acceleration = 8f;
            agent.stoppingDistance = 1.5f;
            agent.autoBraking = true;

            // Set initial destination if target exists
            if (target != null)
                SetDestination();
        }

        void Update()
        {
            // Periodically update destination to target
            if (target != null && Time.time > lastUpdateTime + updateTargetInterval)
            {
                SetDestination();
                lastUpdateTime = Time.time;
            }

            // Optional: Visual feedback when reached destination
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    // Bot has reached destination
                    //Debug.Log("Bot reached destination!");
                }
            }

            UpdateAnimation();
        }

        void SetDestination()
        {
            if (target != null)
                agent.SetDestination(target.position);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            SetDestination();
        }

        public void MoveToPosition(Vector3 position)
        {
            agent.SetDestination(position);
        }

        public void Stop()
        {
            agent.isStopped = true;
        }

        public void Resume()
        {
            agent.isStopped = false;
        }

        public void UpdateAnimation()
        {
            _Animator.SetBool("walk", agent.velocity.magnitude > 0);
        }
    }
}