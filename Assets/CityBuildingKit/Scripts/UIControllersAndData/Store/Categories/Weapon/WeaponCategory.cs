/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData.Models;
using CityBuildingKit.Scripts.UIControllersAndData.StructureType;
using UIControllersAndData.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIControllersAndData.Store.Categories.Weapon
{
    [Serializable]
    public class WeaponCategory : BaseStoreItemData, INamed, IId, ILevel, IAsset, IDamagePoints, IFireRate, IDamageType,
        IStructure, IRequiredHQLevel, IGrassType, IRange, ITimeToExplosion, IAmountOfSlots, ITargetType, IPreferredTarget
    {
        [FormerlySerializedAs("Name")] public string name;
        [FormerlySerializedAs("Id")] public int id;
        public int level;
        public GameObject asset;
        public int damagePoints;
        [FormerlySerializedAs("Range")] public int range;
        [FormerlySerializedAs("FireRate")] public float fireRate;
        [FormerlySerializedAs("DamageType")] public DamageType damageType;
        [FormerlySerializedAs("TargetType")] public TargetType targetType;

        [FormerlySerializedAs("preferredTarget")] [FormerlySerializedAs("PreferredTarget")]
        public EntityType entityType;

        [FormerlySerializedAs("DamageBonus")] public int damageBonus;
        [FormerlySerializedAs("GrassType")] public int grassType;

        [FormerlySerializedAs("PivotCorrection")]
        public int pivotCorrection;

        [FormerlySerializedAs("ConstructionType")]
        public int constructionType;

        public StructureType structureType;
        public int requiredHQLevel;
        public float _timeToExplosion;
        public int _amountOfSlots;


        public int GetAmountOfSlots()
        {
            return _amountOfSlots;
        }

        public GameObject GetAsset()
        {
            return asset;
        }

        public int GetDamagePoints()
        {
            return damagePoints;
        }

        public DamageType GetDamageType()
        {
            return damageType;
        }

        public float GetFireRate()
        {
            return fireRate;
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

        public int GetRange()
        {
            return range;
        }

        public int GetRequiredHQLevel()
        {
            return requiredHQLevel;
        }

        public StructureType GetStructureType()
        {
            return structureType;
        }

        public float GetTimeToExplosion()
        {
            return _timeToExplosion;
        }

        public TargetType GetTargetType()
        {
            return targetType;
        }

        public EntityType GetPreferredTarget()
        {
            return entityType;
        }
    }
}