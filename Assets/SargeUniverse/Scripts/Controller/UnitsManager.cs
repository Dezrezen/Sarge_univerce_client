using System.Collections.Generic;
using System.Linq;
using Controller;
using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Environment.Buildings;
using SargeUniverse.Scripts.Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SargeUniverse.Scripts.Controller
{
    public class UnitsManager : MonoBehaviour
    {
        [SerializeField] private UnitsGrid _grid = null;
        [SerializeField] private Transform _content = null;
        [SerializeField] private List<Building> _trainingCamps = new();


        public UnitsGrid grid => _grid;
        public List<UnitsGroup> ArmyUnits { get; private set; } = new();
        public List<UnitsGroup> TrainingUnits { get; private set; } = new();
        
        public static UnitsManager Instanse { get; private set; }

        public bool updateUnitsGroups = false;

        private void Awake()
        {
            Instanse ??= this;
        }

        private void OnDestroy()
        {
            Instanse = null;
        }

        public void AddTrainingCamp(Building building)
        {
            _trainingCamps.Add(building);
        }
        
        public void SyncUnits(List<Data_Unit> units)
        {
            var list = units.OrderBy(unit => unit.databaseID).ToList();
            
            foreach (var unit in list)
            {
                // Check army group
                var group = ArmyUnits.Find(g => g.UnitsId == unit.id && g.DatabseIds.Contains(unit.databaseID));
                if (group != null)
                {
                    continue;
                }

                // Check training queue
                group = TrainingUnits.Find(g => g.UnitsId == unit.id && g.DatabseIds.Contains(unit.databaseID));
                if (group != null)
                {
                    if (unit.trained == true)
                    {
                        AddToGroup(unit.id, unit.databaseID, ArmyUnits);
                        SpawnUnit(unit);
                        group.RemoveUnit(unit.databaseID);
                        if (group.GroupSize() == 0)
                        {
                            TrainingUnits.Remove(group);
                        }
                        updateUnitsGroups = true;
                    }
                    continue;
                }

                // If empty
                if (unit.trained)
                {
                    AddToGroup(unit.id, unit.databaseID, ArmyUnits);
                    SpawnUnit(unit);
                    updateUnitsGroups = true;
                }
                else
                {
                    AddToGroup(unit.id, unit.databaseID, TrainingUnits, true);
                    updateUnitsGroups = true;
                }
            }
        }

        public void CancelTrain(long databaseId)
        {
            var group = TrainingUnits.Find(g =>g.DatabseIds.Contains(databaseId));
            group.RemoveUnit(databaseId);
            if (group.GroupSize() == 0)
            {
                TrainingUnits.Remove(group);
            }
            updateUnitsGroups = true;
        }

        public void DeleteUnit(long databaseId)
        {
            var group = ArmyUnits.Find(g =>g.DatabseIds.Contains(databaseId));
            group.RemoveUnit(databaseId);
            _grid.RemoveUnit(databaseId);
            if (group.GroupSize() == 0)
            {
                ArmyUnits.Remove(group);
            }
            updateUnitsGroups = true;
        }

        private void AddToGroup(UnitID unitId, long databaseId, List<UnitsGroup> group, bool ceparated = false)
        {
            UnitsGroup gr = null;
            if (ceparated)
            {
                if (group.Count > 0)
                {
                    gr = group.Last();
                    if (gr.UnitsId == unitId)
                    {
                        gr.AddUnit(databaseId);
                        return;
                    }
                }
                
                gr = new UnitsGroup(unitId);
                gr.AddUnit(databaseId);
                group.Add(gr);
            }
            else
            {
                gr = group.Find(g => g.UnitsId == unitId);
                if (gr != null)
                {
                    gr.AddUnit(databaseId);
                }
                else
                {
                    gr = new UnitsGroup(unitId);
                    gr.AddUnit(databaseId);
                    group.Add(gr);
                }
            }
        }

        private void SpawnUnit(Data_Unit unitData)
        {
            var prefab = GameConfig.instance.UnitsConfig.GetUnitData(unitData.id).unitPrefab;
            var unit = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            var placementGrid = GetCamp();
            unit.SetUnitData(unitData);
            unit.PlaceOnGrid(placementGrid);
            _grid.AddUnit(unit);
        }

        private UnitsPlacementGrid GetCamp()
        {
            return _trainingCamps[Random.Range(0, _trainingCamps.Count)].GetComponent<UnitsPlacementGrid>();
        }
    }
}