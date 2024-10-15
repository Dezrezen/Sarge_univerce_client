using System;
using System.Collections;
using System.Collections.Generic;
using SargeUniverse.Scripts.Model.Environment;
using SargeUniverse.Scripts.Utils;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    public class ProjectileSystem : MonoBehaviour
    {
        [SerializeField] private List<Vector2Direction> _spawnPoints = new();
        [SerializeField] private Vector2Direction _offsets;
        [SerializeField] private List<int> _projectilesOverLevel = new();
        [SerializeField] private float _spawnDelay = 0.3f;
        [SerializeField] private Projectile _projectilePrefab = null;

        public void SpawnProjectile(Transform target, float speed, int level = 1)
        {
            var direction = DirectionUtils.GetLookDirection(transform.position, target.position);
            var spawnPosition = transform.position + _spawnPoints[level - 1].GetPoint(direction);

            Projectile projectile;

            if (_projectilesOverLevel == null || _projectilesOverLevel.Count < level || _projectilesOverLevel[level - 1] <= 1)
            {
                projectile = Instantiate(_projectilePrefab, spawnPosition, Quaternion.identity, transform.parent);
                projectile.Init(spawnPosition, target, speed, level);
            }
            else
            {
                for (var i = 0; i < _projectilesOverLevel[level - 1]; i++)
                {
                    var offset = _offsets.GetPoint(direction) * (i % 2 == 0 ? 1 : -1);
                    projectile = Instantiate(_projectilePrefab, spawnPosition + offset, Quaternion.identity, transform.parent);
                    projectile.Init(spawnPosition + offset, target, speed, level);
                }
            }
        }
        
        public void SpawnProjectile(Vector3 targetPosition, float speed, int level = 1)
        {
            var direction = DirectionUtils.GetLookDirection(transform.position + Vector3.up * 2f, targetPosition);
            var spawnPosition = transform.position + _spawnPoints[level - 1].GetPoint(direction);
            
            Projectile projectile;
            
            if (_projectilesOverLevel == null || _projectilesOverLevel.Count < level || _projectilesOverLevel[level - 1] <= 1)
            {
                Spawn(spawnPosition, targetPosition, speed, level);
            }
            else
            {
                for (var i = 0; i < _projectilesOverLevel[level - 1]; i++)
                {
                    var offset = _offsets.GetPoint(direction) * (i % 2 == 0 ? 1 : -1);
                    StartCoroutine(StartProcess(
                        () => Spawn(spawnPosition + offset, targetPosition, speed, level),
                        i * _spawnDelay));
                }
            }
        }

        private void Spawn(Vector3 spawnPosition, Transform target, float speed, int level)
        {
            var projectile = Instantiate(_projectilePrefab, spawnPosition, Quaternion.identity, transform.parent);
            projectile.Init(spawnPosition, target, speed, level);
        }
        
        private void Spawn(Vector3 spawnPosition, Vector3 targetPosition, float speed, int level)
        {
            var projectile = Instantiate(_projectilePrefab, spawnPosition, Quaternion.identity, transform.parent);
            projectile.Init(spawnPosition, targetPosition, speed, level);
        }

        private IEnumerator StartProcess(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }
    }
}