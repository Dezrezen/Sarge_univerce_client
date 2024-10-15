using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.UI;
using UnityEngine;
using Utils;

namespace SargeUniverse.Scripts.Map.Buildings
{
    public class WarBase : MonoBehaviour
    {
        [SerializeField] private UIWarBaseLabel _warBaseLabel = null;
        
        public int X { get; private set; } = 0;
        public int Y { get; private set; } = 0;

        private void Update()
        {
            if (_warBaseLabel != null)
            {
                AdjustNamePosition();
            }
        }

        public void SetBaseName(UIWarBaseLabel warBaseLabel, string baseName)
        {
            _warBaseLabel = warBaseLabel;
            _warBaseLabel.labelText.text = baseName;
        }

        public string GetBaseName()
        {
            return _warBaseLabel.labelText.text;
        }
        
        public void PlaceOnGrid(int x, int y, bool build = false)
        {
            X = x;
            Y = y;
            SetPosition(x, y);
        }

        private void SetPosition(int x, int y)
        {
            transform.position = WarMapManager.Instanse.Grid.CellToWorld(x, y);
        }
        
        private void AdjustNamePosition()
        {
            var end = WarMapManager.Instanse.Grid.GetEndPosition(this);
            
            var plainDownLeft = CameraUtils.plainDownLeft;
            var plainTopRight = CameraUtils.plainTopRight;
            
            var w = plainTopRight.x - plainDownLeft.x;
            var h = plainTopRight.y - plainDownLeft.y;
            
            var endW = end.x - plainDownLeft.x;
            var endH = end.y - plainDownLeft.y;
            
            var screenPoint = new Vector2(endW / w * Screen.width, endH / h * Screen.height + 15);
            _warBaseLabel.rect.anchoredPosition = screenPoint;
        }

        public void Selected()
        {
            if (WarMapManager.Instanse.selectedWarBase != null)
            {
                if (WarMapManager.Instanse.selectedWarBase == this)
                {
                    return;
                }
                WarMapManager.Instanse.selectedWarBase = this;
            }

            WarMapManager.Instanse.selectedWarBase = this;
            UIWarScreenManager.Instance.ShowBaseDetails();
        }

        public void Deselected()
        {
            UIWarScreenManager.Instance.HideBaseDetails();
            WarMapManager.Instanse.selectedWarBase = null;
        }
    }
}