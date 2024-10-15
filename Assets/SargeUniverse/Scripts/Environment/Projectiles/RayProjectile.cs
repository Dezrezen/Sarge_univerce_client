using System.Collections.Generic;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    public class RayProjectile : Projectile
    {
        [SerializeField] private LineRenderer _lineRenderer = null;
        [SerializeField] private List<float> _scaleOverLevel = new();
        [SerializeField] private float _lineSize = 0.5f;
        
        public override void Init(Vector3 startPosition, Transform target, float speed, int level = 1)
        {
            base.Init(startPosition, target, speed, level);
            InitLineRendererSettings(startPosition, target.position, level);
        }

        public override void Init(Vector3 startPosition, Vector3 targetPosition, float speed, int level = 1)
        {
            base.Init(startPosition, targetPosition, speed, level);
            InitLineRendererSettings(startPosition, targetPosition, level);
        }

        private void InitLineRendererSettings(Vector3 startPosition, Vector3 targetPosition, int level = 1)
        {
            _lineRenderer.startWidth = _lineSize;
            _lineRenderer.endWidth = _lineSize;
            
            _lineRenderer.positionCount = 2;
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.SetPosition(0, Vector3.zero);
            _lineRenderer.SetPosition(1, targetPosition - startPosition);
            _lineRenderer.startWidth = _lineSize * _scaleOverLevel[level - 1];
            _lineRenderer.endWidth = _lineSize * _scaleOverLevel[level - 1];
        }
        
        protected override void UpdateProjectilePosition()
        {
            if (_target)
            {
                _lineRenderer.SetPosition(1, _target.position - transform.position);
            }
        }
    }
}