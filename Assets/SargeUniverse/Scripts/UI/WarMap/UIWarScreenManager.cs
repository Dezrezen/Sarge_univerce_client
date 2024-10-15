using UI;
using UnityEngine;

namespace SargeUniverse.Scripts.UI
{
    public class UIWarScreenManager : MonoBehaviour
    {
        [SerializeField] private InterfaceSwitcher _interfaceSwitcher;
        
        public static UIWarScreenManager Instance { get; private set; } = null;

        private void Awake()
        {
            Instance ??= this;
        }

        private void Start()
        {
            _interfaceSwitcher.SwitchPanel<UIWarMapScreen>();
        }

        public void ShowBaseDetails()
        {
            _interfaceSwitcher.ShowPanel<UIWarBaseArmyScreen>();
        }

        public void HideBaseDetails()
        {
            _interfaceSwitcher.ClosePanel<UIWarBaseArmyScreen>();
        }
    }
}