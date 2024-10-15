using System.Collections.Generic;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class RaySphere : MonoBehaviour
    {
        [SerializeField] private List<Sprite> _sphereSprites = new();
        [SerializeField] private float _startScale = 0.1f;
        [SerializeField] private float _finishScale = 1.0f;
        [SerializeField] private float _scaleStep = 0.05f;

        private SpriteRenderer _spriteRenderer = null;
        private int _spriteIndex = 0;
        private float _scale;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _scale = _startScale;
            transform.localScale = Vector3.one * _scale;
        }

        private void FixedUpdate()
        {
            _spriteRenderer.sprite = _sphereSprites[_spriteIndex];
            if (_scale < _finishScale)
            {
                transform.localScale = Vector3.one * _scale;
                _scale += _scaleStep;
            }

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