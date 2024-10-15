using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData.StructureType;
using UIControllersAndData.GameResources;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UnityEngine;
using UnityEngine.Serialization;

public class ResourceGenerator : MonoBehaviour
{
    [HideInInspector] //this shows as empty in inspector - probably because it's a custom class?
    public EconomyBuilding[]
        basicEconomyValues; //newly created buildings will request this - to avoid saving a lot of values

    //this shows as empty in inspector - probably because it's a custom class?
    [FormerlySerializedAs("existingEconomyBuildings")]
    public List<EconomyBuilding> _existingEconomyBuildings = new();

    [FormerlySerializedAs("messageNotifications")]
    public List<MessageNotification> _messageNotifications = new();

    //[HideInInspector]
    public int index = -1;

    public GameObject BasicValues;
    private Component stats;

    private int noOfEconomyBuildings => //xml order: Forge Generator Vault Barrel Summon Tatami
        ShopData.Instance.BuildingsCategoryData.category.Select((buildingLevels) => buildingLevels.levels.Find((items) => ((IStoreBuilding)items).GetStoreType() != StoreType.None)).Where((item) => item != null).Count() + ShopData.Instance.ArmyCategoryData.category.Select((armyBuildingLevels) => armyBuildingLevels.levels.Find((items) => ((IStoreBuilding)items).GetStoreType() != StoreType.None)).Where((item) => item != null).Count();

    
    
    
    public List<EconomyBuilding> ExistingEconomyBuildings
    {
        get => _existingEconomyBuildings;
        set => _existingEconomyBuildings = value;
    }

    public List<MessageNotification> MessageNotifications
    {
        get => _messageNotifications;
        set => _messageNotifications = value;
    }

    // Use this for initialization
    private void Start()
    {
        basicEconomyValues = new EconomyBuilding[noOfEconomyBuildings];

        var tmp = ShopData.Instance.BuildingsCategoryData.category.Select((buildingLevels) => buildingLevels.levels.Find((items) => ((IStoreBuilding)items).GetStoreType() != StoreType.None)).Where((item) => item != null).Count();
        
        stats = GameObject.Find("Stats").GetComponent<Stats>();
        Initialize();
        InitializeEconomy();
    }

    private void Initialize()
    {
        for (var i = 0; i < basicEconomyValues.Length; i++)
        {
            var eB = BasicValues.AddComponent<EconomyBuilding>(); //new EconomyBuilding ();

            basicEconomyValues[i] = eB;
        }
    }

    private void RunEconomy()
    {
        RunProduction(1);
    }

    public void
        FastPaceEconomyAll(int minutesPass) //called by save/load games for already finished/functional buildings
    {
        RunProduction(60 * minutesPass);
    }

    public void InitializeEconomy()
    {
        InvokeRepeating("RunEconomy", 1, 1);
    }

