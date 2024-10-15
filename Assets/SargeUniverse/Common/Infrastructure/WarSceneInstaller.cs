using SargeUniverse.Scripts;
using UnityEngine;
using Zenject;

namespace SargeUniverse.Common.Infrastructure
{
    public class WarSceneInstaller : MonoInstaller
    {
        [SerializeField] private LevelLoader _levelLoader;

        public override void InstallBindings()
        {
            BindLevelLoader();
        }
        
        private void BindLevelLoader()
        {
            Container.Bind<LevelLoader>()
                .FromInstance(_levelLoader)
                .AsSingle()
                .NonLazy();
        }
    }
}