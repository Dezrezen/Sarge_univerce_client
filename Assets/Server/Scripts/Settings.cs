using UnityEngine;
using Zenject;

namespace Server.Scripts
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Server Config")]
    public class Settings : ScriptableObjectInstaller
    {
        
        [Header("Credentials")]
        [SerializeField] private bool _debugMode = true;
        
        [Tooltip("Server IP address.")]
        [SerializeField] private string _debugIp = "127.0.0.1";
        [SerializeField] private string _publicIp = "13.53.124.8";
        public string IP => _debugMode ? _debugIp : _publicIp;

        [Tooltip("Server port number.")]
        [SerializeField] private int _debugPort = 5555;
        [SerializeField] private int _publicPort = 555;
        public int Port => _debugMode ? _debugPort : _publicPort;

        public override void InstallBindings()
        {
            Container.Bind<Settings>()
                .FromInstance(this)
                .AsSingle();
        }
    }
}