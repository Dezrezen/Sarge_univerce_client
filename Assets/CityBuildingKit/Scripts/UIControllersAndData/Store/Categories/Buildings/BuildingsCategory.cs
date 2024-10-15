/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData.Models;
using CityBuildingKit.Scripts.UIControllersAndData.StructureType;
using UIControllersAndData.GameResources;
using UIControllersAndData.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIControllersAndData.Store.Categories.Buildings
{
    [Serializable]
    public class BuildingsCategory : BaseStoreItemData, INamed, IId, ILevel, IRequiredHQLevel, IAsset, IProdBuilding,
        IStoreBuilding, IStructure, IDamagePoints, IFireRate, IHomeBase, IMovable, IBuilder, IGrassType, IEntityType, IHP
    {
        [FormerlySerializedAs("Name")] public string name;
        [FormerlySerializedAs("Id")] public int id;
        [FormerlySerializedAs("Level")] public int level;
        public int requiredHQLevelForUpgrade;
        [FormerlySerializedAs("Asset")] public GameObject asset;

        [FormerlySerializedAs("StructureType")]
        public StructureType structureType; //same as prefab //TODO: I think it should be enum 

        [FormerlySerializedAs("ProdType")] public GameResourceType prodType;
        public bool isProductionBuilding;
        [FormerlySerializedAs("ProdPerHour")] public int prodPerHour;
        [FormerlySerializedAs("StoreType")] public StoreType storeType;

        [FormerlySerializedAs("StoreResource")]
        public GameResourceType storeResource;

        [FormerlySerializedAs("StoreCap")] public int storeCap;
        [FormerlySerializedAs("DamagePoints")] public int damagePoints;

        [FormerlySerializedAs("attackSpeed")] [FormerlySerializedAs("AttackSpeed")]
        public float fireRate;

        [FormerlySerializedAs("GrassType")] public int grassType;

        [FormerlySerializedAs("PivotCorrection")]
        public int pivotCorrection;

        [FormerlySerializedAs("ConstructionType")]
        public int constructionType;


        public bool isMovable;

        public int maxBuildingAmount;

        public int maxNumberTrapsAvailable;

        public int builders;
        public EntityType entityType;

        public GameObject GetAsset()
        {
            return asset;
        }

        public int GetBuilder()
        {
            return builders;
        }

        public int GetDamagePoints()
        {
            return damagePoints;
        }

        public float GetFireRate()
        {
            return fireRate;
        }

        public int GetMaxBuildingAmount()
        {
            return maxBuildingAmount;
        }

        public int GetMaxNumberTrapsAvailable()
        {
            return maxNumberTrapsAvailable;
        }

        public int GetId()
        {
            return id;
        }

        public int GetLevel()
        {
            return level;
        }

        public bool IsMovable()
        {
            return isMovable;
        }

        public string GetName()
        {
            return name;
        }

        public GameResourceType GetProdType()
        {
            return prodType;
        }

        public int GetProdPerHour()
        {
            return prodPerHour;
        }

        public bool IsProductionBuilding()
        {
            return isProductionBuilding;
        }

        public int GetRequiredHQLevel()
        {
            return requiredHQLevelForUpgrade;
        }

        public int GetStoreCap()
        {
            return storeCap;
        }

        public StoreType GetStoreType()
        {
            return storeType;
        }

        public GameResourceType GetStoreResource()
        {
            return storeResource;
        }

        public StructureType GetStructureType()
        {
            return structureType;
        }

        public int GetGrassType()
        {
            return grassType;
        }

        public EntityType GetEntityType()
        {
            return entityType;
        }

        public int GetHP()
        {
            return HP;
        }
    }
}