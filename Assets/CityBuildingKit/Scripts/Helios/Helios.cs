using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Menus;
using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.Controller;
using CityBuildingKit.Scripts.UIControllersAndData.StructureType;
using UIControllersAndData;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UnityEngine;
using UnityEngine.UI;

public class Helios : MonoBehaviour
{
    private const int unitGroupsNo = 1; //the maximum number of groups

    public static Helios Instance;

    //status variables:
    public bool networkLoadReady;

    public int
        // instantiationGroupIndex = -1, //keeps track of total number of groups- must not exceed 4
        instantiationUnitIndex = -1; //each unit will have a unique ID- necessary when killed - not necessarily in order
    // selectedGroupIndex; //0 when initialized, reset to -1 afterwards 

    public Text goldNoLb,
        manaNoLb,
        buildingsDestroyedNoLb,
        unitsLostNoLb,
        unitsRecoveredNoLb; //missioncomplete panel labels

    // public List<GameObject>
    //     grassTarget = new(); //each group heads for its assigned grass target


    public GameObject
        MissionCompletePanel, //activated at battle end
        SmokePf,
        ExplosionPf,
        FirePf,
        Rubble1x1Pf,
        Rubble2x2Pf,
        Rubble3x3Pf,
        GainGoldPf,
        GainManaPf, //prefabs
        EffectsGroup; //parent instantiated effect prefabs


    [SerializeField] private AnimatorHelper heliosAnimation;

    public List<int>
        BuildingValues = new(); //holds loot values for each building; 0 when completely destroyed

    public List<string> BuildingCurrency = new();

    public int[]
        // targetStructureIndex = new int[unitGroupsNo],
        surroundIndex =
            new int[1]; //for each separate group, increments an index to "surround" the buildings - occupy the aiTargets in sequence - 0,1,2, etc 

    public Vector3[] targetCenter = new Vector3[unitGroupsNo];

    // public Vector2[]
    //     unitPosition = new Vector2[unitGroupsNo]; // the position of the group is set by the position of element 0

    // public bool[]
    //     pauseAttack = new bool[unitGroupsNo]; //necessary to avoid errors untill the units state has changed

    public List<GameObject> //buildings and cannons, instantiated and passed at LoadMap
        DeployedUnits = new(), //all units deployed
        selectedList = new();

    public List<GameObject>
        _allUnits = new();

    public List<Vector2>
        aiTargetsFree = new(), //##make private - to see if the list is sorted properly
        aiTargetVectorsCurrent =
            new(); //List of current target vectors - will be passed to each group; aiTargets are the green squares around the building - units go there to attack
    // aiTargetVectorsO =
    //     new(); //O I II III IV V VI VII VIII IX X XI XII XIII XIV XV XVI XVII XVIII 0-18 Roman numerals + O for nulla, in case anyone is wondering

    [SerializeField] private bool updatePathMaster; // used to scatter the updates for units - bools

    private readonly Dictionary<int, int>
        buildingsHealth =
            new(); //100, 100, 100 - you can make this private - useful to increase health to study unit behavior

    private readonly Dictionary<int, int>
        currentDamage = new(); //if 5 units attack a building, the building health (100) is decreased by 5 each second

    private readonly int
        // z depths
        fireZ = -11;

    private readonly float
        intervalTime = 1.0f;

    private readonly float
        // interval between attack updates
        pathUpdateTime = 0.2f;

    private readonly int
        smokeZ = -10;

    private readonly int
        // z depths
        targetZ = -11;

    private readonly int
        // z depths
        zeroZ = 0;

    private int
        // z depths
        //to avoid gioded projectiles from changing Z and going under sprites
        allLootGold, // total gold/mana on map - passed to StatsBattle to properly dimension the gold/mana loot progress bars.
        allLootMana,
        gain, // the loot fraction, based on total building value and damage procentage;
        // updateOCounter, // used to scatter the updates for units - cycles through the unit lists
        processCounter;

    public Dictionary<int, GameObject> //buildings and cannons, instantiated and passed at LoadMap
        DamageBars = new();

    //lists and arrays for buildings and units:
    public Dictionary<int, bool>
        //BuildingGoldBased = new List<bool>(),
        DamageEffectsRunning = new(); //0,0,0... initially then 1,1,1...

    private float
        elapsedTime;

    private GameObject[] grassPatches; //all grass patches on the map - found by label

    private bool
        mapIsEmpty,
        battleOver,
        allDestroyed,
        updatePathO;

    //numeric data:
    private int
        starZ = -6;

    private Component
        statsBattle, transData, saveLoadBattle, soundFx, relay, heliosMsg; //components needed to pass info

    // private float
    // interval between attack updates
    // due to the wall attack absence, paths are long and take time; normally 0.2f
    // updateOTimer, // used to scatter the updates for units - timers
    // updateITimer,
    // updateIITimer,
    // updateIIITimer;

    public List<StructureSelector>
        Buildings { get; set; } = new();

