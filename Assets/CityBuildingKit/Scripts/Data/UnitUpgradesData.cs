using System;
using Enums;

namespace Data
{
    public class UnitUpgradesData
    {
        public UnitID id = 0;
        public uint level = 1;
        public bool isUpgrading = false;
        public DateTime upgradeStartTime;
    }
}