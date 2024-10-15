using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Buildings.Animators
{
    public class TowerSphereAnimator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _sphereSprite = null;
        [SerializeField] private List<SpriteCollection> _sphereSprites = new();
        [SerializeField] private float _delayBetweenSprites = 0.1f;

        private IEnumerator _coroutine = null;
        
        public void PlaySphereAnimation(int level)
        {
            StopAnimation();
            _coroutine = PlayAnimation(_sphereSprites[level - 1].GetSprites());
            StopCoroutine(_coroutine);
        }

        public void StopAnimation()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        private IEnumerator PlayAnimation(List<Sprite> sprites)
        {
            yield return new WaitForEndOfFrame();
            
            while (true)
            {
                foreach (var sprite in sprites)
                {
                    _sphereSprite.sprite = sprite;
                    yield return new WaitForSeconds(_delayBetweenSprites);
                }
            }
        }
    }
}