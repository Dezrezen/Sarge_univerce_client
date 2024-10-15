using SargeUniverse.Scripts.Enums;
using UnityEngine;

namespace SargeUniverse.Scripts.Model.UI
{
    public class ArmouryUnitCardData
    {
        public UnitID id;
        public Sprite unitImage;
        public Sprite unitImageLocked;
        public Sprite levelRankImage;
        public int level;
        public int upgradePrice;
        public bool locked;
    }
}