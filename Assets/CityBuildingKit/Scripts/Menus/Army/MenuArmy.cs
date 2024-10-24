﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Menus;
using UIControllersAndData.Store;
using UIControllersAndData.Units;
using UnityEngine;

public class MenuArmy : MonoBehaviour
{
    //the panel with all the units, also used for selecting the army before attack

    public static MenuArmy Instance;

    public StructureCreator buildingCreator;

    private int buildingsNo; //correlate with MenuUnitBase.cs

    private Component removableCreator, transData, saveLoadMap, soundFX;

    public List<ExistedUnit> ExistingBattleUnits { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        buildingsNo = GameUnitsSettings_Handler.s.buildingTypesNo;

        removableCreator = GameObject.Find("RemovableCreator").GetComponent<RemovableCreator>();

        transData = GameObject.Find("TransData").GetComponent<TransData>();
        saveLoadMap = GameObject.Find("SaveLoadMap").GetComponent<SaveLoadMap>();
        soundFX = GameObject.Find("SoundFX").GetComponent<SoundFX>();
    }

    public void LoadCampaign(int campaignLevel)
    {
        ((TransData)transData).campaignLevel = campaignLevel;
        LoadMultiplayer0();
    }

    public void LoadMultiplayer0()
    {
        var unitsExist = false;

        for (var i = 0; i < Stats.Instance.ExistingUnits.Count; i++)
            if (Stats.Instance.ExistingUnits[i].count > 0)
            {
                unitsExist = true;
                break;
            }

        if (Application.internetReachability == NetworkReachability.NotReachable)
            MessageController.Instance.DisplayMessage("Can't download map.\nNo internet connection.");
        else if (unitsExist && ((TransData)transData).campaignLevel != -1)
            StartCoroutine(LoadMultiplayerMap(0));
        else if (!unitsExist)
            MessageController.Instance.DisplayMessage("Train units for battle.");
        else if (Stats.Instance.gold >= 250 && ((TransData)transData).campaignLevel == -1)
            StartCoroutine(LoadMultiplayerMap(0));
        else
            MessageController.Instance.DisplayMessage("You need more gold.");
    }

    private IEnumerator LoadMultiplayerMap(int levelToLoad) //building loot values = half the price
    {
        if (((TransData)transData).campaignLevel == -1)
            Stats.Instance.gold -= 250; //this is where the price for the battle is payed, before saving the game


        ExistingBattleUnits = Enumerable.Range(1, ShopData.Instance.UnitCategoryData.category.Count)
            .Select(i => new ExistedUnit()).ToList(); // by Fernando 05/26/2020

        foreach (var existedUnit in Stats.Instance.ExistingUnits)
        {
            ExistingBattleUnits[existedUnit.id].id = existedUnit.id;
            ExistingBattleUnits[existedUnit.id].count = existedUnit.count;
            ExistingBattleUnits[existedUnit.id].level = existedUnit.level;
        }

        for (var i = 0; i < ExistingBattleUnits.Count; i++)
        {
            Stats.Instance.occupiedHousing -= Stats.Instance.sizePerUnit[i] * ExistingBattleUnits[i].count;
            Stats.Instance.ExistingUnits.RemoveAll(unit => unit.id == ExistingBattleUnits[i].id);
        }

        Stats.Instance.UpdateUI(); // - optional- no element of the UI is visible at this time
        Stats.Instance.UpdateUnitsNo();

#if !UNITY_WEBPLAYER
        ((SaveLoadMap)saveLoadMap).SaveGameLocalFile(); //local autosave at battle load
#endif

#if UNITY_WEBPLAYER
		((SaveLoadMap)saveLoadMap).SaveGamePlayerPrefs ();
#endif

        ((TransData)transData).removeTimes = ((RemovableCreator)removableCreator).removeTimes;
        ((TransData)transData).housingPerUnit = Stats.Instance.sizePerUnit;

        ((TransData)transData).GoingToBattleUnits = ExistingBattleUnits;

        ((TransData)transData).tutorialBattleSeen = Stats.Instance.tutorialBattleSeen;

        ((TransData)transData).soundOn = ((SoundFX)soundFX).soundOn;
        ((TransData)transData).ambientOn = ((SoundFX)soundFX).ambientOn;
        ((TransData)transData).musicOn = ((SoundFX)soundFX).musicOn;

        for (var i = 0; i < buildingsNo; i++)
        {
            // ((TransData)transData).buildingValues [i] = int.Parse(buildingCreator.structures [i] ["Price"]);
            // ((TransData)transData).buildingCurrency [i] = buildingCreator.structures [i] ["Currency"];
        }

        yield return new WaitForSeconds(0.2f);
        switch (levelToLoad)
        {
            case 0:
                Application.LoadLevel("Map01");
                break;
        }
    }


    //TODO: REMOVE THIS TEST METHOD
    public void Load0MapTest()
    {
        // var unit = new ExistedUnit();
        // unit.id = 3;
        // unit.level = 1;
        // unit.count = 1;
        // Stats.Instance.ExistingUnits.Add(unit);

        var unit2 = new ExistedUnit();
        unit2.id = 2;
        unit2.level = 1;
        unit2.count = 2;
        Stats.Instance.ExistingUnits.Add(unit2);


        StartCoroutine(LoadMultiplayerMap(0));
    }
}