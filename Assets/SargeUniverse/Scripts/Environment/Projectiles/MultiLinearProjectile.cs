using System.Collections.Generic;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    public class MultiLinearProjectile : LinearProjectile
    {
        [Header("Multiple projectiles")]
        [SerializeField] private int _maxCount = 1;
        [SerializeField] private int _skipFrames = 20;

        private List<ProjectileGraphics> _graphicsList = new();
        private Vector3 _startPosition;
        private int _frames = 0; 
        
        public override void Init(Vector3 startPosition, Transform target, float speed, int level = 1)
        {
            base.Init(startPosition, target, speed, level);
            _graphicsList.Add(_projectileGraphics);
            _startPosition = startPosition;
        }

        public override void Init(Vector3 startPosition, Vector3 targetPosition, float speed, int level = 1)
        {
            base.Init(startPosition, targetPosition, speed, level);
            _graphicsList.Add(_projectileGraphics);
            _startPosition = startPosition;
        }
        
        protected override void UpdateProjectilePosition()
        {
            base.UpdateProjectilePosition();
            
            if (_graphicsList.Count < _maxCount)
            {
                if (_frames >= _skipFrames)
                {
                    Spawn();
                    _frames = 0;
                }
                else
                {
                    _frames++;
                }
            }
        }

        private void Spawn()
        {
            var projectile = Instantiate(_projectileGraphics, transform, true);
            projectile.transform.position = new Vector3(
                _startPosition.x, 
                _startPosition.y, 
                _projectileGraphics.transform.position.z
            );
            projectile.transform.position += Vector3.forward * projectile.transform.localPosition.y;
            
            if (_target)
            {
                projectile.SetTarget(_target);
            }
            else
            {
                projectile.SetTarget(_targetPosition);
            }
            _graphicsList.Add(projectile);
        }
    }
}