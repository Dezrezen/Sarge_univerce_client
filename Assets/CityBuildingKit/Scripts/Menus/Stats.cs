using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Menus;
using Assets.Scripts.UIControllersAndData;
using Assets.Scripts.UIControllersAndData.Images;
using Assets.Scripts.UIControllersAndData.Player;
using UIControllersAndData.GameResources;
using UIControllersAndData.Store;
using UIControllersAndData.Units;
using UnityEngine;
using UnityEngine.Serialization;

public class Stats : MonoBehaviour
{
    private const int noOfCreators = 4; //the resource bars on top of the screen

    public static Stats Instance;

    [SerializeField] private bool _showTutorial;

    public BaseCreator[]
        creators = new BaseCreator[noOfCreators]; //the store must update other creators/interfaces after purchases

    public GameObject GhostHelper;

    public bool
        gameWasLoaded,
        tutorialBattleSeen,
        removablesCreated,
        resourcesAdded,
        resourcesSubstracted;

    public int //when user hard buys resources, storage capacity permanently increases 
        structureIndex = -1, //unique index that all structures have - buildings, weapons, walls, etc
        townHallLevel,
        level = 1, //player level
        experience = 1,
        maxExperience = 100;

    [FormerlySerializedAs("dobbits")]
    public int //when user hard buys resources, storage capacity permanently increases 
        builders = 1;

    [FormerlySerializedAs("occupiedDobbits")]
    public int //when user hard buys resources, storage capacity permanently increases 
        occupiedBuilders;

    public int //when user hard buys resources, storage capacity permanently increases 
        remainingCloakTime,
        purchasedCloakTime,
        occupiedHousing, //based on size, not number of units
        maxHousing, //housing refers ONLY to soldiers, not npc/builder
        gold = 5000,
        maxGold = 10000,
        mana = 500,
        maxMana = 1000,
        crystals = 5,
        maxCrystals = 5,
        power = 10000,
        maxPower = 100000,
        supplies,
        maxSupplies = 10000,
        deltaGoldPlus,
        deltaGoldMinus, //when a resource is added/spent, it starts a counter; must be separate because the operations might be simultaneous  
        deltaManaPlus,
        deltaManaMinus,
        deltaCrystalsPlus,
        deltaCrystalsMinus,
        deltaPowerPlus,
        deltaPowerMinus,
        deltaSuppliesPlus,
        deltaSuppliesMinus;

    //public float[] productionRates;

    public int[] sizePerUnit; //based on size, a soldier can occupy more than 1 space

    public List<int> maxBuildingsAllowed;
    public List<int> maxWallsAllowed;
    public List<int> maxWeaponsAllowed;
    public List<int> maxAmbientsAllowed;


    public TextAsset EvoStructuresXML; //variables for loading building characteristics from XML

    //Interface connections

    public Store store; //this component is disabled, GameObject.Find doesn't work

    [SerializeField] private bool isHqExist = true;
    [SerializeField] private int currentHqLevel = 1;


    protected Dictionary<string, string> dictionary;

    private int hours, minutes, seconds; //for cloak remaining label
    protected List<Dictionary<string, string>> levels = new();

    private Component transData;

    public bool IsHqExist
    {
        get => isHqExist;
        set => isHqExist = value;
    }

    public int CurrentHqLevel
    {
        get => currentHqLevel;
        set => currentHqLevel = value;
    }

    public List<ExistedUnit> ExistingUnits { get; set; } = new();

    public bool ShowTutorial
    {
        get => _showTutorial;

        set => _showTutorial = value;
    }

    public bool TutorialCitySeen { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        transData = GameObject.Find("TransData").GetComponent<TransData>();
        sizePerUnit = new int[GameUnitsSettings_Handler.s.unitsNo];
        StartCoroutine("ReturnFromBattle");
        if (ShowTutorial) StartCoroutine("LaunchTutorial");
        GhostHelper.SetActive(_showTutorial);

        UpdateBuildingData();
        UpdateWallData();
        UpdateWeaponData();
        UpdateAmbientData();

        UpdateUnitsNo();
        StartCoroutine("LateUpdateUI");

        // var unit = new ExistedUnit();
        // unit.id = 0;
        // unit.level = 1;
        // unit.count = 10;
        //
        // ExistingUnits.Add(unit);
    }

