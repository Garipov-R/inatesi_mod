using System.Collections;
using UnityEngine;

namespace InatesiCharacter.Shared.Input
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] protected Vector2 m_LookSensitivity = new Vector2(2f, 2f);
        [SerializeField] protected string _LookInputName = "Look";
        [SerializeField] protected string _MoveInputName = "Move";

        public Vector2 LookSensitivity { get => m_LookSensitivity; set => m_LookSensitivity = value; }
        public string LookInputName { get => _LookInputName; set => _LookInputName = value; }
        public string MoveInputName { get => _MoveInputName; set => _MoveInputName = value; }


        #region Unity

        protected virtual void Awake()
        {

        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void Update()
        {

        }

        #endregion

        public bool GetButton(string name) { return GetButtonInternal(name); }

        protected virtual bool GetButtonInternal(string name) { return false; }

        public bool GetButtonDown(string name) { return GetButtonDownInternal(name); }

        protected virtual bool GetButtonDownInternal(string name) { return false; }

        public bool GetButtonUp(string name) { return GetButtonUpInternal(name); }

        protected virtual bool GetButtonUpInternal(string name) { return false; }

        public float GetAxisRaw(string name) { return GetAxisRawInternal(name); }

        protected virtual float GetAxisRawInternal(string name) { return 0; }

        public Vector2 GetVector(string name) { return GetVectorInternal(name); }

        protected virtual Vector2 GetVectorInternal(string name) { return Vector2.zero; }
    }
}