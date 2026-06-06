using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.Shared
{
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
#if UNITY_EDITOR
                Debug.LogError($"[texture {texture.name} is not Readable]");
#endif
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
