using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Menus;
using Assets.Scripts.UIControllersAndData;
using Assets.Scripts.UIControllersAndData.Store;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Unit;
using UIControllersAndData.Units;
using UnityEngine;

//This script is active while the units menu is enabled on screen, then, the relevant info is passed to the unitProc

public class MenuUnit : MenuUnitBase
{
    public static MenuUnit Instance;
    public GameObject UnitProcObj; //target game obj for unit construction progress processor; disabled at start	

    public UnitProc unitProc;

    private string _time;


    private int
        OffScreenY = 500, //Y positions ofscreen
        OnScreenY = 230; //action 0 cancel 1 finished 2 exitmenu

    private int priceInCrystals;

    private bool resetLabels; //the finish now upper right labels 

    private float z = -1f;

    public List<UnitInfo> UnitsInfo { get; set; } = new();


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        trainingTimes = new int[GameUnitsSettings_Handler.s.unitsNo];
        sizePerUnit = new int[GameUnitsSettings_Handler.s.unitsNo];
        trainingIndexes = new int[GameUnitsSettings_Handler.s.unitsNo];

        UpdateData();
    }

    private void FixedUpdate()
    {
        if (pause)
            return;
        if (queCounter > 0)
        {
            ProgressBars(); //fix this - progress bars resets currentSlidVal at reload
        }
        else if (resetLabels)
        {
            _time = "-";
            ShopControllerOLD.Intance.UpdateUnitStatusData("-", "-");

            currentSlidVal = 0;
            progCounter = 0;
            resetLabels = false;
        }
    }


    private void UpdateData()
    {
        var unitCategoryLevels = ShopData.Instance.UnitCategoryData.category;
        var unitCategoryData = unitCategoryLevels.SelectMany(level => level.levels).Where(c => c.level == 1).ToList();

        for (var i = 0; i < unitCategoryData.Count; i++)
        {
            trainingTimes[i] = unitCategoryData[i].TimeToBuild;
            sizePerUnit[i] = unitCategoryData[i].size;

            //in case user exits before passing the info to unit proc - MenuUnit is open
            unitProc.trainingTimes[i] = trainingTimes[i];
            unitProc.sizePerUnit[i] = sizePerUnit[i];
        }
    }

    public void BuyStoreItem(UnitCategory itemData, Action callback)
    {
        if (itemData == null) return;

        rebuild = false;
        var canBuild = true;

        if (itemData.Currency == CurrencyType.Gold)
        {
            if (!Stats.Instance.EnoughGold(itemData.Price))
            {
                canBuild = false;
                MessageController.Instance.DisplayMessage("Insufficient gold.");
            }
        }
        else if (itemData.Currency == CurrencyType.Mana)
        {
            if (!Stats.Instance.EnoughMana(itemData.Price))
            {
                canBuild = false;
                MessageController.Instance.DisplayMessage("Insufficient mana.");
            }
        }
        else
        {
            if (!Stats.Instance.EnoughCrystals(itemData.Price))
            {
                canBuild = false;
                MessageController.Instance.DisplayMessage("Insufficient crystals.");
            }
        }

        if (trainingIndexes[itemData.GetId()] == GameUnitsSettings_Handler.s.maxUnitsNo)
        {
            canBuild = false;
            MessageController.Instance.DisplayMessage(GameUnitsSettings_Handler.s.maxUnitsNo + " units limit.");
        }

        if (Stats.Instance.occupiedHousing + itemData.size > Stats.Instance.maxHousing)
        {
            canBuild = false;
            MessageController.Instance.DisplayMessage("Increase your soldier housing capacity.");
        }

        if (canBuild)
        {
            if (itemData.Currency == CurrencyType.Gold)
            {
                Pay(itemData.Price, 0, 0, 0, 0);
            }
            else if (itemData.Currency == CurrencyType.Mana)
            {
                Pay(0, itemData.Price, 0, 0, 0);
            }
            else if (itemData.Currency == CurrencyType.Power)
            {
                Pay(0, 0, 0, itemData.Price, 0);
            }
            else if (itemData.Currency == CurrencyType.Supplies)
            {
                Pay(0, 0, 0, 0, itemData.Price);
            }
            else
            {
                Stats.Instance.crystals -= itemData.Price;
                Pay(0, 0, itemData.Price, 0, 0);
            }

            Stats.Instance.experience += itemData.XpAward;
            if (Stats.Instance.experience > Stats.Instance.maxExperience)
                Stats.Instance.experience = Stats.Instance.maxExperience;

            Stats.Instance.occupiedHousing += itemData.size;

            Stats.Instance.UpdateUI();
            callback();
            Build(itemData.GetId());

            //AddUnitInfo(itemData.id, itemData.level);
        }
    }


    private void Pay(int gold, int mana, int crystals, int power, int supplies)
    {
        Stats.Instance.SubstractResources(gold, mana, crystals, power, supplies);
    }

    private void Refund(int gold, int mana, int crystals, int power, int supplies)
    {
        Stats.Instance.AddResources(gold, mana, crystals, power, supplies);
    }

    public void PassValuestoProc()
    {
        pause = true;
        unitProc.Pause();

        var queEmpty = true; //verify if there's anything under constuction

        for (var i = 0; i < trainingIndexes.Length; i++)
            if (trainingIndexes[i] > 0)
            {
                queEmpty = false;
                break;
            }

        if (!queEmpty)
        {
            unitProc.currentSlidVal = currentSlidVal;
            unitProc.currentTrainingTime = currentTrainingTime;
            unitProc.queList.Clear(); //clear queIndex/trainingIndex/objIndex dictionary

            for (var i = 0; i < trainingIndexes.Length; i++)
                if (trainingIndexes[i] > 0)
                {
                    var index = ShopControllerOLD.Intance.ListOfUnitStatusItem.FindIndex(x => x.ItemData.GetId() == i);

                    unitProc.queList.Add(new Vector4(
                        ShopControllerOLD.Intance.ListOfUnitStatusItem[index].QIndex.Qindex,
                        ShopControllerOLD.Intance.ListOfUnitStatusItem[index].QIndex.Objindex,
                        trainingIndexes[i],
                        ShopControllerOLD.Intance.ListOfUnitStatusItem[index].Level));
                }

            unitProc.trainingTimes = trainingTimes;
            unitProc.SortList();
            EraseValues();
        }

        unitProc.sizePerUnit = sizePerUnit; //pass the weights regardless
        Stats.Instance.sizePerUnit = sizePerUnit;

        unitProc.Resume();
    }

    private void EraseValues()
    {
        for (var i = 0; i < trainingIndexes.Length; i++)
            if (trainingIndexes[i] > 0)
            {
                var a = trainingIndexes[
                    i]; //while unbuilding, trainingIndexes[i] is modified - no longer valid references
                for (var j = 0; j < a; j++) UnBuild(i, 2);
            }

        currentSlidVal = 0;
        timeRemaining = 0;
        currentTimeRemaining = 0;
        hours = minutes = seconds = 0; //?totalTime
        queList.Clear();
        ShopControllerOLD.Intance.UpdateHitText("Tap on a unit to summon them and read the description.");
    }

    public void LoadValuesfromProc()
    {
        unitProc.Pause();

        pause = true;

        var queEmpty = true;

        if (unitProc.queList.Count > 0) queEmpty = false; //unit proc is disabled at start???

        if (!queEmpty)
        {
            currentSlidVal = unitProc.currentSlidVal;
            currentTrainingTime = unitProc.currentTrainingTime;

            queList.Clear();

            for (var i = 0; i < unitProc.queList.Count; i++) queList.Add(unitProc.queList[i]);

            unitProc.queList.Clear(); //reset remote list
            ReBuild();
        }

        pause = false;
    }

    private void ReBuild()
    {
        rebuild = true;

        queList.Sort(delegate(Vector4 v1, Vector4 v2) // qIndex, objIndex, trainingIndex
        {
            return v1.x.CompareTo(v2.x);
        });

        for (var i = 0; i < queList.Count; i++) // qIndex, objIndex, trainingIndex
        for (var j = 0; j < queList[i].z; j++)
        {
            ShopControllerOLD.Intance.AddStatusUnitFromSave((int)queList[i].y);
            Build((int)queList[i].y);
        }

        progCounter = 0; //delay first bar update 
        var index = ShopControllerOLD.Intance.ListOfUnitStatusItem.FindIndex(x =>
            x.ItemData.GetId() == (int)queList[0].y);
        ShopControllerOLD.Intance.ListOfUnitStatusItem[index].Slider.value = currentSlidVal;

        UnitProcObj.SetActive(false);
        UpdateTime();
    }

    private void Build(int id)
    {
        var levels = ShopData.Instance.GetLevels(id, ShopCategoryType.Unit);

        var unit = levels?.FirstOrDefault(x => ((ILevel)x).GetLevel() == 1);
        if (unit == null) throw new Exception("Unity is null");

        var i = ShopControllerOLD.Intance.ListOfUnitStatusItem.FindIndex(x => x.ItemData.GetId() == id);
        resetLabels = true;
        var iInQue = ShopControllerOLD.Intance.ListOfUnitStatusItem[i].QIndex.inque;

        if (iInQue)
        {
            trainingIndexes[id]++;
            ShopControllerOLD.Intance.UpdateUnitsCountAndProgess(id, trainingIndexes[id]);
            ShopControllerOLD.Intance.UpdateHitText(unit.Description);
        }

        else if (!iInQue)
        {
            trainingIndexes[id]++;
            ShopControllerOLD.Intance.ListOfUnitStatusItem[i].QIndex.inque = true;
            ShopControllerOLD.Intance.ListOfUnitStatusItem[i].QIndex.Qindex = queCounter;

            queCounter++;

            ShopControllerOLD.Intance.UpdateUnitsCountAndProgess(id, trainingIndexes[id]);
            ShopControllerOLD.Intance.UpdateHitText(unit.Description);
        }

        UpdateTime();
    }

    public void UnbuildUnit(UnitCategory itemData)
    {
        UnBuild(itemData.GetId(), 0);
    }

    private void UnBuild(int id, int action) // action 0 cancel 1 finished 2 exitmenu
    {
        var i = ShopControllerOLD.Intance.ListOfUnitStatusItem.FindIndex(x => x.ItemData.GetId() == id);
        var item = ShopControllerOLD.Intance.ListOfUnitStatusItem.Find(x => x.ItemData.GetId() == id);
        if (item == null) return;
        if (action == 0)
        {
            hours = minutes = seconds = 0;
            var
                itemPrice = item.ItemData.Price;

            if (item.ItemData.Currency == CurrencyType.Gold) //return value is max storage capacity allows it
            {
                if (itemPrice < Stats.Instance.maxGold - Stats.Instance.gold)
                {
                    Refund(itemPrice, 0, 0, 0, 0);
                }
                else
                {
                    Refund(Stats.Instance.maxGold - Stats.Instance.gold, 0, 0, 0, 0); //refunds to max storag capacity
                    MessageController.Instance.DisplayMessage("Stop canceling units!\nYou are losing gold!");
                }
            }

            else if (item.ItemData.Currency == CurrencyType.Mana)
            {
                if (itemPrice < Stats.Instance.maxMana - Stats.Instance.mana)
                {
                    Refund(0, itemPrice, 0, 0, 0);
                }
                else
                {
                    Refund(0, Stats.Instance.maxMana - Stats.Instance.mana, 0, 0, 0);
                    MessageController.Instance.DisplayMessage("Stop canceling units!\nYou are losing mana!");
                }
            }
            else
            {
                Refund(0, 0, itemPrice, 0, 0);
            }

            Stats.Instance.occupiedHousing -= item.ItemData.size;
            Stats.Instance.UpdateUI();
        }


        if (trainingIndexes[id] > 1)
        {
            trainingIndexes[id]--;

            ShopControllerOLD.Intance.ListOfUnitStatusItem[i].Slider.value = 0;
            ShopControllerOLD.Intance.UpdateUnitsCountAndProgess(id, trainingIndexes[id]);
        }
        else
        {
            ShopControllerOLD.Intance.ListOfUnitStatusItem[i].QIndex.inque = false;
            ShopControllerOLD.Intance.ListOfUnitStatusItem[i].QIndex.Qindex = 50;
            ShopControllerOLD.Intance.ListOfUnitStatusItem[i].Slider.value = 0;

            queCounter--;
            trainingIndexes[id]--;
            ShopControllerOLD.Intance.RemoveStatusItemFromList(i);
        }

        switch (action)
        {
            case 0:
                ShopControllerOLD.Intance.UpdateHitText("Training canceled.");
                break;
            case 1:
                ShopControllerOLD.Intance.UpdateHitText("Training complete.");
                break;
        }

        UpdateTime();
    }

    private void UpdateTime()
    {
        timeRemaining = 0;

        for (var i = 0; i < trainingIndexes.Length; i++) timeRemaining += trainingIndexes[i] * trainingTimes[i];
        if (ShopControllerOLD.Intance.ListOfUnitStatusItem.Count > 0)
            currentTrainingTime = trainingTimes[ShopControllerOLD.Intance.ListOfUnitStatusItem[0].QIndex.Objindex];
        else
            currentTrainingTime = 0;
        timeRemaining -= currentSlidVal * currentTrainingTime;

        if (timeRemaining > 0)
        {
            hours = (int)timeRemaining / 60;
            minutes = (int)timeRemaining % 60;
            seconds = (int)(60 - currentSlidVal * currentTrainingTime * 60 % 60);
        }

        if (minutes == 60) minutes = 0;
        if (seconds == 60) seconds = 0;

        if (hours > 0)
            _time = hours + " h " + minutes + " m " + seconds + " s ";
        else if (minutes > 0)
            _time = minutes + " m " + seconds + " s ";
        else if (seconds > 0) _time = seconds + " s ";

        if (timeRemaining >= 4320) priceInCrystals = 150;
        else if (timeRemaining >= 2880) priceInCrystals = 70;
        else if (timeRemaining >= 1440) priceInCrystals = 45;
        else if (timeRemaining >= 600) priceInCrystals = 30;
        else if (timeRemaining >= 180) priceInCrystals = 15;
        else if (timeRemaining >= 60) priceInCrystals = 7;
        else if (timeRemaining >= 30) priceInCrystals = 3;
        else if (timeRemaining >= 0) priceInCrystals = 1;

        ShopControllerOLD.Intance.UpdateUnitStatusData(_time, priceInCrystals.ToString());
    }

    private void ProgressBars()
    {
        //Time.deltaTime = 0.016; 60*Time.deltaTime = 1s ; runs at 60fps

        progCounter += Time.deltaTime * 0.5f;
        if (progCounter > progTime)
        {
            var objIndex = ShopControllerOLD.Intance.ListOfUnitStatusItem[0].QIndex.Objindex;
            currentTrainingTime = trainingTimes[objIndex];
            ShopControllerOLD.Intance.ListOfUnitStatusItem[0].Slider.value += Time.deltaTime / trainingTimes[objIndex];
            currentSlidVal = ShopControllerOLD.Intance.ListOfUnitStatusItem[0].Slider.value;
            ShopControllerOLD.Intance.ListOfUnitStatusItem[0].Slider.value =
                Mathf.Clamp(ShopControllerOLD.Intance.ListOfUnitStatusItem[0].Slider.value, 0, 1);

            if (Math.Abs(ShopControllerOLD.Intance.ListOfUnitStatusItem[0].Slider.value - 1) < 0.1f) FinishObject(0);

            progCounter = 0;
            UpdateTime();
        }
    }

    private void FinishObject(int index)
    {
        var objIndex = ShopControllerOLD.Intance.ListOfUnitStatusItem[index].QIndex.Objindex;

        UpdateExistingUnits(index);

        Stats.Instance.UpdateUnitsNo();
        UnBuild(objIndex, 1);
    }

    public void UpdateExistingUnits(int index)
    {
        var existedUnit = new ExistedUnit();
        existedUnit.id = ShopControllerOLD.Intance.ListOfUnitStatusItem[index].ItemData.id;
        existedUnit.count = ShopControllerOLD.Intance.ListOfUnitStatusItem[index].QIndex.Count;
        existedUnit.level = ShopControllerOLD.Intance.ListOfUnitStatusItem[index].Level;

        var indx = Stats.Instance.ExistingUnits.FindIndex(x => x.id == existedUnit.id && x.level == existedUnit.level);
        if (indx != -1)
        {
            Stats.Instance.ExistingUnits[indx].count += existedUnit.count;
            Stats.Instance.ExistingUnits[indx].level = existedUnit.level;
        }
        else
        {
            Stats.Instance.ExistingUnits.Add(existedUnit);

            //Fernando temporal workaround
            var _unitInfo = new UnitInfo();
            _unitInfo.id = existedUnit.id;
            _unitInfo.count = Stats.Instance.ExistingUnits.Find(x => x.id == existedUnit.id).count;
            _unitInfo.level = existedUnit.level;
            Instance.UnitsInfo.Add(_unitInfo);
        }
    }

    private void IncreasePopulation()
    {
        for (var i = 0; i < ShopControllerOLD.Intance.ListOfUnitStatusItem.Count; i++) UpdateExistingUnits(i);
    }

    public void FinishNow()
    {
        if (priceInCrystals <= Stats.Instance.crystals)
        {
            Stats.Instance.crystals -= priceInCrystals;
            Stats.Instance.UpdateUI();
            ShopControllerOLD.Intance.UpdateHitText("Training complete.");
            IncreasePopulation();
            Stats.Instance.UpdateUnitsNo();
            EraseValues();
        }

        else if (timeRemaining > 0)
        {
            MessageController.Instance.DisplayMessage("Not enough crystals");
        }
    }
}