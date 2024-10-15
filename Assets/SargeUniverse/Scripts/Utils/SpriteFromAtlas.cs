using UnityEngine;
using UnityEngine.U2D;

namespace SargeUniverse.Scripts.Utils
{
    public class SpriteFromAtlas : MonoBehaviour
    {
        [SerializeField] private SpriteAtlas _atlas = null;
        
        public Sprite GetSprite(string spriteName)
        {
            return _atlas.GetSprite(spriteName);
        }

        public int GetSpritesCount()
        {
            return _atlas.spriteCount;
        }
    }
}