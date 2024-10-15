using SargeUniverse.Scripts.Enums;
using UnityEngine;

namespace SargeUniverse.Scripts.BattleSystem.Data
{
    public class Tile
    {
        public BuildingID id;
        public Vector2Int position;
        public int index;

        public Tile(BuildingID id, Vector2Int position, int index = -1)
        {
            this.id = id;
            this.position = position;
            this.index = index;
        }
    }
}