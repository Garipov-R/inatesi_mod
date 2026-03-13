using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace InatesiCharacter.Testing.Character.InteractionSystem
{
    public class TransformationObject : MonoBehaviour
    {
        [SerializeField ] private Vector3 _ToRotate = Vector3.zero;
        [SerializeField ] private Vector3 _ToTranslate = Vector3.zero;
        [SerializeField] private float _SpeedRotate = .5f;
        [SerializeField] private float _SpeedTranslate = .5f;
        [Space(10)]
        [SerializeField] private UnityEvent _OnStart;
        [SerializeField] private UnityEvent _OnFinish;

        private Quaternion _startRotation;
        private Vector3 _startPosition;
        private bool _rotate = false;
        private bool _translate = false;
        private bool _rotated = false;
        private bool _translated = false;

        private void Start()
        {
            _startRotation = transform.rotation;
            _startPosition = transform.localPosition;
        }

        private void Update()
        {
            if (_rotate)
            {
                Vector3 targetRotate = _rotated == false ? _startRotation.eulerAngles : _ToRotate;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotate), Time.deltaTime * _SpeedRotate);

                if (Vector3.Distance(transform.rotation.eulerAngles, targetRotate) < .01f)
                {
                    _rotate = false;
                }
            }

            if (_translate)
            {
                Vector3 targetTranslate = _translated == false ? _startPosition : _ToTranslate;
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetTranslate, Time.deltaTime * _SpeedTranslate);

                if (Vector3.Distance(transform.position, targetTranslate) <= 0)
                {
                    _translate = false;

                    EventObserver();
                }
            }
        }

        public void Rotate()
        {
            _rotate = true;
            _rotated = !_rotated;
        }

        public void Translate()
        {
            _translate = true;
            _translated = !_translated;

            EventObserver();

        }

        public void TranslateB(bool translate)
        {
            _translate = true;
            _translated = translate;

            EventObserver();
        }

        private void EventObserver()
        {
            if (_translate)
            {
                _OnStart?.Invoke();
            }
            else
            {
                _OnFinish?.Invoke();
            }
        }
    }
}