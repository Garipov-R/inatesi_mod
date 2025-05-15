using Inatesi.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace InatesiCharacter.SuperCharacter
{
    [Serializable]
    public class AnimatorMonitor
    {
        [SerializeField] protected float _HorizontalMovementDampingTime = 0.1f;
        [SerializeField] protected float _ForwardMovementDampingTime = 0.1f;

        protected Animator _Animator;
        protected GameObject _GameObject;
        protected Transform _Transform;

        private float _HorizontalMovement;
        private float _ForwardMovement;
        private bool _Moving;
        private int _AbilityID;
        private int _Slot0;
        private float _RotaionSpeed;
        private bool _IsGrounded;
        private float _MoveSpeed;
        private int m_AbilityIntData;

        private static int s_HorizontalMovementHash = Animator.StringToHash("HorizontalMovement");
        private static int s_ForwardMovementHash = Animator.StringToHash("ForwardMovement");
        private static int s_MovingHash = Animator.StringToHash("Moving");
        private static int s_AbilityIDHash = Animator.StringToHash("AbilityID");
        private static int s_AbilityChangeHash = Animator.StringToHash("AbilityChange");
        private static int s_Slot0Hash = Animator.StringToHash("Slot0");
        private static int s_Slot0ChangeHash = Animator.StringToHash("Slot0Change");
        private static int s_IsGrounded = Animator.StringToHash("IsGrounded");
        private static int s_RotationSpeed = Animator.StringToHash("RotationSpeed");
        private static int s_MoveSpeed = Animator.StringToHash("MoveSpeed");
        private static int s_AbilityIntDataHash = Animator.StringToHash("AbilityIntData");


        public Animator Animator { get => _Animator; set => _Animator = value; }
        public int AbilityID { get => _AbilityID; set => _AbilityID = value; }
        public float RotaionSpeed { get => _RotaionSpeed; set => _RotaionSpeed = value; }
        public bool IsGrounded { get => _IsGrounded; set => _IsGrounded = value; }
        public float MoveSpeed { get => _MoveSpeed; set => _MoveSpeed = value; }


        public void Initialize(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out Animator Animator)) _Animator = Animator;
            else if (gameObject.GetComponentInChildren<Animator>()) _Animator = gameObject.GetComponentInChildren<Animator>();
            _GameObject = gameObject;
            _Transform = gameObject.transform;
        }

        public void SetHorizontalMovementParameter(float value, float timeScale)
        {
            SetHorizontalMovementParameter(value, timeScale, _HorizontalMovementDampingTime);
        }

        public virtual bool SetHorizontalMovementParameter(float value, float timeScale, float dampingTime)
        {
            var change = _HorizontalMovement != value;
            if (change)
            {
                if (_Animator != null)
                {
                    //_Animator.SetFloat(s_HorizontalMovementHash, value, dampingTime, TimeUtility.DeltaTimeScaled / timeScale);
                    _Animator.SetFloat(s_HorizontalMovementHash, value, dampingTime, Time.timeScale / timeScale);
                    _HorizontalMovement = _Animator.GetFloat(s_HorizontalMovementHash);
                    if (Mathf.Abs(_HorizontalMovement) < 0.001f)
                    {
                        _HorizontalMovement = 0;
                        _Animator.SetFloat(s_HorizontalMovementHash, 0);
                    }
                }
                else
                {
                    _HorizontalMovement = value;
                }
            }

            /*// The item's Animator should also be aware of the updated parameter value.
            if (m_EquippedItems != null)
            {
                for (int i = 0; i < m_EquippedItems.Length; ++i)
                {
                    if (m_EquippedItems[i] != null)
                    {
                        m_EquippedItems[i].SetHorizontalMovementParameter(value, timeScale, dampingTime);
                    }
                }
            }*/

            return change;
        }

        public void SetForwardMovementParameter(float value, float timeScale)
        {
            SetForwardMovementParameter(value, timeScale, _ForwardMovementDampingTime);
        }

        public virtual bool SetForwardMovementParameter(float value, float timeScale, float dampingTime)
        {
            var change = _ForwardMovement != value;
            if (change)
            {
                if (_Animator != null)
                {
                    //_Animator.SetFloat(s_ForwardMovementHash, value, dampingTime, TimeUtility.DeltaTimeScaled / timeScale);
                    _Animator.SetFloat(s_ForwardMovementHash, value, dampingTime, Time.timeScale / timeScale);
                    _ForwardMovement = _Animator.GetFloat(s_ForwardMovementHash);
                    if (Mathf.Abs(_ForwardMovement) < 0.001f)
                    {
                        _ForwardMovement = 0;
                        _Animator.SetFloat(s_ForwardMovementHash, 0);
                    }
                }
                else
                {
                    _ForwardMovement = value;
                }
            }

            /*// The item's Animator should also be aware of the updated parameter value.
            if (m_EquippedItems != null)
            {
                for (int i = 0; i < m_EquippedItems.Length; ++i)
                {
                    if (m_EquippedItems[i] != null)
                    {
                        m_EquippedItems[i].SetForwardMovementParameter(value, timeScale, dampingTime);
                    }
                }
            }*/

            return change;
        }

        public virtual bool SetMovingParameter(bool value)
        {
            var change = _Moving != value;
            if (change)
            {
                if (_Animator != null)
                {
                    _Animator.SetBool(s_MovingHash, value);
                }
                _Moving = value;
            }

            /*// The item's Animator should also be aware of the updated parameter value.
            if (m_EquippedItems != null)
            {
                for (int i = 0; i < m_EquippedItems.Length; ++i)
                {
                    if (m_EquippedItems[i] != null)
                    {
                        m_EquippedItems[i].SetMovingParameter(value);
                    }
                }
            }*/

            return change;
        }

        public void SetAbilityID(int id)
        {
            if (_AbilityID != id)
            {
                if (_Animator != null)
                {
                    _Animator.SetInteger(s_AbilityIDHash, id);
                    SetAbilityIDTrigger(true); //SetAbilityChangeParameter(true);
                }
                _AbilityID = id;
            }
        }

        public bool SetAbilityIDTrigger(bool value)
        {
            if (_Animator != null && _Animator.GetBool(s_AbilityChangeHash) != value)
            {
                if (value)
                {
                    _Animator.SetTrigger(s_AbilityChangeHash);
                }
                else
                {
                    _Animator.ResetTrigger(s_AbilityChangeHash);
                }
                return true;
            }

            return false;
        }

        public void SetSlot0(int id)
        {
            var change = _Slot0 != id;

            if (change)
            {
                if (_Animator != null)
                {
                    _Animator.SetInteger(s_Slot0Hash, id);
                    SetSlot0Change(true); //SetAbilityChangeParameter(true);
                }
                _Slot0 = id;
            }
        }

        public bool SetSlot0Change(bool value)
        {
            if (_Animator != null && _Animator.GetBool(s_Slot0ChangeHash) != value)
            {
                if (value)
                {
                    _Animator.SetTrigger(s_Slot0ChangeHash);
                }
                else
                {
                    _Animator.ResetTrigger(s_Slot0ChangeHash);
                }
                return true;
            }

            return false;
        }

        public void PlayAnimation(string nameAnimation, int layer = 0)
        {
            if (!_Animator)
                return;

            _Animator.Play(nameAnimation, layer);
        }

        private bool IsInTrasition()
        {
            for (int i = 0; i < _Animator.layerCount; ++i)
            {
                if (_Animator.IsInTransition(i))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsInTransition(int layerIndex)
        {
            if (_Animator == null)
            {
                return false;
            }

            return _Animator.IsInTransition(layerIndex);
        }

        public virtual bool SetAbilityIntDataParameter(int value)
        {
            var change = m_AbilityIntData != value;
            if (change)
            {
                if (_Animator != null)
                {
                    _Animator.SetInteger(s_AbilityIntDataHash, value);
                }
                m_AbilityIntData = value;
            }

            /*// The item's Animator should also be aware of the updated parameter value.
            if (m_EquippedItems != null)
            {
                for (int i = 0; i < m_EquippedItems.Length; ++i)
                {
                    if (m_EquippedItems[i] != null)
                    {
                        m_EquippedItems[i].SetAbilityIntDataParameter(value);
                    }
                }
            }*/

            return change;
        }

        public void WithVelocity(Vector3 velocity)
        {
            if (_Animator == null) return;

            var dir = velocity;
            
            var forward = Vector3.Dot(_Transform.forward, dir);
            var right = Vector3.Dot(_Transform.right, dir);

            var angle = MathF.Atan2(forward, right).RadianToDegree().NormalizeDegrees();

            _Animator.SetFloat("Move_y", forward);
            _Animator.SetFloat("Move_x", right);
            _Animator.SetFloat("Move_Direction", angle);
            _Animator.SetFloat("Move_Speed_Grounded", Vector3.Scale(dir, new Vector3(1,0,1)).magnitude);
        }

        public void WithWishVelocity(Vector3 velocity)
        {
            if (_Animator == null) return;

            var dir = velocity;

            var forward = Vector3.Dot(_Transform.forward, dir);
            var right = Vector3.Dot(_Transform.right, dir);

            var angle = MathF.Atan2(forward, right).RadianToDegree().NormalizeDegrees();

            _Animator.SetFloat("Wish_y", forward);
            _Animator.SetFloat("Wish_x", right);
            _Animator.SetFloat("Wish_Direction", angle);
        }

        public void SetParameters()
        {
            if (_Animator == null) return;
            
            _Animator.SetBool(s_IsGrounded, _IsGrounded);
            _Animator.SetFloat(s_RotationSpeed, _RotaionSpeed);
            _Animator.SetFloat(s_MoveSpeed, _MoveSpeed);
        }

        public void SetAvatar(Avatar avatar)
        {
            if (avatar == null) return;
            if (_Animator == null) return;
            _Animator.avatar = avatar;  
        }

        public void ApplyRootMotion(bool state)
        {
            if (_Animator == null) return;
            _Animator.applyRootMotion = state;
        }
    }
}