    private IEnumerator LateUpdateUI()
    {
        yield return new WaitForSeconds(3);
        UpdateUI();
    }

    private void UpdateBuildingData()
    {
        foreach (var lvl in ShopData.Instance.BuildingsCategoryData.category)
        foreach (var buildingsCategory in lvl.levels)
            if (buildingsCategory.level == 1)
                maxBuildingsAllowed.Add(buildingsCategory.MaxCountOfThisItem);

        foreach (var lvl in ShopData.Instance.ArmyCategoryData.category)
        foreach (var militaryCategory in lvl.levels)
            if (militaryCategory.level == 1)
                maxBuildingsAllowed.Add(militaryCategory.MaxCountOfThisItem);
    }

    private void UpdateWallData()
    {
        foreach (var lvl in ShopData.Instance.WallsCategoryData.category)
        foreach (var wallCategory in lvl.levels)
            if (wallCategory.level == 1)
                maxWallsAllowed.Add(wallCategory.MaxCountOfThisItem);
    }

    private void UpdateWeaponData()
    {
        foreach (var lvl in ShopData.Instance.WeaponCategoryData.category)
        foreach (var weaponCategory in lvl.levels)
            if (weaponCategory.level == 1)
                maxWeaponsAllowed.Add(weaponCategory.MaxCountOfThisItem);
    }

    private void UpdateAmbientData()
    {
        foreach (var lvl in ShopData.Instance.AmbientCategoryData.category)
        foreach (var ambientCategory in lvl.levels)
            if (ambientCategory.level == 1)
                maxAmbientsAllowed.Add(ambientCategory.MaxCountOfThisItem);
    }

    public bool EnoughGold(int goldPrice)
    {
        return goldPrice <= gold + deltaGoldPlus - deltaGoldMinus;
    }

    public bool EnoughMana(int manaPrice)
    {
        return manaPrice <= mana + deltaManaPlus - deltaManaMinus;
    }

    public bool EnoughCrystals(int crystalPrice)
    {
        return crystalPrice <= crystals + deltaCrystalsPlus - deltaCrystalsMinus;
    }

    public void AddResources(int dGold, int dMana, int dCrystals, int dPower, int dSupplies)
    {
        deltaGoldPlus += dGold;
        deltaManaPlus += dMana;
        deltaCrystalsPlus += dCrystals;
        deltaPowerPlus += dPower;
        deltaSuppliesPlus += dSupplies;

        if (!resourcesAdded)
        {
            resourcesAdded = true;
            InvokeRepeating("GradualAddResources", 0.1f, 0.1f);
        }

        UpdateCreatorMenus();
        //updates in sequence all buttons from all panels; 
    }

    private void GradualAddResources()
    {
        if (resourcesAdded) //verify if needed
        {
            if (deltaGoldPlus > 10)
            {
                var substract = deltaGoldPlus / 10;


                deltaGoldPlus -= substract;
                gold += substract;
            }
            else if (deltaGoldPlus > 0)
            {
                deltaGoldPlus--;
                gold++;
            }

            if (deltaManaPlus > 10)
            {
                var substract = deltaManaPlus / 10;

                deltaManaPlus -= substract;
                mana += substract;
            }
            else if (deltaManaPlus > 0)
            {
                deltaManaPlus--;
                mana++;
            }

            if (deltaCrystalsPlus > 10)
            {
                var substract = deltaCrystalsPlus / 10;
                deltaCrystalsPlus -= substract;
                crystals += substract;
            }
            else if (deltaCrystalsPlus > 0)
            {
                deltaCrystalsPlus--;
                crystals++;
            }

            if (deltaPowerPlus > 10)
            {
                var substract = deltaPowerPlus / 10;
                deltaPowerPlus -= substract;
                power += substract;
            }
            else if (deltaPowerPlus > 0)
            {
                deltaPowerPlus--;
                power++;
            }

            if (deltaSuppliesPlus > 10)
            {
                var substract = deltaSuppliesPlus / 10;
                deltaSuppliesPlus -= substract;
                supplies += substract;
            }
            else if (deltaSuppliesPlus > 0)
            {
                deltaSuppliesPlus--;
                supplies++;
            }

            ApplyMaxCaps();
            UpdateUI();

            if (deltaGoldPlus == 0 && deltaManaPlus == 0 && deltaCrystalsPlus == 0 && deltaPowerPlus == 0)
            {
                CancelInvoke("GradualAddResources");
                resourcesAdded = false;
            }
        }
    }

