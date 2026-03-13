using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Utility
{
    public class TranlsateObject : MonoBehaviour
    {
        [SerializeField] private Transform[] _MovePoints;
        [SerializeField] private float _MoveSpeed = 1f;
        [SerializeField] private int _startIndex = 0;

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

            if (_moveIndex > _MovePoints.Length || _moveIndex < 0)
                return;

            transform.position = Vector3.MoveTowards(transform.position, _MovePoints[_moveIndex].position, deltaTime * _MoveSpeed);
        }

        public void MoveToPoint(int index)
        {
            _moveIndex = index;
        }
    }
}