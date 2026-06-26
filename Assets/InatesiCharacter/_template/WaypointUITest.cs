using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using InatesiCharacter.Testing.Character.Bots;

namespace InatesiCharacter._template
{
	public class WaypointUITest : MonoBehaviour
	{
		[SerializeField] private UIDocument _UIDocument;

		private UnityEngine.Camera _Camera;
		private Transform _target;
		private Transform _player;
		private VisualElement _waypointElement;
        private VisualElement _damageElement;
        private Vector3 _damageAttackerPosition;

        public UnityEngine.Camera Camera { get => _Camera; set => _Camera = value; }
        public Transform Target { get => _target; set => _target = value; }
        public Transform Player { get => _player; set => _player = value; }


        private void Awake()
        {
			_waypointElement = _UIDocument.rootVisualElement.Q("point");
            _damageElement = _UIDocument.rootVisualElement.Q("point2");

            //Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }

        private void LateUpdate()
        {
            if (_waypointElement == null)
            {
                return;
            }

            UpdateDamageHit();

            if (_target == null)
			{
				return;
			}

			Vector3 screenPos = _Camera.WorldToScreenPoint(_target.position);

            if (screenPos.z < 0)
            {
                screenPos *= -1;
            }

            float uiX = screenPos.x;
			float uiY = Screen.height - screenPos.y;

			Vector2 localPos = RuntimePanelUtils.ScreenToPanel(_waypointElement.panel, new Vector2(uiX, uiY));
			//localPos = new Vector2(uiX, uiY);

            float padding = 20f; // Distance from screen edges
            padding = _waypointElement.contentRect.width;
            float minX = 0;
            float maxX = Screen.width;
            float minY = 0;
            float maxY = Screen.height;

            if (Vector3.Dot((_target.position - _Camera.transform.position).normalized, _Camera.transform.forward) < 0 && false)
			{
				if (localPos.x < Screen.width / 2)
				{
					localPos.x = maxX;
				}
				else
				{
					localPos.x = minX;
				}
			}

            localPos.x = Mathf.Clamp(localPos.x, minX, maxX);
            localPos.y = Mathf.Clamp(localPos.y, minY, maxY);

			_waypointElement.style.left = localPos.x;
			_waypointElement.style.top = localPos.y;


            //_waypointElement.transform.position = new Vector3(localPos.y,localPos.x,0);




        }


        public void RegisterHit(Vector3 attackerPosition)
        {
            if (Camera == null) return;

            _damageAttackerPosition = attackerPosition;

            var element = _damageElement.Q<VisualElement>("damage");
            var color = element.style.unityBackgroundImageTintColor.value;
            element.style.unityBackgroundImageTintColor = new StyleColor( new Color(255, color.g, color.b, 1));

            // UI Toolkit rotation runs clockwise; inverse angle math matches UI space
            //_damageElement.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50));
            //_damageElement.transform.rotation = Quaternion.Euler(0, 0, -angle);

            // Handle visibility lifecycle
            //if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            //fadeCoroutine = StartCoroutine(DisplayIndicatorRoutine());
        }

        public void UpdateDamageHit()
        {
            if (Camera == null) return;

            Vector3 playerForward = _Camera.transform.forward;
            playerForward.y = 0;
            playerForward.Normalize();

            Vector3 directionToAttacker = _damageAttackerPosition - _player.position;
            directionToAttacker.y = 0;
            directionToAttacker.Normalize();

            float angle = Vector3.SignedAngle(playerForward, directionToAttacker, Vector3.up);

            _damageElement.style.rotate = new Rotate(new Angle(angle, AngleUnit.Degree));

            var element = _damageElement.Q<VisualElement>("damage");
            var color = Color.Lerp(
                element.style.unityBackgroundImageTintColor.value,
                new Color(element.style.unityBackgroundImageTintColor.value.r, element.style.unityBackgroundImageTintColor.value.g, element.style.unityBackgroundImageTintColor.value.b, 0),
                Time.deltaTime
            );
            element.style.unityBackgroundImageTintColor = new StyleColor(color);
        }

        public void SetTarget(Transform target)
		{
			_target = target;
		}
	}
}