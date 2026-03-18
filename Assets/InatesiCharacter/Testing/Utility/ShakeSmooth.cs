using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace InatesiCharacter.Testing.Utility.ShakeSmooth
{
    public class ShakeSmooth : MonoBehaviour
    {
        [SerializeField] private Vector3 _Rotate = new Vector3(0, 360, 0);
        [SerializeField] private float _rotateDuration = 2f;
        [SerializeField] private Vector3 _Move = new Vector3(0,0,1);
        [SerializeField] private float _moveDuration = 2f;

        private void Start()
        {   
            transform.DOMove(transform.position + _Move, _moveDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).Play();
            transform.DORotate(_Rotate, _rotateDuration, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental).Play();
        }

        private void OnDisable()
        {
            transform.DOKill();
        }
    }
}