using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment
{
    [System.Serializable]
    public class SpriteCollection
    {
        [SerializeField] private List<Sprite> _collection = new();
        
        private int _spriteIndex = 0;

        public List<Sprite> GetSprites()
        {
            return _collection;
        }
        
        public void Reset()
        {
            _spriteIndex = 0;
        }
        
        public Sprite GetSprite()
        {
            var sprite = _collection[_spriteIndex];
            _spriteIndex = _spriteIndex + 1 >= _collection.Count ? _spriteIndex = 0 : _spriteIndex + 1;
            return sprite;
        }
        
        public Sprite GetReverseSprite()
        {
            var sprite = _collection[_spriteIndex];
            _spriteIndex = _spriteIndex > 0 ? _spriteIndex - 1 : _collection.Count - 1;
            return sprite;
        }

        public Sprite GetFirstSprite()
        {
            return _collection.First();
        }
        
        public Sprite GetLastSprite()
        {
            return _collection.Last();
        }

        public bool IsLastSpriteInCollection()
        {
            return _spriteIndex + 1 == _collection.Count;
        }
    }
}