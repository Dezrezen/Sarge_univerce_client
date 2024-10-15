using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SargeUniverse.Scripts.Utils
{
    public class AnimationSequence : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _previewImage = null;
        [SerializeField] private SpriteRenderer _animationImage = null;
        [SerializeField] private List<Sprite> _sprites = new();
        
        [Header("Sprite Atlas")]
        [SerializeField] private SpriteFromAtlas _spriteFromAtlas = null;
        [SerializeField] private string _textureName = "";
        private int _totalSprites = 0;
        private int _index = 0;

        public float fadeTimer = 0.3f;
        public float animationDelay = 0.03f;

        private void Start()
        {
            _totalSprites = _spriteFromAtlas.GetSpritesCount();
        }

        public void Play()
        {
            _previewImage.enabled = true;
            _animationImage.enabled = false;
            
            StartCoroutine(PlayAnimation());
            StartCoroutine(HidePreview());
        }

        private IEnumerator HidePreview()
        {
            yield return new WaitForSeconds(fadeTimer);
            _previewImage.enabled = false;
        }
        
        private IEnumerator PlayAnimation()
        {
            _animationImage.enabled = true;
            yield return new WaitForEndOfFrame();

            foreach (var t in _sprites)
            {
                _animationImage.sprite = t;
                yield return new WaitForSeconds(animationDelay);
            }
        }
    }
}