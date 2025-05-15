using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InatesiCharacter.Testing.Audio
{
    public class AudioRandomPlayer : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> _clipList = new List<AudioClip>();

        private AudioSource _audioSource;
        private int _n;
        private float _delayTime = 5f;


        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

            PlayAudio();
        }

        private void Update()
        {
            _delayTime -= Time.deltaTime;

            if (_delayTime < 0) 
            {
                PlayAudio();
            }
        }

        private void PlayAudio()
        {
            if (_audioSource==null) return;

            if (_clipList == null) return;

            if (_clipList.Count == 0) return;

            _n = Random.Range(0, _clipList.Count - 1);

            _audioSource.Stop();
            _audioSource.PlayOneShot(_clipList[_n]);

            _delayTime = _clipList[_n].length + 2;
        }
    }
}