using System;
using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UnityEditor;
using UnityEngine;

public class PowerStorage : MonoBehaviour, IInitializer
{
    [SerializeField] private StructureSelector _structureSelector;

    public int MaxCapacity { get; set; }
    public string UniqGuid { get; set; }
    public float CurrentAmountOfPower { get; set; }
    public BaseStoreItemData Data { get; set; }

    private void Awake()
    {
        UniqGuid = Guid.NewGuid().ToString();
    }

    public void Initialize()
    {
        Data = ShopData.Instance.GetStructureData(_structureSelector.Id, ShopCategoryType.Buildings,
            _structureSelector.Level);
        if (Data != null)
        {
            PowerStorageController.Instance.AddToList(this);
            MaxCapacity = ((IStoreBuilding)Data).GetStoreCap();
        }
    }

    public void ModifyPowerAmount(float value)
    {
        CurrentAmountOfPower++;
    }

    public bool IsStorageAvailable()
    {
        return CurrentAmountOfPower < MaxCapacity;
    }
}