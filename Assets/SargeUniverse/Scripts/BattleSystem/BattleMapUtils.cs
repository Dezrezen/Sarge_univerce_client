using System;
using SargeUniverse.Scripts.BattleSystem.Data;
using UnityEngine;

namespace SargeUniverse.Scripts.BattleSystem
{
    public class BattleMapUtils
    {
        public static BattleVector2 GridToWorldPosition(Vector2Int position)
        {
            return new BattleVector2(
                position.x * Constants.GridCellSize + Constants.GridCellSize / 2f, 
                position.y * Constants.GridCellSize + Constants.GridCellSize / 2f
            );
        }

        public static Vector2Int WorldToGridPosition(BattleVector2 position)
        {
            return new Vector2Int(
                (int)Math.Floor(position.x / Constants.GridCellSize), 
                (int)Math.Floor(position.y / Constants.GridCellSize)
            );
        }
    }
}