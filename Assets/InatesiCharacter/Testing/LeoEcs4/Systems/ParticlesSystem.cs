using InatesiCharacter.SuperCharacter;
using InatesiCharacter.Testing.LeoEcs;
using InatesiCharacter.Testing.LeoEcs.Shared;
using InatesiCharacter.Testing.LeoEcs4.Components;
using InatesiCharacter.Testing.LeoEcs4.Events;
using InatesiCharacter.Testing.LeoEcs4.PoolSystems;
using InatesiCharacter.Testing.Shared.Components;
using Leopotam.EcsLite;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using UnityEngine.VFX;

namespace InatesiCharacter.Testing.LeoEcs4.Systems
{
    public class ParticlesSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilter _ParticleFilter;
        private EcsFilter _HitFilter;
        private EcsFilter _DamageFilter;
        private EcsFilter _ParticleEventFilter;
        private EcsPool<ParticleComponent> _ParticlePool;
        private EcsPool<HitComponent> _HitPool;
        private EcsPool<DamageComponent> _DamagePool;
        private EcsPool<ParticleEvent> _ParticleEventPool;
        private IObjectPool<VisualEffect> _ExternalParticlesObjectPool;
        private IObjectPool<AudioSource> _AudioSourcePool;
        private Dictionary<VisualEffect, IObjectPool<VisualEffect>> _DictionaryExternalParticlesPool = new();
        private SharedData _SharedData;
        private EcsWorld _EcsWorld;
        private EcsFilter _CharacterFilter;
        private EcsPool<CharacterComponent> _CharacterPool;

        public void Init(IEcsSystems systems)
        {
            _EcsWorld = systems.GetWorld();
            _SharedData = systems.GetShared<SharedData>();
            _ParticleFilter = systems.GetWorld().Filter<ParticleComponent>().End();
            _ParticlePool = systems.GetWorld().GetPool<ParticleComponent>();
            _HitPool = systems.GetWorld().GetPool<HitComponent>();
            _HitFilter = systems.GetWorld().Filter<HitComponent>().End();
            _DamagePool = systems.GetWorld().GetPool<DamageComponent>();
            _DamageFilter = systems.GetWorld().Filter<DamageComponent>().End();
            _ParticleEventPool = systems.GetWorld().GetPool<ParticleEvent>();
            _ParticleEventFilter = systems.GetWorld().Filter<ParticleEvent>().End();
            _CharacterFilter = systems.GetWorld().Filter<CharacterComponent>().End();
            _CharacterPool = systems.GetWorld().GetPool<CharacterComponent>();


            if (_SharedData.ParticleSettingsSO != null)
            {
                _AudioSourcePool = new ObjectPool<AudioSource>(
                    () => CreateAudioSource(_SharedData.ParticleSettingsSO.AudioSource),
                    OnGetAudio,
                    OnReleaseAudio,
                    OnDestroyAudio,
                    _SharedData.ParticleSettingsSO.CollectionCheck,
                    _SharedData.ParticleSettingsSO.DefaultCapacity,
                    _SharedData.ParticleSettingsSO.MaxSize
                );

                foreach (var item in _SharedData.ParticleSettingsSO.VisualEffectList)
                {
                    ObjectPool<VisualEffect> pool = new(
                        () => CreateParticle(item),
                        OnGetParticle,
                        OnReleaseParticle,
                        OnDestroyPool,
                        _SharedData.ParticleSettingsSO.CollectionCheck,
                        _SharedData.ParticleSettingsSO.DefaultCapacity,
                        _SharedData.ParticleSettingsSO.MaxSize
                    );
                    _DictionaryExternalParticlesPool.Add(item, pool);
                }
            }
            
        }

