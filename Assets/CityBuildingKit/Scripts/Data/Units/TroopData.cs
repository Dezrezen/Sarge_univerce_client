using System.Collections.Generic;

namespace CityBuildingKit.Scripts.Data.Units
{
    [System.Serializable]
    public class TroopData
    {
        public string name;
        public List<UnitStats> stats = new();
        public TroopUpgradeInfo upgradeInfo;
        public List<int> speedPerBarrack = new();
    }
}