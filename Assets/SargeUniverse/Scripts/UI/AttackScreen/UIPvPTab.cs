using Controller;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Sound;
using TMPro;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SargeUniverse.Scripts.UI.AttackScreen
{
    public class UIPvPTab : UIScreen
    {
        [SerializeField] private TMP_Text _opponentRankText = null;
        
        [SerializeField] private Button _findMatchButton = null;
        [SerializeField] private TMP_Text _findMatchPriceText = null;
        
        private NetworkPacket _networkPacket;
        private ResponseParser _responseParser;
        private LevelLoader _levelLoader;
        private PlayerSyncController _playerSyncController;
        
        [Inject]
        private void Construct(
            NetworkPacket networkPacket,
            ResponseParser responseParser,
            LevelLoader levelLoader,
            PlayerSyncController playerSyncController)
        {
            _networkPacket = networkPacket;
            _responseParser = responseParser;
            _levelLoader = levelLoader;
            _playerSyncController = playerSyncController;
        }
        
        protected override void SubscribeForEvents()
        {
            _findMatchButton.onClick.AddListener(SearchOpponent);
        }

        protected override void UnSubscribeFromEvents()
        {
            _findMatchButton.onClick.RemoveListener(SearchOpponent);
        }

        private void SearchOpponent()
        {
            SoundSystem.Instance.PlaySound("click");
            if (_playerSyncController.HaveTrainedUnits())
            {
                _responseParser.onBattleFound.AddListener(OnBattleFound);
                _networkPacket.SentFindBattleRequest();
            }
            else
            {
                UIManager.Instanse.ShowMessage("No trained units found. Train more units for attack");
            }
        }

        private void OnBattleFound()
        {
            _responseParser.onBattleFound.RemoveListener(OnBattleFound);
            _levelLoader.LoadScene(Constants.BattleScene);
        }
    }
}