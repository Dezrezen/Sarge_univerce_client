using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Unit;
using UnityEngine;

public class SargeLocation : MonoBehaviour, IInitializer
{
    public static SargeLocation Instance;

    [SerializeField] private List<SargeSpawnPoint> _sargeSpawnPoints;

    [SerializeField] private StructureSelector _structureSelector;

    private bool isSargeLocated = false;
    private UnitCategoryLevels unitData;
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        _structureSelector.OkEvent.AddListener(OnFinishBuilding);
        _structureSelector.FinishBuildingEvent.AddListener(OnFinishBuilding);
    }

    private void OnFinishBuilding()
    {
        if (!isSargeLocated)
        {
            PlaceSarge();    
        }
    }

    private void PlaceSarge()
    {
        if (_sargeSpawnPoints.Count > 0)
        {
            var pointToSpawnSarge = _sargeSpawnPoints.FirstOrDefault(point => point.IsPointAvailable);
            if (pointToSpawnSarge)
            {
                var unitCategory = ShopData.Instance.UnitCategoryData;
                unitData = unitCategory.category.Find((unit) => unit.levels[0].GetUnit() == GameUnit.Sarge);
                if (unitData != null)
                {
                    GameObject sargeAsset = ((IAsset)unitData.levels[0]).GetAsset();
                    if (sargeAsset)
                    {
                        GameObject sarge = Instantiate(sargeAsset, pointToSpawnSarge.SpawnPoint.transform.position, Quaternion.identity,pointToSpawnSarge.transform);
                        isSargeLocated = true;
                    }
                }
            }
        }
    }
    
    public void Initialize()
    {
        OnFinishBuilding();
        
        //TODO: maybe we'll need to add some mechasnism to re-spawn the Sarge in 
        // an available spawn point because after loading
        // of save the sarge might be under some construction
        
    }
}