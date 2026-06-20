using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace InatesiCharacter._template
{
	public class WaypointUITest : MonoBehaviour
	{
		[SerializeField] private UIDocument _UIDocument;

		private UnityEngine.Camera _Camera;
		private Transform _target;
		private VisualElement _waypointElement;

        public UnityEngine.Camera Camera { get => _Camera; set => _Camera = value; }
        public Transform Target { get => _target; set => _target = value; }


        private void Awake()
        {
			_waypointElement = _UIDocument.rootVisualElement.Q("point");
        }

        private void LateUpdate()
        {
			if (_target == null)
			{
				return;
			}

			if (_waypointElement == null)
			{
				return;
			}

			Vector3 screenPos = _Camera.WorldToScreenPoint(_target.position);

			float uiX = screenPos.x;
			float uiY = Screen.height - screenPos.y;

			Vector2 localPos = RuntimePanelUtils.ScreenToPanel(_waypointElement.panel, new Vector2(uiX, uiY));

            _waypointElement.style.left = localPos.x;
            _waypointElement.style.top = localPos.y;
        }

        public void SetTarget(Transform target)
		{
			_target = target;
		}
	}
}