    private void RunProduction(int timefactor) //timefactor seconds=1 minutes=60
    {
        for (var i = 0; i < ExistingEconomyBuildings.Count; i++)
        {
            var prodType = ExistingEconomyBuildings[i].ProdType;

            if (prodType != GameResourceType.None)
            {
                var produce = (float)ExistingEconomyBuildings[i].ProdPerHour / 3600 * timefactor;
                var displayNotification = false;

                switch (prodType)
                {
                    case GameResourceType.Gold:
                        if (ExistingEconomyBuildings[i].storedGold + produce < ExistingEconomyBuildings[i].StoreCap)
                        {
                            ExistingEconomyBuildings[i].ModifyGoldAmount(produce);
                            //if((float)existingEconomyBuildings [i].storedGold/existingEconomyBuildings [i].StoreCap>0.1f)//to display when 10% full
                            if (ExistingEconomyBuildings[i].storedGold > 1)
                                displayNotification = true;
                        }
                        else //fill storage
                        {
                            ExistingEconomyBuildings[i].ModifyGoldAmount
                            (ExistingEconomyBuildings[i].StoreCap -
                             ExistingEconomyBuildings[i].storedGold);
                            displayNotification = true;
                        }

                        if (displayNotification)
                            DisplayHarvestNotification(ExistingEconomyBuildings[i].structureIndex,
                                ExistingEconomyBuildings[i].storedGold); //i

                        break;
                    case GameResourceType.Mana:
                        if (ExistingEconomyBuildings[i].storedMana + produce < ExistingEconomyBuildings[i].StoreCap)
                        {
                            ExistingEconomyBuildings[i].ModifyManaAmount(produce);
                            //if((float)existingEconomyBuildings [i].storedMana/existingEconomyBuildings [i].StoreCap>0.1f)//to display when 10% full
                            if (ExistingEconomyBuildings[i].storedMana > 1)
                                displayNotification = true;
                        }
                        else //fill storage
                        {
                            ExistingEconomyBuildings[i].ModifyManaAmount
                            (ExistingEconomyBuildings[i].StoreCap -
                             ExistingEconomyBuildings[i].storedMana);
                            displayNotification = true;
                        }

                        if (displayNotification)
                            DisplayHarvestNotification(ExistingEconomyBuildings[i].structureIndex,
                                ExistingEconomyBuildings[i].storedMana);
                        break;
                    case GameResourceType.Power:
                        if (ExistingEconomyBuildings[i].storedPower + produce < ExistingEconomyBuildings[i].StoreCap)
                        {
                            ExistingEconomyBuildings[i].ModifyPowerAmount(produce);
                            //if((float)existingEconomyBuildings [i].storedGold/existingEconomyBuildings [i].StoreCap>0.1f)//to display when 10% full
                            if (ExistingEconomyBuildings[i].storedPower > 1)
                                displayNotification = true;
                        }
                        else //fill storage
                        {
                            ExistingEconomyBuildings[i].ModifyPowerAmount
                            (ExistingEconomyBuildings[i].StoreCap -
                             ExistingEconomyBuildings[i].storedPower);
                            displayNotification = true;
                        }

                        if (displayNotification)
                            DisplayHarvestNotification(ExistingEconomyBuildings[i].structureIndex,
                                ExistingEconomyBuildings[i].storedPower); //i

                        break;

                    case GameResourceType.Supplies:
                        if (ExistingEconomyBuildings[i].storedSupplies + produce < ExistingEconomyBuildings[i].StoreCap)
                        {
                            ExistingEconomyBuildings[i].ModifySuppliesAmount(produce);
                            //if((float)existingEconomyBuildings [i].storedGold/existingEconomyBuildings [i].StoreCap>0.1f)//to display when 10% full
                            if (ExistingEconomyBuildings[i].storedSupplies > 1)
                                displayNotification = true;
                        }
                        else //fill storage
                        {
                            ExistingEconomyBuildings[i].ModifySuppliesAmount
                            (ExistingEconomyBuildings[i].StoreCap -
                             ExistingEconomyBuildings[i].storedSupplies);
                            displayNotification = true;
                        }

                        if (displayNotification)
                            DisplayHarvestNotification(ExistingEconomyBuildings[i].structureIndex,
                                ExistingEconomyBuildings[i].storedSupplies); //i

                        break;
                }
            }
        }
    }

    public void FastPaceProductionIndividual(int buildingListIndex, int timefactor) //timefactor seconds=1 minutes=60
    {
        var prodType = ExistingEconomyBuildings[buildingListIndex].ProdType;

        if (prodType != GameResourceType.None)
        {
            var produce = (float)ExistingEconomyBuildings[buildingListIndex].ProdPerHour / 3600 * 60 * timefactor;

            switch (prodType)
            {
                case GameResourceType.Gold:
                    print("produces gold");
                    if (ExistingEconomyBuildings[buildingListIndex].storedGold + produce <=
                        ExistingEconomyBuildings[buildingListIndex].StoreCap)
                        ExistingEconomyBuildings[buildingListIndex].ModifyGoldAmount(produce);
                    else //fill storage
                        ExistingEconomyBuildings[buildingListIndex].ModifyGoldAmount
                        (ExistingEconomyBuildings[buildingListIndex].StoreCap -
                         ExistingEconomyBuildings[buildingListIndex].storedGold);
                    break;

                case GameResourceType.Mana:
                    print("produces mana");
                    if (ExistingEconomyBuildings[buildingListIndex].storedMana + produce <=
                        ExistingEconomyBuildings[buildingListIndex].StoreCap)
                        ExistingEconomyBuildings[buildingListIndex].ModifyManaAmount(produce);
                    else //fill storage
                        ExistingEconomyBuildings[buildingListIndex].ModifyManaAmount
                        (ExistingEconomyBuildings[buildingListIndex].StoreCap -
                         ExistingEconomyBuildings[buildingListIndex].storedMana);
                    break;
                case GameResourceType.Power:
                    if (ExistingEconomyBuildings[buildingListIndex].storedPower + produce <=
                        ExistingEconomyBuildings[buildingListIndex].StoreCap)
                        ExistingEconomyBuildings[buildingListIndex].ModifyPowerAmount(produce);
                    else //fill storage
                        ExistingEconomyBuildings[buildingListIndex].ModifyPowerAmount
                        (ExistingEconomyBuildings[buildingListIndex].StoreCap -
                         ExistingEconomyBuildings[buildingListIndex].storedPower);
                    break;
                case GameResourceType.Supplies:
                    if (ExistingEconomyBuildings[buildingListIndex].storedSupplies + produce <=
                        ExistingEconomyBuildings[buildingListIndex].StoreCap)
                        ExistingEconomyBuildings[buildingListIndex].ModifySuppliesAmount(produce);
                    else //fill storage
                        ExistingEconomyBuildings[buildingListIndex].ModifySuppliesAmount
                        (ExistingEconomyBuildings[buildingListIndex].StoreCap -
                         ExistingEconomyBuildings[buildingListIndex].storedSupplies);
                    break;
            }
        }
    }

