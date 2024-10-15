using Assets.Scripts.Menus;
using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData.Models;
using CityBuildingKit.Scripts.UIControllersAndData.StructureType;
using UIControllersAndData.Store;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ConstructionSelector : MonoBehaviour
{
    public bool
        finishedOffLine, //for offline production then load
        isSelected = true, //for initial processing, right after the construction is instantiated
        battleMap; //flag - some components only exist in hometown/battlemap

    public float progTime = 0.57f, progCounter; //for progress timer, one minute

    public int
        elapsedTime, //for offline production then load
        iRow,
        jCol,
        buildingTime = 1,
        remainingTime = 1,
        priceno, //price displayed for "finish now" button. based on remaining time
        storageAdd, //passes maxStorage to stats
        xpAdd,
        constructionIndex = -1, //unique ID for constructions
        grassType;

    public Text TimeCounterLb;

    [SerializeField] private Text Price; //own child obj - has the price label

    [SerializeField] private Slider ProgressBar; //own child obj

    public GameObject ParentGroup; //to parent the building after it's finished

    [FormerlySerializedAs("StructureType")] [FormerlySerializedAs("structureType")] [SerializeField]
    private StructureType _structureType; //Toolhouse, Cannon, ArcherTower, etc 

    public string structureClass; //Building,Wall,Weapon,Ambient
    //controls the behaviour of a "building under construction", from placement to completion

    /*
        <ProdType>None</ProdType>					<!-- resource produced - gold/mana/none-->	
        <ProdPerHour>0</ProdPerHour>				<!-- the amount of the resource generated per hour -->			
        
        <StoreType>None</StoreType>					<!-- resource stored - none/gold/mana/dual/soldiers-->	
        <StoreCap>0</StoreCap>						<!-- gold/mana/dual/soldiers storage -->
    */

    private int hours, minutes, seconds; //for time remaining label

    private bool inConstruction = true;

    private GameObject[] selectedGrassType;


    private Component soundFX, relay, stats; //, resourceGenerator;

    public bool IsProductionBuilding { get; set; }

    public ShopCategoryType CategoryType { get; set; } = ShopCategoryType.None;

    public int Level { get; set; } = -1;

    public int Id { get; set; } = -1;

    public StructureType StructureType
    {
        get => _structureType;
        set => _structureType = value;
    }

    private void Start()
    {
        //GroupBuildings = GameObject.Find("GroupBuildings");

        relay = GameObject.Find("Relay").GetComponent<Relay>();
        soundFX = GameObject.Find("SoundFX").GetComponent<SoundFX>();

        if (!battleMap)
            //resourceGenerator = GameObject.Find("ResourceGenerator").GetComponent<ResourceGenerator>();
            stats = GameObject.Find("Stats").GetComponent<Stats>();

        //init price so user can't click fast on price 0
        //also adjusts by one minute to avoid starting in the next upper interval - visible at 30 or 60 minutes for example

        //remainingTime = buildingTime * (1 - (int)((UISlider)ProgressBar.GetComponent("UISlider")).value);

        remainingTime = buildingTime - 1;
        UpdatePrice(remainingTime);
    }


    private void FixedUpdate()
    {
        if (inConstruction) ProgressBarUpdate();
    }

    public void DetermineParentGroup()
    {
        switch (structureClass) //Building,Wall,Weapon,Ambient
        {
            case "Building":
                ParentGroup = GameObject.Find("GroupBuildings");
                break;
            case "Wall":
                ParentGroup = GameObject.Find("GroupWalls");
                break;
            case "Weapon":
                ParentGroup = GameObject.Find("GroupWeapons");
                break;
            case "Ambient":
                ParentGroup = GameObject.Find("GroupAmbients");
                break;
        }
    }

    private void ProgressBarUpdate()
    {
        progCounter += Time.deltaTime * 0.5f;
        if (progCounter > progTime)
        {
            progCounter = 0;

            ProgressBar.value += Time.deltaTime / buildingTime; //update progress bars values

            ProgressBar.value = Mathf.Clamp(ProgressBar.value, 0, 1);

            remainingTime = (int)(buildingTime * (1 - ProgressBar.value));

            UpdatePrice(remainingTime);
            UpdateTimeCounter(remainingTime);

            if (ProgressBar.value == 1) //building finished - the progress bar has reached 1												
            {
                ((SoundFX)soundFX).BuildingFinished();

                if (!battleMap) //if this building is not finished on a battle map
                {
                    //xml order: Forge Generator Vault Barrel Summon Tatami
                    ((Stats)stats).occupiedBuilders--; //the builder previously assigned becomes available

                    if (_structureType ==
                        StructureType.BuilderStation) //increases total storage in Stats																	
                    {
                        var buildingStation = ShopData.Instance.GetStructureData(Id, ShopCategoryType.Buildings, Level);
                        var builderCount = ((IBuilder)buildingStation).GetBuilder();
                        ((Stats)stats).builders += builderCount;
                    }
                    else if (_structureType == StructureType.Tatami) //increases total storage in Stats																	
                    {
                        ((Stats)stats).maxHousing += storageAdd;
                    }

                    else if (_structureType == StructureType.Barrel) //increases total storage in Stats																	
                    {
                        ((Stats)stats).maxMana += storageAdd;
                    }
                    else if (_structureType == StructureType.Vault)
                    {
                        ((Stats)stats).maxGold += storageAdd;
                    }

                    ((Stats)stats).UpdateUI();
                }

                foreach (Transform child in transform) //parenting and destruction of components no longer needed
                    if (child.gameObject.CompareTag("Structure")) //structureType
                    {
                        child.gameObject.SetActive(true);

                        var structureSelector = child.gameObject.GetComponent<StructureSelector>();
                        structureSelector.inConstruction = false;

                        structureSelector.Id = Id;
                        structureSelector.Level = Level;
                        structureSelector.CategoryType = CategoryType;

                        if (battleMap)
                        {
                            structureSelector.battleMap = true;
                        }
                        else if (IsProductionBuilding)
                        {
                            structureSelector.structureType = _structureType;
                            structureSelector.isProductionBuilding = true;

                            structureSelector.LateRegisterAsProductionBuilding(); //0.5f

                            if (finishedOffLine) structureSelector.LateCalculateElapsedProduction(elapsedTime); //1.0f
                            /*
                            MessageNotification m = child.GetComponent<MessageNotification> ();
                            m.structureIndex = constructionIndex;
                            ((ResourceGenerator)resourceGenerator).RegisterMessageNotification (m);
                            */
                        }
                        
                        structureSelector.FinishBuildingEvent.Invoke();
                        

                        foreach (Transform childx in transform)
                            if (childx.gameObject.CompareTag("Grass"))
                            {
                                var o = child.gameObject;
                                childx.gameObject.transform.parent = o.transform;
                                o.transform.parent = ParentGroup.transform;

                                break;
                            }

                        break;
                    }

                Destroy(gameObject);
                inConstruction = false;
            }
        }
    }


    /*
    private void RegisterAsProductionBuilding()
    {	
        for (int i = 0; i < ((ResourceGenerator)resourceGenerator).basicEconomyValues.Length; i++) 
        {
            if (((ResourceGenerator)resourceGenerator).basicEconomyValues [i].StructureType == structureType) 
            {	
                CopyBasicValues (i, ((ResourceGenerator)resourceGenerator).basicEconomyValues [i]);
                break;
            }							
        }
    }

    private void CopyBasicValues(int i, EconomyBuilding basicValuesEB)
    {		
        EconomyBuilding myEconomyParams = new EconomyBuilding();

        myEconomyParams.structureIndex = constructionIndex;
        myEconomyParams.ProdPerHour = basicValuesEB.ProdPerHour;
        myEconomyParams.StoreCap = basicValuesEB.StoreCap;
        myEconomyParams.StructureType = structureType;
        myEconomyParams.ProdType = basicValuesEB.ProdType;
        myEconomyParams.StoreType = basicValuesEB.StoreType;
        myEconomyParams.StoreResource = basicValuesEB.StoreResource;

        ((ResourceGenerator)resourceGenerator).existingEconomyBuildings.Add (myEconomyParams);
    }
    */

    private void UpdateTimeCounter(int remainingTime) //calculate remaining time
    {
        hours = remainingTime / 60;
        minutes = remainingTime % 60;
        seconds = (int)(60 - ProgressBar.value * buildingTime * 60 % 60);

        if (minutes == 60) minutes = 0;
        if (seconds == 60) seconds = 0;

        UpdateTimeLabel();
    }

    private void UpdateTimeLabel() //update the time labels on top
    {
        if (hours > 0 && minutes > 0 && seconds >= 0)
            TimeCounterLb.text = hours + " h " + minutes + " m " + seconds + " s ";
        else if (minutes > 0 && seconds >= 0)
            TimeCounterLb.text = minutes + " m " + seconds + " s ";
        else if (seconds > 0) TimeCounterLb.text = seconds + " s ";
    }


    private void UpdatePrice(int remainingTime) //update the price label on the button, based on remaining time		
    {
        /*
        0		30		1
        30		60		3
        60		180		7
        180		600		15
        600		1440	30
        1440	2880	45
        2880	4320	70
        4320			150
         */

        if (remainingTime >= 4320)
            priceno = 150;
        else if (remainingTime >= 2880)
            priceno = 70;
        else if (remainingTime >= 1440)
            priceno = 45;
        else if (remainingTime >= 600)
            priceno = 30;
        else if (remainingTime >= 180)
            priceno = 15;
        else if (remainingTime >= 60)
            priceno = 7;
        else if (remainingTime >= 30)
            priceno = 3;
        else if (remainingTime >= 0) priceno = 1;

        Price.text = priceno.ToString();
    }

    public void Finish()
    {
        //if(!battleMap) //no need to check, the finish button is not visible on the battle map
        //{
        if (!((Relay)relay).pauseInput && !((Relay)relay).delay) //panels are open / buttons were just pressed 
        {
            ((SoundFX)soundFX).Click();
            if (((Stats)stats).crystals >= priceno)
            {
                ((Stats)stats).crystals -= priceno;
                ((Stats)stats).UpdateUI();
                ProgressBar.value = 1;
            }
            else
            {
                MessageController.Instance.DisplayMessage("Insufficient crystals");
            }
        }
        //}
    }
}