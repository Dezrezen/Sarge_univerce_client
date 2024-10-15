using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    public class ProjectileGraphics : MonoBehaviour
    {
        protected Transform _target = null;
        protected Vector3 _targetPosition = Vector3.zero;

        public virtual void SetTarget(Transform target)
        {
            _target = target;
        }
        
        public virtual void SetTarget(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
        }

        public virtual void Complete()
        {
            
        }
    }
}