    public void RegisterMessageNotification(MessageNotification m)
    {
        MessageNotifications.Add(m); //ProductionBuildings.Add (building);
    }

    private void DisplayHarvestNotification(int index, float amount)
    {
        for (var i = 0; i < MessageNotifications.Count; i++)
            if (MessageNotifications[i].structureIndex == index)
            {
                if (!MessageNotifications[i].isReady)
                {
                    MessageNotifications[i].FadeIn();
                    MessageNotifications[i].isReady = true;
                }

                MessageNotifications[i].SetLabel(0, "+ " + amount.ToString("0.00"));
                break;
            }
    }

    private void ResetHarvestNotification(int index)
    {
        for (var i = 0; i < MessageNotifications.Count; i++)
            if (MessageNotifications[i].structureIndex == index)
            {
                if (MessageNotifications[i].isReady)
                {
                    MessageNotifications[i].FadeOut();
                    MessageNotifications[i].isReady = false;
                }

                break;
            }
    }

    public void Harvest(int index)
    {
        for (var i = 0; i < ExistingEconomyBuildings.Count; i++)
            if (ExistingEconomyBuildings[i].structureIndex == index)
            {
                switch (ExistingEconomyBuildings[i].ProdType)
                {
                    case GameResourceType.Gold:
                        ((Stats)stats).AddResources((int)ExistingEconomyBuildings[i].storedGold, 0, 0, 0, 0);
                        ExistingEconomyBuildings[i].storedGold -= (int)ExistingEconomyBuildings[i].storedGold;
                        break;
                    case GameResourceType.Mana:
                        ((Stats)stats).AddResources(0, (int)ExistingEconomyBuildings[i].storedMana, 0, 0, 0);
                        ExistingEconomyBuildings[i].storedMana -= (int)ExistingEconomyBuildings[i].storedMana;
                        break;
                    case GameResourceType.Power:
                        var storedPower = (int)ExistingEconomyBuildings[i].storedPower;
                        var availablePowerStorage = PowerStorageController.Instance.FindAnyNotFulledStorage();
                        for (var j = 0; j < storedPower; j++)
                            if (availablePowerStorage && availablePowerStorage.IsStorageAvailable())
                            {
                                availablePowerStorage.ModifyPowerAmount(j);
                                ExistingEconomyBuildings[i].storedPower--;
                            }
                            else
                            {
                                availablePowerStorage = PowerStorageController.Instance.FindAnyNotFulledStorage();
                                if (availablePowerStorage && availablePowerStorage.IsStorageAvailable())
                                {
                                    availablePowerStorage.ModifyPowerAmount(j);
                                    ExistingEconomyBuildings[i].storedPower--;
                                }
                            }

                        // PowerStorageController.Instance.AddPowerToAnyAvailableStorage((int)ExistingEconomyBuildings[i]
                        //     .storedPower);
                        // ((Stats)stats).AddResources(0, 0, 0, (int)ExistingEconomyBuildings[i].storedPower);
                        // ExistingEconomyBuildings[i].storedPower -= (int)ExistingEconomyBuildings[i].storedPower;
                        break;

                    case GameResourceType.Supplies:
                        ((Stats)stats).AddResources(0, 0, 0, 0, (int)ExistingEconomyBuildings[i].storedSupplies);
                        ExistingEconomyBuildings[i].storedSupplies -= (int)ExistingEconomyBuildings[i].storedSupplies;
                        break;
                }

                ResetHarvestNotification(i);
                ((Stats)stats).UpdateUI();
                break;
            }
    }

    public EconomyBuilding GetEconomyBuilding(int index)
    {
        return ExistingEconomyBuildings[index];
    }

    public void UpdateBasicValues(int idOfStructure, ShopCategoryType categoryType, int level, StructureType strutureType)
    {
        var tmp = BasicValues.GetComponents<EconomyBuilding>().ToList();

        var economyBuilding = tmp.Find(x => x.StructureType == strutureType);

        var levels = ShopData.Instance.GetLevels(idOfStructure, categoryType);
        var structure = levels.FirstOrDefault(x => ((ILevel)x).GetLevel() == level);
        if (structure != null && economyBuilding != null)
        {
            economyBuilding.ProdType = ((IProdBuilding)structure).GetProdType();
            economyBuilding.ProdPerHour = ((IProdBuilding)structure).GetProdPerHour();
            economyBuilding.StoreType = ((IStoreBuilding)structure).GetStoreType(); //Internal, Distributed
            economyBuilding.StoreResource = ((IStoreBuilding)structure).GetStoreResource();
            economyBuilding.StoreCap = ((IStoreBuilding)structure).GetStoreCap();
        }
    }

    public void RemoveFromExisting(EconomyBuilding item)
    {
        Destroy(item);
        ExistingEconomyBuildings.Remove(item);
    }

    public void RemoveMessageNotifications(MessageNotification item)
    {
        MessageNotifications.Remove(item);
    }
}