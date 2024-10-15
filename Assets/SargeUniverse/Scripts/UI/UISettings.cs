using Controller;
using SargeUniverse.Scripts.Sound;
using SargeUniverse.Scripts.UI.Elements;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace SargeUniverse.Scripts.UI
{
    public class UISettings : UIScreen
    {
        [SerializeField] private Button _closeButton = null;

        [Header("Settings Buttons")]
        [SerializeField] private SwitchButton _sfxButton = null;
        [SerializeField] private SwitchButton _musicButton = null;
        [SerializeField] private Slider _sfxVolumeSlider = null;
        [SerializeField] private Slider _musicVolumeSlider = null;

        [SerializeField] private Button _privateServiceButton = null;
        [SerializeField] private Button _termsOfServiceButton = null;
        [SerializeField] private Button _referralProgramButton = null;
        [SerializeField] private Button _supportCreatorButton = null;

        protected override void Start()
        {
            _sfxButton.SetState(SoundSystem.Instance.IsSoundAvailable);
            _musicButton.SetState(SoundSystem.Instance.IsMusicAvailable);

            _sfxVolumeSlider.value = SoundSystem.Instance.SoundVolume;
            _musicVolumeSlider.value = SoundSystem.Instance.MusicVolume;
            
            base.Start();
        }
        
        public override void ShowScreen()
        {
            base.ShowScreen();
            UIManager.Instanse.LockScreenMove = true;
        }

        public override void HideScreen()
        {
            base.HideScreen();
            UIManager.Instanse.LockScreenMove = false;
        }
        
        protected override void SubscribeForEvents()
        {
            _closeButton.onClick.AddListener(CloseButtonClick);
            
            _sfxButton.onSwitchState.AddListener(SfxButtonClick);
            _musicButton.onSwitchState.AddListener(MusicButtonClick);
            
            _sfxVolumeSlider.onValueChanged.AddListener(SfxSliderValueChange);
            _musicVolumeSlider.onValueChanged.AddListener(MusicSliderValueChange);
            
            _privateServiceButton.onClick.AddListener(PrivateServiceClick);
            _termsOfServiceButton.onClick.AddListener(TermsOfServiceClick);
            _referralProgramButton.onClick.AddListener(ReferralProgramClick);
            _supportCreatorButton.onClick.AddListener(SupportCreatorClick);
        }

        protected override void UnSubscribeFromEvents()
        {
            _closeButton.onClick.RemoveListener(CloseButtonClick);
            
            _sfxButton.onSwitchState.RemoveListener(SfxButtonClick);
            _musicButton.onSwitchState.RemoveListener(MusicButtonClick);
            
            _sfxVolumeSlider.onValueChanged.RemoveListener(SfxSliderValueChange);
            _musicVolumeSlider.onValueChanged.RemoveListener(MusicSliderValueChange);
            
            _privateServiceButton.onClick.RemoveListener(PrivateServiceClick);
            _termsOfServiceButton.onClick.RemoveListener(TermsOfServiceClick);
            _referralProgramButton.onClick.RemoveListener(ReferralProgramClick);
            _supportCreatorButton.onClick.RemoveListener(SupportCreatorClick);
        }

        private void CloseButtonClick()
        {
            InterfaceSwitcher.CloseModalPanel<UISettings>();
        }
        
        private void SfxButtonClick(bool state)
        {
            SoundSystem.Instance.PlaySound("click");
            if (state)
            {
                SoundSystem.Instance.EnableSound();
            }
            else
            {
                SoundSystem.Instance.DisableSounds();
            }
        }

        private void MusicButtonClick(bool state)
        {
            SoundSystem.Instance.PlaySound("click");
            if (state)
            {
                SoundSystem.Instance.EnableMusic();
            }
            else
            {
                SoundSystem.Instance.DisableMusic();
            }
        }

        private void SfxSliderValueChange(float value)
        {
            SoundSystem.Instance.SetSoundVolume(value);
        }

        private void MusicSliderValueChange(float value)
        {
            SoundSystem.Instance.SetMusicVolume(value);
        }

        private void PrivateServiceClick()
        {
            SoundSystem.Instance.PlaySound("click");
        }

        private void TermsOfServiceClick()
        {
            SoundSystem.Instance.PlaySound("click");
        }

        private void ReferralProgramClick()
        {
            SoundSystem.Instance.PlaySound("click");
        }

        private void SupportCreatorClick()
        {
            SoundSystem.Instance.PlaySound("click");
        }
    }
}