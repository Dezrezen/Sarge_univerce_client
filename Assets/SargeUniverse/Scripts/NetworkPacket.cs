using SargeUniverse.Scripts.Enums;
using Server;
using Server.Scripts;
using UnityEngine;
using Zenject;

namespace SargeUniverse.Scripts
{
    public class NetworkPacket
    {
        private static readonly string username_key = "username";
        private static readonly string password_key = "password";

        private readonly Sender _packetSender = null;

        [Inject]
        private NetworkPacket(INetworkClient networkClient)
        {
            _packetSender = new Sender(networkClient);
        }
        
        public void AuthRequest(string userName)
        {
            var deviceID = SystemInfo.deviceUniqueIdentifier;
            var password = PlayerPrefs.GetString(password_key, string.Empty);
            PlayerPrefs.SetString(username_key, userName);
            
            var packet = new Packet();
            packet.Write((int)RequestID.Auth);
            packet.Write(deviceID);
            packet.Write(userName);
            packet.Write(password);
            _packetSender.SendPacket(packet);
        }
        
        public void SyncRequest()
        {
            var packet = new Packet();
            packet.Write((int)RequestID.Sync);
            packet.Write(SystemInfo.deviceUniqueIdentifier);
            _packetSender.SendPacket(packet);
        }
        
        public void SendBuildRequest(BuildingID buildingId, int x, int y)
        {
            var packet = new Packet();
            packet.Write((int)RequestID.Build);
            packet.Write(SystemInfo.deviceUniqueIdentifier);
            packet.Write(buildingId.ToString());
            packet.Write(x);
            packet.Write(y);
            _packetSender.SendPacket(packet);
        }

        public void SentMoveBuildingRequest(long databaseId, int xPos, int yPos)
        {
            var packet = new Packet();
            packet.Write((int)RequestID.Replace);
            packet.Write(databaseId);
            packet.Write(xPos);
            packet.Write(yPos);
            _packetSender.SendPacket(packet);
        }

        public void SendCollectRequest(long databaseId)
        {
            var packet = new Packet();
            packet.Write((int)RequestID.Collect);
            packet.Write(databaseId);
            _packetSender.SendPacket(packet);
        }

        public void SendUpgradeRequest(long databaseId)
        {
            var packet = new Packet();
            packet.Write((int)RequestID.Upgrade);
            packet.Write(databaseId);
            _packetSender.SendPacket(packet);
        }

        public void SendInstantBuildRequest(long databaseId)
        {
            var packet = new Packet();
            packet.Write((int)RequestID.InstantBuild);
            packet.Write(databaseId);
            _packetSender.SendPacket(packet);
        }

        public void SendTrainUnitRequest(UnitID unitId)
        {
            var packet = new Packet();
            packet.Write((int)RequestID.TrainUnit);
            packet.Write(unitId.ToString());
            _packetSender.SendPacket(packet);
        }
        
        public void SendCancelTrainUnitRequest(long databaseId)
        {
            var packet = new Packet();
            packet.Write((int)RequestID.CancelTrainUnit);
            packet.Write(databaseId);
            _packetSender.SendPacket(packet);
        }
        
        public void SendRemoveUnitRequest(long databaseId)
        {
            var packet = new Packet();
            packet.Write((int)RequestID.DeleteUnit);
            packet.Write(databaseId);
            _packetSender.SendPacket(packet);
        }

        public void SentFindBattleRequest()
        {
            var packet = new Packet();
            packet.Write((int)RequestID.BattleFind);
            _packetSender.SendPacket(packet);
        }
        
        public void SendStartBattleRequest(byte[] opponentData, int battleType)
        {
            var packet = new Packet();
            packet.Write((int)RequestID.BattleStart);
            packet.Write(opponentData.Length);
            packet.Write(opponentData);
            packet.Write(battleType);
            _packetSender.SendPacket(packet);
        }
        
        public void SendAddMoneyRequest()
        {
            var packet = new Packet();
            packet.Write((int)RequestID.AddMoney);
            _packetSender.SendPacket(packet);
        }

        public void SendRestartGameRequest(bool removeFlag)
        {
            var packet = new Packet();
            packet.Write((int)RequestID.RestartGame);
            packet.Write(removeFlag);
            _packetSender.SendPacket(packet);
        }
    }
}