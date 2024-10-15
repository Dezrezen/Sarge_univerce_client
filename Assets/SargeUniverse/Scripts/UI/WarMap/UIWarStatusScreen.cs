using TMPro;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace SargeUniverse.Scripts.UI
{
    public class UIWarStatusScreen : UIScreen
    {
        [Header("Player Section")]
        [SerializeField] private TMP_Text _playerName = null;
        [SerializeField] private Slider _playerBattleProgress = null;
        [SerializeField] private TMP_Text _playerBattleProgressValue = null;
        [SerializeField] private TMP_Text _playerArmy = null;
        
        [Header("Enemy Section")]
        [SerializeField] private TMP_Text _enemyName = null;
        [SerializeField] private Slider _enemyBattleProgress = null;
        [SerializeField] private TMP_Text _enemyBattleProgressValue = null;
        [SerializeField] private TMP_Text _enemyArmy = null;

        [Header("Results")] 
        [SerializeField] private TMP_Text _warStatusLabel = null;
        [SerializeField] private Button _warResultButton = null;
        
        protected override void SubscribeForEvents()
        {
            _warResultButton.onClick.AddListener(ShowWarResults);
        }

        protected override void UnSubscribeFromEvents()
        {
            _warResultButton.onClick.RemoveListener(ShowWarResults);
        }

        private void ShowWarResults()
        {
            _warStatusLabel.gameObject.SetActive(!_warStatusLabel.gameObject.activeInHierarchy);
        }
    }
}