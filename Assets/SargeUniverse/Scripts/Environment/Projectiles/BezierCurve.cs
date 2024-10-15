using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Projectiles
{
    public class BezierCurve
    {
        public Vector3 pointA;
        public Vector3 pointB;
        public Vector3 pointC;
        public readonly float duration;
        public float timer;

        public BezierCurve(Vector3 startPosition, Vector3 targetPosition, float arcHeight, float duration, float time = 0)
        {
            pointA = startPosition;
            pointC = targetPosition;
            pointB = startPosition + (targetPosition - startPosition) / 2f + Vector3.up * arcHeight;
            this.duration = duration;
            timer = time;
        }
    }
}