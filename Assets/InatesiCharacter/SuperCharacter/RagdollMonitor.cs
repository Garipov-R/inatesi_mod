using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using InatesiCharacter.SuperCharacter;

namespace InatesiCharacter.SuperCharacter
{
    [Serializable]
    public class RagdollMonitor
    {
        public class BoneTransform
        {
            private Vector3 _Position;
            private Quaternion _Rotation;

            public Vector3 Position { get => _Position; set => _Position = value; }
            public Quaternion Rotation { get => _Rotation; set => _Rotation = value; }

            public BoneTransform()
            {
                _Position = Vector3.zero;
                _Rotation = Quaternion.identity;
            }
        }

        [SerializeField] private string _RagdollLayerMask = "CharacterIgnore";
        [SerializeField] private string _NotRagdollLayerMask = "CharacterIgnore";

        private Rigidbody[] _Rigidbodies;
        private CharacterMotionBase _CharacterMotionBase;
        private Transform[] _Bones;
        private BoneTransform[] _lastBoneTransforms;
        private BoneTransform[] _ragdollBoneTransforms;
        private float _elapsedResetBonesTime;
        private Transform _HipsBone;
        private Transform _HeadBone;
        private Transform _SpineBone;
        private Transform _RightHandBone;

        public Transform HipsBone { get => _HipsBone; set => _HipsBone = value; }
        public Transform HeadBone { get => _HeadBone; set => _HeadBone = value; }
        public Transform SpineBone { get => _SpineBone; set => _SpineBone = value; }
        public Transform RightHand { get => _RightHandBone; set => _RightHandBone = value; }
        public bool IsActive { get; private set; }


        public void Initialize(CharacterMotionBase characterMotionTest)
        {
            if (characterMotionTest.AnimatorMonitor.Animator == null) 
            {
                IsActive = false; 
                return; 
            } 

            _CharacterMotionBase = characterMotionTest;

            if (_CharacterMotionBase.AnimatorMonitor.Animator.avatar == null) return;
            if (_CharacterMotionBase.AnimatorMonitor.Animator.GetBoneTransform(HumanBodyBones.Spine) == null) return;

             _Rigidbodies = _CharacterMotionBase.GetComponentsInChildren<Rigidbody>();
            
            _HipsBone = _CharacterMotionBase.AnimatorMonitor.Animator.GetBoneTransform(HumanBodyBones.Hips);
            _HeadBone = _CharacterMotionBase.AnimatorMonitor.Animator.GetBoneTransform(HumanBodyBones.Head);
            _SpineBone = _CharacterMotionBase.AnimatorMonitor.Animator.GetBoneTransform(HumanBodyBones.Spine);
            _RightHandBone = _CharacterMotionBase.AnimatorMonitor.Animator.GetBoneTransform(HumanBodyBones.RightHand);
            _Bones = _HipsBone.GetComponentsInChildren<Transform>();

            _lastBoneTransforms = new BoneTransform[_Bones.Length];
            _ragdollBoneTransforms = new BoneTransform[_Bones.Length];

            for (int boneIndex = 0; boneIndex < _Bones.Length; boneIndex++)
            {
                _lastBoneTransforms[boneIndex] = new BoneTransform();
                _ragdollBoneTransforms[boneIndex] = new BoneTransform();
            }

            PopulateBoneTransforms(_lastBoneTransforms);

            if (_Rigidbodies != null && _Rigidbodies.Length > 0)
            {
                IsActive = true;    
            }
        }

        public void EnableRagdoll()
        {
            SetRagdoll(true);
        }

        public void DisableRagdoll()
        {
            SetRagdoll(false);
        }

        private void SetRagdoll(bool state)
        {
            _elapsedResetBonesTime = 0;

            if (_Rigidbodies == null)
                return;

            if (_Rigidbodies.Length == 0)
                return;

            if (state == false)
            {
                //AlignRotationToHips();
                //AlignPositionToHips();
                // _CharacterMotionTest.transform.position = _HipsBone.position;
            }

            if (_CharacterMotionBase.CharacterController) _CharacterMotionBase.CharacterController.enabled = !state;
            if (_CharacterMotionBase.Collider) _CharacterMotionBase.Collider.enabled = !state;
            _CharacterMotionBase.AnimatorMonitor.Animator.enabled = !state;
            _CharacterMotionBase.IsFreeze = state;

            foreach (var rigidbody in _Rigidbodies)
            {
                if (rigidbody.gameObject == _CharacterMotionBase.gameObject)
                    continue;

                rigidbody.isKinematic = !state;
                rigidbody.GetComponent<Collider>().enabled = state;
                rigidbody.gameObject.layer = state ? LayerMask.NameToLayer( _RagdollLayerMask) : _CharacterMotionBase.gameObject.layer;
                rigidbody.useGravity = state;
            }

            if (state == true)
            {
                //Force(_CharacterMotionBase.Velocity * 20f, ForceMode.VelocityChange);
            }
        }

        private void AlignRotationToHips()
        {
            _CharacterMotionBase.DeltaRotation = Vector3.zero;

            Vector3 originalHipsPosition = _HipsBone.position;
            Quaternion originalHipsRotation = _HipsBone.rotation;

            Vector3 desiredDirection = _HipsBone.up * -1;
            desiredDirection.y = 0;
            desiredDirection.Normalize();

            Quaternion fromToRotation = Quaternion.FromToRotation(_CharacterMotionBase.transform.forward, desiredDirection);
            _CharacterMotionBase.transform.rotation *= fromToRotation;

            _HipsBone.position = originalHipsPosition;
            _HipsBone.rotation = originalHipsRotation;
        }

