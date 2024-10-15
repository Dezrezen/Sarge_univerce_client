using SargeUniverse.Common.Controller;
using SargeUniverse.Common.View;
using SargeUniverse.Scripts;
using SargeUniverse.Scripts.Controller;
using UnityEngine;
using Zenject;

namespace SargeUniverse.Common.Infrastructure
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private LevelLoader _levelLoader;
        [SerializeField] private BuildingsManager _buildingsManager = null;
        
        public override void InstallBindings()
        {
            BindFactory();
            
            BindLevelLoader();
            
            BindPlayerSyncController();
            
            BindBuildingsManager();
        }
        
        private void BindFactory() {
            Container.BindFactory<UnitView, IUnitView, UnitView.Factory>()
                .FromFactory<MainFactory<UnitView, IUnitView>>();
        }
        
        private void BindLevelLoader()
        {
            Container.Bind<LevelLoader>()
                .FromInstance(_levelLoader)
                .AsSingle()
                .NonLazy();
        }

        private void BindPlayerSyncController()
        {
            Container.Bind<PlayerSyncController>()
                .FromNewComponentOnRoot()
                .AsSingle()
                .NonLazy();
        }

        private void BindBuildingsManager()
        {
            Container.Bind<BuildingsManager>()
                .FromInstance(_buildingsManager)
                .AsSingle()
                .NonLazy();
        }
    }
}