        public void Run2(IEcsSystems systems)
        {
            foreach (var entity in _ParticleEventFilter)
            {
                ref var particleEventComponent = ref _ParticleEventPool.Get(entity);

                var ray = particleEventComponent.hit.transform != null ? new Ray(particleEventComponent.hit.point + particleEventComponent.hit.normal * 0.01f, -particleEventComponent.hit.normal) : new Ray();
                var visualEffectParticle = SurfaceManager.singleton.GetParticle(
                    ray,
                    particleEventComponent.hit.transform != null ? particleEventComponent.hit.collider : null,
                    particleEventComponent.hit.transform != null ? particleEventComponent.hit.point : Vector3.zero
                );

                AudioClip hitAudio = null;


                if (particleEventComponent.hit.transform != null)
                {
                    var characterLayer = LayerMask.LayerToName(particleEventComponent.hit.transform.gameObject.layer);
                    if ("Character" == characterLayer || "Player" == characterLayer || "CharacterHitCollider" == characterLayer)
                    {
                        visualEffectParticle = null;
                    }
                    else
                    {
                        hitAudio = SurfaceManager.singleton.GetHitAudio(
                            ray,
                            particleEventComponent.hit.transform != null ? particleEventComponent.hit.collider : null,
                            particleEventComponent.hit.transform != null ? particleEventComponent.hit.point : Vector3.zero
                        );
                    }
                }

                /*SendEventObjectPool.Send(
                                        systems.GetWorld(),
                                        characterComponent.CharacterSO.BloodMeshDecal,
                                        characterComponent.CharacterMotionBase.transform.position,
                                        Quaternion.LookRotation(-characterComponent.CharacterMotionBase.Up),
                                        Pooling.PoolType.Particle
                                    );*/

                //var audioSource = 

                if (hitAudio != null)
                {
                    //audioSource.transform.position = particleEventComponent.hit.point;
                    //audioSource.clip = hitAudio;
                    //audioSource.Play();
                }
            }
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var particle in _ParticleFilter)
            {
                ref var particleComponent = ref _ParticlePool.Get(particle);

                if (particleComponent.lifetime > 0)
                {
                    particleComponent.lifetime -= Time.deltaTime;
                    continue;
                }

                if (particleComponent.lifetime <= 0 && particleComponent.gameObject.activeSelf == true)
                {
                    foreach (var item in _DictionaryExternalParticlesPool.Keys)
                    {
                        if (item.visualEffectAsset == particleComponent.visualEffect.visualEffectAsset)
                        {
                            if (_DictionaryExternalParticlesPool.TryGetValue(item, out IObjectPool<VisualEffect> pool))
                            {
                                pool.Release(particleComponent.visualEffect);
                            }
                        }
                    }

                    if (particleComponent.audioSource != null)
                    {
                        _AudioSourcePool.Release(particleComponent.audioSource);
                    }
                }
            }

            foreach (var entity in _DamageFilter)
            {
                ref var damageCom = ref _DamagePool.Get(entity);

                if (damageCom.isHit == false) continue;
            }

