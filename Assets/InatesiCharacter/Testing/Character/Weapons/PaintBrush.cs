using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace InatesiCharacter.Testing.Character.Weapons
{
    public class PaintBrush : WeaponBase
    {
        [SerializeField] private Color _color = Color.white;
        Texture2D _texture;

        public override void Init()
        {
            base.Init();

            
        }

        private void Awake()
        {
            _texture = new Texture2D(10, 10, TextureFormat.RGBA32, false);
        }

        public override void UpdateTick()
        {
            base.UpdateTick();
            if (Inatesi.Inputs.Input.Pressed("Attack"))
            {
                RaycastHit hit;
                var isHit = Raycast(
                    out hit
                );

                if (isHit)
                {
                    var renderer = hit.transform.GetComponent<Renderer>();

                    //_texture = renderer.material.mainTexture as Texture2D;

                    var x = Random.Range(0, 10);
                    var y = Random.Range(0, 10);
                    //_texture.Get(hit.textureCoord.x, hit.textureCoord.y);
                    _texture.SetPixel(x, y, Random.ColorHSV(0, 1, 0, 1, 0, 1, 0, 1));

                    renderer.material.mainTexture = _texture;

                    _texture.Apply();
                }

            }
                
        }
    }
}