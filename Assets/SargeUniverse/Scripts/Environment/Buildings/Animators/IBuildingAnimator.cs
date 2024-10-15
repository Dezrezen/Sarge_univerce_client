using System;
using SargeUniverse.Scripts.Enums;

namespace SargeUniverse.Scripts.Environment.Buildings.Animators
{
    public interface IBuildingAnimator
    {
        void SetDirection(int level, MovementDirection direction);
        void PlayIdleAnimation(int level);
        void PlayIdleAnimation(int level, MovementDirection direction);
        void PlayAttackAnimation(int level, Action callback = null);
        void PlayAttackAnimation(int level, MovementDirection direction, Action callback = null);
        void PlayDestroyAnimation();
        void StopAnimation();
    }
}