using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.LeoEcs4.Events;
using InatesiCharacter.Testing.Shared;
using Leopotam.EcsLite;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class PlayerPickUpRigidbodySystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _PlayerCharacterFilter;
        private EcsPool<PlayerComponent> _playerPool;
        private EcsPool<CharacterComponent> _characterPool;
        private Rigidbody _SelectedRigidbody;
        private Quaternion _SelecterdRigidbodyRotation;
        private float _distanceSelectedRigidbody;
        private int _layerSelectedRigidbody;


        public void Init(IEcsSystems systems)
        {
            _PlayerCharacterFilter = systems.GetWorld().Filter<CharacterComponent>().Inc<PlayerComponent>().End();
            _playerPool = systems.GetWorld().GetPool<PlayerComponent>();
            _characterPool = systems.GetWorld().GetPool<CharacterComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (GameSettings.IsPause) return;

            foreach (var characterEntity in _PlayerCharacterFilter)
            {
                ref var characterComponent = ref _characterPool.Get(characterEntity);
                ref var playerComponent = ref _playerPool.Get(characterEntity);

                if (characterComponent.Dead == true) continue;

                if (Inatesi.Inputs.Input.Pressed("Use"))
                {
                    if (_SelectedRigidbody != null)
                    {
                        LostObject();
                        characterComponent.InventoryInteraction2.EnableCurrentWeapon();
                        playerComponent.canAttack = true;
                    }
                    else
                    {
                        RaycastHit hit;
                        var isHit = Raycast(
                            out hit,
                            characterComponent.CharacterMotionBase.LookSource.LookPosition(),
                            characterComponent.CharacterMotionBase.LookSource.Transform.forward,
                            1f + Vector3.Distance(characterComponent.CharacterMotionBase.transform.position, characterComponent.CharacterMotionBase.LookSource.Transform.position),
                            characterComponent.CharacterMotionBase.RaycastLayer
                        );

                        if (isHit == true)
                        {
                            if (hit.transform.TryGetComponent(out Rigidbody rigidbody))
                            {
                                var distanceBetweenPlayerAndObject =
                                    Vector3.Distance(
                                        characterComponent.CharacterMotionBase.transform.position + characterComponent.CharacterMotionBase.Up * (characterComponent.CharacterMotionBase.Height - characterComponent.CharacterMotionBase.Radius / 2),
                                        hit.transform.position
                                    );

                                if (1f >= rigidbody.mass)
                                {
                                    if (SelectObject(rigidbody, characterComponent.CharacterMotionBase.Radius))
                                    {
                                        characterComponent.InventoryInteraction2.DisableCurrentWeapon();
                                        playerComponent.canAttack = false;
                                    }
                                }
                            }
                        }
                    }
                }

                var playerPos = (characterComponent.CharacterMotionBase.transform.position + characterComponent.CharacterMotionBase.Up * (characterComponent.CharacterMotionBase.Height - characterComponent.CharacterMotionBase.Radius / 2));
                var playerDir = characterComponent.CharacterMotionBase.LookSource.LookDirection();
                KeepObject(playerPos, playerDir, out bool isLostObject);

                if (isLostObject == true)
                {
                    LostObject();
                    characterComponent.InventoryInteraction2.EnableCurrentWeapon();
                    playerComponent.canAttack = true;
                }

                if (Inatesi.Inputs.Input.Pressed("Attack"))
                {
                    if (_SelectedRigidbody != null)
                    {
                        _SelectedRigidbody.AddForce(playerDir * 5f, ForceMode.VelocityChange); 
                        LostObject();
                        characterComponent.InventoryInteraction2.EnableCurrentWeapon();
                        playerComponent.canAttack = true;
                    }
                }
            }

            
        }


        private bool SelectObject(Rigidbody rigidbody, float distance)
        {
            if (rigidbody.isKinematic) return false;
            if (_SelectedRigidbody != null) return false;



            _SelectedRigidbody = rigidbody;

            _distanceSelectedRigidbody = distance + distance * .5f;
            //_SelectedRigidbody.isKinematic = true;
            _SelectedRigidbody.freezeRotation = false;
            _SelectedRigidbody.constraints = RigidbodyConstraints.None;
            _SelectedRigidbody.useGravity = false;
            _SelecterdRigidbodyRotation = Quaternion.identity;
            //_SelecterdRigidbodyRotation = Quaternion.Euler(Vector3.up);
            //_SelectedRigidbody.freezeRotation = true;
            _layerSelectedRigidbody = _SelectedRigidbody.gameObject.layer;
            _SelectedRigidbody.gameObject.layer = LayerMask.NameToLayer("CharacterIgnore");

            return true;
        }

        private void KeepObject(Vector3 position, Vector3 direction, out bool isLostObject)
        {
            isLostObject = false;

            if (_SelectedRigidbody == null)
                return;

            if (_SelectedRigidbody.isKinematic == false)
            {
                var playerPos = position;
                var magnetposition = playerPos + direction * _distanceSelectedRigidbody;
                var resultPosition = (magnetposition - _SelectedRigidbody.worldCenterOfMass);

                var force = 0f;
                if (Vector3.Distance(magnetposition, _SelectedRigidbody.worldCenterOfMass) < .01f)
                {
                    force = 30;
                }
                else
                {
                    force = 30;
                }

                if (Vector3.Distance(magnetposition, _SelectedRigidbody.worldCenterOfMass) > 1f)
                {
                    isLostObject = true;
                }

                _SelectedRigidbody.rotation = Quaternion.Lerp(_SelectedRigidbody.rotation, _SelecterdRigidbodyRotation, Time.deltaTime * 30f);
                _SelectedRigidbody.angularVelocity = Vector3.zero;
                _SelectedRigidbody.linearVelocity = Vector3.MoveTowards(_SelectedRigidbody.linearVelocity, resultPosition * force, Time.deltaTime * 400f);
                _SelectedRigidbody.centerOfMass = Vector3.zero;
            }

            if (_SelectedRigidbody.isKinematic)
            {
                isLostObject = true;
            }
        }

        private void LostObject()
        {
            if (_SelectedRigidbody != null)
            {
                _SelectedRigidbody.useGravity = true;
                _SelectedRigidbody.freezeRotation = false;
                _SelectedRigidbody.gameObject.layer = _layerSelectedRigidbody;
                _SelectedRigidbody = null;
            }
        }

        private bool Raycast(out RaycastHit hit, Vector3 position, Vector3 direction, float distance, LayerMask layerMask)
        {
            var isHit = Physics.Raycast(
                position,
                direction,
                out hit,
                distance,
                layerMask,
                QueryTriggerInteraction.Collide
            );

            return isHit;
        }
    }
}
