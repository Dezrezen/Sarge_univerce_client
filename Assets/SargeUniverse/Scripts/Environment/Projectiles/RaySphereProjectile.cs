using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    public class RaySphereProjectile : RayProjectile
    {
        [Header("Sphere settings")]
        [SerializeField] private SphereGraphics _sphereGraphics = null;
        [SerializeField] private float _startScale = 0.1f;
        [SerializeField] private float _finishScale = 1.0f;
        [SerializeField] private float _scaleStep = 0.05f;
        private float _scale;
        
        public override void Init(Vector3 startPosition, Transform target, float speed, int level = 1)
        {
            base.Init(startPosition, target, speed, level);
            InitSphere(target.position);
        }

        public override void Init(Vector3 startPosition, Vector3 targetPosition, float speed, int level = 1)
        {
            base.Init(startPosition, targetPosition, speed, level);
            InitSphere(targetPosition);
        }

        private void InitSphere(Vector3 targetPosition)
        {
            _sphereGraphics.transform.position = targetPosition;
            
            _scale = _startScale;
            _sphereGraphics.UpdateScale(_scale);
        }
        
        protected override void UpdateProjectilePosition()
        {
            base.UpdateProjectilePosition();
            _sphereGraphics.UpdateSphereGraphics();
            
            if (_scale > _finishScale)
            {
                return;
            }
            
            _sphereGraphics.UpdateScale(_scale);
            _scale += _scaleStep;
        }
    }
}