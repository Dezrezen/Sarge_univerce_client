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

namespace UIControllersAndData.Store.Categories.Military
{
    [Serializable]
    public class ArmyCategory : BaseStoreItemData, INamed, IId, ILevel, IAsset, IProdBuilding, IStoreBuilding,
        IStructure, ITotalUnitCapacity, IRequiredHQLevel, IGrassType, IGameResourceToTrainUnit, IMovable, IEntityType
    {
        public string name;
        public int id;
        public int level;
        [FormerlySerializedAs("Asset")] public GameObject asset;
        [FormerlySerializedAs("StoreCap")] public int storeCap;
        [FormerlySerializedAs("StoreType")] public StoreType storeType;

        [FormerlySerializedAs("StoreResource")]
        public GameResourceType storeResource;

        [FormerlySerializedAs("ProdType")] public GameResourceType prodType;
        [FormerlySerializedAs("ProdPerHour")] public int prodPerHour;

        [FormerlySerializedAs("StructureType")]
        public StructureType structureType;

        [FormerlySerializedAs("GrassType")] public int grassType;

        [FormerlySerializedAs("PivotCorrection")]
        public int pivotCorrection;

        [FormerlySerializedAs("ConstructionType")]
        public int constructionType;

        public int requiredHQLevel;

        public int totalUnitCapacity;

        public GameResourceType gameResourceToTrainUnit;

        public bool isMovable;
        
        public EntityType entityType;
        
        public GameObject GetAsset()
        {
            return asset;
        }

        public GameResourceType GetGameResourceToTrainUnit()
        {
            return gameResourceToTrainUnit;
        }

        public int GetGrassType()
        {
            return grassType;
        }

        public int GetId()
        {
            return id;
        }

        public int GetLevel()
        {
            return level;
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
            return false;
        }

        public int GetRequiredHQLevel()
        {
            return requiredHQLevel;
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

        public int GetTotalUnitCapacity()
        {
            return totalUnitCapacity;
        }

        public bool IsMovable()
        {
            return isMovable;
        }

        public EntityType GetEntityType()
        {
            return entityType;
        }
    }
}