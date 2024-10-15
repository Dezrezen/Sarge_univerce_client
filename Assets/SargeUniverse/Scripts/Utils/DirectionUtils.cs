using System;
using System.Collections.Generic;
using SargeUniverse.Scripts.Enums;
using UnityEngine;

namespace SargeUniverse.Scripts.Utils
{
    public static class DirectionUtils
    {
        public static MovementDirection GetLookDirection(Vector3 position, Vector3 targetPosition)
        {
            var lookDirection = MovementDirection.South;
            var direction = targetPosition - position;
            
            switch (direction.y)
            {
                case > 0 when direction.y >= 3 * Mathf.Abs(direction.x):
                    lookDirection = MovementDirection.North;
                    break;
                case > 0:
                    if (direction.y < 3 * Mathf.Abs(direction.x) && Mathf.Abs(direction.x) < 3 * direction.y)
                    {
                        lookDirection = direction.x > 0 ? MovementDirection.NorthEast : MovementDirection.NorthWest;
                    }
                    else
                    {
                        lookDirection = direction.x > 0 ? MovementDirection.East : MovementDirection.West;
                    }
                    break;
                case < 0 when Mathf.Abs(direction.y) >= 3 * Mathf.Abs(direction.x):
                    lookDirection = MovementDirection.South;
                    break;
                case < 0:
                    if (Mathf.Abs(direction.y) < 3 * Mathf.Abs(direction.x) &&
                        Mathf.Abs(direction.x) < 3 * Mathf.Abs(direction.y))
                    {
                        lookDirection = direction.x > 0 ? MovementDirection.SouthEast : MovementDirection.SouthWest;
                    }
                    else
                    {
                        lookDirection = direction.x > 0 ? MovementDirection.East : MovementDirection.West;
                    }
                    break;
                default:
                    lookDirection = direction.x > 0 ? MovementDirection.East : MovementDirection.West;
                    break;
            }
            
            return lookDirection;
        }

        public static List<MovementDirection> GetRotationList(MovementDirection from, MovementDirection to, bool skipFirst = true)
        {
            List<MovementDirection> route = new();
            var fromValue = (int)from;
            var toValue = (int)to;
            var enumLength = Enum.GetValues(typeof(MovementDirection)).Length;
            
            if (fromValue < toValue)
            {
                if (toValue - fromValue <= enumLength / 2)
                {
                    for (var i = fromValue; i <= toValue; i++)
                    {
                        route.Add((MovementDirection)i);
                    }
                }
                else
                {
                    if (fromValue >= 0)
                    {
                        for (var i = fromValue; i >= 0; i--)
                        {
                            route.Add((MovementDirection)i);
                        }
                    }
                    for (var i = enumLength - 1; i >= toValue; i--)
                    {
                        route.Add((MovementDirection)i);
                    }
                }
            }
            else
            {
                route = GetRotationList(to, from, false);
                route.Reverse();
                
            }

            if (skipFirst)
            {
                route.RemoveAt(0);
            }

            return route;
        }
    }
}