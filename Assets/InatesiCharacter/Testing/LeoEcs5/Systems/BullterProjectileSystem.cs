using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.LeoEcs5.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs5.Systems
{
    public class BulletProjectileSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsPool<BulletProjectileComponent> _BulletProjectilePool;
        private EcsFilter _BulletProjectileFilter;
        private RaycastHit[] _results = new RaycastHit[32];

        public void Init(IEcsSystems systems)
        {
            _BulletProjectilePool = systems.GetWorld().GetPool<BulletProjectileComponent>();
            _BulletProjectileFilter = systems.GetWorld().Filter<BulletProjectileComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _BulletProjectileFilter)
            {
                ref var bulletProjectileComponent = ref _BulletProjectilePool.Get(entity);

                if (bulletProjectileComponent.destroyed == true)
                    continue;

                var maxDistance = (bulletProjectileComponent.moveSpeed * Time.fixedDeltaTime);

                if (bulletProjectileComponent.isRigidbody == true)
                {
                    //maxDistance = bulletProjectileComponent.moveDirection.magnitude + bulletProjectileComponent.rigidbody.linearVelocity.magnitude;
                }

                var hitsCount = Physics.SphereCastNonAlloc(
                    bulletProjectileComponent.transform.position,
                    .1f,
                    bulletProjectileComponent.moveDirection,
                    _results,
                    .2f + maxDistance,
                    Configs.Config.s_DamageLayerMask,
                    QueryTriggerInteraction.Ignore
                );

                if (hitsCount > 0)
                {
                    for (int i = 0; i < hitsCount; i++)
                    {
                        if (_results[i].transform != null && _results[i].transform != bulletProjectileComponent.transform)
                        {
                            if ((bulletProjectileComponent.ignoreLayer.value & (1 << _results[i].transform.gameObject.layer)) != 0)
                            {
                                continue;

                            }
                            else
                            {
                            }

                            //Debug.Log(_results[i].transform.name + "   dsadasdasd");
                            bulletProjectileComponent.destroyed = true;


                            var newEntity = systems.GetWorld().NewEntity();

                            var hitPool = systems.GetWorld().GetPool<DamageComponent>();
                            hitPool.Add(newEntity);
                            ref var hitComponent = ref hitPool.Get(newEntity);

                            var velocity = (_results[i].point - bulletProjectileComponent.transform.position).normalized * 10f;
                            //var velocity = (_results[i].point - bulletProjectileComponent.transform.position).normalized + bulletProjectileComponent.addForce;
                            //hitComponent.first = transform.root.gameObject;
                            //hitComponent.other = cast.transform.gameObject;
                            hitComponent.owner = null; // player
                            hitComponent.target = _results[i].transform.root.gameObject;
                            hitComponent.damage = 100;
                            hitComponent.velocity = velocity;
                            hitComponent.position = _results[i].point;
                            hitComponent.hit = _results[i];
                            hitComponent.isHit = true;
                            hitComponent.sizeParticle = .67f;
                            hitComponent.speedParticle = 30;
                            hitComponent.ray = new Ray(bulletProjectileComponent.transform.position, bulletProjectileComponent.moveDirection);


                            if (_results[i].rigidbody != null)
                            {
                                _results[i].rigidbody.AddForce(velocity, ForceMode.VelocityChange);
                            }

                            Shared.ParticlesManager.SendParticleEvent(systems.GetWorld(), _results[i]);

                            if (i == 0)
                            {
                                if (bulletProjectileComponent.isRigidbody == false)
                                {
                                    bulletProjectileComponent.transform.position = _results[i].point;
                                }
                            }

                            break;
                        }
                    }
                }

                bulletProjectileComponent.remainLifeTime -= Time.deltaTime;

                if (bulletProjectileComponent.destroyed == false)
                {
                    if (bulletProjectileComponent.isRigidbody == false)
                    {
                        bulletProjectileComponent.transform.position += bulletProjectileComponent.moveSpeed * Time.fixedDeltaTime * bulletProjectileComponent.moveDirection;
                    }
                }


                if (bulletProjectileComponent.destroyed == true)
                {
                    //GameObject.Destroy(bulletProjectileComponent.gameObject);
                    //bulletProjectileComponent.gameObject.SetActive(false);
                    bulletProjectileComponent.remainLifeTime = 0;
                }

                if (bulletProjectileComponent.remainLifeTime <= 0)
                {
                    if (bulletProjectileComponent.isRigidbody == false)
                    {
                        if (bulletProjectileComponent.isTriggerOnDestroy)
                        {
                            if (bulletProjectileComponent.gameObject.TryGetComponent(out Collider component))
                            {
                                component.isTrigger = bulletProjectileComponent.isTriggerOnDestroy;
                            }
                        }
                    }


                    if (bulletProjectileComponent.isRigidbody == true)
                    {
                        //bulletProjectileComponent.rigidbody.isKinematic = true;
                    }

                    GameObject.Destroy(bulletProjectileComponent.gameObject, bulletProjectileComponent.lifetimeAfterDestroy);

                    _BulletProjectilePool.Del(entity);
                    continue;
                }
            }
        }
    }
}
