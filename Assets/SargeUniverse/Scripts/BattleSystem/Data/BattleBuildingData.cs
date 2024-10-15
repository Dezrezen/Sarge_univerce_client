using System;
using System.Collections.Generic;
using SargeUniverse.Scripts.BattleSystem.PathFinding;
using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;

namespace SargeUniverse.Scripts.BattleSystem.Data
{
    public class BattleBuildingData
    {
        public Data_Building buildingData = null;
        public float health = 0;
        public int target = -1;
        public double attackTimer = 0;
        private double _damagePercent = 0;
        public BattleVector2 worldCenterPosition;
        public Callbacks.AttackCallback attackCallback = null;
        public Callbacks.DoubleCallback destroyCallback = null;
        public Callbacks.FloatCallback damageCallback = null;
        public Callbacks.BlankCallback starCallback = null;

        public int lootSuppliesStorage = 0;
        public int lootPowerStorage = 0;
        public int lootEnergyStorage = 0;

        public int lootedSupplies = 0;
        public int lootedPower = 0;
        public int lootedEnergy = 0;

        public void Initialize()
        {
            health = buildingData.health;
            _damagePercent = buildingData.percentage;
            lootedSupplies = 0;
            lootedPower = 0;
            lootedEnergy = 0;
        }

        public void InitStoredResources(int hqLevel)
        {
            switch (buildingData.id)
            {
                case BuildingID.hq:
                    lootedSupplies = BattleTrophies.GetStorageSuppliesAndPowerLoot(hqLevel, buildingData.suppliesStorage);
                    lootedPower = BattleTrophies.GetStorageSuppliesAndPowerLoot(hqLevel, buildingData.powerStorage);
                    lootedEnergy = BattleTrophies.GetStorageEnergyLoot(hqLevel, buildingData.energyStorage);
                    break;
                case BuildingID.supplydrop:
                case BuildingID.supplyvault:
                    lootedSupplies = BattleTrophies.GetStorageSuppliesAndPowerLoot(hqLevel, buildingData.suppliesStorage);
                    break;
                case BuildingID.powerplant:
                case BuildingID.powerstorage:
                    lootedPower = BattleTrophies.GetStorageSuppliesAndPowerLoot(hqLevel, buildingData.powerStorage);
                    break;
            }
        }
        
        public BuildingID GetId()
        {
            return buildingData.id;
        }
        
        public void TakeDamage(float damage, ref BattleGrid battleGrid, ref List<Tile> blockedTiles, ref double percentage, ref bool fiftyPercentStar, ref bool hqStar, ref bool allStar)
        {
            if (health <= 0)
            {
                return;
            }
            health -= damage;

            damageCallback?.Invoke(buildingData.databaseID, damage);

            if (health < 0)
            {
                health = 0;
            }

            var loot = 1d - health / (double)buildingData.health;
            if (lootSuppliesStorage > 0)
            {
                lootedSupplies = (int)Math.Floor(lootSuppliesStorage * loot);
            }

            if (lootPowerStorage > 0)
            {
                lootedPower = (int)Math.Floor(lootPowerStorage * loot);
            }

            if (lootEnergyStorage > 0)
            {
                lootedEnergy = (int)Math.Floor(lootEnergyStorage * loot);
            }

            if (health <= 0)
            {
                for (var x = buildingData.x; x < buildingData.x + buildingData.columns; x++)
                {
                    for (var y = buildingData.y; y < buildingData.y + buildingData.rows; y++)
                    {
                        battleGrid[x, y].blocked = false;
                        for (var i = 0; i < blockedTiles.Count; i++)
                        {
                            if (blockedTiles[i].position.x == x && blockedTiles[i].position.y == y)
                            {
                                blockedTiles.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
                
                if (_damagePercent > 0)
                {
                    percentage += _damagePercent;
                }

                destroyCallback?.Invoke(buildingData.databaseID, _damagePercent);

                if (buildingData.id == Enums.BuildingID.hq && !hqStar)
                {
                    hqStar = true;
                    starCallback?.Invoke();
                }

                var p = (int)Math.Floor(percentage * 100d);
                if (p >= 50 && !fiftyPercentStar)
                {
                    fiftyPercentStar = true;
                    starCallback?.Invoke();
                }

                if (p >= 100 && !allStar)
                {
                    allStar = true;
                    starCallback?.Invoke();
                }
            }
        }
    }
}