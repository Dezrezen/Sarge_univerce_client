using System.Collections.Generic;
using SargeUniverse.Scripts.Environment.Buildings.Animators;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Buildings
{
    public class TachyonTower : BattleBuilding
    {
        
        [SerializeField] private SpriteRenderer _sphereSprite = null;
        [SerializeField] private List<Sprite> _towerSprites = new();
        [SerializeField] private TowerSphereAnimator _sphereAnimator = null;
        
        protected override void SetBuildingSprite()
        {
            var buildingLevel = BuildingData.level == 0 ? 1 : BuildingData.level;
            _buildingSprite.sprite = _towerSprites[buildingLevel - 1];
            _sphereAnimator.PlaySphereAnimation(buildingLevel);
        }
        
        public override void DestroyBuilding()
        {
            base.DestroyBuilding();
            _sphereAnimator.StopAnimation();
            _sphereSprite.enabled = false;
        }
        
        protected override void StartUpgradeProcess()
        {
            if (_buildingData.level == 0)
            {
                _sphereSprite.enabled = false;
            }
            base.StartUpgradeProcess();
        }

        protected override void FinishUpgradeProcess()
        {
            base.FinishUpgradeProcess();
            if (_buildingData.level == 1)
            {
                _sphereSprite.enabled = true;
            }
        }
    }
}