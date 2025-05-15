using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leopotam.EcsLite;
using TMPro;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs3.Shared.Systems
{
    public class WaterSystem : IEcsRunSystem
    {
        private Dictionary<Collider, Rigidbody> _bodyDictionary = new Dictionary<Collider, Rigidbody>();
        private Dictionary<GameObject, Rigidbody> _bodyDictionary2 = new Dictionary<GameObject, Rigidbody>();
        EcsFilter hitFilter;
        EcsPool<HitComponent> hitPool;

        public void Run(IEcsSystems systems)
        {
            hitFilter = systems.GetWorld().Filter<HitComponent>().End();
            hitPool = systems.GetWorld().GetPool<HitComponent>();

            foreach (var hitEntity in hitFilter)
            {
                ref var hitComponent = ref hitPool.Get(hitEntity);

                if (hitComponent.other == null) continue;

                if (hitComponent.other.TryGetComponent(out Rigidbody rigidbody))
                {
                    //rigidbody.useGravity = !hitComponent.isEnter;

                    if (hitComponent.isEnter)
                    {
                        if (_bodyDictionary.ContainsValue(rigidbody) == false)
                        {
                            var collider = rigidbody.GetComponent<Collider>();

                            if (collider == null)
                            {
                                collider = rigidbody.GetComponentInChildren<Collider>();
                            }

                            //if (collider == null) continue;

                            _bodyDictionary2.Add(rigidbody.gameObject, rigidbody);
                            _bodyDictionary.Add(collider, rigidbody);
                        }
                    }
                    else
                    {
                        if (_bodyDictionary.ContainsValue(rigidbody) == true)
                        {
                            _bodyDictionary.Remove(rigidbody.GetComponent<Collider>());
                        }

                        if (_bodyDictionary2.ContainsValue(rigidbody) == true)
                        {
                            _bodyDictionary2.Remove(rigidbody.gameObject);
                        }


                        rigidbody.useGravity = true; 
                    }
                }

                
            }

            if (_bodyDictionary2.Keys == null)
            {
                _bodyDictionary2.Clear();
            }

            foreach (var g in _bodyDictionary2.Keys)
            {
                if (g == null)
                {
                    _bodyDictionary2.Remove(g);
                    continue;
                }

                WaterUpdate(_bodyDictionary2[g]);
            }

            foreach (var collider in _bodyDictionary.Keys) 
            {
                continue;

                if (collider == null)
                { 
                    _bodyDictionary.Remove(collider);
                    continue; 
                }


                var rigidbody = _bodyDictionary[collider];
                CheckWater(collider, rigidbody);
            }
        }


        private Collider[] _waterTestCache = new Collider[64];

        private void WaterUpdate(Rigidbody rigidbody)
        {
            float waterDepth = 0;

            var hitCount = Physics.OverlapBoxNonAlloc(rigidbody.transform.position, rigidbody.transform.lossyScale, _waterTestCache, rigidbody.transform.rotation, 1 << LayerMask.NameToLayer("Water"));

            if (hitCount <= 0) return;

            for (int i = 0; i < hitCount; i++)
            {
                if (!_waterTestCache[i].enabled)
                {
                    continue;
                }

                var water = _waterTestCache[i];

                var headPoint = rigidbody.transform.position ;

                if (water.bounds.Contains(headPoint))
                {
                    waterDepth = 1f;
                }
            }

            if (waterDepth >= .5f)
            {
                //_surfer.MoveType = MoveType.Swim;

                //_underwater = true;
                rigidbody.useGravity = false;
                rigidbody.angularVelocity = Vector3.Lerp(rigidbody.angularVelocity, Vector3.zero, Time.deltaTime * 2f);

                rigidbody.linearVelocity = Vector3.MoveTowards(rigidbody.linearVelocity, Vector3.up, Time.deltaTime * 200f);

                //rigidbody.transform.localRotation = Quaternion.RotateTowards(rigidbody.transform.localRotation, Quaternion.LookRotation(Vector3.up), Time.deltaTime * 300f);
            }
            else if (waterDepth <= .5f)
            {
                rigidbody.angularVelocity = Vector3.Lerp(rigidbody.angularVelocity, Vector3.zero, Time.deltaTime * 10f);
                rigidbody.linearVelocity = Vector3.Lerp(rigidbody.linearVelocity, Vector3.zero, Time.deltaTime * 10f);

                //rigidbody.transform.localRotation = Quaternion.RotateTowards(rigidbody.transform.localRotation, Quaternion.LookRotation(Vector3.up), Time.deltaTime * 300f);
            }
            else
            {
                rigidbody.useGravity = true;
            }
        }

        private void CheckWater(Collider collider, Rigidbody rigidbody)
        {
            float waterDepth = 0;
            var extents = collider.bounds.extents;
            //extents.x *= 0.9f;
            //extents.z *= 0.9f;
            var center = collider.transform.position;
            //center.y += extents.y;

            var hitCount = Physics.OverlapBoxNonAlloc(center, extents, _waterTestCache, collider.transform.rotation, 1 << LayerMask.NameToLayer("Water"));

            if (hitCount <= 0) return;

            for (int i = 0; i < hitCount; i++)
            {
                if (!_waterTestCache[i].enabled)
                {
                    continue;
                }

                var water = _waterTestCache[i];

                var headPoint = collider.transform.position + new Vector3(0, collider.bounds.size.y, 0);

                if (water.bounds.Contains(headPoint))
                {
                    waterDepth = 1f;
                }
                else
                {
                    var closetsPoint = water.ClosestPoint(headPoint);
                    var dist = Vector3.Distance(closetsPoint, headPoint);
                    waterDepth = Mathf.Max(0.1f, (collider.bounds.size.y - dist) / collider.bounds.size.y);
                }
            }

            if (waterDepth >= .5f)
            {
                //_surfer.MoveType = MoveType.Swim;

                //_underwater = true;
                rigidbody.useGravity = false;
                rigidbody.angularVelocity = Vector3.Lerp(rigidbody.angularVelocity, Vector3.zero, Time.deltaTime * 10f);

                rigidbody.linearVelocity = Vector3.MoveTowards(rigidbody.linearVelocity, new Vector3(0, collider.bounds.size.y, 0) - new Vector3(0, rigidbody.position.y, 0), Time.deltaTime * 200f);

                //rigidbody.transform.localRotation = Quaternion.RotateTowards(rigidbody.transform.localRotation, Quaternion.LookRotation(Vector3.up), Time.deltaTime * 300f);
            }
            else if (waterDepth <= .5f)
            {
                rigidbody.angularVelocity = Vector3.Lerp(rigidbody.angularVelocity, Vector3.zero, Time.deltaTime * 10f);
                rigidbody.linearVelocity = Vector3.Lerp(rigidbody.linearVelocity, Vector3.zero, Time.deltaTime * 10f);
            }
            else
            {
                rigidbody.useGravity = true;
            }
            //else if (_surfer.MoveType == MoveType.Swim)
            /*else if (_underwater == true)
            {
                //_surfer.MoveType = MoveType.Walk;

                //_underwater = false;
            }*/
        }
    }
}
