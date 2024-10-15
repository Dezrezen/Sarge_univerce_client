using System;
using SargeUniverse.Scripts.Sound;
using SargeUniverse.Scripts.UI;
using SargeUniverse.Scripts.UI.BattleSystem;
using UI;
using UnityEngine;

namespace SargeUniverse.Scripts.Controller.UI
{
    public class UIBattleManager : MonoBehaviour
    {
        [SerializeField] private InterfaceSwitcher _interfaceSwitcher;
        
        [Header("Message Window")] 
        [SerializeField] private UIMessageBox _messageBox;
        
        public static UIBattleManager Instanse { get; private set; } = null;

        private void Awake()
        {
            Instanse ??= this;
        }

        private void Start()
        {
            SoundSystem.Instance.PlayMusic("battle_theme");
            _interfaceSwitcher.SwitchPanel<UIBattle>();
        }

        private void OnDestroy()
        {
            Instanse = null;
        }
    }
}