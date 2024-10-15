using SargeUniverse.Scripts.Map;
using SargeUniverse.Scripts.Map.Buildings;
using SargeUniverse.Scripts.UI;
using UnityEngine;

namespace SargeUniverse.Scripts.Controller
{
    public class WarMapManager : MonoBehaviour
    {
        private const int MaxBases = 10;
        private const int HighTierBases = 3;
        private const int MidTierBases = 3;

        [SerializeField] private WarMapGrid _grid = null;
        [SerializeField] private Transform _buildingsContent = null;
        [SerializeField] private WarBase _htBasePrefab = null;
        [SerializeField] private WarBase _mtBasePrefab = null;
        [SerializeField] private WarBase _ltBasePrefab = null;

        [Header("Game Canvas Elements")]
        [SerializeField] private Transform _content = null;
        [SerializeField] private UIWarBaseLabel _baseNamePrefab = null;
        
        public WarMapGrid Grid => _grid;

        public WarBase selectedWarBase = null;
        
        public static WarMapManager Instanse { get; private set; }
        

        private void Awake()
        {
            Instanse ??= this;
        }

        private void Start()
        {
            InstantiateBases();
        }

        private void OnDestroy()
        {
            Instanse = null;
        }

        private void InstantiateBases()
        {
            for (var i = 0; i < MaxBases; i++)
            {
                var x = 4 + (i % 2) * 2;
                var y = 17 - i + (i % 2);
                
                switch (i)
                {
                    case < HighTierBases:
                        InstantiateBase(_htBasePrefab, y-x, y, i+1);
                        InstantiateBase(_htBasePrefab, y, y-x, i+1);
                        break;
                    case < HighTierBases + MidTierBases:
                        InstantiateBase(_mtBasePrefab, y-x, y, i+1);
                        InstantiateBase(_mtBasePrefab, y, y-x, i+1);
                        break;
                    default:
                        InstantiateBase(_ltBasePrefab, y-x, y, i+1);
                        InstantiateBase(_ltBasePrefab, y, y-x, i+1);
                        break;
                }
            }
        }

        private void InstantiateBase(WarBase prefab, int x, int y, int index)
        {
            var warBase = Instantiate(prefab, Vector3.zero, Quaternion.identity, _buildingsContent);
            warBase.PlaceOnGrid(x, y);

            var warBaseName = Instantiate(_baseNamePrefab, Vector3.zero, Quaternion.identity, _content);
            warBase.SetBaseName(warBaseName, index + ". Player_" + index);
            
            _grid.AddWarBase(warBase);
            
            
        }
    }
}