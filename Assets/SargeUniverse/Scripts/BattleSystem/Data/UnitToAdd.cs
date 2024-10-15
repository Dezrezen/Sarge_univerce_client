using SargeUniverse.Scripts.Data;

namespace SargeUniverse.Scripts.BattleSystem.Data
{
    public class UnitToAdd
    {
        public BattleSystem.Data.BattleUnitData unitData = null;
        public int x;
        public int y;
        public Callbacks.UnitSpawned callback = null;
        public Callbacks.AttackCallback attackCallback = null;
        public Callbacks.IndexCallback dieCallback = null;
        public Callbacks.FloatCallback damageCallback = null;
        public Callbacks.FloatCallback healCallback = null;
        public Callbacks.IndexCallback targetCallback = null;
    }
}