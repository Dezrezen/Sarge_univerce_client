using SargeUniverse.Common.Controller;
using SargeUniverse.Common.View;
using SargeUniverse.Scripts;
using UnityEngine;
using Zenject;

namespace SargeUniverse.Common.Infrastructure
{
    public class BattleSceneInstaller : MonoInstaller
    {
        [SerializeField] private LevelLoader _levelLoader;
        
        public override void InstallBindings()
        {
            BindFactory();

            BindLevelLoader();
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
    }
}