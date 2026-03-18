using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Utility
{
    public class TranlsateObject : MonoBehaviour
    {
        [SerializeField] private Transform[] _MovePoints;
        [SerializeField] private float _MoveSpeed = 1f;
        [SerializeField] private int _startIndex = 0;
        [SerializeField] private bool _loop = false;

        private int _moveIndex;


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
                        _moveIndex++;

                        if (_moveIndex >= _MovePoints.Length)
                        {
                            _moveIndex = 0;
                        }
                        else if (_moveIndex < 0)
                        {
                            _moveIndex = _MovePoints.Length - 1;
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