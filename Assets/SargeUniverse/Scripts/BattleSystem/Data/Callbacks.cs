using SargeUniverse.Scripts.Enums;

namespace SargeUniverse.Scripts.BattleSystem.Data
{
    public class Callbacks
    {
        public delegate void UnitSpawned(long databaseId, int x, int y);
        public delegate void AttackCallback(long index, long target);
        public delegate void IndexCallback(long index);
        public delegate void FloatCallback(long index, float value);
        public delegate void DoubleCallback(long index, double value);
        public delegate void BlankCallback();
        public delegate void ProjectileCallback(int databaseId, BattleVector2 current, BattleVector2 target);
    }
    
}