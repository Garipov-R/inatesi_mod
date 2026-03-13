using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace InatesiCharacter.Testing.Utility.ShakeSmooth
{
    public class ShakeSmooth : MonoBehaviour
    {
        private void Start()
        {   
            transform.DOMove(transform.position + transform.forward * 1f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).Play();
            transform.DORotate(Vector3.up * 360, 2f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental).Play();
        }
    }
}