using UnityEngine;

namespace SargeUniverse.Scripts.BattleSystem.PathFinding
{
    public interface IBattleGrid
    {
        Vector2Int Size { get; }
        Cell this[Vector2Int position] { get; }
        void Reset();
    }
}