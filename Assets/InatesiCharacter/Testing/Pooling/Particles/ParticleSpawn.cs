using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace InatesiCharacter.Testing.Pooling.Particles
{
    public class ParticleSpawn : MonoBehaviour
    {
        [SerializeField] private int _DefaultCapacity = 20;
        [SerializeField] private int _MaxSize = 100;
        [SerializeField] private float _ParticleLifeTime = .5f;
        [SerializeField] private float _UpdateTime = .01f;
        [SerializeField] private Particle _Particle;
        [SerializeField] private bool _update = true;

        IObjectPool<Particle> _ObjectPool;
        private GameObject _gameObject;

        private void Awake()
        {
            _ObjectPool = new ObjectPool<Particle>(CreateParticle, OnGet, OnRelease, OnDestroyPool, true, _DefaultCapacity, _MaxSize);
            //_ObjectPool.Get()
            //_ObjectPool.Get();
        }


        private float timer;
        private void Update()
        {
            if (_update == false)
            {
                return; 
            }

            if (timer < 0)
            {
                timer = .01f;
                Show();
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }

        public void Show()
        {
            _ObjectPool.Get(out Particle particle);
            particle.transform.localScale = Random.value * Vector3.one + Vector3.one / 4;
            particle.transform.localPosition = Vector3.zero + Random.insideUnitSphere * 2;
            particle.transform.rotation = Quaternion.identity * Random.rotation;
            particle.GetComponent<Renderer>().material.color = Random.ColorHSV();
            particle.S_Timer = _ParticleLifeTime;
        }

        private Particle CreateParticle()
        {
            Particle a = Instantiate(_Particle, Vector3.zero + Random.insideUnitSphere * 3, Quaternion.identity * Random.rotation);
            a.Pool = _ObjectPool;
            a.Pool.Release(a);
            a.GetComponent<Renderer>().material.color = Random.ColorHSV();
            a.transform.SetParent(this.transform);
            return a;
        }

        private void OnGet(Particle particle)
        {
            particle.gameObject.SetActive(true);
        }

        private void OnRelease(Particle particle)
        {
            particle.gameObject.SetActive(false);
        }

        private void OnDestroyPool(Particle gameObject)
        {
            //Destroy(gameObject);
        }
    }
}