            foreach (var entity in _ParticleEventFilter)
            {
                ref var particleEventComponent = ref _ParticleEventPool.Get(entity);
                IObjectPool<VisualEffect> objectPool = null;

                var ray = particleEventComponent.hit.transform != null ? new Ray(particleEventComponent.hit.point + particleEventComponent.hit.normal * 0.01f, -particleEventComponent.hit.normal) : new Ray();
                var visualEffectParticle = SurfaceManager.singleton.GetParticle(
                    ray,
                    particleEventComponent.hit.transform != null ? particleEventComponent.hit.collider : null,
                    particleEventComponent.hit.transform != null ? particleEventComponent.hit.point : Vector3.zero
                );
                AudioClip hitAudio = null;


                if (particleEventComponent.hit.transform != null)
                {
                    var characterLayer = LayerMask.LayerToName(particleEventComponent.hit.transform.gameObject.layer);
                    if ("Character" == characterLayer || "Player" == characterLayer || "CharacterHitCollider" == characterLayer)
                    {
                        visualEffectParticle = null;
                        //visualEffectParticle = SurfaceManager.singleton.GetVisualEffectFromName("flesh");
                        hitAudio = SurfaceManager.singleton.GetHitAudioFromName("flesh");
                    }
                    else
                    {
                        hitAudio = SurfaceManager.singleton.GetHitAudio(
                            ray,
                            particleEventComponent.hit.transform != null ? particleEventComponent.hit.collider : null,
                            particleEventComponent.hit.transform != null ? particleEventComponent.hit.point : Vector3.zero
                        );
                    }
                }

                if (visualEffectParticle == null)
                {
                    foreach (var poolItem in _DictionaryExternalParticlesPool.Keys)
                    {
                        _DictionaryExternalParticlesPool.TryGetValue(poolItem, out objectPool);
                        break;
                    }
                }
                else
                {
                    foreach (var poolItem in _DictionaryExternalParticlesPool.Keys)
                    {
                        if (poolItem.visualEffectAsset == visualEffectParticle.visualEffectAsset)
                        {
                            _DictionaryExternalParticlesPool.TryGetValue(poolItem, out objectPool);
                        }
                    }
                }

                if (objectPool == null)
                    continue;

                objectPool.Get(out VisualEffect visualEffect);
                _AudioSourcePool.Get(out AudioSource audioSource);

                

                if (hitAudio != null)
                {
                    audioSource.transform.position = particleEventComponent.hit.point;
                    audioSource.clip = hitAudio;
                    audioSource.Play();
                }

                foreach (var particle in _ParticleFilter)
                {
                    ref var particleComponent = ref _ParticlePool.Get(particle);

                    if (particleComponent.gameObject == visualEffect.gameObject && particleComponent.lifetime <= 0)
                    {
                        particleComponent.audioSource = audioSource;
                        var position = particleEventComponent.hit.transform != null ? particleEventComponent.hit.point : particleEventComponent.position;
                        var normal = particleEventComponent.hit.transform != null ? particleEventComponent.hit.normal : particleEventComponent.normal;

                        Color pixelColor = Color.gray;

                        // Check what type of object was hit
                        string hitLayer = particleEventComponent.hit.transform != null ? LayerMask.LayerToName(particleEventComponent.hit.transform.gameObject.layer) : "fuck you";

                        if (hitLayer == LayerMask.LayerToName(0)) // default
                        {
                            if (particleEventComponent.hit.collider is TerrainCollider)
                            {
                                pixelColor = TerrainTextureReader.GetColorAtWorldPosition(position, particleEventComponent.hit.transform);
                            }
                            else if (particleEventComponent.hit.collider.GetComponent<SkinnedMeshRenderer>() != null)
                            {
                                pixelColor = SkinnedMeshTextureReader.GetColorAtWorldPosition(position, particleEventComponent.hit.transform);
                            }
                            else
                            {
                                pixelColor = MeshTextureReader.GetColorAtWorldPosition(position, particleEventComponent.hit);
                            }

                            if (visualEffect.HasVector4("Color"))
                                visualEffect.SetVector4("Color", new Vector4(pixelColor.r, pixelColor.g, pixelColor.b, 1));
                        }


                        particleComponent.gameObject.transform.position = position;
                        particleComponent.gameObject.transform.rotation = Quaternion.LookRotation(normal);
                        particleComponent.lifetime = visualEffect.HasFloat("lifetime") ? visualEffect.GetFloat("lifetime") : 1f;

                        visualEffect.Play();
                    }
                }
            }




            return;
            foreach (var entity in _ParticleEventFilter)
            {
                ref var particleEventComponent = ref _ParticleEventPool.Get(entity);
                Debug.Log("particleEventComponent");
                IObjectPool<VisualEffect> objectPool = null;
                foreach ( var poolItem in _DictionaryExternalParticlesPool.Keys)
                {
                    if (particleEventComponent.visualEffectAsset == null)
                    {
                        Debug.Log(particleEventComponent.visualEffectAsset == null);
                        objectPool = _ExternalParticlesObjectPool;
                        break;
                    }

                    Debug.Log(particleEventComponent.visualEffectAsset.name);
                    if (poolItem.visualEffectAsset == particleEventComponent.visualEffectAsset)
                    {
                        _DictionaryExternalParticlesPool.TryGetValue(poolItem, out objectPool);
                    }
                }

                if (objectPool == null)
                    continue;

                objectPool.Get(out VisualEffect visualEffect);
                Debug.Log(visualEffect.visualEffectAsset.name);

                foreach (var particle in _ParticleFilter)
                {
                    ref var particleComponent = ref _ParticlePool.Get(particle);

                    if (particleComponent.gameObject == visualEffect.gameObject && particleComponent.lifetime <= 0)
                    {
                        if (particleEventComponent.visualEffect != null)
                        {
                            //visualEffect = particleEventComponent.visualEffect;
                        }

                        //visualEffect.visualEffectAsset = particleEventComponent.visualEffectAsset ? particleEventComponent.visualEffectAsset : _SharedData.ParticleSettingsSO.Particle.GetComponent<VisualEffect>().visualEffectAsset;

                        var position = particleEventComponent.hit.transform != null ? particleEventComponent.hit.point : particleEventComponent.position;
                        var normal = particleEventComponent.hit.transform != null ? particleEventComponent.hit.normal : particleEventComponent.normal;

                        Color pixelColor = Color.gray;

                        // Check what type of object was hit
                        string hitLayer = particleEventComponent.hit.transform != null ? LayerMask.LayerToName(particleEventComponent.hit.transform.gameObject.layer) : "fuck you";
                        if (hitLayer == LayerMask.LayerToName(0)) // default
                        {
                            if (particleEventComponent.hit.collider is TerrainCollider)
                            {
                                pixelColor = TerrainTextureReader.GetColorAtWorldPosition(position, particleEventComponent.hit.transform);
                            }
                            else if (particleEventComponent.hit.collider.GetComponent<SkinnedMeshRenderer>() != null)
                            {
                                pixelColor = SkinnedMeshTextureReader.GetColorAtWorldPosition(position, particleEventComponent.hit.transform);
                            }
                            else
                            {
                                pixelColor = MeshTextureReader.GetColorAtWorldPosition(position, particleEventComponent.hit);
                            }

                            visualEffect.SetVector4("Color", new Vector4(pixelColor.r, pixelColor.g, pixelColor.b, 1));
                            visualEffect.SetFloat("Size", particleEventComponent.sizeParticle);
                            visualEffect.SetFloat("Speed", particleEventComponent.speedParticle);
                        }


                        particleComponent.gameObject.transform.position = position;
                        particleComponent.gameObject.transform.rotation = Quaternion.LookRotation(normal);
                        particleComponent.lifetime = visualEffect.GetFloat("lifetime");

                        visualEffect.Play();
                    }
                }
            }