        public void AlignPositionToHips()
        {
            Vector3 originalHipsPosition = _HipsBone.position;
            _CharacterMotionBase.transform.position = _HipsBone.position;

            /*Vector3 positionOffset = _lastBoneTransforms[0].Position;
            positionOffset.y = 0;
            positionOffset = _CharacterMotionTest.transform.rotation * positionOffset;
            _CharacterMotionTest.transform.position -= positionOffset;

            if (Physics.Raycast(_CharacterMotionTest.transform.position, Vector3.down, out RaycastHit hitInfo))
            {
                _CharacterMotionTest.transform.position = new Vector3(
                    _CharacterMotionTest.transform.position.x,
                    hitInfo.point.y,
                    _CharacterMotionTest.transform.position.z
                );
            }*/

            _HipsBone.position = originalHipsPosition;
        }

        public void Force(Vector3 force, ForceMode forceMode = ForceMode.Force)
        {
            if (_Rigidbodies == null) return;
            if (_Rigidbodies.Length == 0) return;

            foreach (var rigidbody in _Rigidbodies)
            {
                rigidbody.AddForce(force, forceMode);
            }
        }

        public void Force(Vector3 force, ForceMode forceMode = ForceMode.Force, HumanBodyBones humanBodyBones = HumanBodyBones.Spine)
        {
            if (_Rigidbodies == null) return;
            if (_Rigidbodies.Length == 0) return;

            foreach (var rigidbody in _Rigidbodies)
            {
                if (_CharacterMotionBase.AnimatorMonitor.Animator.GetBoneTransform(humanBodyBones) == rigidbody.transform)
                    continue;

                rigidbody.AddForce(force, forceMode);
            }
        }

        public void ResettingBonesBehaviour()
        {
            //_elapsedResetBonesTime += Time.deltaTime;
            //float elapsedPercentage = _elapsedResetBonesTime / _timeToResetBones;
            _elapsedResetBonesTime += Time.deltaTime;
            float elapsedPercentage = _elapsedResetBonesTime / .5f;

            for (int boneIndex = 0; boneIndex < _Bones.Length; boneIndex++)
            {
                _Bones[boneIndex].localPosition = Vector3.Lerp(
                    _ragdollBoneTransforms[boneIndex].Position,
                    _lastBoneTransforms[boneIndex].Position,
                    elapsedPercentage);

                _Bones[boneIndex].localRotation = Quaternion.Lerp(
                    _ragdollBoneTransforms[boneIndex].Rotation,
                    _lastBoneTransforms[boneIndex].Rotation,
                    elapsedPercentage);
            }

            /*if (elapsedPercentage >= 1)
            {
                _currentState = ZombieState.StandingUp;
                DisableRagdoll();

                _animator.Play(_standUpStateName);
            }*/
        }

        private void PopulateBoneTransforms(BoneTransform[] boneTransforms)
        {
            for (int boneIndex = 0; boneIndex < _Bones.Length; boneIndex++)
            {
                boneTransforms[boneIndex].Position = _Bones[boneIndex].localPosition;
                boneTransforms[boneIndex].Rotation = _Bones[boneIndex].localRotation;
            }
        }

        public void CopyBoneTransforms(Transform hipsBoneTransforms)
        {
            if (hipsBoneTransforms == null)
                return;

            var bones = hipsBoneTransforms.GetComponentsInChildren<Transform>();

            if (bones == null)
                return;

            for (int boneIndex = 0; boneIndex < _Bones.Length; boneIndex++)
            {
                if (bones[boneIndex] == null) continue;
                bones[boneIndex].localPosition = _Bones[boneIndex].localPosition;
                bones[boneIndex].localRotation = _Bones[boneIndex].localRotation;
            }
        }

        public void PopulateAnimationStartBoneTransforms(string clipName, BoneTransform[] boneTransforms)
        {
            Vector3 positionBeforeSampling = _CharacterMotionBase.transform.position;
            Quaternion rotationBeforeSampling = _CharacterMotionBase.transform.rotation;

            /*_CharacterMotionTest.AnimatorMonitor.Animator.Play(clipName, 0, 0);
            PopulateBoneTransforms(boneTransforms);
            Debug.Log(54);*/

            foreach (AnimationClip clip in _CharacterMotionBase.AnimatorMonitor.Animator.runtimeAnimatorController.animationClips)
            {
                Debug.Log(clip.name);
                if (clip.name == clipName)
                {
                    Debug.Log(54);
                    clip.SampleAnimation(_CharacterMotionBase.gameObject, 0);
                    PopulateBoneTransforms(boneTransforms);
                    break;
                }
            }

            _CharacterMotionBase.transform.position = positionBeforeSampling;
            _CharacterMotionBase.transform.rotation = rotationBeforeSampling;
        }

        public void SetLastBoneTransforms(string clipName)
        {
            PopulateAnimationStartBoneTransforms(clipName, _lastBoneTransforms);
        }

        public void SetRagdollBoneTransforms()
        {
            PopulateBoneTransforms(_ragdollBoneTransforms);
        }
    }
}
