using SargeUniverse.Scripts;

namespace Server.Scripts
{
    public class Sender
    {
        private INetworkClient _networkClient = null;
        
        public Sender(INetworkClient networkClient)
        {
            _networkClient = networkClient;
        }
        
        public void SendPacket(Packet packet)
        {
            SendData(packet.ToArray());
        }

        private void SendData(byte[] bytes)
        {
            _networkClient.GetWsClient().SendData(bytes);
        }
    }
}