using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.Character.FPS_Utility
{
    public class FirstPersonCameraHelper : MonoBehaviour
    {
        [SerializeField] private Transform _RightHandTransform;

        public Transform RightHandTransform { get => _RightHandTransform; set => _RightHandTransform = value; }


        public void SetRightHandObject(GameObject weapon)
        {
            if (_RightHandTransform.childCount > 0)
            {
                for (int i = 0; i < _RightHandTransform.childCount; i++)
                {
                    //Destroy(_RightHandTransform.GetChild(i).gameObject);
                }
            }

            if (weapon != null)
            {
                weapon.transform.SetParent(_RightHandTransform);

                //Debug.Log(weapon.transform.root.name);
            }
        }
    }
}
