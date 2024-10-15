using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Enums;
using UI.Elements;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace SargeUniverse.Scripts.Environment.Buildings
{
    public class ResourcesBuilding : ConstructionBuilding
    {
        private bool _collecting = false;

        public UICollectButton CollectButton { get; set; } = null;

        public UnityEvent<long> onCollectResources = new();
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (CollectButton && CollectButton != null)
            {
                Destroy(CollectButton.gameObject);
            }
            onCollectResources.RemoveAllListeners();
        }
        
        protected override void Update()
        {
            base.Update();
            
            if (CollectButton == null)
            {
                return;
            }

            switch (_buildingId)
            {
                case BuildingID.supplydrop:
                    if (BuildingData.suppliesStorage >= Constants.MinCollectValue)
                    {
                        CollectButton.gameObject.SetActive(!_collecting && BuildingData.isConstructing == false);
                    }
                    else
                    {
                        CollectButton.gameObject.SetActive(false);
                    }
                    break;
                case BuildingID.powerplant:
                    if (BuildingData.powerStorage >= Constants.MinCollectValue)
                    {
                        CollectButton.gameObject.SetActive(!_collecting && BuildingData.isConstructing == false);
                    }
                    else
                    {
                        CollectButton.gameObject.SetActive(false);
                    }
                    break;
            }

            AdjustButtonPosition();
        }

        private void AdjustButtonPosition()
        {
            var end = BuildingsManager.Instanse.grid.GetEndPosition(this);

            var plainDownLeft = CameraUtils.plainDownLeft;
            var plainTopRight = CameraUtils.plainTopRight;

            var w = plainTopRight.x - plainDownLeft.x;
            var h = plainTopRight.y - plainDownLeft.y;

            var endW = end.x - plainDownLeft.x;
            var endH = end.y - plainDownLeft.y;

            var screenPoint = new Vector2(endW / w * Screen.width, endH / h * Screen.height);
            CollectButton.rect.anchoredPosition = screenPoint;
        }

        public void Collect()
        {
            CollectButton.gameObject.SetActive(false);
            _collecting = true;
            
            onCollectResources.Invoke(BuildingData.databaseID);
        }
        
        public void SetCollectState(int amount)
        {
            _collecting = false;
            switch (_buildingId)
            {
                case BuildingID.supplydrop:
                    BuildingData.suppliesStorage -= amount;
                    break;
                case BuildingID.powerplant:
                    BuildingData.powerStorage -= amount;
                    break;
            }
        }
    }
}