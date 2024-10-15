using System.Collections.Generic;
using System.Linq;
using SargeUniverse.Scripts.Enums;

namespace SargeUniverse.Scripts.Data
{
    public class InitializationData
    {
        public long accountID = 0;
        public string password = "";
        public string[] versions;
        public List<ServerBuilding> serverBuildings = new();
        public List<ServerUnit> serverUnits = new();
        public List<Research> research = new();
        public List<BuildingCount> buildingsLimits = new();

        public ServerBuilding GetServerBuilding(string buildingId, int level)
        {
            return serverBuildings.Find(building => building.id == buildingId && building.level == level);
        }

        public int GetBuildingUnlockLevel(string buildingId, int level = 1)
        {
            var limits = buildingsLimits.FindAll(limit => limit.buildingId == buildingId && (limit.maxLevel > level || (level == 1 && limit.maxLevel == 1)));
            if (limits.Count >= 1)
            {
                limits.Sort((b1, b2) => b1.hqLevel.CompareTo(b2.hqLevel));
            }
            return limits.Count == 0 ? -1 : limits.First().hqLevel;
        }
        
        public List<BuildingCount> GetBuildingLimits(int level)
        {
            return buildingsLimits.FindAll(limit =>
                limit.hqLevel == level && limit.buildingId != BuildingID.hq.ToString());
        }
        
        public int GetBuildingMaxLevel(string buildingId)
        {
            var buildingsList = serverBuildings.FindAll(b => b.id == buildingId);
            buildingsList.Sort(delegate(ServerBuilding left, ServerBuilding right)
                {
                    if (left == null) return -1;
                    if (right == null) return 1;
                    return left.level == right.level ? 0 : left.level.CompareTo(right.level);
                });
            return buildingsList.Last().level;
        }

        public ServerUnit GetServerUnit(UnitID unitId, int level = 1)
        {
            return serverUnits.Find(unit => unit.id == unitId && unit.level == level);
        }

        
    }
}