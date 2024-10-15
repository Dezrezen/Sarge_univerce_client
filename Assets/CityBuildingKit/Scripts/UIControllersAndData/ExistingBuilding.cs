using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData.StructureType;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Buildings;
using UnityEngine;

public class ExistingBuilding : MonoBehaviour
{
    [SerializeField] private StructureCreator _structureCreator;
    [SerializeField] private StructureSelector _structureSelector;
    [SerializeField] private ShopCategoryType _categoryType = ShopCategoryType.None; // Building

    [SerializeField] private StructureType _structureType = StructureType.None;

    private BuildingCategoryLevels buildingData;


    private void Awake()
    {
        var buildingCategory = ShopData.Instance.BuildingsCategoryData;
        buildingData =
            buildingCategory.category.Find(
                building => building.levels[0].GetStructureType() == _structureType);
        _structureSelector.Id = buildingData.levels[0].GetId();
        _structureSelector.Level = buildingData.levels[0].GetLevel();
        _structureSelector.CategoryType = _categoryType;
        Stats.Instance.structureIndex++;
        _structureSelector.structureIndex = Stats.Instance.structureIndex;
    }

    private void Start()
    {
        if (_structureSelector && _structureCreator)
            _structureCreator.AddStructure(_structureSelector.Id, 1,
                buildingData.levels[0].MaxCountOfThisItem);
    }
}