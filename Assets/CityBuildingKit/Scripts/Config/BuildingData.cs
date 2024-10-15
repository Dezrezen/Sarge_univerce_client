using System.Collections.Generic;
using Enums;
using SargeUniverse.Scripts.Enums;
using UnityEngine;

namespace Config.Building
{
    [System.Serializable]
    public class BuildingData
    {
        public string Name = "";
        public SargeUniverse.Scripts.Environment.Buildings.Building buildingPrefab;
        public SargeUniverse.Scripts.Environment.Buildings.Building opponentPrefab;
        public BuildingID buildingId = BuildingID.hq;
        public BuildingType buildingType = BuildingType.Base;
        public List<Sprite> playerLevelSprites = new();
        public List<Sprite> opponentLevelSprite = new();

        public Sprite GetBuildingSprite(int level)
        {
            return playerLevelSprites[level - 1];
        }

        public Sprite GetNextLevelBuildingSprite(int level)
        {
            return playerLevelSprites.Count > level ? GetBuildingSprite(level + 1) : GetBuildingSprite(level);
        }

        public Sprite GetOpponentBuildingSprite(int level)
        {
            return opponentLevelSprite[level - 1];
        }
    }
}