using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UIControllersAndData.Store;
using UIControllersAndData.GameResources;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UnityEngine;

public class StructureCreator : BaseCreator
{
    private int index; //instead of xml index building 2,3 producing/storing something, we will have 0,1,2...
    //xml order: Forge Generator Vault Barrel Summon

    private void Start()
    {
        InitializeComponents(); //this is the only class who will initiate component, since there is no need to receive thee same call from all children
        ReadStructures();
        StartCoroutine("UpdateLabelStats");

        switch (structureXMLTag)
        {
            case "Building":
            {
                var buildingCategoryLevels = ShopData.Instance.BuildingsCategoryData.category;
                var buildingsCategoryData = buildingCategoryLevels.SelectMany(level => level.levels)
                    .Where(c => c.level == _structuressWithLevel).ToList();

                var militaryCategoryLevels = ShopData.Instance.ArmyCategoryData.category;
                var militaryCategoryData = militaryCategoryLevels.SelectMany(level => level.levels)
                    .Where(c => c.level == _structuressWithLevel).ToList();

                totalStructures = buildingsCategoryData.Count + militaryCategoryData.Count;
                structurePf = new GameObject[totalStructures];
                constructionTypes = new int[totalStructures];
                grassTypes = new int[totalStructures];
                pivotCorrections = new int[totalStructures];

                for (var i = 0; i < buildingsCategoryData.Count; i++)
                {
                    structurePf[i] = buildingsCategoryData[i].asset;
                    constructionTypes[i] = buildingsCategoryData[i].constructionType;
                    grassTypes[i] = buildingsCategoryData[i].grassType;
                    pivotCorrections[i] = buildingsCategoryData[i].pivotCorrection;
                }

                var j = 0;
                for (var i = buildingsCategoryData.Count; i < totalStructures; i++)
                {
                    structurePf[i] = militaryCategoryData[j].asset;
                    constructionTypes[i] = militaryCategoryData[j].constructionType;
                    grassTypes[i] = militaryCategoryData[j].grassType;
                    pivotCorrections[i] = militaryCategoryData[j].pivotCorrection;
                    j++;
                }

                RegisterBasicEconomyValues(buildingsCategoryData);
                RegisterBasicEconomyValues(militaryCategoryData);
                break;
            }

            case "Wall":
            {
                var wallsCategoryLevels = ShopData.Instance.WallsCategoryData.category;
                var wallsCategoryData = wallsCategoryLevels.SelectMany(level => level.levels)
                    .Where(c => c.level == _structuressWithLevel).ToList();

                totalStructures = wallsCategoryData.Count;
                structurePf = new GameObject[totalStructures];
                constructionTypes = new int[totalStructures];
                grassTypes = new int[totalStructures];
                pivotCorrections = new int[totalStructures];


                for (var i = 0; i < wallsCategoryData.Count; i++)
                {
                    structurePf[i] = wallsCategoryData[i].asset;
                    constructionTypes[i] = wallsCategoryData[i].constructionType;
                    grassTypes[i] = wallsCategoryData[i].grassType;
                    pivotCorrections[i] = wallsCategoryData[i].pivotCorrection;
                }

                break;
            }

            case "Weapon":
            {
                var weaponCategoryLevels = ShopData.Instance.WeaponCategoryData.category;
                var weaponCategoryData = weaponCategoryLevels.SelectMany(level => level.levels)
                    .Where(c => c.level == _structuressWithLevel).ToList();

                totalStructures = weaponCategoryData.Count;
                structurePf = new GameObject[totalStructures];
                constructionTypes = new int[totalStructures];
                grassTypes = new int[totalStructures];
                pivotCorrections = new int[totalStructures];


                for (var i = 0; i < weaponCategoryData.Count; i++)
                {
                    structurePf[i] = weaponCategoryData[i].asset;
                    constructionTypes[i] = weaponCategoryData[i].constructionType;
                    grassTypes[i] = weaponCategoryData[i].grassType;
                    pivotCorrections[i] = weaponCategoryData[i].pivotCorrection;
                }

                break;
            }

            case "Ambient":
            {
                var ambientCategoryLevels = ShopData.Instance.AmbientCategoryData.category;
                var ambientCategoryData = ambientCategoryLevels.SelectMany(level => level.levels)
                    .Where(c => c.level == _structuressWithLevel).ToList();

                totalStructures = ambientCategoryData.Count;
                structurePf = new GameObject[totalStructures];
                constructionTypes = new int[totalStructures];
                grassTypes = new int[totalStructures];
                pivotCorrections = new int[totalStructures];

                for (var i = 0; i < ambientCategoryData.Count; i++)
                {
                    structurePf[i] = ambientCategoryData[i].asset;
                    constructionTypes[i] = ambientCategoryData[i].constructionType;
                    grassTypes[i] = ambientCategoryData[i].grassType;
                    pivotCorrections[i] = ambientCategoryData[i].pivotCorrection;
                }

                break;
            }
        }
    }

    private void RegisterBasicEconomyValues<T>(List<T> data) where T : IProdBuilding, IStoreBuilding, IStructure
    {
        for (var i = 0; i < data.Count; i++)
        {
            var isvalid = false;

            if (data[i].GetProdType() != GameResourceType.None)
            {
                ((ResourceGenerator)resourceGenerator).basicEconomyValues[index].ProdType = data[i].GetProdType();
                ((ResourceGenerator)resourceGenerator).basicEconomyValues[index].ProdPerHour = data[i].GetProdPerHour();
                isvalid = true;
            }

            if (data[i].GetStoreType() != StoreType.None)
                // if (((ResourceGenerator)resourceGenerator).basicEconomyValues.Length < index)
            {
                ((ResourceGenerator)resourceGenerator).basicEconomyValues[index].StoreType =
                    data[i].GetStoreType(); //Internal, Distributed
                ((ResourceGenerator)resourceGenerator).basicEconomyValues[index].StoreResource =
                    data[i].GetStoreResource();
                ((ResourceGenerator)resourceGenerator).basicEconomyValues[index].StoreCap = data[i].GetStoreCap();
                isvalid = true;
            }

            if (isvalid)
            {
                ((ResourceGenerator)resourceGenerator).basicEconomyValues[index].StructureType =
                    data[i].GetStructureType();
                index++;
            }
        }
    }

    public void AddStructure(int currentSelection, int value, int maxCount)
    {
        AddToExistingStructure(currentSelection, value, maxCount);
    }
}