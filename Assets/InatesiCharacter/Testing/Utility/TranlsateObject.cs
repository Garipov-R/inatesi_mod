using KinematicCharacterController;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace InatesiCharacter.Testing.Utility
{
    [RequireComponent(typeof(PhysicsMover))]
    [RequireComponent(typeof(MyMovingPlatform))]
    public class TranlsateObject : MonoBehaviour
    {
        [SerializeField] private Transform[] _MovePoints;
        [SerializeField] private float _MoveSpeed = 1f;
        [SerializeField] private int _startIndex = 0;
        [SerializeField] private bool _loop = false;
        [SerializeField] private float _nextPointTime = .5f;
        [SerializeField] private UnityEvent _OnFinishPoint;
        [SerializeField] private UnityEvent _OnStartPoint;

        private int _moveIndex;
        private float _nextPointTimer = 0;


        private void Awake()
        {
            MoveToPoint(_startIndex);
        }

        public void Tick(float deltaTime)
        {
            if (_MovePoints == null) return;

            if (_MovePoints.Length == 0)
                return;

            if (_loop)
            {
                if (_moveIndex < _MovePoints.Length || _moveIndex >= 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _MovePoints[_moveIndex].position, deltaTime * _MoveSpeed);

                    if (Vector3.Distance(transform.position, _MovePoints[_moveIndex].position) <= 0)
                    {
                        if (_nextPointTimer > 0)
                        {
                            if (_nextPointTimer == _nextPointTime)
                            {
                                _OnFinishPoint?.Invoke();
                            }

                            _nextPointTimer -= deltaTime;
                        }
                        else
                        {
                            _moveIndex++;

                            if (_moveIndex >= _MovePoints.Length)
                            {
                                _moveIndex = 0;
                            }
                            else if (_moveIndex < 0)
                            {
                                _moveIndex = _MovePoints.Length - 1;
                            }

                            _nextPointTimer = _nextPointTime;

                            _OnStartPoint?.Invoke();
                        }
                    }
                }
                else
                {
                    _moveIndex = 0;
                }
            }
            else
            {
                if (_moveIndex > _MovePoints.Length || _moveIndex < 0)
                    return;

                transform.position = Vector3.MoveTowards(transform.position, _MovePoints[_moveIndex].position, deltaTime * _MoveSpeed);

                if (Vector3.Distance(transform.position, _MovePoints[_moveIndex].position) <= 0)
                {
                    if (_nextPointTimer == 0)
                    {
                        _OnFinishPoint?.Invoke();
                        _nextPointTimer = deltaTime;
                    }
                }
                else
                {
                    _nextPointTimer = 0;
                }
            }
        }

        public void MoveToPoint(int index)
        {
            _moveIndex = index;
        }

        public void MoveToNextPoint()
        {
            _moveIndex++;

            if (_moveIndex >= _MovePoints.Length)
            {
                _moveIndex = 0;
            }
        }

        public void SetLoop(bool loop)
        {
            _loop = loop;
        }
    }
}