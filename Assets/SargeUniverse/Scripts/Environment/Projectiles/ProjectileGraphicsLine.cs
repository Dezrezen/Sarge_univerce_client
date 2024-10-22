using System;
using System.Collections.Generic;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    [RequireComponent(typeof(LineRenderer))]
    public class ProjectileGraphicsLine : ProjectileGraphics
    {
        [SerializeField] private List<Sprite> _sprites = new();
        [SerializeField] private float _size = 1f;
        
        private LineRenderer _lineRenderer = null;
        private Material _material = null;
        private int _spriteIndex = 0;
        
        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _material = new Material(_lineRenderer.material);
        }

        private void Start()
        {
            _lineRenderer.startWidth = _size;
            _lineRenderer.endWidth = _size;
        }

        private void FixedUpdate()
        {
            _material.mainTexture = _sprites[_spriteIndex].texture;
            _lineRenderer.material = _material;
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