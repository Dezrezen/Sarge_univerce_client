using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData.Models;
using CityBuildingKit.Scripts.UIControllersAndData.StructureType;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Buildings;
using UnityEngine;

public class HomeBase : MonoBehaviour
{
    [SerializeField] private StructureSelector _structureSelector;
    [SerializeField] private ShopCategoryType _categoryType  = ShopCategoryType.None;  // Building

    public static HomeBase Instance;


    private  BuildingCategoryLevels homeBaseBuildingData;
    private void Awake()
    {
        var buildingCategory = ShopData.Instance.BuildingsCategoryData;
        homeBaseBuildingData = buildingCategory.category.Find((building) => building.levels[0].GetStructureType() == StructureType.HomeBase);
        
        Instance = this;
        _structureSelector.Id = homeBaseBuildingData.levels[0].GetId();
        _structureSelector.Level = homeBaseBuildingData.levels[0].GetLevel();
        _structureSelector.CategoryType = _categoryType;
    }
    

    public bool IsCanBuildAnyBuilding()
    {
        var existedBuildings = GameObject.FindGameObjectsWithTag("Structure");
        var maxAmountOfBuildings =
            ((IHomeBase)homeBaseBuildingData.levels[_structureSelector.Level]).GetMaxBuildingAmount();
        return existedBuildings.Length < maxAmountOfBuildings;
    }
}
