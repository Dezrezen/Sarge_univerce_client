using System;
using System.Collections.Generic;
using SargeUniverse.Scripts.Enums;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Buildings.Animators
{
    public class HiddenDroneAnimator : BuildingAnimator
    {

        [Header("Hidden drone animation Settings")]
        [SerializeField] protected List<SpriteCollection> _releasingSprites = new();
        [SerializeField] protected float _delayBetweenReleasingSlides = 0.1f;
        
        private bool _hidden = true;
        
        public override void PlayIdleAnimation(int level, MovementDirection direction)
        {
            if (_hidden)
            {
                _spriteRenderer.sprite = _releasingSprites[level - 1].GetFirstSprite();
                SetDirection(direction);
            }
            else
            {
                if (direction != _direction)
                {
                    base.PlayIdleAnimation(level, direction);
                }
                else
                {
                    PlayHideAnimation(level);
                }
            }
        }

        public override void PlayAttackAnimation(int level, MovementDirection direction, Action callback = null)
        {
            if (_hidden)
            {
                PlayUnHideAnimation(level);
                base.PlayIdleAnimation(level, direction);
            }
            base.PlayAttackAnimation(level, direction, callback);
        }
        
        private void PlayHideAnimation(int level)
        {
            var sprites = new List<Sprite>(_releasingSprites[level - 1].GetSprites());
            sprites.Reverse();
            _coroutines.Enqueue(PlayAnimation(sprites, _delayBetweenReleasingSlides));
            PlayAnimation();
            _hidden = true;
        }

        private void PlayUnHideAnimation(int level)
        {
            var sprites = new List<Sprite>(_releasingSprites[level - 1].GetSprites());
            _coroutines.Enqueue(PlayAnimation(sprites, _delayBetweenReleasingSlides));
            PlayAnimation();
            _hidden = false;
        }
    }
}