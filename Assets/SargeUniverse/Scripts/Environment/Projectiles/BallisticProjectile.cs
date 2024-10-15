using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    public class BallisticProjectile : Projectile
    {
        [SerializeField] private float _arcHeight = 1f;
        private BezierCurve _curve;
        
        public override void Init(Vector3 startPosition, Transform target, float speed, int level = 1)
        {
            base.Init(startPosition, target, speed, level);
            _curve = new BezierCurve(startPosition, target.position, _arcHeight, _time);
        }

        public override void Init(Vector3 startPosition, Vector3 targetPosition, float speed, int level = 1)
        {
            base.Init(startPosition, targetPosition, speed, level);
            _curve = new BezierCurve(startPosition, targetPosition, _arcHeight, _time);
        }

        protected override void UpdateProjectilePosition()
        {
            _curve.timer += 1f * Time.deltaTime / _curve.duration;
            var m1 = Vector3.Lerp(_curve.pointA, _curve.pointB, _curve.timer);
            var m2 = Vector3.Lerp(_curve.pointB, _curve.pointC, _curve.timer);
            transform.position = Vector3.Lerp(m1, m2, _curve.timer);
        }
    }
}