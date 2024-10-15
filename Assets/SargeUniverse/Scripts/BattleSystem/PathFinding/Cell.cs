using UnityEngine;

namespace SargeUniverse.Scripts.BattleSystem.PathFinding
{
    public class Cell
    {
        public bool blocked;
        public bool closed;
        public double priority;
        public double G;
        public double H;

        public Vector2Int location;
        public Cell parent;
        public int queueIndex;

        public Cell(Vector2Int location) => this.location = location;
        public override string ToString() => $"[{location.x},{location.y}]";
    }
}