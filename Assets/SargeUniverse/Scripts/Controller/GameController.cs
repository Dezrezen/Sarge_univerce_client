using System.Collections.Generic;
using Controller;
using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;
using UnityEngine;
using Zenject;

namespace SargeUniverse.Scripts.Controller
{
    public class GameController
    {
        private InitializationData _initializationData;
        
        private INetworkClient _networkClient;
        private NetworkPacket _networkPacket;
        private ResponseParser _responseParser;
        
        [Inject]
        private void Construct(
            INetworkClient networkClient, 
            NetworkPacket networkPacket, 
            ResponseParser responseParser)
        {
            _networkClient = networkClient;
            _networkPacket = networkPacket;
            _responseParser = responseParser;

            InitCallbacks();
        }

        private void InitCallbacks()
        {
            _responseParser.onReceiveInitializationData.AddListener(SetInitData);
        }

        public void InitGameScene()
        {
            if (PlayerPrefs.HasKey("username"))
            {
                if (_networkClient.GetConnectionState())
                {
                    UIManager.Instanse.ShowMainUI();
                    _networkPacket.SyncRequest();
                }
                else
                {
                    AuthPlayer();
                }
            }
            else
            {
                UIManager.Instanse.ShowLogInScreen();
            }
        }

        public void ClearGameData()
        {
            PlayerPrefs.DeleteKey("username");
            PlayerPrefs.DeleteKey("password");
        }

        private void AuthPlayer()
        {
            var userName = PlayerPrefs.GetString("username");
            _networkPacket.AuthRequest(userName);
        }

        private void SetInitData(InitializationData data)
        {
            _initializationData = data;
        }

        public ServerBuilding GetServerBuilding(BuildingID buildingId, int level = 1)
        {
            return GetServerBuilding(buildingId.ToString(), level);
        }
        
        public ServerBuilding GetServerBuilding(string buildingId, int level)
        {
            return _initializationData.GetServerBuilding(buildingId, level);
        }

        public int GetBuildingMaxLevel(BuildingID buildingId)
        {
            return _initializationData.GetBuildingMaxLevel(buildingId.ToString());
        }
        
        public int GetBuildingUnlockLevel(BuildingID buildingId, int level = 1)
        {
            return _initializationData.GetBuildingUnlockLevel(buildingId.ToString(), level);
        }

        public BuildingCount GetBuildingCount(BuildingID buildingId)
        {
            BuildingCount limits = null;
            var hqLevel = BuildingsManager.Instanse.GetHqLevel();
            limits = _initializationData.buildingsLimits.Find(limit =>
                limit.hqLevel == hqLevel && limit.buildingId == buildingId.ToString());

            if (limits != null)
            {
                limits.have = BuildingsManager.Instanse.GetTotalBuilding(buildingId);
            }

            return limits;
        }

        public List<BuildingCount> GetBuildingLimits(int level)
        {
            return _initializationData.GetBuildingLimits(level);
        }
        
        public ServerUnit GetServerUnit(UnitID unitId, int level = 1)
        {
            return _initializationData.GetServerUnit(unitId, level);
        }
    }
}