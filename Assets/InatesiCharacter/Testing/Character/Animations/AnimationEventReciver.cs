using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace InatesiCharacter.Testing.Character.Animations
{
	public class AnimationEventReciver : MonoBehaviour
	{
		[SerializeField] private UnityEvent<string> _OnAttackEvent;

		public void SendAnimationEvent(string nameMethod)
		{
            _OnAttackEvent?.Invoke(nameMethod);
        }
	}
}