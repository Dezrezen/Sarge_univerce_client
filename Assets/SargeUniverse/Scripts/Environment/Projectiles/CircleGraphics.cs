using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Utils;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    [RequireComponent(typeof(LineRenderer))]
    public class CircleGraphics : ProjectileGraphics 
    {
        [SerializeField] [Range(0,100)] private int _segments = 100;
        [SerializeField] [Range(0,5)] private float _xRadius = 5f;
        [SerializeField] [Range(0,5)] private float _yRadius = 5f;
        [SerializeField] private float _lineSize = 1f;
        
        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public override void SetTarget(Transform target)
        {
            base.SetTarget(target);
            var direction = DirectionUtils.GetLookDirection(transform.position, _target.position);
            UpdateRadius(direction);
            DrawCircle();
        }
        
        public override void SetTarget(Vector3 targetPosition)
        {
            base.SetTarget(targetPosition);
            var direction = DirectionUtils.GetLookDirection(transform.position, targetPosition);
            UpdateRadius(direction);
            DrawCircle();
        }

        private void UpdateRadius(MovementDirection direction)
        {
            switch (direction)
            {
                case MovementDirection.North:
                    _xRadius = 0.6f;
                    _yRadius = 0.3f;
                    transform.rotation = Quaternion.identity;
                    break;
                case MovementDirection.NorthWest:
                    _xRadius = 0.1f;
                    _yRadius = 0.6f;
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45f));
                    break;
                case MovementDirection.West:
                    _xRadius = 0.25f;
                    _yRadius = 0.6f;
                    transform.rotation = Quaternion.identity;
                    break;
                case MovementDirection.SouthWest:
                    _xRadius = 0.5f;
                    _yRadius = 0.6f;
                    transform.rotation = Quaternion.identity;
                    break;
                case MovementDirection.South:
                    _xRadius = 0.6f;
                    _yRadius = 0.6f;
                    transform.rotation = Quaternion.identity;
                    break;
                case MovementDirection.SouthEast:
                    _xRadius = 0.5f;
                    _yRadius = 0.6f;
                    transform.rotation = Quaternion.identity;
                    break;
                case MovementDirection.East:
                    _xRadius = 0.25f;
                    _yRadius = 0.6f;
                    transform.rotation = Quaternion.identity;
                    break;
                case MovementDirection.NorthEast:
                    _xRadius = 0.1f;
                    _yRadius = 0.6f;
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 45.5f));
                    break;
            }
        }
        
        private void DrawCircle()
        {
            _lineRenderer.startWidth = 0.5f;
            _lineRenderer.endWidth = 0.5f;

            _lineRenderer.positionCount = _segments + 1;
            _lineRenderer.useWorldSpace = false;
            CreatePoints();
            
            _lineRenderer.startWidth = _lineSize;
            _lineRenderer.endWidth = _lineSize;
        }

        private void CreatePoints()
        {
            var angle = 20f;
            for (var i = 0; i < _segments + 1; i++)
            {
                var x = Mathf.Sin (Mathf.Deg2Rad * angle) * _xRadius;
                var y = Mathf.Cos (Mathf.Deg2Rad * angle) * _yRadius;

                _lineRenderer.SetPosition (i, new Vector3(x, y, 0));

                angle += 360f / _segments;
            }
        }
    }
}