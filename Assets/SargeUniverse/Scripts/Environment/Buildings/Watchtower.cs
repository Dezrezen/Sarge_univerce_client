using System.Collections.Generic;
using SargeUniverse.Scripts.Sound;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Buildings
{
    public class Watchtower : BattleBuilding
    {
        [SerializeField] private SpriteRenderer _roofSprite = null;
        [SerializeField] private SpriteRenderer _gunSprite = null;
        [SerializeField] private List<Sprite> _towerSprites = new();
        [SerializeField] private List<Sprite> _roofSprites = new();
        
        protected override void SetBuildingSprite()
        {
            var buildingLevel = BuildingData.level == 0 ? 1 : BuildingData.level;
            _buildingSprite.sprite = _towerSprites[buildingLevel - 1];
            _roofSprite.sprite = _roofSprites[buildingLevel - 1];
        }

        public override void Attack(Transform targetTransform)
        {
            base.Attack(targetTransform);
            SoundSystem.Instance.PlaySound("watchtower_shoot");
        }

        public override void DestroyBuilding()
        {
            base.DestroyBuilding();
            _roofSprite.enabled = false;
        }

        protected override void StartUpgradeProcess()
        {
            if (_buildingData.level == 0)
            {
                _roofSprite.enabled = false;
                _gunSprite.enabled = false;
            }
            base.StartUpgradeProcess();
        }

        protected override void FinishUpgradeProcess()
        {
            base.FinishUpgradeProcess();
            if (_buildingData.level == 1)
            {
                _roofSprite.enabled = true;
                _gunSprite.enabled = true;
            }
        }
    }
}