    public void SubstractResources(int dGold, int dMana, int dCrystals, int dPower, int dSupplies)
    {
        deltaGoldMinus += dGold;
        deltaManaMinus += dMana;
        deltaCrystalsMinus += dCrystals;
        deltaPowerMinus += dPower;
        deltaSuppliesMinus += dSupplies;

        if (!resourcesSubstracted)
        {
            resourcesSubstracted = true;
            InvokeRepeating("GradualSubstractResources", 0.1f, 0.1f);
        }

        UpdateCreatorMenus();
        //updates in sequence all buttons from all panels; 
    }


    private void GradualSubstractResources()
    {
        if (resourcesSubstracted) //verify if needed
        {
            if (deltaGoldMinus > 10)
            {
                var substract = deltaGoldMinus / 10;

                deltaGoldMinus -= substract; //substract is a negative value here
                gold -= substract;
            }
            else if (deltaGoldMinus > 0)
            {
                deltaGoldMinus--;
                gold--;
            }

            if (deltaManaMinus > 10)
            {
                var substract = deltaManaMinus / 10;

                deltaManaMinus -= substract;
                mana -= substract;
            }
            else if (deltaManaMinus > 0)
            {
                deltaManaMinus--;
                mana--;
            }

            if (deltaCrystalsMinus > 10)
            {
                var substract = deltaCrystalsMinus / 10;
                deltaCrystalsMinus -= substract;
                crystals -= substract;
            }
            else if (deltaCrystalsMinus > 0)
            {
                deltaCrystalsMinus--;
                crystals--;
            }

            if (deltaCrystalsMinus > 10)
            {
                var substract = deltaCrystalsMinus / 10;
                deltaCrystalsMinus -= substract;
                crystals -= substract;
            }
            else if (deltaCrystalsMinus > 0)
            {
                deltaCrystalsMinus--;
                crystals--;
            }


            if (deltaPowerMinus > 10)
            {
                var substract = deltaPowerMinus / 10;

                deltaPowerMinus -= substract; //substract is a negative value here
                power -= substract;
            }
            else if (deltaPowerMinus > 0)
            {
                deltaPowerMinus--;
                power--;
            }

            if (deltaSuppliesMinus > 10)
            {
                var substract = deltaSuppliesMinus / 10;

                deltaSuppliesMinus -= substract; //substract is a negative value here
                supplies -= substract;
            }
            else if (deltaSuppliesMinus > 0)
            {
                deltaSuppliesMinus--;
                supplies--;
            }


            ApplyMaxCaps();
            UpdateUI();

            if (deltaGoldMinus == 0 && deltaManaMinus == 0 && deltaCrystalsMinus == 0 && deltaPowerMinus == 0)
            {
                CancelInvoke("GradualSubstractResources");
                resourcesSubstracted = false;
            }
        }
    }

    private IEnumerator ReturnFromBattle()
    {
        yield return new WaitForSeconds(1.5f);

        if (((TransData)transData).battleOver)
        {
            ((TransData)transData).ReturnFromBattle();
            TutorialCitySeen = true; //since we have already been to battle, no tutorial 
        }
    }

    private IEnumerator LaunchTutorial()
    {
        yield return new WaitForSeconds(10.0f);
        if (!TutorialCitySeen)
            GhostHelper.SetActive(
                true); //since this is a delayed function, we will activate the first time tutorial here 
    }

    public void ApplyMaxCaps() //cannot exceed storage+bought capacity
    {
        if (gold > maxGold) gold = maxGold;
        if (mana > maxMana) mana = maxMana;
        if (power > maxPower) power = maxPower;
        if (supplies > maxSupplies) supplies = maxSupplies;
        //if (experience > maxExperience) { experience = maxExperience; }
    }

