using SargeUniverse.Scripts;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Sound;
using UnityEngine;
using Zenject;

namespace SargeUniverse.Common.Infrastructure
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private SoundSystem _soundSystem;
        
        public override void InstallBindings()
        {
            BindNetworkClient();
            BindNetworkPacket();
            BindResponseParser();
            
            BindSoundManager();
            BindGameController();
        }
        
        private void BindNetworkClient()
        {
            Container.BindInterfacesTo<NetworkClient>()
                .FromNewComponentOnRoot()
                .AsSingle()
                .NonLazy();
        }
        
        private void BindNetworkPacket()
        {
            Container.Bind<NetworkPacket>()
                .AsSingle();
        }
        
        private void BindResponseParser()
        {
            Container.Bind<ResponseParser>()
                .AsSingle()
                .NonLazy();
        }
        
        private void BindSoundManager()
        {
            Container.Bind<SoundSystem>()
                .FromComponentInNewPrefab(_soundSystem)
                .AsSingle()
                .NonLazy();
        }
        
        private void BindGameController()
        {
            Container.Bind<GameController>()
                .AsSingle()
                .NonLazy();
        }
    }
}