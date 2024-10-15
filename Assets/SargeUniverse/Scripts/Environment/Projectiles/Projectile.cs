using SargeUniverse.Scripts.Sound;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    public class Projectile : MonoBehaviour, IProjectile
    {
        [SerializeField] protected ProjectileGraphics _projectileGraphics = null;
        [SerializeField] private ProjectileExplosion _projectileExplosion = null;
        
        protected Transform _target = null;
        protected Vector3 _targetPosition = Vector3.zero;
        protected Vector3 _offset = Vector3.zero;
        protected float _speed = 0;
        protected float _time = 0;

        public virtual void Init(Vector3 startPosition, Transform target, float speed, int level = 1)
        {
            if (target == null)
            {
                return;
            }
            
            _target = target;
            _offset = Vector3.zero;
            var distance = Vector3.Distance(startPosition, target.position);
            _speed = speed;
            _time = distance / speed;
            transform.position = startPosition;

            _projectileGraphics.SetTarget(_target.position);
        }
        
        public virtual void Init(Vector3 startPosition, Vector3 targetPosition, float speed, int level = 1)
        {
            _targetPosition = targetPosition;
            _offset = Vector3.zero;
            var distance = Vector3.Distance(startPosition, targetPosition);
            _speed = speed;
            _time = distance / speed;
            transform.position = new Vector3(startPosition.x, startPosition.y, -1);
            
            _projectileGraphics.SetTarget(_targetPosition);
        }
        
        private void Update()
        {
            if (_time <= 0)
            {
                return;
            }

            _time -= Time.deltaTime;
            if (_time > 0)
            {
                UpdateProjectilePosition();
            }
            else
            {
                SoundSystem.Instance?.PlaySound("projectile_explosion");
                DestroyProjectile();
            }
        }
        
        protected virtual void UpdateProjectilePosition()
        {
            
        }
       
        private void DestroyProjectile()
        {
            _projectileGraphics.Complete();
            if (_projectileExplosion)
            {
                _projectileExplosion.PlayDestroyAnimation(() => Destroy(gameObject));
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}