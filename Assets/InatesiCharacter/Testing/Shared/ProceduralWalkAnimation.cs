using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Testing.Shared
{
    public class ProceduralWalkAnimation : MonoBehaviour
    {
        public struct LegMovement
        {
            public float Progress;
            public Vector3 FromPosition;
            public Vector3 ToPosition;

            public Vector3 Evaluate(in Vector3 up, AnimationCurve animationCurve)
            {
                return Vector3.Lerp(FromPosition, ToPosition, Progress) + up * animationCurve.Evaluate(Progress);
            }
        }

        [SerializeField] private Transform _body;
        [SerializeField] private List<Transform> _legs;
        [SerializeField] private AnimationCurve _legAnimCurve;
        [SerializeField] private float _stepLength = .2f;
        [SerializeField] private float _stepSpeed = .2f;

        private List<LegData> _legData = new List<LegData>();
        private RaycastHit _hitInfo;


        private void Awake()
        {
            foreach (var leg in _legs)
            {
                var legData = new LegData
                {
                    transform = leg.transform,
                    currentPosition = leg.position,
                    rightOffset = Vector3.Dot(_body.right, (leg.position - _body.position).normalized),
                    forwardOffset = Vector3.Dot(_body.forward, (leg.position - _body.position).normalized)
                };
                _legData.Add(legData);
            }
        }

        private void Update()
        {
            int index = 0;
            foreach (var legData in _legData)
            {
                Ray ray = new Ray(
                    _body.position + Vector3.up * 2f + _body.right * legData.rightOffset * 1f + _body.forward * legData.forwardOffset,  
                    -Vector3.up + _body.forward * .3f
                );
                var cast = Physics.SphereCast(ray, .1f, out _hitInfo, 6, Configs.Config.s_DefaultLayerMask, QueryTriggerInteraction.Ignore);
                if (cast)
                {
                    legData.Update(_stepSpeed, _legAnimCurve);


                    if (Vector3.Distance(_hitInfo.point, legData.currentPosition) > _stepLength)
                    {
                        //legData.currentPosition = hitInfo.point;
                        //legData.Setup(hitInfo.point);
                    }

                    //Debug.DrawLine(ray.origin, hitInfo.point, Color.red, .1f);
                    //Debug.Log(legData.IsMoving + " " + Vector3.Distance(legData.currentPosition, hitInfo.point) + " " + legData.currentPosition);


                    if (!CanMove(index)) continue;

                    if (!legData.IsMoving && !(Vector3.Distance(legData.currentPosition, _hitInfo.point) > _stepLength))
                    {
                        continue;
                    }

                    legData.MoveTo(_hitInfo.point, _stepSpeed, _legAnimCurve);
                }

                index++;
            }
        }

        public bool CanMove(int legIndex)
        {
            var legsCount = _legData.Count;
            var n1 = _legData[(legIndex + legsCount - 1) % legsCount];
            var n2 = _legData[(legIndex + 1) % legsCount];
            return !n1.IsMoving && !n2.IsMoving;
        }

        public class LegData
        {
            public Vector3 currentPosition;
            public LegMovement? LegMovement;
            public Transform transform;
            public float forwardOffset;
            public float rightOffset;
            public bool IsMoving => LegMovement != null;

            public void Setup(Vector3 leg)
            {
                currentPosition = leg;
            }

            public void Update(float stepSpeed, AnimationCurve stepCurve)
            {
                if (LegMovement != null)
                {
                    var m = LegMovement.Value;
                    m.Progress = Mathf.Clamp01(m.Progress + Time.deltaTime * stepSpeed);
                    currentPosition = m.Evaluate(Vector3.up, stepCurve);
                    LegMovement = m.Progress < 1 ? m : null;
                }

                transform.position = currentPosition;
            }

            public void MoveTo(Vector3 targetPosition, float stepSpeed, AnimationCurve stepCurve)
            {
                


                if (LegMovement == null)
                {
                    LegMovement = new LegMovement
                    {
                        Progress = 0,
                        FromPosition = currentPosition,
                        ToPosition = targetPosition,
                    };
                }
                else
                {
                    LegMovement = new LegMovement
                    {
                        Progress = LegMovement.Value.Progress,
                        FromPosition = LegMovement.Value.FromPosition,
                        ToPosition = targetPosition
                    };
                }
            }
        }
    }
}