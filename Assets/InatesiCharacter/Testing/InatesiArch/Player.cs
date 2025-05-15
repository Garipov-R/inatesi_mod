using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InatesiCharacter.Camera;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.InatesiArch.Character;
using InatesiCharacter.Testing.InatesiArch.Character.Abilities;
using UnityEngine;
using Input = Inatesi.Inputs.Input;
using InatesiCharacter.Testing.InatesiArch.Character.Abilities.Attacks;

namespace InatesiCharacter.Testing.InatesiArch
{
    public partial class Player : CharacterBase
    {
        public bool FirstPersonCamera { get; set; }

        public Player()
        {

        }

        public Player(string name /* TODO:   Client client */) : this()
        {

        }

        public override void Respawn()
        {
            Abilities.Add(new Move());
            Abilities.Add(new Jump());
            Abilities.Add(new Attack());
            //Abilities.Add(new Crouch());
            Abilities.Add(new SwitchMovementType());
            //Abilities.Add(new Fly());
            //Abilities.Add(new Run());
            //Abilities.Add(new Crouch2());

            Abilities.Add(new InventorySystems.CharacterInventoryInteraction());

            base.Respawn();

            InputInitialization();

            SetupCamera(out LookSource lookSource, CharacterGameObject);

            //CharacterMotionTest.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.green;
            CharacterMotion.LookSource = lookSource;

            CharacterGameObject.layer = LayerMask.NameToLayer("Player");
            for (int i = 0; i < CharacterGameObject.transform.childCount; i++)
            {
                CharacterGameObject.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Player");
            }

            SetCameraView(FirstPersonCamera);
        }

        public override void Simulate()
        {
            base.Simulate();

            if (Input.Pressed("Use"))
            {
                var usableObject = FindUsable();

                if (usableObject != null)
                {
                    usableObject.GetComponent<UsableObjects.UsableObject>().Event?.Invoke();
                }
            }

            if (Input.Pressed("View"))
            {
                FirstPersonCamera = !FirstPersonCamera;

                SetCameraView(FirstPersonCamera);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Backspace))
            {
                CharacterMotion.SetPositionAndRotation(StartPosition, Quaternion.identity);  
            }
        }

        public void SetCameraView(bool fpc)
        {
            var camera = CharacterMotion.LookSource.GameObject.GetComponent<Camera.CameraMotion>();
            var anchorOffset = camera.AnchorOffset;

            if (FirstPersonCamera)
            {
                var renderers = CharacterMotion.GetComponentsInChildren<Renderer>();
                foreach (var render in renderers)
                {
                    render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                    render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }

                var height = CharacterMotion.GetComponent<CapsuleCollider>().height;

                camera.AnchorOffset = new Vector3(
                    0,
                    height - (height / 4f), //CharacterMotion.RagdollMonitor.HeadBone.position.y - CharacterMotion.transform.position.y + 0.18f,
                    anchorOffset.z
                );
                camera.ZoomAmount = 0;

                CharacterMotion.FirstPersonSettings.ShowFoot();
            }
            else
            {
                var renderers = CharacterMotion.GetComponentsInChildren<Renderer>();
                foreach (var render in renderers)
                {
                    render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }

                var height = CharacterMotion.GetComponent<CapsuleCollider>().height;

                camera.AnchorOffset = new Vector3(
                    0.6f,
                    height, //CharacterMotion.RagdollMonitor.HeadBone.position.y - CharacterMotion.transform.position.y + 0.18f,
                    anchorOffset.z
                );
                camera.ZoomAmount = camera.StartZoom;

                CharacterMotion.FirstPersonSettings.HideFoot();
            }
        }

        public virtual GameObject FindUsable()
        {
            /*if (CharacterLocomotion == null)
                return null;*/

            if (CharacterGameObject == null)
                return null;

            if (CharacterMotion.LookSource == null)
                return null;

            var transform = CharacterMotion.LookSource.Transform;
            var raycastHits = Physics.RaycastAll(transform.position, transform.forward);
            foreach (var hit in raycastHits)
            {
                if (hit.transform.gameObject == transform)
                    continue;

                if (hit.transform.TryGetComponent(out UsableObjects.UsableObject usableObject))
                {
                    //usableObject

                    return usableObject.gameObject;
                }
                    

                //return hit.transform.gameObject;
            }

            return null;
        }





        // исправить
        private void SetupCamera(out LookSource cameraLookSource, GameObject follow)
        {
            var cameraResource = Resources.Load<GameObject>("Prefabs/Camera/[Camera]");
            var cameraGameObject = GameObject.Instantiate(cameraResource);
            cameraLookSource = cameraGameObject.GetComponent<LookSource>();
            var cameraMotion = cameraGameObject.GetComponent<CameraMotion>();
            cameraMotion.Follow = follow.transform;

#if UNITY_EDITOR || UNITY_STANDALONE
            cameraMotion.CursorLockMode = CursorLockMode.Locked;
            cameraMotion.MouseInput = true;
            cameraMotion.CursorVisible = false;
#elif UNITY_ANDROID
            cameraMotion.CursorLockMode = CursorLockMode.None;
            cameraMotion.MouseInput = false;
            cameraMotion.CursorVisible = false;
#endif
        }

        private static void InputInitialization()
        {
            var inputResource = Resources.Load<GameObject>("Prefabs/Inputs/[Input2]");
            var inputGameObject = GameObject.Instantiate(inputResource);
            //var inputSetuper = inputGameObject.GetComponent<Inatesi.Inputs.UnityInput>();
        }
    }
}
