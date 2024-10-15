using System;
using System.Collections.Generic;
using System.Linq;
using SargeUniverse.Scripts.Enums;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment
{
    [Serializable]
    public class SpriteDirectionCollection : ICloneable
    {
        [SerializeField] private List<Sprite> _north = new();
        [SerializeField] private List<Sprite> _northWest = new();
        [SerializeField] private List<Sprite> _west = new();
        [SerializeField] private List<Sprite> _southWest = new();
        [SerializeField] private List<Sprite> _south = new();
        [SerializeField] private List<Sprite> _southEast = new();
        [SerializeField] private List<Sprite> _east = new();
        [SerializeField] private List<Sprite> _northEast = new();

        private MovementDirection _direction = MovementDirection.North;
        private int _spriteIndex = 0;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        
        public void Reset()
        {
            _spriteIndex = 0;
        }

        public void SetDirection(MovementDirection direction)
        {
            if (_direction == direction)
            {
                return;
            }
            _direction = direction;
            Reset();
        }
        
        public Sprite GetSprite(int offset = 0)
        {
            return GetSpriteFromList(GetDirectionList(), offset);
        }
        
        public Sprite GetFirstSprite()
        {
            return GetDirectionList().First();
        }

        public int GetCollectionSize()
        {
            return GetDirectionList().Count;
        }

        public bool IsLastSpriteInCollection()
        {
            return _spriteIndex + 1 == GetDirectionList().Count;
        }

        private List<Sprite> GetDirectionList()
        {
            return _direction switch
            {
                MovementDirection.North => _north,
                MovementDirection.NorthWest => _northWest,
                MovementDirection.West => _west,
                MovementDirection.SouthWest => _southWest,
                MovementDirection.South => _south,
                MovementDirection.SouthEast => _southEast,
                MovementDirection.East => _east,
                MovementDirection.NorthEast => _northEast,
                _ => _north
            };
        }
        
        private Sprite GetSpriteFromList(IReadOnlyList<Sprite> list, int offset = 0)
        {
            var sprite = list[_spriteIndex];
            _spriteIndex = _spriteIndex + 1 >= list.Count ? _spriteIndex = offset : _spriteIndex + 1;
            return sprite;
        }
    }
}