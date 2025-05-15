using System.Collections;
using UnityEngine;

namespace InatesiCharacter.SuperCharacter.MovementTypes
{
    public abstract class MovementType
    {
        [SerializeField] protected int _AbilityIntData = 0;

        public int AbilityIntData { get => _AbilityIntData; set => _AbilityIntData = value; }

        protected GameObject _GameObject;
        protected Transform _Transform;
        protected CharacterMotionBase _CharacterMotion;
        protected ILookSource _LookSource;
        protected float _YawDelta;

        private bool m_ForceIndependentLook;
        public abstract bool FirstPersonPerspective { get; }
        public ILookSource LookSource { get => _LookSource; set => _LookSource = value; }

        public virtual void Awake()
        {
            /*
            EventHandler.RegisterEvent<ILookSource>(m_GameObject, "OnCharacterAttachLookSource", OnAttachLookSource);
            EventHandler.RegisterEvent<bool>(m_GameObject, "OnCharacterForceIndependentLook", OnForceIndependentLook);
            */
        }

        public virtual void Initialize(CharacterMotionBase primaryCharacterMotion)
        {
            _CharacterMotion = primaryCharacterMotion;
            _GameObject = primaryCharacterMotion.gameObject;
            _Transform = primaryCharacterMotion.transform;
            _LookSource = _CharacterMotion.LookSource;
        }

        protected virtual void OnAttachLookSource(ILookSource lookSource)
        {
            _LookSource = lookSource;
        }

        public virtual void ChangeMovementType(bool activate) { }

        public abstract float GetDeltaYawRotation(
            float characterHorizontalMovement, 
            float characterForwardMovement, 
            float cameraHorizontalMovement, 
            float cameraVerticalMovement
        );

        public virtual Quaternion GetDeltaRotation()
        {
            var deltaRotaion = new Vector3();

            deltaRotaion.Set(0, _YawDelta, 0);

            return _Transform.rotation * Quaternion.Euler(deltaRotaion);
        }

        public virtual Quaternion GetRotation(float characterHorizontalMovement = 0, float characterForwardMovement = 0)
        {
            return Quaternion.identity;
        }

        public abstract Vector2 GetInputVector(Vector2 inputVector);

        public virtual bool UseIndependentLook(bool characterLookDirection) { return m_ForceIndependentLook; }

        private void OnForceIndependentLook(bool forceIndependentLook)
        {
            m_ForceIndependentLook = forceIndependentLook;
        }

        public virtual void OnDestroy()
        {
            //EventHandler.UnregisterEvent<ILookSource>(m_GameObject, "OnCharacterAttachLookSource", OnAttachLookSource);
            //EventHandler.UnregisterEvent<bool>(m_GameObject, "OnCharacterForceIndependentLook", OnForceIndependentLook);
        }
    }
}