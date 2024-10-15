/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData.Models;
using UIControllersAndData.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIControllersAndData.Store.Categories.Unit
{
    [Serializable]
    public class UnitCategory : BaseStoreItemData, INamed, IId, ILevel, IAsset, IDamagePoints, IFireRate, IDamageType,
        IUnit, IAmountOfSlots, ITargetType, IRange, IPreferredTarget, IUnitType, IMovementSpeed,
        IDurationOfSargeSpecialAbility, IRequiredBarracksLevel, IEntityType
    {
        public string name;
        public int id;
        public int level;
        [FormerlySerializedAs("Size")] public int size;
        public int priceToSpeedUpUpgrade;
        public GameObject asset;
        public int damagePoints;
        [FormerlySerializedAs("attackSpeed")] public float fireRate;
        public DamageType damageType;
        public int _amountOfSlotsInTrap;
        public GameUnit unit;
        public TargetType targetType;
        public TargetType type;
        public int rangeAttack;

        public EntityType preferredTarget;

        public int movementSpeed;
        public float durationOfSargeSpecialAbility;
        public int requiredBarracksLevel;
        public EntityType entityType;

        public int GetAmountOfSlots()
        {
            return _amountOfSlotsInTrap;
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

        public float GetDurationOfSargeSpecialAbility()
        {
            return durationOfSargeSpecialAbility;
        }

        public EntityType GetEntityType()
        {
            return entityType;
        }

        public float GetFireRate()
        {
            return fireRate;
        }

        public int GetId()
        {
            return id;
        }

        public int GetLevel()
        {
            return level;
        }

        public int GetMovementSpeed()
        {
            return movementSpeed;
        }

        public string GetName()
        {
            return name;
        }

        public EntityType GetPreferredTarget()
        {
            return preferredTarget;
        }

        public int GetRange()
        {
            return rangeAttack;
        }

        public int GetRequiredBarracksLevel()
        {
            return requiredBarracksLevel;
        }

        public TargetType GetTargetType()
        {
            return targetType;
        }

        public GameUnit GetUnit()
        {
            return unit;
        }

        public TargetType GetUnitType()
        {
            return type;
        }
    }
}