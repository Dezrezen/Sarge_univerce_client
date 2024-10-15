using System;
using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData;
using UIControllersAndData.Store;
using UnityEngine;

public class TrainingCamp : MonoBehaviour, IInitializer
{
    [SerializeField] private StructureSelector _structureSelector;
    public string UniqGuid { get; set; }
    public BaseStoreItemData Data { get; set; }

    private void Awake()
    {
        UniqGuid = Guid.NewGuid().ToString();
    }

    public void Initialize()
    {
        Data = ShopData.Instance.GetStructureData(_structureSelector.Id, ShopCategoryType.Army,
            _structureSelector.Level);

        if (Data != null) TrainingCampController.Instance.AddToList(this);
    }

    //TODO: FINISH the slots and other things when the UI mockups will be ready
}