            return;
            foreach (var entity in _DamageFilter)
            {
                ref var damageCom = ref _DamagePool.Get(entity);

                if (damageCom.isHit == false) continue;

                _ExternalParticlesObjectPool.Get(out VisualEffect particleGO);

                foreach (var particle in _ParticleFilter)
                {
                    ref var particleComponent = ref _ParticlePool.Get(particle);

                    if (particleComponent.gameObject ==  particleGO)
                    {
                        var visualEffect = particleGO.GetComponent<VisualEffect>();
                        visualEffect.visualEffectAsset = particleComponent.visualEffectAsset ? particleComponent.visualEffectAsset : visualEffect.visualEffectAsset;

                        particleComponent.gameObject.transform.position = damageCom.position;

                        Color pixelColor = Color.gray;

                        // Check what type of object was hit
                        string hitLayer = LayerMask.LayerToName(damageCom.hit.transform.gameObject.layer);
                        if (hitLayer == "Character" || 
                            //characterlayer == "CharacterIgnore" || 
                            hitLayer == "Player" || 
                            hitLayer == "CharacterHitCollider"
                        )
                        {
                            //SurfaceManager.singleton.GetFootstep(damageCom.hit.collider, damageCom.hit.point);
                            pixelColor = Color.red;
                        }
                        else if (hitLayer == LayerMask.LayerToName(0)) // default
                        {
                            if (damageCom.hit.collider is TerrainCollider)
                            {
                                pixelColor = TerrainTextureReader.GetColorAtWorldPosition(damageCom.hit.point, damageCom.hit.transform);
                            }
                            else if (damageCom.hit.collider.GetComponent<SkinnedMeshRenderer>() != null)
                            {
                                pixelColor = SkinnedMeshTextureReader.GetColorAtWorldPosition(damageCom.hit.point, damageCom.hit.transform);
                            }
                            else
                            {
                                pixelColor = MeshTextureReader.GetColorAtWorldPosition(damageCom.hit.point, damageCom.hit);
                            }

                            particleComponent.lifetime = visualEffect.GetFloat("lifetime");
                            particleComponent.gameObject.transform.rotation = Quaternion.LookRotation(damageCom.hit.normal);
                            visualEffect.SetVector4("Color", new Vector4(pixelColor.r, pixelColor.g, pixelColor.b, 1));
                            visualEffect.SetFloat("Size", damageCom.sizeParticle);
                            visualEffect.SetFloat("Speed", damageCom.speedParticle);
                        }


                        visualEffect.GetComponent<VisualEffect>().Play();
                    }
                }
            }
        }

        private AudioSource CreateAudioSource(AudioSource audioSource)
        {
            GameObject g = GameObject.Instantiate(audioSource.gameObject);

            return g.GetComponent<AudioSource>();
        }

        private void OnGetAudio(AudioSource audioSource)
        {
            audioSource.Stop();
            audioSource.gameObject.SetActive(true);
        }

        private void OnReleaseAudio(AudioSource audioSource)
        {
            audioSource.Stop();
            audioSource.gameObject.SetActive(false);
        }

        private void OnDestroyAudio(AudioSource audioSource)
        {

        }

        private VisualEffect CreateParticle(VisualEffect visualEffectPrefab)
        {
            GameObject g = GameObject.Instantiate(visualEffectPrefab.gameObject, Vector3.zero + Random.insideUnitSphere * 3, Quaternion.identity * Random.rotation) ;
            VisualEffect visualEffect = g.GetComponent<VisualEffect>();
            //a.Pool = _ObjectPool;
            //a.Pool.Release(a);
            //a.GetComponent<Renderer>().material.color = Random.ColorHSV();
            //a.transform.SetParent(this.transform);

            var entity = _EcsWorld.NewEntity();

            var particlePool = _EcsWorld.GetPool<ParticleComponent>();
            particlePool.Add(entity);
            ref var particleComponent = ref particlePool.Get(entity);
            particleComponent.gameObject = g;
            particleComponent.visualEffect = visualEffect;
            particleComponent.pool = _ExternalParticlesObjectPool;
            particleComponent.lifetime = 0;

            return visualEffect;
        }

        private void OnGetParticle(VisualEffect particle)
        {
            particle.gameObject.SetActive(true);
        }

        private void OnReleaseParticle(VisualEffect particle)
        {
            //visualEffect.visualEffectAsset = _SharedData.ParticleSettingsSO.Particle.GetComponent<VisualEffect>().visualEffectAsset;
            particle.gameObject.SetActive(false);
        }

        private void OnDestroyPool(VisualEffect particle)
        {
            GameObject.Destroy(particle.gameObject);
        }
    }


    public static class TexturePixelReader
    {
        public static Color GetPixelColor(Texture2D texture, Vector2 uv)
        {
            if (texture == null) return Color.gray;

            // Convert UV coordinates to pixel coordinates
            int x = Mathf.FloorToInt(uv.x * texture.width);
            int y = Mathf.FloorToInt(uv.y * texture.height);

            // Clamp coordinates to texture bounds
            x = Mathf.Clamp(x, 0, texture.width - 1);
            y = Mathf.Clamp(y, 0, texture.height - 1);

            return texture.GetPixel(x, y);
        }

        public static Color GetPixelBilinear(Texture2D texture, Vector2 uv)
        {
            if (texture == null) return Color.gray;

            // Use bilinear filtering for smoother results
            return texture.GetPixelBilinear(uv.x, uv.y);
        }
    }

    public static class TerrainTextureReader
    {
        public static int textureIndex = 0; // Which terrain texture layer to read from


        public static Color GetColorAtWorldPosition(Vector3 worldPos, Transform transform)
        {
            var terrain = transform.GetComponent<Terrain>();

            if (terrain == null || terrain.terrainData == null)
                return Color.gray;

            // Convert world position to terrain UV coordinates
            Vector3 terrainPos = worldPos - terrain.transform.position;
            Vector3 normalizedPos = new Vector3(
                terrainPos.x / terrain.terrainData.size.x,
                terrainPos.z / terrain.terrainData.size.z
            );

            // Get the alpha map coordinates
            int x = Mathf.FloorToInt(normalizedPos.x * terrain.terrainData.alphamapWidth);
            int y = Mathf.FloorToInt(normalizedPos.z * terrain.terrainData.alphamapHeight);

            // Clamp coordinates
            x = Mathf.Clamp(x, 0, terrain.terrainData.alphamapWidth - 1);
            y = Mathf.Clamp(y, 0, terrain.terrainData.alphamapHeight - 1);

            // Get the alpha map data
            float[,,] alphaMap = terrain.terrainData.GetAlphamaps(x, y, 1, 1);


            Vector2 uv = new Vector2(
                terrainPos.x / terrain.terrainData.size.x,
                terrainPos.z / terrain.terrainData.size.z
            );

            TerrainData terrainData = terrain.terrainData;
            float[] textureMix = GetTerrainTextureMix(worldPos, terrainData, terrain.GetPosition());
            int textureIndex = GetTextureIndex(worldPos, textureMix);
            return TexturePixelReader.GetPixelColor(terrainData.splatPrototypes[textureIndex].texture, uv);

            // For simplicity, return the strength of the specified texture
            // In practice, you might want to sample the actual texture
            return new Color(alphaMap[0, 0, textureIndex], alphaMap[0, 0, textureIndex], alphaMap[0, 0, textureIndex]);
        }

        // Alternative: Sample from terrain detail texture
        public static Color GetDetailColorAtWorldPosition(Vector3 worldPos, Texture2D detailTexture, Terrain terrain)
        {
            if (terrain == null || detailTexture == null)
                return Color.gray;

            if (detailTexture == null) return Color.gray;
            if (detailTexture.isReadable == false) return Color.gray;

            Vector3 terrainPos = worldPos - terrain.transform.position;
            Vector2 uv = new Vector2(
                terrainPos.x / terrain.terrainData.size.x,
                terrainPos.z / terrain.terrainData.size.z
            );

            return TexturePixelReader.GetPixelColor(detailTexture, uv);
        }

        static float[] GetTerrainTextureMix(Vector3 worldPos, TerrainData terrainData, Vector3 terrainPos)
        {
            try
            {
                // returns an array containing the relative mix of textures
                // on the main terrain at this world position.

                // The number of values in the array will equal the number
                // of textures added to the terrain.

                // calculate which splat map cell the worldPos falls within (ignoring y)
                int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
                int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

                // get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
                float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

                // extract the 3D array data to a 1D array:
                float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];

                for (int n = 0; n < cellMix.Length; n++)
                {
                    cellMix[n] = splatmapData[0, 0, n];
                }

                return cellMix;
            }
            catch
            {
                return new float[0];
            }
        }

        static int GetTextureIndex(Vector3 worldPos, float[] textureMix)
        {
            // returns the zero-based index of the most dominant texture
            // on the terrain at this world position.
            float maxMix = 0;
            int maxIndex = 0;

            // loop through each mix value and find the maximum
            for (int n = 0; n < textureMix.Length; n++)
            {
                if (textureMix[n] > maxMix)
                {
                    maxIndex = n;
                    maxMix = textureMix[n];
                }
            }

            return maxIndex;
        }
    }

    public static class MeshTextureReader
    {
        public static Color GetColorAtWorldPosition(Vector3 worldPos, RaycastHit hit)
        {
            return GetMeshMaterialAtPoint(worldPos, hit);
/*

            Mesh mesh = null;
            if (transform.TryGetComponent(out MeshFilter meshFilter))
            {
                mesh = meshFilter.mesh;
            }

            var meshRenderer = transform.GetComponent<MeshRenderer>();
            Texture2D texture = null;

            if (texture == null && meshRenderer != null)
            {
                texture = meshRenderer.material.mainTexture as Texture2D;
            }

            if (mesh == null )
            {
                return Color.gray;
            }

            if (meshRenderer == null)
            {
                return Color.gray;
            }

            if (texture == null)
            {
                return meshRenderer.sharedMaterial.color;
            }

            if (texture.isReadable == false)
            {
                return Color.gray;
            }
                

            // Convert world position to local position
            Vector3 localPos = transform.InverseTransformPoint(worldPos);

            // Find the closest triangle and its UV coordinates
            return GetColorFromClosestTriangle(localPos, mesh, meshRenderer, texture);*/
        }

        private static Color GetColorFromClosestTriangle(Vector3 localPos, Mesh mesh, MeshRenderer meshRenderer, Texture2D texture, bool useBilinearFiltering = true)
        {
            Vector3[] vertices = mesh.vertices;
            Vector2[] uvs = mesh.uv;
            int[] triangles = mesh.triangles;

            float closestDistance = float.MaxValue;
            Vector2 closestUV = Vector2.zero;

            // Iterate through all triangles to find the closest one
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 v0 = vertices[triangles[i]];
                Vector3 v1 = vertices[triangles[i + 1]];
                Vector3 v2 = vertices[triangles[i + 2]];

                Vector2 uv0 = uvs[triangles[i]];
                Vector2 uv1 = uvs[triangles[i + 1]];
                Vector2 uv2 = uvs[triangles[i + 2]];

                // Find the closest point on this triangle
                Vector3 closestPoint = ClosestPointOnTriangle(localPos, v0, v1, v2);
                float distance = Vector3.Distance(localPos, closestPoint);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    // Calculate barycentric coordinates to interpolate UV
                    Vector3 barycentric = Barycentric(closestPoint, v0, v1, v2);
                    closestUV = uv0 * barycentric.x + uv1 * barycentric.y + uv2 * barycentric.z;
                }
            }

            // Sample the texture
            if (useBilinearFiltering)
                return TexturePixelReader.GetPixelBilinear(texture, closestUV);
            else
                return TexturePixelReader.GetPixelColor(texture, closestUV);
        }

        private static Vector3 ClosestPointOnTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            // Implementation of finding closest point on triangle
            // This is a simplified version - you might want a more robust implementation
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 ap = p - a;

            float d1 = Vector3.Dot(ab, ap);
            float d2 = Vector3.Dot(ac, ap);

            if (d1 <= 0f && d2 <= 0f)
                return a;

            Vector3 bp = p - b;
            float d3 = Vector3.Dot(ab, bp);
            float d4 = Vector3.Dot(ac, bp);

            if (d3 >= 0f && d4 <= d3)
                return b;

            Vector3 cp = p - c;
            float d5 = Vector3.Dot(ab, cp);
            float d6 = Vector3.Dot(ac, cp);

            if (d6 >= 0f && d5 <= d6)
                return c;

            // Edge cases and interior point calculation would go here
            // For simplicity, returning the closest vertex
            float distToA = Vector3.Distance(p, a);
            float distToB = Vector3.Distance(p, b);
            float distToC = Vector3.Distance(p, c);

            if (distToA <= distToB && distToA <= distToC) return a;
            if (distToB <= distToA && distToB <= distToC) return b;
            return c;
        }

        private static Vector3 Barycentric(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 v0 = b - a;
            Vector3 v1 = c - a;
            Vector3 v2 = p - a;

            float d00 = Vector3.Dot(v0, v0);
            float d01 = Vector3.Dot(v0, v1);
            float d11 = Vector3.Dot(v1, v1);
            float d20 = Vector3.Dot(v2, v0);
            float d21 = Vector3.Dot(v2, v1);
            float denom = d00 * d11 - d01 * d01;

            float v = (d11 * d20 - d01 * d21) / denom;
            float w = (d00 * d21 - d01 * d20) / denom;
            float u = 1.0f - v - w;

            return new Vector3(u, v, w);
        }


        private static Color GetMeshMaterialAtPoint(Vector3 worldPosition, RaycastHit raycastHit)
        {
            Renderer r = raycastHit.collider.GetComponent<Renderer>();
            MeshCollider mc = raycastHit.collider as MeshCollider;
            Texture2D texture = null;

            if (r == null)
            {
                return Color.gray;
            }

            if (r.material.HasProperty("_MainTex"))
            {
                texture = r.material.mainTexture as Texture2D;
            }

            if (texture == null)
            {
                if (r.material.HasColor("_Color"))
                {
                    return r.material.color;
                }
                else
                {
                    return Color.gray;
                }
            }
                

            if (texture.isReadable == false)
            {
                return r.material.color;
            }
                

            // Get UV coordinates from the hit point
            Vector2 pixelUV = raycastHit.textureCoord;

            int x = Mathf.FloorToInt(pixelUV.x * texture.width);
            int y = Mathf.FloorToInt(pixelUV.y * texture.height);

            if (r.materials.Length < 1)
            {
                if (r == null || r.sharedMaterial == null) // r.sharedMaterial.mainTexture == null
                {
                    return Color.gray;
                }
            }
            else if (!mc || mc.convex)
            {
                return r.material.color;
            }

            int materialIndex = -1;
            Mesh m = mc.sharedMesh;
            int triangleIdx = raycastHit.triangleIndex;
            int lookupIdx1 = m.triangles[triangleIdx * 3];
            int lookupIdx2 = m.triangles[triangleIdx * 3 + 1];
            int lookupIdx3 = m.triangles[triangleIdx * 3 + 2];
            int subMeshesNr = m.subMeshCount;

            for (int i = 0; i < subMeshesNr; i++)
            {
                int[] tr = m.GetTriangles(i);

                for (int j = 0; j < tr.Length; j += 3)
                {
                    if (tr[j] == lookupIdx1 && tr[j + 1] == lookupIdx2 && tr[j + 2] == lookupIdx3)
                    {
                        materialIndex = i;

                        break;
                    }
                }

                if (materialIndex != -1) break;
            }

            string textureName = r.materials[materialIndex].mainTexture.name;
            texture = r.materials[materialIndex].mainTexture as Texture2D;

            if (texture == null)
                return r.material.color;

            if (texture.isReadable == false)
                return r.material.color;

            return texture.GetPixel(x, y);
        }
    }

    public static class SkinnedMeshTextureReader
    {
        public static Color GetColorAtWorldPosition(Vector3 worldPos, Transform transform)
        {
            var skinnedMeshRenderer = transform.GetComponent<SkinnedMeshRenderer>();
            var bakedMesh = new Mesh();
            Texture2D texture = null;

            if (texture == null && skinnedMeshRenderer != null)
            {
                texture = skinnedMeshRenderer.material.mainTexture as Texture2D;
            }

            if (skinnedMeshRenderer == null)
                return Color.gray;

            if (texture == null)
            {
                return skinnedMeshRenderer.sharedMaterial.color;
            }

            if (texture.isReadable == false)
            {
                return Color.gray;
            }

            // Bake the current pose into a mesh
            skinnedMeshRenderer.BakeMesh(bakedMesh);

            // Convert world position to local position in the skinned mesh's space
            Vector3 localPos = transform.InverseTransformPoint(worldPos);

            // Find closest triangle (similar to regular mesh approach)
            return GetColorFromBakedMesh(localPos, bakedMesh, texture);
        }

        private static Color GetColorFromBakedMesh(Vector3 localPos, Mesh bakedMesh, Texture2D texture, bool useBilinearFiltering = true)
        {
            Vector3[] vertices = bakedMesh.vertices;
            Vector2[] uvs = bakedMesh.uv;
            int[] triangles = bakedMesh.triangles;

            float closestDistance = float.MaxValue;
            Vector2 closestUV = Vector2.zero;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 v0 = vertices[triangles[i]];
                Vector3 v1 = vertices[triangles[i + 1]];
                Vector3 v2 = vertices[triangles[i + 2]];

                Vector2 uv0 = uvs[triangles[i]];
                Vector2 uv1 = uvs[triangles[i + 1]];
                Vector2 uv2 = uvs[triangles[i + 2]];

                Vector3 closestPoint = ClosestPointOnTriangle(localPos, v0, v1, v2);
                float distance = Vector3.Distance(localPos, closestPoint);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    Vector3 barycentric = Barycentric(closestPoint, v0, v1, v2);
                    closestUV = uv0 * barycentric.x + uv1 * barycentric.y + uv2 * barycentric.z;
                }
            }

            if (useBilinearFiltering)
                return TexturePixelReader.GetPixelBilinear(texture, closestUV);
            else
                return TexturePixelReader.GetPixelColor(texture, closestUV);
        }

        private static Vector3 Barycentric(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 v0 = b - a;
            Vector3 v1 = c - a;
            Vector3 v2 = p - a;

            float d00 = Vector3.Dot(v0, v0);
            float d01 = Vector3.Dot(v0, v1);
            float d11 = Vector3.Dot(v1, v1);
            float d20 = Vector3.Dot(v2, v0);
            float d21 = Vector3.Dot(v2, v1);
            float denom = d00 * d11 - d01 * d01;

            float v = (d11 * d20 - d01 * d21) / denom;
            float w = (d00 * d21 - d01 * d20) / denom;
            float u = 1.0f - v - w;

            return new Vector3(u, v, w);
        }

        private static Vector3 ClosestPointOnTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            // Implementation of finding closest point on triangle
            // This is a simplified version - you might want a more robust implementation
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 ap = p - a;

            float d1 = Vector3.Dot(ab, ap);
            float d2 = Vector3.Dot(ac, ap);

            if (d1 <= 0f && d2 <= 0f)
                return a;

            Vector3 bp = p - b;
            float d3 = Vector3.Dot(ab, bp);
            float d4 = Vector3.Dot(ac, bp);

            if (d3 >= 0f && d4 <= d3)
                return b;

            Vector3 cp = p - c;
            float d5 = Vector3.Dot(ab, cp);
            float d6 = Vector3.Dot(ac, cp);

            if (d6 >= 0f && d5 <= d6)
                return c;

            // Edge cases and interior point calculation would go here
            // For simplicity, returning the closest vertex
            float distToA = Vector3.Distance(p, a);
            float distToB = Vector3.Distance(p, b);
            float distToC = Vector3.Distance(p, c);

            if (distToA <= distToB && distToA <= distToC) return a;
            if (distToB <= distToA && distToB <= distToC) return b;
            return c;
        }
    }
}