using InatesiCharacter.Testing.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InatesiCharacter.Testing.Audio
{
    public class AudioRandomPlayer : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> _clipList = new List<AudioClip>();
        [SerializeField] private float _delayBetweenClips = .1f;

        private AudioSource _audioSource;
        private int _n = -1;
        private int _lastIndex = -1;
        private float _delayTime = 5f;
        private float _audioTimeOnPause = 0;



        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

            PlayAudio();

            G.OnPauseGameAction += OnPause;
        }

        private void OnDestroy()
        {
            G.OnPauseGameAction -= OnPause;
        }

        private void Update()
        {
            _delayTime -= G.IsPause == false ? Time.deltaTime : 0;

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

            _delayTime = _clipList[_n].length + _delayBetweenClips;

            if (_lastIndex == _n)
            {
                _n = _n + 1 >= _clipList.Count - 1 ? 0 : _n + 1;
            }

            _lastIndex = _n;

            _audioSource.PlayOneShot(_clipList[_n]);
        }

        private void OnPause(bool state)
        {
            if (_audioSource == null) return;

            if (state)
            {
                if (_audioSource.isPlaying)
                    _audioTimeOnPause = _audioSource.time;

                _audioSource.Pause();
            }
            else
            {
                //_audioSource.time = _audioTimeOnPause;
                _audioSource.UnPause();
            }
        }
    }
}