    public void VerifyMaxReached()
    {
        if (gold == maxGold) MessageController.Instance.DisplayMessage("Increase Gold storage capacity.");
        if (mana == maxMana) MessageController.Instance.DisplayMessage("Increase Mana storage capacity.");
        if (power == maxPower) MessageController.Instance.DisplayMessage("Increase Power storage capacity.");
        if (supplies == maxSupplies) MessageController.Instance.DisplayMessage("Increase Supplies storage capacity.");
    }

    public void UpdateCreatorMenus()
    {
        for (var i = 0; i < creators.Length; i++) creators[i].UpdateButtons();
    }

    public void UpdateUI() //updates numbers and progress bars
    {
        var data = Player.Instance.GetPlayer();
        data.PlayerName = "Player Name";
        data.LevelData.Level = level;
        data.ExperienceData.CurrentExp = experience;

        data.PlayerResources.Builder.Type = GameResourceType.Builder;
        data.PlayerResources.Builder.CurrentValue = occupiedBuilders;
        data.PlayerResources.Builder.MaxValue = builders;

        data.PlayerResources.Housing.Type = GameResourceType.Housing;
        data.PlayerResources.Housing.CurrentValue = occupiedHousing;
        data.PlayerResources.Housing.MaxValue = maxHousing;

        data.PlayerResources.Gold.Type = GameResourceType.Gold;
        data.PlayerResources.Gold.CurrentValue = gold;
        data.PlayerResources.Gold.MaxValue = maxGold;

        data.PlayerResources.Mana.Type = GameResourceType.Mana;
        data.PlayerResources.Mana.CurrentValue = mana;
        data.PlayerResources.Mana.MaxValue = maxMana;

        data.PlayerResources.Crystal.Type = GameResourceType.Crystal;
        data.PlayerResources.Crystal.CurrentValue = crystals;
        data.PlayerResources.Crystal.MaxValue = maxCrystals;

        data.PlayerResources.Power.Type = GameResourceType.Power;
        data.PlayerResources.Power.CurrentValue = power;
        data.PlayerResources.Power.MaxValue = maxPower;

        data.PlayerResources.Supplies.Type = GameResourceType.Supplies;
        data.PlayerResources.Supplies.CurrentValue = supplies;
        data.PlayerResources.Supplies.MaxValue = maxSupplies;

        data.CloakData.RemainingCloakTime = remainingCloakTime;
        data.CloakData.PurchasedCloakTime = purchasedCloakTime;

        Player.Instance.PlayerEvt.Invoke(data);
    }

    public void UpdateUnitsNo()
    {
        var allUnits = 0;
        for (var i = 0; i < ExistingUnits.Count; i++) allUnits += ExistingUnits[i].count;

        Player.Instance.GetPlayer().AllUnits = allUnits;
        Player.Instance.PlayerEvt.Invoke(Player.Instance.GetPlayer());
    }

    public bool IsEnoughCurrency(int requiredAmount, CurrencyType currencyType)
    {
        switch (currencyType)
        {
            case CurrencyType.Crystal:
                return requiredAmount <= crystals;
//			case CurrencyType.Dollars: //I'm not sure what do I need to use here...
//				return requiredAmount <= 0;
            case CurrencyType.Gold:
                return requiredAmount <= gold;
            case CurrencyType.Mana:
                return requiredAmount <= mana;
        }

        return false;
    }

    public Sprite GetCurrencyIcon(CurrencyType currencyType)
    {
        switch (currencyType)
        {
            case CurrencyType.Crystal:
                return ImageControler.GetImage("crystal");
//			case CurrencyType.Dollars: //I'm not sure what do I need to use here...
//				return requiredAmount <= 0;
            case CurrencyType.Gold:
                return ImageControler.GetImage("gold");
            case CurrencyType.Mana:
                return ImageControler.GetImage("mana");
        }

        return null;
    }

    public void IncreaseHqLevel()
    {
        currentHqLevel += 1;
        Debug.LogError("currentHqLevel: " + currentHqLevel);
    }

    public void DecreaseHqLevel()
    {
        currentHqLevel -= 1;
        Debug.LogError("currentHqLevel: " + currentHqLevel);
    }
}