using Controller;
using SargeUniverse.Scripts.Controller;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Elements
{
    public class UiBuild : UIScreen
    {
        [SerializeField] private Button _buildButton = null;
        [SerializeField] private Button _cancelButton = null;

        [SerializeField] private RectTransform _anchorPosition = null;
        [SerializeField] private RectTransform _buttonConfirm = null;
        [SerializeField] private RectTransform _buttonCancel = null;

        [SerializeField] private float _width = 0.05f;
        [SerializeField] private float _height = 0.05f;
        private Vector2 _size = Vector2.one;
        
        protected void Start()
        {
            base.Start();
            
            _anchorPosition.anchorMin = Vector3.zero;
            _anchorPosition.anchorMax = Vector3.zero;
            /*_buttonConfirm.anchorMin = Vector3.zero;
            _buttonConfirm.anchorMax = Vector3.zero;
            _buttonCancel.anchorMin = Vector3.zero;
            _buttonCancel.anchorMax = Vector3.zero;*/
            
            _size = new Vector2(Screen.height * _height, Screen.height * _height);
        }
        
        protected override void SubscribeForEvents()
        {
            _buildButton.onClick.AddListener(OnBuildButtonClick);
            _cancelButton.onClick.AddListener(OnCancelButtonClick);
        }

        protected override void UnSubscribeFromEvents()
        {
            _buildButton.onClick.RemoveListener(OnBuildButtonClick);
            _cancelButton.onClick.RemoveListener(OnCancelButtonClick);
        }

        public override void HideScreen()
        {
            base.HideScreen();
            InterfaceSwitcher.CloseModalScreen();
        }

        private void OnBuildButtonClick()
        {
            if (BuildingsManager.Instanse.ConfirmBuild())
            {
                HideScreen();
            }
        }

        private void OnCancelButtonClick()
        {
            BuildingsManager.Instanse.CancelBuild();
            HideScreen();
        }

        private void Update()
        {
            if (!BuildingsManager.Instanse.activeBuilding || !BuildingsManager.Instanse.buildMode)
            {
                OnCancelButtonClick();
                return;
            }
            var end = BuildingsManager.Instanse.grid.GetEndPosition(BuildingsManager.Instanse.activeBuilding);
        
            var plainDownLeft = CameraUtils.plainDownLeft;
            var plainTopRight = CameraUtils.plainTopRight;
        
            var w = plainTopRight.x - plainDownLeft.x;
            var h = plainTopRight.y - plainDownLeft.y;
        
            var endW = end.x - plainDownLeft.x;
            var endH = end.y - plainDownLeft.y;
                
            var screenPoint = new Vector2(endW / w * Screen.width, endH / h * Screen.height);
            _anchorPosition.anchoredPosition = screenPoint;
        }
    }
}