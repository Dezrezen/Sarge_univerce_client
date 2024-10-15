using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    public interface IProjectile
    {
        void Init(Vector3 startPosition, Transform target, float speed, int level);
        void Init(Vector3 startPosition, Vector3 targetPosition, float speed, int level);
    }
}