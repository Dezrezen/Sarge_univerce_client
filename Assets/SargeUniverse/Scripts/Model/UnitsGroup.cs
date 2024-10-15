using System.Collections.Generic;
using System.Linq;
using SargeUniverse.Scripts.Enums;

namespace SargeUniverse.Scripts.Model
{
    public class UnitsGroup
    {
        public UnitID UnitsId { get; private set; }
        public List<long> DatabseIds { get; } = new();

        public UnitsGroup(UnitID unitId)
        {
            UnitsId = unitId;
        }

        public void AddUnit(long databaseId)
        {
            DatabseIds.Add(databaseId);
        }

        public long GetUnit()
        {
            long databaseId = 0;
            if (DatabseIds.Count > 0)
            {
                databaseId = DatabseIds.First();
            }
            return databaseId;
        }

        public void RemoveUnit(long databaseId)
        {
            DatabseIds.Remove(databaseId);
        }

        public int GroupSize()
        {
            return DatabseIds.Count;
        }
    }
}