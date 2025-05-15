using System.Collections;
using UnityEngine;

namespace InatesiCharacter
{
    public class TargetFramerate : MonoBehaviour
    {
        [SerializeField] [Range(-1f, 200)] private int _Frame = 60;


        private void OnValidate()
        {
            Application.targetFrameRate = _Frame;
        }   

        private void Start()
        {
            Application.targetFrameRate = _Frame;
        }
    }
}