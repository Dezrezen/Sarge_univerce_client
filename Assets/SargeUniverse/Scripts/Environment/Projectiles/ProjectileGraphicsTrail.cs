using System.Collections.Generic;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    [RequireComponent(typeof(TrailRenderer))]
    public class ProjectileGraphicsTrail : ProjectileGraphics
    {
        [SerializeField] private List<Sprite> _sprites = new();

        private TrailRenderer _trailRenderer = null;
        private Material _material = null;
        private int _spriteIndex = 0;

        private void Awake()
        {
            _trailRenderer = GetComponent<TrailRenderer>();
            _material = new Material(_trailRenderer.material);
        }

        private void Update()
        {
            _material.mainTexture = _sprites[_spriteIndex].texture;
            _trailRenderer.material = _material;
            if (_spriteIndex + 1 == _sprites.Count)
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