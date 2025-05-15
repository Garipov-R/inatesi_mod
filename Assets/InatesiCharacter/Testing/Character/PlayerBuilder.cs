using InatesiCharacter.Camera;
using InatesiCharacter.SuperCharacter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace InatesiCharacter.Testing.Character
{
    public static class PlayerBuilder
    {
        private static CharacterMotionBase _characterMotionBase;

        public static void Build(CharacterMotionBase characterMotionBase)
        {
            //InputInitialization();
            SetupCamera(out LookSource cameraLookSource, characterMotionBase.gameObject);
            characterMotionBase.LookSource = cameraLookSource;

            SetCameraView(false, characterMotionBase);

            _characterMotionBase = characterMotionBase;
        }

        private static void InputInitialization()
        {
            var inputResource = Resources.Load<GameObject>(Configs.Config.c_InputPath);
            var inputGameObject = GameObject.Instantiate(inputResource);

            //var inputSetuper = inputGameObject.GetComponent<Inatesi.Inputs.UnityInput>();
        }

        public static void SetupCamera(out LookSource cameraLookSource, GameObject player)
        {
            var cameraResource = Resources.Load<GameObject>(Configs.Config.c_CameraPath);
            var cameraGameObject = GameObject.Instantiate(cameraResource);
            cameraLookSource = cameraGameObject.GetComponent<LookSource>();
            var cameraMotion = cameraGameObject.GetComponent<CameraMotion>();
            cameraMotion.Follow = player ?  player.transform : null;
        }

        public static void SetCameraView(bool fpc, Transform target)
        {
            if (_characterMotionBase != null && target != null)
            {
                var camera = _characterMotionBase.LookSource.GameObject.GetComponent<CameraMotion>();
                camera.Follow = target;
                SetCameraView(fpc, _characterMotionBase);
                camera.AnchorOffset = Vector3.zero;
                camera.ZoomAmount = camera.StartZoom;
            }
        }

        public static void SetCameraView(bool fpc, CharacterMotionBase CharacterMotion = null)
        {
            if (_characterMotionBase != null) 
            {
                CharacterMotion = _characterMotionBase;
            }
            else
            {
                return;
            }

            var camera = CharacterMotion.LookSource.GameObject.GetComponent<Camera.CameraMotion>();
            var anchorOffset = camera.AnchorOffset;

            if (fpc)
            {
                var renderers = CharacterMotion.GetComponentsInChildren<Renderer>();
                foreach (var render in renderers)
                {
                    //render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                    //render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }

                var height = CharacterMotion.GetComponent<CapsuleCollider>().height;
                height = CharacterMotion.Height;

                camera.AnchorOffset = new Vector3(
                    0,
                    height - CharacterMotion.Radius / 2, //CharacterMotion.RagdollMonitor.HeadBone.position.y - CharacterMotion.transform.position.y + 0.18f,
                    anchorOffset.z
                );
                camera.ZoomAmount = 0;

                camera.Camera.cullingMask = ~((1 << LayerMask.NameToLayer("FPC")) | (1 << LayerMask.NameToLayer("Player")));

                //CharacterMotion.FirstPersonSettings.ShowFoot();
            }
            else
            {
                var renderers = CharacterMotion.GetComponentsInChildren<Renderer>();
                foreach (var render in renderers)
                {
                    //render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }

                var height = CharacterMotion.Height - CharacterMotion.Radius / 2;

                camera.AnchorOffset = new Vector3(
                    0.3f,
                    height, //CharacterMotion.RagdollMonitor.HeadBone.position.y - CharacterMotion.transform.position.y + 0.18f,
                    anchorOffset.z
                );
                camera.ZoomAmount = camera.StartZoom;

                
                camera.Camera.cullingMask = ~(1 << LayerMask.NameToLayer("FPC"));

                //CharacterMotion.FirstPersonSettings.HideFoot();
            }
        }
    }
}
