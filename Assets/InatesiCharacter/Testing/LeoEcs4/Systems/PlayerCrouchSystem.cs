using InatesiCharacter.Movements.SourceEngine.TraceUtility;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.Character;
using InatesiCharacter.Testing.LeoEcs3;
using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.Shared;
using Leopotam.EcsLite;
using RootMotion.FinalIK;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class PlayerCrouchSystem : IEcsRunSystem, IEcsInitSystem
    {
        bool _uncrouchDown = false;
        float _crouchLerp = 0f;
        CharacterMotionBase _CharacterMotionBase;
        private SharedData _sharedData;
        EcsFilter _playerFilter;
        EcsPool<PlayerComponent> _playerPool;
        EcsFilter _characterFilter;
        EcsPool<CharacterComponent> _characterPool;
        bool _canUncrouch;
        float _crouchingHeight;
        float _heightDifference;
        Collider[] _colliders = new Collider[8];
        float _lerpHeight;


        public void Init(IEcsSystems systems)
        {
            _sharedData = systems.GetShared<SharedData>();

            _playerFilter = systems.GetWorld().Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _playerPool = systems.GetWorld().GetPool<PlayerComponent>();

            _characterFilter = systems.GetWorld().Filter<CharacterComponent>().End();
            _characterPool = systems.GetWorld().GetPool<CharacterComponent>();

            foreach (var entity in _playerFilter)
            {
                ref var characterComponent = ref _characterPool.Get(entity);
                ref var playerComponent = ref _playerPool.Get(entity);
                _CharacterMotionBase = characterComponent.CharacterMotionBase;
            }
        }

        public void Run(IEcsSystems systems)
        {
            if (GameSettings.IsPause) return;

            var wishCrouch = Inatesi.Inputs.Input.Down("Crouch");

            foreach (int entity in _playerFilter)
            {
                if (_playerPool.Has(entity))
                {
                    ref var playerComponent = ref _playerPool.Get(entity);
                    ref var characterComponent = ref _characterPool.Get(entity);

                    if (characterComponent.CharacterMotionBase.IsNoclip)
                        continue;

                    if (_CharacterMotionBase == null)
                    {
                        _CharacterMotionBase = characterComponent.CharacterMotionBase;
                    }


                    _crouchingHeight = Mathf.Clamp(_CharacterMotionBase.MoveConfig.CrouchHeight, 0.01f, 1f);
                    _heightDifference = _CharacterMotionBase.DefaultHeight - (_CharacterMotionBase.DefaultHeight * _crouchingHeight);

                    if (_CharacterMotionBase.OnGrounded)
                        _uncrouchDown = false;

                    // Crouching input
                    if (_CharacterMotionBase.OnGrounded)
                        _crouchLerp = Mathf.Lerp(_crouchLerp, wishCrouch ? 1f : 0f, Time.deltaTime * 15f);
                    else if (!_CharacterMotionBase.OnGrounded && !wishCrouch && _crouchLerp < 0.95f)
                        _crouchLerp = 0f;
                    else if (!_CharacterMotionBase.OnGrounded && wishCrouch)
                        _crouchLerp = 1f;

                    if (_crouchLerp > 0.9f && !characterComponent.Crouched)
                    {
                        characterComponent.Crouched = true;
                        _CharacterMotionBase.Height = _CharacterMotionBase.Height * _crouchingHeight;
                        //CapsuleCollider capsuleCollider = (CapsuleCollider)_CharacterMotionBase.Collider;
                        //capsuleCollider.height = _CharacterMotionBase.DefaultHeight * crouchingHeight;

                        var KLDFHJG = _heightDifference * (_CharacterMotionBase.OnGrounded ? Vector3.zero : _CharacterMotionBase.Up);
                        //KLDFHJG = _heightDifference * _CharacterMotionBase.Up;

                        _CharacterMotionBase.SetPositionAndRotation(
                            _CharacterMotionBase.transform.position + KLDFHJG,
                            _CharacterMotionBase.transform.rotation
                            );

                        foreach (Transform child in _CharacterMotionBase.transform)
                        {
                            child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y * _crouchingHeight, child.localPosition.z);
                        }

                        _uncrouchDown = !_CharacterMotionBase.OnGrounded;


                        PlayerBuilder.SetCameraView(playerComponent.fpc);

                        _CharacterMotionBase.AnimatorMonitor.SetAbilityID(4);
                    }
                    else if (characterComponent.Crouched)
                    {
                        _canUncrouch = true;

                        CapsuleCollider capsuleCollider = (CapsuleCollider)_CharacterMotionBase.Collider;
                        Vector3 point1 = capsuleCollider.center + _CharacterMotionBase.Up * capsuleCollider.height * 1.5f * (1f); // 0.5f
                        Vector3 point2 = capsuleCollider.center + -_CharacterMotionBase.Up * capsuleCollider.height / 2 * (.2f); // 0.5f
                        Vector3 startPos = _CharacterMotionBase.transform.position;
                        Vector3 endPos = _CharacterMotionBase.transform.position + (_uncrouchDown ? Vector3.zero : _CharacterMotionBase.Up) * _heightDifference;

                        int overlap = Tracer.CharacterOverlap(
                            point1,
                            point2,
                            startPos,
                            _CharacterMotionBase.transform.rotation,
                            _CharacterMotionBase.Collider as CapsuleCollider,
                            _colliders,
                            _CharacterMotionBase.RaycastLayer,
                            QueryTriggerInteraction.Ignore,
                            0
                        );

                        Trace trace = Tracer.TraceCapsule(
                            point1 + startPos,
                            point2 + startPos,
                            capsuleCollider.radius,
                            startPos,
                            endPos,
                            capsuleCollider.contactOffset,
                            _CharacterMotionBase.RaycastLayer
                        );

                        if (trace.hitCollider != null)
                            _canUncrouch = false;

                        if (overlap > 0)
                            _canUncrouch = false;


                        if (_canUncrouch && _crouchLerp <= 0.9f)
                        {
                            characterComponent.Crouched = false;

                            _CharacterMotionBase.Height = _CharacterMotionBase.DefaultHeight;

                            var KLDFHJG = !_uncrouchDown ? Vector3.up * _heightDifference / 2 : _CharacterMotionBase.Up;
                            _CharacterMotionBase.SetPositionAndRotation(
                                        _CharacterMotionBase.transform.position + KLDFHJG,
                                        _CharacterMotionBase.transform.rotation
                                    );

                            characterComponent.Crouched = false;

                            PlayerBuilder.SetCameraView(playerComponent.fpc);

                            _CharacterMotionBase.AnimatorMonitor.SetAbilityID(0);

                            foreach (Transform child in _CharacterMotionBase.transform)
                            {
                                child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y / _crouchingHeight, child.localPosition.z);
                            }

                            PlayerBuilder.SetCameraView(playerComponent.fpc);

                            _CharacterMotionBase.AnimatorMonitor.SetAbilityID(0);
                        }
                    }

                    if (!_canUncrouch)
                        _crouchLerp = 1f;
                }
            }
        }
    }
}