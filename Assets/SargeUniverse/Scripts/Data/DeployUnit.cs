using System.Collections.Generic;

namespace SargeUniverse.Scripts.Data
{
    public class DeployUnit
    {
        public readonly List<Data_Unit> unitsData = null;
        public readonly int level;

        public DeployUnit(Data_Unit unitData)
        {
            unitsData = new List<Data_Unit> { unitData };
            level = unitData.level;
        }
    }
}