using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Model;
using SargeUniverse.Scripts.Sound;
using TMPro;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SargeUniverse.Scripts.UI.BattleSystem
{
    public class UIBattle : UIScreen
    {
        [Header("Header")]
        [SerializeField] private TMP_Text _timerText = null;
        
        [Header("Footer")]
        [SerializeField] private Button _surrenderButton = null;
        [SerializeField] private TMP_Text _overallDamageText = null;
        [SerializeField] private Button _exitButton = null;
        [SerializeField] private Button _nextButton = null;
        

        [Header("Units")]
        [SerializeField] private UnitDeployCard _cardPrefab = null;
        [SerializeField] private Transform _armyCardsContent = null;

        private NetworkPacket _networkPacket;
        private ResponseParser _responseParser;
        private LevelLoader _levelLoader = null;

        [Inject]
        private void Construct(LevelLoader levelLoader, NetworkPacket networkPacket, ResponseParser responseParser)
        {
            _networkPacket = networkPacket;
            _responseParser = responseParser;
            _levelLoader = levelLoader;
        }
        
        public override void ShowScreen()
        {
            base.ShowScreen();
            InitDeployUnits();
        }
        
        protected override void SubscribeForEvents()
        {
            _surrenderButton.onClick.AddListener(OnSurrender);
            _exitButton.onClick.AddListener(OnExit);
            _nextButton.onClick.AddListener(OnNextClick);
        }

        protected override void UnSubscribeFromEvents()
        {
            _surrenderButton.onClick.RemoveListener(OnSurrender);
            _exitButton.onClick.RemoveListener(OnExit);
            _nextButton.onClick.RemoveListener(OnNextClick);
        }

        private void InitDeployUnits()
        {
            var deployUnits = BattleSync.Instance().GetDeployUnits();
            foreach (var deployUnit in deployUnits)
            {
                var card = Instantiate(_cardPrefab, _armyCardsContent);
                card.InitCard(deployUnit.Key, deployUnit.Value.level, deployUnit.Value.unitsData.Count);
            }
        }

        private void OnSurrender()
        {
            BattleControl.Instance.Surrender();
        }

        private void OnExit()
        {
            SoundSystem.Instance.PlaySound("click");
            BattleControl.Instance.Stop();
            _levelLoader.LoadScene(Constants.GameScene);
        }

        private void OnNextClick()
        {
            SoundSystem.Instance.PlaySound("click");
            _responseParser.onBattleFound.AddListener(OnBattleFound);
            _networkPacket.SentFindBattleRequest();
        }

        private void OnBattleFound()
        {
            _responseParser.onBattleFound.RemoveListener(OnBattleFound);
            _levelLoader.LoadScene(Constants.BattleScene);
        }
    }
}