    [SerializeField] private ArmyBattleController armyBattleController;
    
    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        statsBattle = GameObject.Find("StatsBattle").GetComponent<StatsBattle>();
        transData = GameObject.Find("TransData").GetComponent<TransData>();
        saveLoadBattle = GameObject.Find("SaveLoadBattle").GetComponent<SaveLoadBattle>();
        soundFx = GameObject.Find("SoundFX").GetComponent<SoundFX>();
        relay = GameObject.Find("Relay").GetComponent<Relay>();
        heliosMsg = GameObject.Find("HeliosMsg").GetComponent<MessageController>();
    }


    // Update is called once per frame
    private void Update()
    {
        if (mapIsEmpty || battleOver) return;

        // for (int i = 0; i <= instantiationGroupIndex; i++) 
        // {
        // if (updateTarget[0])
        {
            // StartCoroutine(UpdateTargetGroup(0));
        }
        // }

        if (updatePathMaster)
            StartUpdateUnits();
        // UpdatePaths();
        // StartCoroutine(UpdateTargetGroup());

        //Damage
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= intervalTime)
        {
            if (DeployedUnits.Count ==
                0) //all deployed units have died, no more slots to deploy another group/no more available units
            {
                var availableUnits = 0;

                for (var i = 0; i < ((StatsBattle)statsBattle).AvailableUnits.Count; i++)
                {
                    availableUnits += ((StatsBattle)statsBattle).AvailableUnits[i].count;
                    availableUnits += ((StatsBattle)statsBattle).DeployedUnits[i].count;
                }

                if ( /*instantiationGroupIndex == 3 ||*/ availableUnits == 0)
                {
                    if (!battleOver)
                        StartCoroutine("MissionComplete");
                    return;
                }
            }

            elapsedTime = 0.0f;

            //TODO: CHECK IT
            // for (var i = 0; i < currentDamage.Count; i++) currentDamage[i] = 0; //reset the current damage array of 4

            var noBuildingUnderAttack = true;

            // for (var i = 0; i < DeployedUnits.Count; i++)
            //     if (DeployedUnits[i] != null)
            //         if (DeployedUnits[i].GetComponent<FighterController>().CurrentState ==
            //             FighterController.UnitState.Attack)
            //         {
            //             //TODO: need to rework this part - it shouldn't be - 0
            //             var fighterController = DeployedUnits[i].GetComponent<FighterController>();
            //             var structureIndex = fighterController.GrassTarget.StructureIndex;
            //             if (currentDamage.Count > 0 && currentDamage[structureIndex] != null)
            //                 currentDamage[structureIndex] +=
            //                     fighterController.DamagePoints;
            //         }

            noBuildingUnderAttack = false;

            if (buildingsHealth.Count == 0 || noBuildingUnderAttack)
                return; //print ("no building under attack + buildingshealth.count=0");}//empty map - no buildings || no building under attack

            var gainPerBuilding = new int[Buildings.Count]; //same count as Buildings

            // foreach (var kvp in currentDamage)
            //     if (kvp.Value > 0)
            //     {
            //         // var data = Buildings.Find(value => value.structureIndex == kvp.Key).StructureData;
            //
            //         //TODO: CHECK IT
            //         // gain = (currentDamage [i]*BuildingValues[k])/100;
            //         // gainPerBuilding[k] += gain;
            //         buildingsHealth[kvp.Key] -= 1 * kvp.Value;
            //         DamageBars[kvp.Key].GetComponent<Slider>().value =
            //             buildingsHealth[kvp.Key] * 0.01f; //since full building health was 100
            //     }

            // for (var i = 0; i <= instantiationGroupIndex; i++)
            // {
            //     int damage;
            //     currentDamage.TryGetValue(i, out damage);
            //     if (damage > 0)
            //     {
            //         var k = GetListIndex(targetStructureIndex[i]);
            //         //print(k.ToString());
            //         gain = currentDamage[i] * BuildingValues[k] / 100;
            //         gainPerBuilding[k] += gain;
            //         BuildingsHealth[k] -= 1 * currentDamage[i];
            //         DamageBars[k].GetComponent<Slider>().value =
            //             BuildingsHealth[k] * 0.01f; //since full building health was 100
            //     }
            // }

            for (var i = 0; i < gainPerBuilding.Length; i++)
                if (gainPerBuilding[i] > 0)
                {
                    Vector2 pos = Buildings[i].transform.position;

                    if (BuildingCurrency[i] == "Gold")
                    {
                        var GainGold = Instantiate(GainGoldPf);
                        GainGold.transform.SetParent(EffectsGroup.transform);
                        GainGold.transform.position = new Vector3(pos.x, pos.y + 100, smokeZ);
                        ((StatsBattle)statsBattle).gold +=
                            gainPerBuilding[i]; //approximates a bit- the building health might reach -17 at last hit 
                        GainGold.GetComponentInChildren<Text>().text = "+ " + gainPerBuilding[i];
                        GainGold.transform.SetParent(EffectsGroup.transform);
                    }
                    else
                    {
                        var GainMana = Instantiate(GainManaPf);
                        GainMana.transform.SetParent(EffectsGroup.transform);
                        GainMana.transform.position = new Vector3(pos.x, pos.y + 100, smokeZ);
                        ((StatsBattle)statsBattle).mana += gainPerBuilding[i];
                        GainMana.GetComponentInChildren<Text>().text = "+ " + gainPerBuilding[i];
                    }
                }

            ((StatsBattle)statsBattle).UpdateUI();


            // for (var k = 0; k < targetStructureIndex.Length; k++)
            foreach (var kvp in Buildings.Select((value, i) => (value, i)))
            {
                // var structureIndex = targetStructureIndex[k];
                // var i = GetListIndex(structureIndex);

                if (!DamageEffectsRunning[kvp.value.structureIndex] &&
                    buildingsHealth[kvp.value.structureIndex] <
                    90) //instantiates smoke the first time the building is attacked/below 90 health
                {
                    //((SoundFX)SoundFx).BuildingBurn(); //sound gets too clogged, remain silent
                    DamageEffectsRunning[kvp.value.structureIndex] = true;

                    var Smoke = Instantiate(SmokePf, new Vector3(kvp.value.transform.position.x,
                        kvp.value.transform.position.y,
                        smokeZ), Quaternion.identity);

                    Smoke.transform.parent = EffectsGroup.transform;

                    var Fire = Instantiate(FirePf, new Vector3(kvp.value.transform.position.x,
                        kvp.value.transform.position.y,
                        fireZ), Quaternion.identity);
                    Fire.transform.parent = EffectsGroup.transform;
                }

                if (buildingsHealth[kvp.value.structureIndex] < 0
                    && !kvp.value.StructureGrassSelector.isDestroyed
                   ) //explodes once, then marks the grass as destroyed  
                {
                    ((SoundFX)soundFx).BuildingExplode();
                    ((StatsBattle)statsBattle).buildingsDestroyed++;

                    Talk(
                        "Target destroyed. Incoming EMP shockwave.\nRetreat to a safe distance.\nExpect brief communications blackout.");
                    HeliosTween.Instance.PlayFadeOut();
                    StartCoroutine("GarbleMessage");

                    DamageBars[kvp.value.structureIndex].SetActive(false);


                    var Explosion = Instantiate(ExplosionPf,
                        new Vector3(kvp.value.transform.position.x, kvp.value.transform.position.y, smokeZ),
                        Quaternion.identity);

                    //to instantiate rubble of different sizes - different prefabs or you can scale down a bit
                    var buildingType = kvp.value.structureType;
                    var buildingClass = kvp.value.structureClass;

                    if (buildingType == StructureType.Chessboard || buildingType == StructureType.Toolhouse ||
                        buildingType == StructureType.Summon)
                    {
                        var Rubble2x2 = Instantiate(Rubble2x2Pf,
                            new Vector3(kvp.value.transform.position.x, kvp.value.transform.position.y, zeroZ),
                            Quaternion.identity);
                        AdjustRubbleZ(Rubble2x2);
                        Rubble2x2.transform.parent = EffectsGroup.transform;
                    }

                    else if (buildingClass == "Weapon" || buildingType == StructureType.Tatami)
                    {
                        var Rubble1x1 = Instantiate(Rubble1x1Pf,
                            new Vector3(kvp.value.transform.position.x, kvp.value.transform.position.y, zeroZ),
                            Quaternion.identity);
                        AdjustRubbleZ(Rubble1x1);
                        Rubble1x1.transform.parent = EffectsGroup.transform;
                    }
                    else
                    {
                        var Rubble3x3 = Instantiate(Rubble3x3Pf,
                            new Vector3(kvp.value.transform.position.x, kvp.value.transform.position.y, zeroZ),
                            Quaternion.identity);
                        AdjustRubbleZ(Rubble3x3);
                        Rubble3x3.transform.parent = EffectsGroup.transform;
                    }

                    var
                        structureClass = kvp.value.structureClass;

                    var structureType = kvp.value.structureType;

                    if (structureClass == "Weapon" && (structureType == StructureType.Cannon ||
                                                       structureType == StructureType.ArcherTower ||
                                                       structureType == StructureType.Catapult))
                    {
                        kvp.value.GetComponentInChildren<PerimeterCollider>().enabled =
                            false; //prevent the perimeter collider from firing again
                        kvp.value.GetComponent<TurretControllerBase>().fire = false;
                        kvp.value.GetComponent<TurretControllerBase>().enabled = false;
                        kvp.value.transform.Find("Turret").gameObject.SetActive(false);
                    }

                    kvp.value.transform.Find("Sprites").gameObject.SetActive(false);
                    kvp.value.transform.Find("SelectBuildingButton").gameObject.SetActive(false);

                    if (!kvp.value.gameObject.activeSelf) //constructions- the building is disabled
                    {
                        kvp.value.transform.parent.GetComponent<ConstructionSelector>().enabled = false;
                        kvp.value.transform.parent.Find("Dobbit").gameObject.SetActive(false);
                        kvp.value.transform.parent.Find("Sprites").gameObject.SetActive(false);
                    }

                    for (var j = 0;
                         j < Buildings.Count;
                         j++) //change the obstacles after a building is destroyed
                        //TODO: CHECK IT
                        // if (targetStructureIndex[j] == structureIndex)
                    {
                        //Buildings[j].StructureGrassSelector.isDestroyed = true;

                        kvp.value.StructureGrassSelector.isDestroyed = true;


                        allDestroyed = Buildings.All(structures => structures.StructureGrassSelector.isDestroyed);
                        if (allDestroyed)
                        {
                            StartCoroutine("MissionComplete");
                            return;
                        }


                        var grassType =
                            kvp.value.StructureGrassSelector
                                .grassType; //this section will make the grass passable, except its center

                        if (grassType == 2) break;

                        //disregard type 2 grass, the rubble prefab is too big to make any portion of it passable; 
                        //applies only to toolhouse, but since you can have only 1 toolhouse, there is no situation
                        //when a building is surrounded with toolhouses
                        var AIObstaclesParentObj =
                            kvp.value.StructureGrassSelector.transform
                                .Find("AIObstacles"); //AiObstacles parent object						
                        var aiObstaclesChildren =
                            AIObstaclesParentObj
                                .GetComponentsInChildren<Transform>(); //all the little obstacle cubes

                        switch
                            (grassType) // to see the red obstacles/green targets, activate the mesh renderers of the cubes inside the battlemap grass prefabs
                        {
                            case 4:
                                //skip all 4 center tiles - they remain obstacles
                                for (var l = 1; l < aiObstaclesChildren.Length; l++) // 1 skip parent
                                    if (l != 3 && l != 4 && l != 6 && l != 7) //skip center
                                    {
                                        MarkAsPassable(aiObstaclesChildren[l]);
                                        aiObstaclesChildren[l].gameObject.tag = "Untagged";
                                    }

                                break;
                        }

                        GridManager.instance
                            .UpdateObstacleArray(); //for the square gizmos to appear properly at GridManager/ShowObstacleBlocks
                        break;
                    }

                    processCounter = 0;

                    //TODO: CHECK IT
                    // for (var j = 0;
                    //      j <= instantiationGroupIndex;
                    //      j++) //the index for TargetBuildingIndex is the unit group !!!
                    //     // if (targetStructureIndex[j] == structureIndex)
                    //     ProcessUnitGroup(j);
                }
            }
        }
    }

    public void InitializeCurrentDamage()
    {
        foreach (var building in Buildings) currentDamage.Add(building.structureIndex, 0);
    }

    public void InitializeDamageBars(List<GameObject> bars)
    {
        var i = Buildings.Select(value => value.structureIndex == 1);

        foreach (var item in Buildings.Select((value, i) => (value, i)))
            DamageBars.Add(item.value.structureIndex, bars[item.i]);
    }

    private void Talk(string text)
    {
        ((MessageController)heliosMsg).DisplayMessage(text);
    }

    private IEnumerator GarbleMessage() //EMP in progress
    {
        yield return new WaitForSeconds(2.5f);
        ((MessageController)heliosMsg).GarbleMessage();
    }


    private IEnumerator GetFirstBuilding()
    {
        yield return new WaitForSeconds(3.0f);
        Talk("Tactical situation assessment:");

        FindNearestBuilding();
    }

    public void NetworkLoadReady()
    {
        //HeliosUI.SetActive (true);
        Talk("Chimera online, ready to receive.");
        HeliosTween.Instance.PlayFadeOut();

        PrepareBuildings();
        // if (_allUnits.Count > 0) grassTarget = GameObject.FindGameObjectsWithTag("Grass").ToList();
        // StartCoroutine("GetFirstBuilding");
        networkLoadReady = true;
    }

    private void PrepareBuildings()
    {
        buildingsHealth.Clear();

        for (var i = 0; i < Buildings.Count; i++)
        {
            var structureSelector = Buildings[i].GetComponent<StructureSelector>();
            PrepareLoot(structureSelector.structureType); //adds the buildings half-value from TransData 

            var id = structureSelector.Id;
            var categoryType = structureSelector.CategoryType;

            var levels = ShopData.Instance.GetLevels(id, categoryType);

            if (levels != null)
            {
                var building = levels?.FirstOrDefault(x => ((ILevel)x).GetLevel() == structureSelector.Level);
                if (building != null)
                {
                    buildingsHealth.Add(structureSelector.structureIndex, building.HP);
                    var sl = DamageBars[structureSelector.structureIndex].GetComponent<Slider>();
                    sl.maxValue = building.HP / 100;
                    sl.value = sl.maxValue;
                }
            }

            DamageEffectsRunning.Add(structureSelector.structureIndex, false);
        }


        ((StatsBattle)statsBattle).maxStorageGold = allLootGold;
        ((StatsBattle)statsBattle).maxStorageMana = allLootMana;
        ((StatsBattle)statsBattle).UpdateUnitsNo();
    }

    private void PrepareLoot(StructureType buidingType)
    {
        var i = 0;


        foreach (var b in ShopData.Instance.BuildingsCategoryData.category)
            if (b.name.Contains(buidingType.ToString()))
            {
                i = b.id;
                break;
            }

        foreach (var b in ShopData.Instance.ArmyCategoryData.category)
            if (b.name.Contains(buidingType.ToString()))
            {
                i = b.id;
                break;
            }

        var currency = ((TransData)transData).buildingCurrency[i];

        var value = ((TransData)transData).buildingValues[i];

        BuildingValues.Add(value);
        BuildingCurrency.Add(currency);

        if (currency == "Gold")
            allLootGold += value;
        else if (currency == "Crystals")
            allLootGold += value;
        else
            allLootMana += value;
    }

    // public void FindSpecificBuilding() //user has tapped on a building
    // {
    //     aiTargetsFree.Clear();
    //
    //     grassPatches = GameObject.FindGameObjectsWithTag("Grass");
    //
    //     for (var i = 0; i < grassPatches.Length; i++)
    //         if (grassPatches[i].GetComponent<GrassSelector>().grassIndex == targetStructureIndex[selectedGroupIndex])
    //         {
    //             grassTarget[selectedGroupIndex] = grassPatches[i];
    //             break;
    //         }
    //
    //     SelectTargetGrid();
    // }

    public void FindNearestBuilding() //auto-select next target
    {
        //TODO: need to rework this part - we should use this method or - the SearchAlivePreferredBuildings()

        aiTargetsFree.Clear();
        var allGrassPatches = GameObject.FindGameObjectsWithTag("Grass");
        var selectedGrassPatches = new List<GameObject>();

        for (var i = 0; i < allGrassPatches.Length; i++)
            if (allGrassPatches[i].GetComponent<GrassSelector>().structureClass == "Building" ||
                allGrassPatches[i].GetComponent<GrassSelector>().structureClass == "Weapon")
                selectedGrassPatches.Add(allGrassPatches[i]);
        grassPatches = selectedGrassPatches.ToArray();

        if (grassPatches.Length == 0) //nothing was found on the map
        {
            mapIsEmpty = true;
            if (!battleOver)
                StartCoroutine("MissionComplete");
            return;
        }

        foreach (var unit in _allUnits)
            SearchAliveBuildings(unit.transform.position, unit.GetComponent<FighterController>());

        foreach (var unit in _allUnits)
        {
            var fighterController = unit.GetComponent<FighterController>();
            fighterController.GrassTarget =
                grassPatches[(int)fighterController.GrassTargetCoords.y].GetComponent<GrassSelector>();
            SelectTargetGrid(unit.transform.position, fighterController);
        }
    }

    private int
        GetListIndex(
            int structureIndex) //translates a real structureIndex (ex: 32) into a possibly smaller list index (ex:2)
    {
        //this happends when the user builds and destroys buildings - the structureIndex grows	
        var listIndex = 0;
        for (var i = 0; i < Buildings.Count; i++)
            if (Buildings[i].structureIndex == structureIndex)
            {
                listIndex = i;
                break;
            }

        return listIndex;
    }

    public void KillUnit(int unitIndex)
    {
        // switch (groupIndex)
        // {
        // case 0:
        for (var i = 0; i < _allUnits.Count; i++)
            if (_allUnits[i].GetComponent<Selector>().index == unitIndex)
            {
                ((SoundFX)soundFx).SoldierDie();
                _allUnits.RemoveAt(i);
                RemoveFromDeployed(unitIndex);
                break;
            }

        // break;
        // }
    }

    private void RemoveFromDeployed(int unitIndex)
    {
        for (var i = 0; i < DeployedUnits.Count; i++)
            if (DeployedUnits[i].GetComponent<Selector>().index == unitIndex)
            {
                DeployedUnits.RemoveAt(i);
                ((StatsBattle)statsBattle).unitsLost++;
                break;
            }
    }

    private void SearchAliveBuildings(Vector3 unitPosition, FighterController fighterController)
    {
        var grassTargetCoords = new List<Vector2>();
        grassPatches = GameObject.FindGameObjectsWithTag("Grass");
        for (var i = 0; i < grassPatches.Length; i++)
            if (!grassPatches[i].GetComponent<GrassSelector>().isDestroyed)
            {
                grassTargetCoords.Add(new Vector2(
                    Vector2.Distance(grassPatches[i].transform.position, unitPosition), i));
                allDestroyed = false;
            }

        grassTargetCoords.Sort(delegate(Vector2 d1, Vector2 d2) { return d1.x.CompareTo(d2.x); });
        fighterController.GrassTargetCoords = grassTargetCoords[0];

        if (allDestroyed)
        {
            mapIsEmpty = true;
            if (!battleOver)
                StartCoroutine("MissionComplete");
        }
    }

    public void SearchAlivePreferredBuildings(EntityType preferredTarget, Vector3 unitPosition,
        FighterController fighterController)
    {
        aiTargetsFree.Clear();
        // grassTargets.Clear();
        grassPatches = GameObject.FindGameObjectsWithTag("Grass");


        var grasses = new List<GrassSelector>();
        foreach (var grass in grassPatches) grasses.Add(grass.GetComponent<GrassSelector>());

        allDestroyed = grasses.All(structures => structures.isDestroyed);

        if (allDestroyed)
        {
            StartCoroutine("MissionComplete");
            return;
        }

        bool isAnyPreferredAlive = grasses.Find(v => v.EntityType == preferredTarget && !v.isDestroyed);
        if (!isAnyPreferredAlive)
        {
            FindNearestBuilding();
            return;
        }

        for (var i = 0; i < grassPatches.Length; i++)
        {
            var grassSelector = grassPatches[i].GetComponent<GrassSelector>();

            if (!grassSelector.isDestroyed && grassSelector.EntityType == preferredTarget)
            {
                fighterController.GrassTargetCoords = new Vector2(
                    Vector2.Distance(grassPatches[i].transform.position, unitPosition), i);

                fighterController.GrassTarget = grassPatches[i].GetComponent<GrassSelector>();
                SelectTargetGrid(unitPosition, fighterController);

                allDestroyed = false;
                break;
            }
        }
    }

    public void SelectTarget(Vector3 unitPosition, FighterController fighterController)
    {
        SelectTargetGrid(unitPosition, fighterController);
    }
    
    private void SelectTargetGrid(Vector3 unitPosition, FighterController fighterController)
    {
        aiTargetsFree.Clear();
        var AITargetsParentObj = fighterController.GrassTarget.transform.Find("AITargets"); //AiTargets Parent Object

        var aiTargetsChildren =
            AITargetsParentObj
                .GetComponentsInChildren<Transform>(); //Transform of all AI green squares sorounding a building, free or not

        //also selects AITargets parent, but it's in the middle

        int col = 0, row = 0;

        for (var i = 0; i < aiTargetsChildren.Length; i++) // 1 skip parent
        {
            var indexCell = GridManager.instance.GetGridIndex(aiTargetsChildren[i].position);
            col = GridManager.instance.GetColumn(indexCell);
            row = GridManager.instance.GetRow(indexCell);
            if (i == 0)
            {
                if (DeployedUnits.Count > 0)
                    Talk("New target in grid " + col + "-" + row + ".\n" +
                         "Transmitting coordinates.");
                else
                    Talk("Estimated value " + allLootGold + " gold, " + allLootMana + " mana.");
            }
            else if (!GridManager.instance.nodes[row, col].isObstacle)
            {
                aiTargetsFree.Add(
                    new Vector2(Vector2.Distance(aiTargetsChildren[i].position, unitPosition), i));
            }
        }

        aiTargetsFree.Sort(delegate(Vector2 d1, Vector2 d2) { return d1.x.CompareTo(d2.x); });

        aiTargetVectorsCurrent.Clear();

        for (var i = 0; i < aiTargetsFree.Count; i++)
            aiTargetVectorsCurrent.Add(aiTargetsChildren[(int)aiTargetsFree[i].y].position);

        for (var i = 0; i < aiTargetVectorsCurrent.Count; i++)
            fighterController.path.pathFinder.AITargetVectorsO.Add(aiTargetVectorsCurrent[i]);

        ResetSurroundIndex();

        // pauseAttack[selectedGroupIndex] = false;
    }

    private void MarkAsPassable(Transform transform)
    {
        var indexCell = GridManager.instance.GetGridIndex(transform.position);
        var col = GridManager.instance.GetColumn(indexCell);
        var row = GridManager.instance.GetRow(indexCell);
        GridManager.instance.nodes[row, col].MarkAsFree();
    }

    // private void CopyTargetLists()
    // {
    //     aiTargetVectorsO.Clear();
    //     for (var i = 0; i < aiTargetVectorsCurrent.Count; i++) aiTargetVectorsO.Add(aiTargetVectorsCurrent[i]);
    // }

    private void ResetSurroundIndex()
    {
        // surroundIndex[0] = 0;
    }

    public void StopUpdateUnits()
    {
        updatePathMaster = false;
    }

    public void StartUpdateUnits()
    {
        StartCoroutine("ResumeUpdateUnits");
    }

    public void UpdatePaths() //makes sure that not all units update paths at the same time - performance
    {
        // if (updatePathO)
        {
            // updateOTimer += Time.deltaTime;

            // if (updateOTimer > pathUpdateTime) //time between nextunit updatePath
            {
                // updateOTimer = 0;

                // if (updateOCounter < _allUnits.Count) //counter is an integer that cycles through list.Count
                // {
                //     if (_allUnits[updateOCounter] == null) return; // the unit just died
                //
                //     _allUnits[updateOCounter].GetComponent<FighterPathFinder>().FindPath();
                //     _allUnits[updateOCounter].GetComponent<FighterController>().ChangeTarget();
                //     _allUnits[updateOCounter].GetComponent<FighterController>().RevertToPathWalk();
                //
                //     updateOCounter++;
                // }
                // else
                // {
                //     ResetUpdatePaths(0);
                // }
            }
        }
    }

    private void ResetUpdatePaths(int index)
    {
        // switch (index)
        // {
        //     case 0:
        //         updatePathO = false;
        //         updateOCounter = 0;
        //         updateOTimer = 0;
        //         break;
        // }

        if (!updatePathO)
            updatePathMaster = false;
    }

    private void AdjustRubbleZ(GameObject rubble)
    {
        var pivotPos = rubble.transform.GetChild(0).position; //pivot
        var spritesPos = rubble.transform.GetChild(1).position; //sprites

        var correctiony = 10 / (pivotPos.y + 3300); //ex: fg 10 = 0.1   bg 20 = 0.05  
        //all y values must be positive, so we add the grid origin y 3207 +100 to avoid divide by 0; 
        //otherwise depth glitches around y 0

        rubble.transform.GetChild(1).position =
            new Vector3(spritesPos.x, spritesPos.y, zeroZ - correctiony); //	transform.GetChild(2).position   
    }

    private IEnumerator LateProcessUnitGroup()
    {
        processCounter += 2;
        yield return new WaitForSeconds(processCounter);

        if (((Relay)relay).deploying)
            StartCoroutine(LateProcessUnitGroup());
        else
            ProcessUnitGroup();
    }

    public void ProcessUnitGroup()
    {
        // FindNearestBuilding();
        if (battleOver)
            return;

        if (((Relay)relay).deploying)
        {
            StartCoroutine(LateProcessUnitGroup());
            return;
        }

        var groupDead = false;

        if (groupDead)
            return;


        foreach (var unit in _allUnits)
            SearchAliveBuildings(unit.transform.position, unit.GetComponent<FighterController>());

        // if (!allDestroyed)
        // {
        //     FindNearestBuilding();
        // }
        // else
        // {
        //     mapIsEmpty = true;
        //     if (!battleOver)
        //         StartCoroutine("MissionComplete");
        // }
        if (allDestroyed)

        {
            mapIsEmpty = true;
            if (!battleOver)
                StartCoroutine("MissionComplete");
        }
    }


    private void UpdateSelectStars() //the little stars above each unit
    {
        for (var i = 0; i < DeployedUnits.Count; i++)
            if (DeployedUnits[i] != null)
                DeployedUnits[i].transform.GetChild(0).gameObject.SetActive(false);

        for (var i = 0; i < selectedList.Count; i++)
            if (selectedList[i] != null)
                selectedList[i].transform.GetChild(0).gameObject.SetActive(true);
    }

    public IEnumerator UpdateTargetGroup()
    {
        yield return new WaitForSeconds(1.5f);
        if (!allDestroyed)
            UpdateTarget();
    }

    public void UpdateTarget()
    {
        // if (instantiationGroupIndex == -1)
        //     return;

        // updatePathMaster = false; //stop update sequence for all
        // StopCoroutine("ResumeUpdateUnits"); //stop resumeupdate in case it was called

        // var currentList = new List<GameObject>();
        // currentList.Clear();


        // switch (index)
        // {
        //     case 0:
        //         if (_allUnits.Count > 0)
        //         {
        //             Talk("Squad 1 moving to objective.");
        //             currentList = _allUnits;
        //             updatePathO = true;
        //             updateOCounter = 0;
        //         }
        //
        //         break;
        // }

        ResetSurroundIndex(); //this is used to suround a building with units

        StartCoroutine("ResumeUpdateUnits");
        elapsedTime = 0.0f;
        // pauseAttack[index] = false;
    }

    private IEnumerator ResumeUpdateUnits()
    {
        yield return new WaitForSeconds(0.3f);
        updatePathMaster = true;
    }

    private void DisablePanels()
    {
        ((Relay)relay).pauseInput = true;
    }

    public void Retreat()
    {
        Talk("Retreat order received.");
        StartCoroutine("MissionComplete");
    }

    private IEnumerator MissionComplete()
    {
        Talk("Well done. Return to base.");
        battleOver = true;
        DisablePanels();
        yield return new WaitForSeconds(0.5f);

        for (var i = 0; i < DeployedUnits.Count; i++)
            if (DeployedUnits[i] != null)
            {
                DeployedUnits[i].GetComponent<FighterController>().GrassTarget = null;
                DeployedUnits[i].GetComponent<FighterController>().RevertToIdle();
            }

        ActivateEndGame();
    }

    private void ActivateEndGame()
    {
        MissionCompletePanel.SetActive(true);

        goldNoLb.text = ((StatsBattle)statsBattle).gold.ToString();
        manaNoLb.text = ((StatsBattle)statsBattle).mana.ToString();
        buildingsDestroyedNoLb.text = ((StatsBattle)statsBattle).buildingsDestroyed.ToString();
        unitsLostNoLb.text = ((StatsBattle)statsBattle).unitsLost.ToString();

        var remainingUnits = 0;

        for (var i = 0; i < ((StatsBattle)statsBattle).AvailableUnits.Count; i++)
        {
            remainingUnits += ((StatsBattle)statsBattle).AvailableUnits[i].count;
            ((TransData)transData).ReturnedFromBattleUnits[i] = ((TransData)transData).GoingToBattleUnits[i];
        }

        unitsRecoveredNoLb.text = remainingUnits.ToString();

        ((SaveLoadBattle)saveLoadBattle).SaveAttack();

        ((TransData)transData).goldGained = (int)((StatsBattle)statsBattle).gold;
        ((TransData)transData).manaGained = (int)((StatsBattle)statsBattle).mana;
        ((TransData)transData).battleOver =
            true; //this variable is checked at game.unity load to see if the user is returning from battle or just started the game
        ((TransData)transData).tutorialBattleSeen = ((StatsBattle)statsBattle).tutorialBattleSeen;
    }

    public void SetDamageForStructure(int structureIndex, int damage, Shooter shooter)
    {
        if (currentDamage.Count > 0 && currentDamage[structureIndex] != null)
        {
            currentDamage[structureIndex] += damage;

            buildingsHealth[structureIndex] -= damage;
            DamageBars[structureIndex].GetComponent<Slider>().value =
                buildingsHealth[structureIndex] * 0.01f; //since full building health was 100


            // if (buildingsHealth[structureIndex] <= 0)
            // building.StructureGrassSelector
            // .isDestroyed = true;
            // shooter.FighterController.RevertToIdle();


            // var structureIndex = targetStructureIndex[k];
            // var i = GetListIndex(structureIndex);
        }

        var building = Buildings.Find(structure => structure.structureIndex == structureIndex);
        ///
        ///

        if (building.StructureGrassSelector.isDestroyed) shooter.FighterController.RevertToIdle();

        if (!DamageEffectsRunning[structureIndex] &&
            buildingsHealth[structureIndex] <
            90) //instantiates smoke the first time the building is attacked/below 90 health
        {
            //((SoundFX)SoundFx).BuildingBurn(); //sound gets too clogged, remain silent
            DamageEffectsRunning[structureIndex] = true;

            var Smoke = Instantiate(SmokePf, new Vector3(building.transform.position.x,
                building.transform.position.y,
                smokeZ), Quaternion.identity);

            Smoke.transform.parent = EffectsGroup.transform;

            var Fire = Instantiate(FirePf, new Vector3(building.transform.position.x,
                building.transform.position.y,
                fireZ), Quaternion.identity);
            Fire.transform.parent = EffectsGroup.transform;
        }

        if (buildingsHealth[structureIndex] <= 0
            && !building.StructureGrassSelector.isDestroyed
           ) //explodes once, then marks the grass as destroyed  
        {
            building.StructureGrassSelector
                .isDestroyed = true;
            shooter.FighterController.RevertToIdle();


            ((SoundFX)soundFx).BuildingExplode();
            ((StatsBattle)statsBattle).buildingsDestroyed++;

            Talk(
                "Target destroyed. Incoming EMP shockwave.\nRetreat to a safe distance.\nExpect brief communications blackout.");
            HeliosTween.Instance.PlayFadeOut();
            StartCoroutine("GarbleMessage");

            DamageBars[structureIndex].SetActive(false);


            var Explosion = Instantiate(ExplosionPf,
                new Vector3(building.transform.position.x, building.transform.position.y, smokeZ),
                Quaternion.identity);

            //to instantiate rubble of different sizes - different prefabs or you can scale down a bit
            var buildingType = building.structureType;
            var buildingClass = building.structureClass;

            if (buildingType == StructureType.Chessboard || buildingType == StructureType.Toolhouse ||
                buildingType == StructureType.Summon)
            {
                var Rubble2x2 = Instantiate(Rubble2x2Pf,
                    new Vector3(building.transform.position.x, building.transform.position.y, zeroZ),
                    Quaternion.identity);
                AdjustRubbleZ(Rubble2x2);
                Rubble2x2.transform.parent = EffectsGroup.transform;
            }

            else if (buildingClass == "Weapon" || buildingType == StructureType.Tatami)
            {
                var Rubble1x1 = Instantiate(Rubble1x1Pf,
                    new Vector3(building.transform.position.x, building.transform.position.y, zeroZ),
                    Quaternion.identity);
                AdjustRubbleZ(Rubble1x1);
                Rubble1x1.transform.parent = EffectsGroup.transform;
            }
            else
            {
                var Rubble3x3 = Instantiate(Rubble3x3Pf,
                    new Vector3(building.transform.position.x, building.transform.position.y, zeroZ),
                    Quaternion.identity);
                AdjustRubbleZ(Rubble3x3);
                Rubble3x3.transform.parent = EffectsGroup.transform;
            }

            var
                structureClass = building.structureClass;

            var structureType = building.structureType;

            if (structureClass == "Weapon" && (structureType == StructureType.Cannon ||
                                               structureType == StructureType.ArcherTower ||
                                               structureType == StructureType.Catapult))
            {
                building.GetComponentInChildren<PerimeterCollider>().enabled =
                    false; //prevent the perimeter collider from firing again
                building.GetComponent<TurretControllerBase>().fire = false;
                building.GetComponent<TurretControllerBase>().enabled = false;
                building.transform.Find("Turret").gameObject.SetActive(false);
            }

            building.transform.Find("Sprites").gameObject.SetActive(false);
            building.transform.Find("SelectBuildingButton").gameObject.SetActive(false);

            if (!building.gameObject.activeSelf) //constructions- the building is disabled
            {
                building.transform.parent.GetComponent<ConstructionSelector>().enabled = false;
                building.transform.parent.Find("Dobbit").gameObject.SetActive(false);
                building.transform.parent.Find("Sprites").gameObject.SetActive(false);
            }

            building.StructureGrassSelector.isDestroyed = true;


            allDestroyed = Buildings.All(structures => structures.StructureGrassSelector.isDestroyed);
            if (allDestroyed)
            {
                StartCoroutine("MissionComplete");
                return;
            }


            var grassType =
                building.StructureGrassSelector
                    .grassType; //this section will make the grass passable, except its center

            if (grassType == 2) return;

            //disregard type 2 grass, the rubble prefab is too big to make any portion of it passable; 
            //applies only to toolhouse, but since you can have only 1 toolhouse, there is no situation
            //when a building is surrounded with toolhouses
            var AIObstaclesParentObj =
                building.StructureGrassSelector.transform
                    .Find("AIObstacles"); //AiObstacles parent object						
            var aiObstaclesChildren =
                AIObstaclesParentObj
                    .GetComponentsInChildren<Transform>(); //all the little obstacle cubes

            switch
                (grassType) // to see the red obstacles/green targets, activate the mesh renderers of the cubes inside the battlemap grass prefabs
            {
                case 4:
                    //skip all 4 center tiles - they remain obstacles
                    for (var l = 1; l < aiObstaclesChildren.Length; l++) // 1 skip parent
                        if (l != 3 && l != 4 && l != 6 && l != 7) //skip center
                        {
                            MarkAsPassable(aiObstaclesChildren[l]);
                            aiObstaclesChildren[l].gameObject.tag = "Untagged";
                        }

                    break;
            }

            GridManager.instance
                .UpdateObstacleArray(); //for the square gizmos to appear properly at GridManager/ShowObstacleBlocks


            processCounter = 0;

            //TODO: CHECK IT
            // for (var j = 0;
            //      j <= instantiationGroupIndex;
            //      j++) //the index for TargetBuildingIndex is the unit group !!!
            //     // if (targetStructureIndex[j] == structureIndex)
            //     ProcessUnitGroup(j);
        }
    }
    
    public List<GrassSelector> GetAllBuildingTargets()
    {
        var buildingList = GameObject.FindGameObjectsWithTag("Grass");
        return buildingList.Select(building => building.GetComponent<GrassSelector>()).ToList();
    }

    public void CompleteMission()
    {
        StartCoroutine("MissionComplete");
    }

    public void DeployUnit(GameObject unitGO)
    {
        DeployedUnits.Add(unitGO);
        armyBattleController.SpawnTroop(unitGO.GetComponent<FighterController>());
    }
}