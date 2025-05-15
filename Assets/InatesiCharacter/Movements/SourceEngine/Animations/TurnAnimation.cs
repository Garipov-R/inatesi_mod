using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Movements.SourceEngine.Animations
{
    public class TurnAnimation : StateMachineBehaviour
    {
        private SourceEngine.SourceEngineMovement2 sourceEngineMovement2;
        [SerializeField] private bool _RootMotion;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (sourceEngineMovement2 == null) 
            {
                sourceEngineMovement2 = animator.transform.GetComponent<SourceEngine.SourceEngineMovement2>();
            }

            if (sourceEngineMovement2 != null) 
            {
                sourceEngineMovement2.TurnAnim = true;
                if (_RootMotion) sourceEngineMovement2.AnimatorMonitor.Animator.applyRootMotion = true;
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float current_time = stateInfo.normalizedTime * stateInfo.length;
            if (current_time > .5f && animator.IsInTransition(0))
            {
                animator.rootRotation = sourceEngineMovement2.desired_rotation;
                //animator.transform.rotation = sourceEngineMovement2.desired_rotation;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (sourceEngineMovement2 == null)
            {
                sourceEngineMovement2 = animator.transform.GetComponent<SourceEngine.SourceEngineMovement2>();
            }

            if (sourceEngineMovement2 != null)
            {
                sourceEngineMovement2.TurnAnim = false;
                if (_RootMotion) sourceEngineMovement2.AnimatorMonitor.Animator.applyRootMotion = false;
                //animator.transform.rotation = sourceEngineMovement2.desired_rotation;
            }
        }

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}