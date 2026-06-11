#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace InatesiCharacter.Testing.SurfaceManagers.Editors
{
    [CustomEditor(typeof(SurfaceDataSO))]
    public class SurfaceDataEditor : Editor
    {
        [SerializeField] private VisualTreeAsset _VisualTreeAsset;


        public override VisualElement CreateInspectorGUI()
        {
            if (_VisualTreeAsset == null) return null;

            var root = _VisualTreeAsset.CloneTree();
            var surfaceDataSO = target as SurfaceDataSO;

            root.Q<Button>("insert-textures").clicked += () => 
            {
                var renderAmount = 0;
                List< Texture> textures = new();
                var renderer = GameObject.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
                foreach (var render in renderer)
                {
                    if (render == null)
                    {
                        continue;
                    }

                    if (render.gameObject.layer != LayerMask.NameToLayer("Default"))
                    {
                        continue;
                    }

                    if (render.materials == null && render.materials.Length == 0)
                    {
                        continue;
                    }

                    foreach (var material in render.materials)
                    {
                        if (material.HasTexture("_MainTex") == false)
                        {
                            continue;
                        }

                        if (material.GetTexture("_MainTex") == null)
                        {
                            continue;
                        }

                        renderAmount++;
                        textures.Add( material.mainTexture);
                    }
                }


                if (textures != null)
                {
                    foreach (var addTexture in textures)
                    {
                        if (addTexture == null)
                            continue;

                        bool canAdd = true;
                        foreach (var registeredTexture in surfaceDataSO.RegisteredTextures)
                        {
                            if (registeredTexture.texture == addTexture)
                            {
                                canAdd = false;
                                break;
                            }
                        }

                        if (canAdd)
                        {
                            if (surfaceDataSO.RegisteredTextures == null)
                            {
                                surfaceDataSO.RegisteredTextures = new();
                            }

                            surfaceDataSO.RegisteredTextures.Add(
                                new RegisteredMaterial
                                {
                                    texture = addTexture,
                                    textureName = addTexture.name
                                }
                            );
                        }
                    }
                }
                
            };

            serializedObject.ApplyModifiedProperties();
            //RegisteredMaterialPropertyDrawer;

            return root;
        }
    }
}
#endif