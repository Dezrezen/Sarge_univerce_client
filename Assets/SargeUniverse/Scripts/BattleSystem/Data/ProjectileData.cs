using SargeUniverse.Scripts.Enums;

namespace SargeUniverse.Scripts.BattleSystem.Data
{
    public class ProjectileData
    {
        public int id = 0;
        public int target = -1;
        public float damage = 0;
        public float splash = 0;
        public float timer = 0;
        public TargetType type = TargetType.Unit;
        public bool heal = false;
        public bool follow = true;
        public BattleVector2 position = new();
    }
}