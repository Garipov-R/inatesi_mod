using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.InatesiArch.Character.Abilities;
using InatesiCharacter.Testing.InatesiArch.Character;
using InatesiCharacter.Testing.InatesiArch;
using System.Collections;
using UnityEngine;
using Input = Inatesi.Inputs.Input;
using System.Threading.Tasks;
using RootMotion.FinalIK;


namespace InatesiCharacter.Testing.InatesiArch.Character.Abilities
{
    public class Crouch2 : AbilityBase
    {
        private float _crouchLerp;

        public override void Update()
        {
            GetCharacterVars(out CharacterVars cv);

            if (cv.Flying == true) return;

            float crouchingHeight = Mathf.Clamp(_CharacterMotion.MoveConfig.CrouchHeight, 0.01f, 1f);
            bool grounded = CharacterMotion.OnGrounded;
            bool wantsToCrouch = Input.Down("Crouch");

            if (grounded)
                _crouchLerp = Mathf.Lerp(_crouchLerp, wantsToCrouch ? 1f : 0f, Time.deltaTime * _CharacterMotion.MoveConfig.CrouchingSpeed);
            else if (!grounded && !wantsToCrouch && _crouchLerp < 0.95f)
                _crouchLerp = 0f;
            else if (!grounded && wantsToCrouch)
                _crouchLerp = 1f;

            //Debug.Log($"{_crouchLerp} {wantsToCrouch}");

            // Collider and position changing
            if (_crouchLerp > 0.9f && !cv.Crouched)
            {
                // Begin crouching
                cv.Crouched = true;
                CharacterMotion.CharacterController.height = CharacterMotion.Height * crouchingHeight;
                CharacterMotion.CharacterController.center = new Vector3(0, CharacterMotion.CharacterController.center.y * crouchingHeight, 0);

                _CharacterMotion.AnimatorMonitor.SetAbilityID(4);
            }


            if (_crouchLerp <= 0.9f)
            {

                cv.Crouched = false;
                CharacterMotion.CharacterController.height = CharacterMotion.Height;
                CharacterMotion.CharacterController.center = new Vector3(0, CharacterMotion.Height / 2, 0);

                _CharacterMotion.AnimatorMonitor.SetAbilityID(0);
            }



            if (Game.PlayerInstance != null)
            {
                Game.PlayerInstance.SetCameraView(true);
            }

            ApplyCharacterVars(cv);

            return;
            if (Input.Pressed("Crouch"))
            {
                //cv.Crouched = !cv.Crouched;
                _CharacterMotion.SpeedMove = cv.Crouched ? _CharacterMotion.SpeedMove / 2 : _CharacterMotion.SpeedMove * 2;


                if (cv.Crouched == true)
                {
                    _CharacterMotion.AnimatorMonitor.SetAbilityID(4);

                }
                else
                {
                    _CharacterMotion.AnimatorMonitor.SetAbilityID(0);
                }

                CrouchTimer();

                ApplyCharacterVars(cv);

                RefreshCollider();

                if (Game.PlayerInstance != null)
                {
                    Game.PlayerInstance.SetCameraView(true);
                }
            }
        }

        private void RefreshCollider()
        {
            GetCharacterVars(out CharacterVars cv);


            var d = 1f;
            //d = CharacterMotion.RagdollMonitor.HeadBone.position.y - CharacterMotion.transform.position.y;
            var h2 = CharacterMotion.CharacterController.height;
            var c2 = CharacterMotion.CharacterController.center.y;

            if (cv.Crouched == false)
            {
                h2 += d;
                c2 = c2 + (d / 2);
            }
            else
            {
                h2 -= d;
                c2 = c2 - (d / 2);
            }

            CharacterMotion.CharacterController.enabled = false;
            CharacterMotion.CharacterController.height = h2;
            CharacterMotion.CharacterController.center =
                new Vector3(CharacterMotion.CharacterController.center.x, c2, CharacterMotion.CharacterController.center.z);
            //CharacterMotion.CharacterController.center += Vector3.up * c2;
            CharacterMotion.CharacterController.enabled = true;
            /*var height = (CharacterMotion.RagdollMonitor.HeadBone.transform.position.y - CharacterMotion.transform.position.y) / 2;

            CharacterMotion.CharacterController.height = 
                cv.Crouched == true ? CharacterMotion.CharacterController.height - height : CharacterMotion.CharacterController.height + height;
            var center = CharacterMotion.CharacterController.center;
            center.y = cv.Crouched == true ? center.y - (height / 2) : center.y + (height / 2);
            CharacterMotion.CharacterController.center = center;*/
        }

        public async void CrouchTimer()
        {
            float timer = 0f;
            while (timer < 0.3f)
            {
                timer += Time.deltaTime;
                await Task.Yield();
            }

            //RefreshCollider();

            if (Game.PlayerInstance == null)
                return;

            Game.PlayerInstance.SetCameraView(true);

            /*var y = Game.PlayerInstance.FirstPersonCamera ? CharacterMotion.CharacterController.height / 4f : CharacterMotion.RagdollMonitor.HeadBone.position.y - CharacterMotion.transform.position.y;

            if (Game.PlayerInstance.FirstPersonCamera)
                y = CharacterMotion.CharacterController.height - (CharacterMotion.CharacterController.height / 4f);
            else
                y = CharacterMotion.CharacterController.height;

            var camera = CharacterMotion.LookSource.GameObject.GetComponent<Camera.CameraMotion>();
            var anchorCamera = camera.AnchorOffset;
            camera.AnchorOffset = new Vector3(
                anchorCamera.x,
                y,
                anchorCamera.z
            );*/
        }
    }
}