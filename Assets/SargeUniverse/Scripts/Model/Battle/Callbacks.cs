using SargeUniverse.Scripts.Enums;
using UnityEngine;

namespace SargeUniverse.Scripts.Model.Battle
{
    public class Callbacks
    {
        public delegate void UnitSpawned(long databaseId, float x, float y);
        public delegate void AttackCallback(long unitId, long targetId, TargetType targetType);
        public delegate void EmptyCallback(long value);
        public delegate void IntCallback(long targetId, int value);
        public delegate void DamageCallback(long targetId, TargetType targetType, int damage);
        public delegate void PositionCallback(long id, Vector2 position);
    }
}