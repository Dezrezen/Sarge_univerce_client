using System;
using SargeUniverse.Scripts.Enums;
using UnityEngine;

namespace SargeUniverse.Scripts.Model.Environment
{
    [Serializable]
    public class Vector2Direction
    {
        public string name = "Level 1";
        public Vector2 north = Vector2.zero;
        public Vector2 northWest = Vector2.zero;
        public Vector2 west = Vector2.zero;
        public Vector2 southWest = Vector2.zero;
        public Vector2 south = Vector2.zero;
        public Vector2 southEast = Vector2.zero;
        public Vector2 east = Vector2.zero;
        public Vector2 northEast = Vector2.zero;

        public Vector3 GetPoint(MovementDirection direction)
        {
            return direction switch
            {
                MovementDirection.North => north,
                MovementDirection.NorthWest => northWest,
                MovementDirection.West => west,
                MovementDirection.SouthWest => southWest,
                MovementDirection.South => south,
                MovementDirection.SouthEast => southEast,
                MovementDirection.East => east,
                MovementDirection.NorthEast => northEast,
                _ => Vector2.zero
            };
        }
    }
}