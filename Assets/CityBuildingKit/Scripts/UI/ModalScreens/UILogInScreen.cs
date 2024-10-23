using SargeUniverse.Scripts;
using SargeUniverse.Scripts.Enums;
using TMPro;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class UILogInScreen : UIScreen
    {
        [SerializeField] private TMP_InputField _inputField = null;
        [SerializeField] private Button _closeButton = null;
        [SerializeField] private Button _logInButton = null;
        [SerializeField] private Button _eliteButton = null;

        private NetworkPacket _networkPacket;
        
        [Inject]
        private void Construct(NetworkPacket networkPacket)
        {
            _networkPacket = networkPacket;
        }
        
        public override void ShowScreen()
        {
            base.ShowScreen();
            _closeButton.gameObject.SetActive(PlayerPrefs.HasKey("PlayerPrefs"));
        }
        
        protected override void SubscribeForEvents()
        {
            _closeButton.onClick.AddListener(OnCloseButton);
            _logInButton.onClick.AddListener(OnLogInButton);
            _eliteButton.onClick.AddListener(OnEliteButton);
        }

        protected override void UnSubscribeFromEvents()
        {
            _closeButton.onClick.RemoveListener(OnCloseButton);
            _logInButton.onClick.RemoveListener(OnLogInButton);
            _eliteButton.onClick.RemoveListener(OnEliteButton);
        }

        private void OnCloseButton()
        {
            InterfaceSwitcher.CloseModalPanel<UILogInScreen>();
        }

        private void OnLogInButton()
        {
            if (_inputField.text != null)
            {
                _networkPacket.AuthRequest(_inputField.text);
                InterfaceSwitcher.CloseModalPanel<UILogInScreen>();
            }
        }
        private void OnEliteButton()
        {
            if (_inputField.text != null)
            {
                _networkPacket.AuthRequest(_inputField.text, StartBaseType.Elite);
                InterfaceSwitcher.CloseModalPanel<UILogInScreen>();
            }
        }
    }
}