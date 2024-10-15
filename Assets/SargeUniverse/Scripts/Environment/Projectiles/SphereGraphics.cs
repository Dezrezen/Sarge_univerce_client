using System.Collections.Generic;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SphereGraphics : MonoBehaviour
    {
        [SerializeField] private List<Sprite> _sphereSprites = new();
        
        private SpriteRenderer _spriteRenderer = null;
        private int _spriteIndex = 0;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void UpdateScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        public void UpdateSphereGraphics()
        {
            _spriteRenderer.sprite = _sphereSprites[_spriteIndex];
            if (_spriteIndex + 1 == _sphereSprites.Count)
            {
                _spriteIndex = 0;
            }
            else
            {
                _spriteIndex++;
            }
        }
    }
}