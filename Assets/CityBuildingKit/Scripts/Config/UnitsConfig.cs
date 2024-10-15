using System.Collections.Generic;
using System.Linq;
using Config.Units;
using SargeUniverse.Scripts.Enums;
using UnityEngine;
using Zenject;

namespace Config
{
    [CreateAssetMenu(fileName = "UnitsConfig", menuName = "Data/UnitsConfig")]
    public class UnitsConfig : ScriptableObjectInstaller
    {
        public List<UnitData> unitsData = new();

        public UnitData GetUnitData(UnitID id)
        {
            return unitsData.First(unit => unit.id == id);
        }
        
        public override void InstallBindings()
        {
            Container.Bind<UnitsConfig>()
                .FromInstance(this)
                .AsSingle();
        }
    }
}