using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    public class LinearProjectile : Projectile
    {
        protected override void UpdateProjectilePosition()
        {
            var targetPosition = _target ? _target.position : _targetPosition;
            
            transform.position = Vector3.MoveTowards(
                transform.position, 
                targetPosition + _offset, 
                _speed * Time.deltaTime);
        }
    }
}