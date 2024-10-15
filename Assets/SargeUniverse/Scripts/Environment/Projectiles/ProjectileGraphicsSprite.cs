using System.Collections;
using SargeUniverse.Scripts.Utils;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ProjectileGraphicsSprite : ProjectileGraphics
    {
        [SerializeField] private SpriteDirectionCollection _collection;
        [SerializeField] private float _delayBetweenSlides = 0.05f;
        
        private SpriteRenderer _spriteRenderer = null;
        private IEnumerator _courotine = null;
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        public override void SetTarget(Transform target)
        {
            base.SetTarget(target);
            var direction = DirectionUtils.GetLookDirection(transform.position, _target.position);
            _collection.SetDirection(direction);

            _courotine = PlayLoopAnimation();
            StartCoroutine(_courotine);
        }
        
        public override void SetTarget(Vector3 targetPosition)
        {
            base.SetTarget(targetPosition);
            var direction = DirectionUtils.GetLookDirection(transform.position, targetPosition);
            _collection.SetDirection(direction);

            _courotine = PlayLoopAnimation();
            StartCoroutine(_courotine);
        }
        
        public override void Complete()
        {
            if (_courotine != null)
            {
                StopCoroutine(_courotine);
                _courotine = null;
            }
        }
        
        private IEnumerator PlayLoopAnimation()
        {
            yield return new WaitForFixedUpdate();
            while (true)
            {
                _spriteRenderer.sprite = _collection.GetSprite();
                yield return new WaitForSeconds(_delayBetweenSlides);
            }
        }
    }
}