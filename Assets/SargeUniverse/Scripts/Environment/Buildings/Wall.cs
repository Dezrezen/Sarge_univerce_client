using System.Collections.Generic;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Buildings
{
    public class Wall : ConstructionBuilding
    {
        [SerializeField] private List<Sprite> _connectionLevelSprites = new();
        
        [SerializeField] private SpriteRenderer _leftConnection = null;
        [SerializeField] private SpriteRenderer _rightConnection = null;

        public override void EditMode(bool flag)
        {
            base.EditMode(flag);
            if (flag)
            {
                SetLeftConnection(false);
                SetRightConnection(false);
            }
        }

        protected override void SetBuildingSprite()
        {
            base.SetBuildingSprite();
            var buildingLevel = BuildingData.level == 0 ? 1 : BuildingData.level;
            SetConnectionImage(_connectionLevelSprites[buildingLevel - 1]);
        }
        
        private void SetConnectionImage(Sprite sprite)
        {
            _leftConnection.sprite = sprite;
            _rightConnection.sprite = sprite;
        }

        public void SetLeftConnection(bool state)
        {
            _leftConnection.gameObject.SetActive(state);
        }

        public void SetRightConnection(bool state)
        {
            _rightConnection.gameObject.SetActive(state);
        }
    }
}