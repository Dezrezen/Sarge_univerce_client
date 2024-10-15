using UnityEngine;

namespace SargeUniverse.Scripts.BattleSystem.Data
{
    public struct BattleVector2
    {
        public float x;
        public float y;

        public BattleVector2(float x, float y)
        {
            this.x = x; this.y = y;
        }

        public static BattleVector2 LerpUnclamped(BattleVector2 a, BattleVector2 b, float t)
        {
            return new BattleVector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }

        public static float Distance(BattleVector2 a, BattleVector2 b)
        {
            var diffX = a.x - b.x;
            var diffY = a.y - b.y;
            return Mathf.Sqrt(diffX * diffX + diffY * diffY);
        }

        public static float Distance(Vector2Int a, Vector2Int b)
        {
            return Distance(new BattleVector2(a.x, a.y), new BattleVector2(b.x, b.y));
        }

        public static BattleVector2 LerpStatic(BattleVector2 source, BattleVector2 target, float deltaTime, float speed)
        {
            if ((Mathf.Approximately(source.x, target.x) && Mathf.Approximately(source.y, target.y)) || speed <= 0) 
            {
                return source;
            }

            var distance = Distance(source, target);
            var t = speed * deltaTime;
            if (t > distance)
            {
                t = distance;
            }

            return LerpUnclamped(source, target, distance == 0 ? 1 : t / distance);
        }
    }
}