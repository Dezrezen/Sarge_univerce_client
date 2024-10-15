using System;
using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Buildings;
using UnityEngine;

public class SupplyDrop : MonoBehaviour, IInitializer
{
    [SerializeField] private StructureSelector _structureSelector;


    [SerializeField] private bool _isInitalizeAtStart;

    private BuildingCategoryLevels supplyDropBuildingData;

    public int MaxCapacity { get; set; }
    public string UniqGuid { get; set; }
    public float CurrentAmountOfSupply { get; set; }
    public BaseStoreItemData Data { get; set; }

    private void Start()
    {
        UniqGuid = Guid.NewGuid().ToString();

        if (_isInitalizeAtStart) Initialize();
    }

    public void Initialize()
    {
        Data = ShopData.Instance.GetStructureData(_structureSelector.Id, ShopCategoryType.Buildings,
            _structureSelector.Level);
        if (Data != null)
        {
            SupplyDropController.Instance.AddToList(this);
            MaxCapacity = ((IStoreBuilding)Data).GetStoreCap();
        }
    }

    public void ModifySupplyAmount(float value)
    {
        CurrentAmountOfSupply++;
    }

    public bool IsSupplyStorageAvailable()
    {
        return CurrentAmountOfSupply < MaxCapacity;
    }
}