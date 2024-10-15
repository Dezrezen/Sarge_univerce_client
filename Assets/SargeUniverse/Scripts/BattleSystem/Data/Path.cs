using System.Collections.Generic;
using System.Linq;
using SargeUniverse.Scripts.BattleSystem.PathFinding;
using UnityEngine;

namespace SargeUniverse.Scripts.BattleSystem.Data
{
    public class Path
    {
        public Vector2Int startLocation;
        public Vector2Int endLocation;
        public List<Cell> points = null;
        public float length = 0;
        public List<Tile> blocks = null;
        
        public Path()
        {
            length = 0;
            points = null;
            blocks = new List<Tile>();
        }
        
        public bool Create(ref PathBuilder pathBuilder, Vector2Int start, Vector2Int end)
        {
            points = pathBuilder.BuildPath(new Vector2Int(start.x, start.y), new Vector2Int(end.x, end.y));
            if (!IsValid(ref points, new Vector2Int(start.x, start.y), new Vector2Int(end.x, end.y)))
            {
                points = null;
                return false;
            }

            startLocation.x = start.x;
            startLocation.y = start.y;
            endLocation.x = end.x;
            endLocation.y = end.y;
            return true;
        }

        public static bool IsValid(ref List<Cell> points, Vector2Int start, Vector2Int end)
        {
            if (points == null || !points.Any() || !points.Last().location.Equals(end) ||
                !points.First().location.Equals(start))
            {
                return false;
            }

            return true;
        }
    }
}