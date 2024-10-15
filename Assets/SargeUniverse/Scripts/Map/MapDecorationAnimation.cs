using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SargeUniverse.Scripts.Map
{
    public class MapDecorationAnimation : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        [SerializeField] private List<Sprite> _sprites = new();

        [SerializeField] private float _spriteSwapDelay = 0.1f;
        [SerializeField] private float _delayAfterEnd = 0.1f;

        private IEnumerator _coroutine = null;
        
        private void Start()
        {
            _coroutine = PlayAnimation();
            StartCoroutine(_coroutine);
        }

        private void OnDestroy()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
        }

        private IEnumerator PlayAnimation()
        {
            yield return new WaitForEndOfFrame();
            while (true)
            {
                foreach (var sprite in _sprites)
                {
                    _spriteRenderer.sprite = sprite;
                    yield return new WaitForSeconds(_spriteSwapDelay);
                }
                yield return new WaitForSeconds(_delayAfterEnd);
            }
        }
    }
}