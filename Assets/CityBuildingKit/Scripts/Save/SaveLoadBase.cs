using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Assets.Scripts.Menus;
using Assets.Scripts.UIControllersAndData.Player;
using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData;
using CityBuildingKit.Scripts.UIControllersAndData.StructureType;
using Newtonsoft.Json;
using UIControllersAndData.Settings;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Buildings;
using UIControllersAndData.Store.Categories.Unit;
using UIControllersAndData.Units;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadBase : MonoBehaviour
{
    [SerializeField] private GhostHelper _ghostHelper;

    [SerializeField] private GameObject[] // populated by OtherGameData.Instance.(SCRIPTABLE OBJECTS)
        ConstructionPrefabs,
        GrassPrefabs, //three types of grass patches
        RemovablePrefabs;

    public GameObject
        RemovableTimerPf,
        UnitProc, //the data for units under construction is extracted from unitproc, 
        //that handles construction in a much simpler manner, having no graphics
        MenuUnit, //the object that holds the MenuUnit script; since it's disabled, must be linked manually;
        BuildingCannon,
        DamageBar;

    //lists for these elements - unknown number of elements
    public List<StructureSelector> //make protected
        LoadedBuildings = new();

    public List<GameObject> //make protected
        LoadedConstructions = new(),
        LoadedGrass = new(), //list is used to parent the grass to the buildings

        //LoadedCannons = new List<GameObject>(), 			//list is used to parent the cannons to the cannon group
        LoadedDamageBars = new(), //list is used to parent the damagebars to the damagebar group
        inRemovalList = new();

    public StructureCreator buildingCreator, wallCreator, weaponCreator, ambientCreator;
    public MenuMain menuMain;

    protected GameObject[] Construction, Removables; //Grass, 
    protected string cultureFormat = "en-US";

    protected GameObject
        EconomyBuildings,
        GroupConstructions,
        GroupBuildings, //object used to parent the buildings, once they are instantiated
        GroupRemovables,
        GroupWalls,
        GroupAmbients,
        GroupWeapons,
        GroupDamageBars,
        currentRemovable,
        currentStructure,
        currentGrass;

    protected int[] existingBuildings;

    protected List<EconomyBuilding>
        existingEconomyBuildings = new();

    //PlayerPrefs save variables
    protected string filePath;
    //records how many buildings of each type, when they are built/game is loaded

    protected bool //make protected
        isFileSave = true,
        isBattleMap = false; //true=filesave false=PlayerPrefs 

    protected int lineIndex;

    protected Node[,] nodes;

    protected Component
        removableCreator,
        soundFX,
        stats,
        transData,
        menuUnit,
        unitProc,
        resourceGenerator; //removableCreator, 

    //removableList = new List<GameObject>();

    //protected List<GameObject[]> removableList = new List<GameObject[]>();


    protected DateTime saveDateTime, loadDateTime; //saveTime, currentTime

    protected string savefile, currentLine;
    protected string[] savefileLines;

    //protected string filePath;

    protected StreamReader sReader;
    protected StreamWriter sWriter;
    protected TimeSpan timeDifference;


    protected int
        zeroZ = 0, //layer depths for building, correlate with BuildingCreator.cs
        grassZ = 2, //layer depths for grass, correlate with BuildingCreator.cs
        cannonZ = -5,
        damageZ = -7;

    protected void InitializeComponents()
    {
        /*
            print ("This version was released without being finished, and without the extensive three-month testing our previous versions went through." +
            "Features that are still in work:" +
            "\n- game-wide features like economy, upgrades and player progress;" +
            "\n- pathfinder and wall attacks (do not completely surround buildings with walls);" +
            "\n- since walls are not attacked, paths can be very long, which will create lag/low framerate;" +
            "\n- projectile trajectory/effects/sounds during battle are not set/rudimentary;" +
            "\n- numeric data processing, like loot/resource type on the attack map is inaccurate;" +
            "\n- minor glitches and imperfections;" +
            "\nHopefully, all these issues will be addressed in our next release.");
        */

        ConstructionPrefabs = OtherGameData.Instance.ProgressStatus.Category.ToArray();
        RemovablePrefabs = OtherGameData.Instance.Removables.Category.ToArray();
        GrassPrefabs = OtherGameData.Instance.Grass.Category.ToArray(); //three types of grass patches
        existingBuildings =
            new int[GameUnitsSettings_Handler.s
                .buildingTypesNo]; //the entire array is transfered to BuildingCreator.cs; 		

        GroupConstructions = GameObject.Find("GroupConstructions");
        GroupBuildings = GameObject.Find("GroupBuildings");
        GroupRemovables = GameObject.Find("GroupRemovables");
        GroupWalls = GameObject.Find("GroupWalls");
        GroupAmbients = GameObject.Find("GroupAmbients");
        GroupWeapons = GameObject.Find("GroupWeapons");

        //filePath = Application.persistentDataPath +"/";//iphone		

        //	buildingCreator = GameObject.Find ("BuildingCreator").GetComponent<StructureCreator> ();
        transData = GameObject.Find("TransData").GetComponent<TransData>();

        if (!isBattleMap)
        {
            EconomyBuildings = GameObject.Find("EconomyBuildings");
            removableCreator = GameObject.Find("RemovableCreator").GetComponent<RemovableCreator>();

            menuUnit = MenuUnit.GetComponent("MenuUnit"); //MenuUnit.sc - necessary when saving only
            unitProc = UnitProc.GetComponent("UnitProc"); //UnitProc.sc script

            stats = GameObject.Find("Stats").GetComponent<Stats>(); // Stats.cs for HUD data
            resourceGenerator = GameObject.Find("ResourceGenerator").GetComponent<ResourceGenerator>();
        }
        else
        {
            GroupDamageBars = GameObject.Find("GroupDamageBars");
        }

        soundFX = GameObject.Find("SoundFX").GetComponent<SoundFX>();

        loadDateTime = DateTime.Now; //current time		
        nodes = GridManager.instance.nodes;
    }

    // TODO: check AdjustStructureZ  
    protected void AdjustStructureZ(GameObject structure, int pivotIndex, int spriteIndex)
    {
        var pivot = structure.transform.GetChild(pivotIndex);
        var pivotPos = pivot.position; //pivot
        var sprites = structure.transform.GetChild(spriteIndex);
        var spritesPos = sprites.position; //sprites
        var correctiony = 10 / (pivotPos.y + 3300); //ex: fg 10 = 0.1   bg 20 = 0.05  
        //all y values must be positive, so we add the grid origin y 3207 +100 to avoid divide by 0; 
        //otherwise depth glitches around y 0
        structure.transform.GetChild(spriteIndex).position =
            new Vector3(spritesPos.x, spritesPos.y, zeroZ - correctiony); //   
    }

    protected void ReadNextLine()
    {
        if (isFileSave)
        {
            currentLine = sReader.ReadLine();
        }
        else
        {
            currentLine = savefileLines[lineIndex];
            lineIndex++;
        }
    }

    protected void InstantiatePositionalStructures()
    {
        while (currentLine != "###GridStruct###") //Buildings - read till next header is found 
        {
            //Buildings: buildingType, buildingIndex, position.x, position.y
            ReadNextLine();

            if (currentLine != "###GridStruct###") //if next category reached, skip
            {
                var lineStructure = currentLine.Split(","[0]);

                string structureClass = lineStructure[0],
                    structureType = lineStructure[1];

                int structureIndex = int.Parse(lineStructure[2]),
                    grassType = int.Parse(lineStructure[3]);

                float posX = float.Parse(lineStructure[4], CultureInfo.InvariantCulture),
                    posY = float.Parse(lineStructure[5], CultureInfo.InvariantCulture);

                var id = int.Parse(lineStructure[6]);
                var level = int.Parse(lineStructure[7]);
                var categoryType = (ShopCategoryType)Enum.Parse(typeof(ShopCategoryType), lineStructure[8]);
                var amountOfPower = lineStructure.Length > 9 ? float.Parse(lineStructure[9]) : 0;
                if (structureClass == "Building")
                {
                    var someStructure = ShopData.Instance.GetAssetForLevel(id, categoryType, level);
                    var isProductionBuilding = ShopData.Instance.IsProductionBuilding(id, categoryType, level);
                    var data = ShopData.Instance.GetStructureData(id, categoryType, level);
                    var entityType = ((BuildingsCategory)data).GetEntityType();
                    if (someStructure)
                    {
                        var Building = Instantiate(someStructure, new Vector3(posX, posY, zeroZ), Quaternion.identity);
                        var Grass = Instantiate(GrassPrefabs[grassType], new Vector3(posX, posY, grassZ),
                            Quaternion.identity);

                        if (isBattleMap)
                        {
                            //GameObject GrassCannon = (GameObject)Instantiate (BuildingCannon, new Vector3 (posX, posY, cannonZ), Quaternion.identity);
                            var GrassDamageBar = Instantiate(DamageBar);
                            GrassDamageBar.transform.SetParent(GroupDamageBars.transform);
                            GrassDamageBar.transform.position = new Vector3(posX, posY + grassType * 100, grassZ);
                            //ProcessCannon (GrassCannon, structureIndex);
                            ProcessDamageBar(GrassDamageBar, structureIndex);
                        }
                        else
                        {
                            buildingCreator.ReloadExistingStructures(id, data.MaxCountOfThisItem);
                        }

                        existingBuildings[id]++;
                        ProcessPositionalStructure(Building, Grass, structureClass, structureIndex, grassType, id,
                            level, categoryType, isProductionBuilding, amountOfPower, entityType);
                    }
                }
                else if (structureClass == "Weapon")
                {
                    var weaponStructure = ShopData.Instance.GetAssetForLevel(id, categoryType, level);
                    if (weaponStructure)
                    {
                        var data = ShopData.Instance.GetStructureData(id, ShopCategoryType.Weapon, level);

                        var Weapon = Instantiate(weaponStructure, new Vector3(posX, posY, zeroZ), Quaternion.identity);
                        var Grass = Instantiate(GrassPrefabs[grassType], new Vector3(posX, posY, grassZ),
                            Quaternion.identity);

                        if (id == 3 && categoryType == ShopCategoryType.Weapon)
                        {
                            var dronePad = Weapon.GetComponent<DronePad>();
                            if (dronePad) dronePad.LaunchDrone(id, level, categoryType);
                        }

                        if (isBattleMap)
                        {
                            //GameObject GrassCannon = (GameObject)Instantiate (BuildingCannon, new Vector3 (posX, posY, cannonZ), Quaternion.identity);
                            var GrassDamageBar = Instantiate(DamageBar);
                            GrassDamageBar.transform.SetParent(GroupDamageBars.transform);
                            GrassDamageBar.transform.position = new Vector3(posX, posY + grassType * 100, grassZ);
                            //ProcessCannon (GrassCannon, structureIndex);
                            ProcessDamageBar(GrassDamageBar, structureIndex);
                        }
                        else
                        {
                            weaponCreator.ReloadExistingStructures(id, data.MaxCountOfThisItem);
                        }

                        ProcessPositionalStructure(Weapon, Grass, structureClass, structureIndex, grassType, id, level,
                            categoryType);
                    }
                }
            }
        }
    }

    protected void InstantiateGridStructures()
    {
        while (currentLine != "###Construction###") //Walls
        {
            //Construction: buildingType, constructionIndex, buildingTime, remainingTime, storageIncrease, position.x, position.y
            ReadNextLine();
            if (currentLine != "###Construction###")
            {
                var lineStructure = currentLine.Split(","[0]); //reads the line, values separated by ","

                var structureClass = lineStructure[0];
                var structureType = Enum.Parse<StructureType>(lineStructure[1]);
                var structureIndex = int.Parse(lineStructure[2]);

                var iRow = int.Parse(lineStructure[3]);
                var jCol = int.Parse(lineStructure[4]);

                var id = int.Parse(lineStructure[5]);
                var level = int.Parse(lineStructure[6]);
                var categoryType = (ShopCategoryType)Enum.Parse(typeof(ShopCategoryType), lineStructure[7]);

                var posX = nodes[iRow, jCol].position.x;
                var posY = nodes[iRow, jCol].position.y;


                var prefabIndex = -1;

                if (structureClass == "StoneWall")
                {
                    switch (structureType)
                    {
                        case StructureType.StoneTower:
                            prefabIndex = 0;
                            break;
                        case StructureType.StoneWallNE:
                            prefabIndex = 1;
                            break;
                        case StructureType.StoneWallNW:
                            prefabIndex = 2;
                            break;
                    }

                    var wallStructure = ShopData.Instance.GetAssetForLevel(id, categoryType, level);
                    if (wallStructure)
                    {
                        var Wall = Instantiate(wallStructure, new Vector3(posX, posY, zeroZ), Quaternion.identity);
                        var Grass = Instantiate(GrassPrefabs[1], new Vector3(posX, posY, grassZ), Quaternion.identity);

                        var data = ShopData.Instance.GetStructureData(id, categoryType, level);

                        if (!isBattleMap) wallCreator.ReloadExistingStructures(prefabIndex, data.MaxCountOfThisItem);

                        ProcessGridBasedStructure(Wall, Grass, structureClass, structureType, structureIndex, iRow,
                            jCol, id, level, categoryType);
                    }
                }
                else if (structureClass == "WoodFence")
                {
                    switch (structureType)
                    {
                        case StructureType.WoodCornerN:
                            prefabIndex = 0;
                            break;
                        case StructureType.WoodCornerS:
                            prefabIndex = 1;
                            break;
                        case StructureType.WoodCornerE:
                            prefabIndex = 2;
                            break;
                        case StructureType.WoodCornerW:
                            prefabIndex = 3;
                            break;
                        case StructureType.WoodFenceNE:
                            prefabIndex = 4;
                            break;
                        case StructureType.WoodFenceNW:
                            prefabIndex = 5;
                            break;
                        case StructureType.WoodEndNE:
                            prefabIndex = 6;
                            break;
                        case StructureType.WoodEndNW:
                            prefabIndex = 7;
                            break;
                        case StructureType.WoodEndSE:
                            prefabIndex = 8;
                            break;
                        case StructureType.WoodEndSW:
                            prefabIndex = 9;
                            break;
                    }

                    var fenceStructure = ShopData.Instance.GetAssetForLevel(id, categoryType, level);
                    if (fenceStructure)
                    {
                        var Fence = Instantiate(fenceStructure, new Vector3(posX, posY, zeroZ),
                            Quaternion.identity);
                        var Grass = Instantiate(GrassPrefabs[1], new Vector3(posX, posY, grassZ),
                            Quaternion.identity);


                        var data = ShopData.Instance.GetStructureData(id, categoryType, level);
                        if (!isBattleMap) wallCreator.ReloadExistingStructures(prefabIndex, data.MaxCountOfThisItem);

                        ProcessGridBasedStructure(Fence, Grass, structureClass, structureType, structureIndex, iRow,
                            jCol, id, level, categoryType);
                    }
                }
                else if (structureClass == "Ambient")
                {
                    var ambientStructure = ShopData.Instance.GetAssetForLevel(id, categoryType, level);
                    if (ambientStructure)
                    {
                        var ambient = Instantiate(ambientStructure, new Vector3(posX, posY, zeroZ),
                            Quaternion.identity);
                        var grass = Instantiate(GrassPrefabs[0], new Vector3(posX, posY, grassZ), Quaternion.identity);
                        var data = ShopData.Instance.GetStructureData(id, categoryType, level);
                        if (!isBattleMap)
                            ambientCreator.ReloadExistingStructures(id, data.MaxCountOfThisItem);

                        ProcessGridBasedStructure(ambient, grass, structureClass, structureType, structureIndex, iRow,
                            jCol, id, level, categoryType);
                    }
                }
            }
        }
    }

    protected void InstantiateConstructions() //adapt to all prefab types
    {
        while (currentLine != "###Removables###") //Construction
        {
            //Construction: buildingType, constructionIndex, buildingTime, remainingTime, storageIncrease, position.x, position.y
            ReadNextLine();
            if (currentLine != "###Removables###")
            {
                var lineConstruction = currentLine.Split(","[0]); //reads the line, values separated by ","

                var structureClass = lineConstruction[0];
                var structureType = Enum.Parse<StructureType>(lineConstruction[1]);

                int structureIndex = int.Parse(lineConstruction[2]),
                    grassType = int.Parse(lineConstruction[3]),
                    buildingTime = int.Parse(lineConstruction[4]),
                    remainingTime = int.Parse(lineConstruction[5]),
                    storageAdd = int.Parse(lineConstruction[6]),
                    iRow = int.Parse(lineConstruction[7]),
                    jCol = int.Parse(lineConstruction[8]);

                var id = int.Parse(lineConstruction[9]);
                var level = int.Parse(lineConstruction[10]);
                var categoryType = (ShopCategoryType)Enum.Parse(typeof(ShopCategoryType), lineConstruction[11]);

                float posX = float.Parse(lineConstruction[12], CultureInfo.InvariantCulture),
                    posY = float.Parse(lineConstruction[13], CultureInfo.InvariantCulture);

                var Grass = Instantiate(GrassPrefabs[grassType], new Vector3(posX, posY, grassZ), Quaternion.identity);
                var constructionPrefab = Instantiate(ConstructionPrefabs[grassType], new Vector3(posX, posY, zeroZ),
                    Quaternion.identity);

                var construction = constructionPrefab.GetComponent<ConstructionSelector>();
                construction.Id = id;
                construction.Level = level;
                construction.CategoryType = categoryType;
                var data = ShopData.Instance.GetStructureData(id, categoryType, level);
                if (structureClass == "Building")
                {
                    var someStructure = ShopData.Instance.GetAssetForLevel(id, categoryType, level);

                    var Building = Instantiate(someStructure, new Vector3(posX, posY, zeroZ), Quaternion.identity);
                    ProcessConstruction(Building, constructionPrefab, Grass, structureClass, structureType,
                        structureIndex, grassType, buildingTime, remainingTime, storageAdd, iRow, jCol);
                    if (isBattleMap)
                    {
                        //GameObject GrassCannon = (GameObject)Instantiate (BuildingCannon, new Vector3 (posX, posY, cannonZ), Quaternion.identity);
                        var GrassDamageBar = Instantiate(DamageBar);
                        GrassDamageBar.transform.SetParent(GroupDamageBars.transform);
                        GrassDamageBar.transform.position = new Vector3(posX, posY + grassType * 100, grassZ);
                        //ProcessCannon (GrassCannon, structureIndex);
                        ProcessDamageBar(GrassDamageBar, structureIndex);
                    }
                    else
                    {
                        buildingCreator.ReloadExistingStructures(id, data.MaxCountOfThisItem);
                    }

                    existingBuildings[id]++;
                }
                else if (structureClass == "Weapon")
                {
                    if (isBattleMap)
                    {
                        //GameObject GrassCannon = (GameObject)Instantiate (BuildingCannon, new Vector3 (posX, posY, cannonZ), Quaternion.identity);
                        var GrassDamageBar = Instantiate(DamageBar);
                        GrassDamageBar.transform.SetParent(GroupDamageBars.transform);
                        GrassDamageBar.transform.position = new Vector3(posX, posY + grassType * 100, grassZ);
                        //ProcessCannon (GrassCannon, structureIndex);
                        ProcessDamageBar(GrassDamageBar, structureIndex);
                    }
                    else
                    {
                        weaponCreator.ReloadExistingStructures(id, data.MaxCountOfThisItem);
                    }

                    var weaponStructure = ShopData.Instance.GetAssetForLevel(id, categoryType, level);
                    if (weaponStructure)
                    {
                        var Weapon = Instantiate(weaponStructure, new Vector3(posX, posY, zeroZ), Quaternion.identity);
                        ProcessConstruction(Weapon, constructionPrefab, Grass, structureClass, structureType,
                            structureIndex, grassType, buildingTime, remainingTime, storageAdd, iRow, jCol);
                    }
                }
                else if (structureClass == "Ambient")
                {
                    if (!isBattleMap)
                        ambientCreator.ReloadExistingStructures(id, data.MaxCountOfThisItem);

                    var ambientStructure = ShopData.Instance.GetAssetForLevel(id, categoryType, level);
                    if (ambientStructure)
                    {
                        var Ambient = Instantiate(ambientStructure, new Vector3(posX, posY, zeroZ),
                            Quaternion.identity);
                        ProcessConstruction(Ambient, constructionPrefab, Grass, structureClass, structureType,
                            structureIndex, grassType, buildingTime, remainingTime, storageAdd, iRow, jCol);
                    }
                }
            }
        }
    }

    protected void InstantiateRemovables()
    {
        var removeTimes = new int[GameUnitsSettings_Handler.s.removableTypesNo];

        if (!isBattleMap)
            removeTimes = ((RemovableCreator)removableCreator).removeTimes;
        else
            removeTimes = ((TransData)transData).removeTimes;

        while (currentLine != "###RemovableTimers###") //Removables
        {
            //Construction: buildingType, constructionIndex, buildingTime, remainingTime, storageIncrease, position.x, position.y
            ReadNextLine();
            if (currentLine != "###RemovableTimers###")
            {
                var lineRemovable = currentLine.Split(","[0]); //reads the line, values separated by ","

                var removableType = lineRemovable[0];
                var removableIndex = int.Parse(lineRemovable[1]);
                var inRemoval = int.Parse(lineRemovable[2]);

                var iColumn = int.Parse(lineRemovable[3]);
                var jRow = int.Parse(lineRemovable[4]);

                var posX = nodes[iColumn, jRow].position.x;
                var posY = nodes[iColumn, jRow].position.y;

                var Grass = Instantiate(GrassPrefabs[1], new Vector3(posX, posY, grassZ), Quaternion.identity);

                int removeTime = 0,
                    prefabIndex = -1;

                switch (removableType)
                {
                    case "ClamA":
                        prefabIndex = 0;
                        break;
                    case "ClamB":
                        prefabIndex = 1;
                        break;
                    case "ClamC":
                        prefabIndex = 2;
                        break;
                    case "TreeA":
                        prefabIndex = 3;
                        break;
                    case "TreeB":
                        prefabIndex = 4;
                        break;
                    case "TreeC":
                        prefabIndex = 5;
                        break;
                    case "TreeD":
                        prefabIndex = 6;
                        break;
                }

                var Removable = Instantiate(RemovablePrefabs[prefabIndex], new Vector3(posX, posY, zeroZ),
                    Quaternion.identity);
                //if(!isBattleMap)
                removeTime = removeTimes[prefabIndex];

                ProcessRemovable(Removable, Grass, removableType, removableIndex, inRemoval, removeTime, iColumn, jRow);
            }
        }
    }

    protected void InstantiateRemovableTimers()
    {
        while (currentLine != "###Numerics###") //Removable Timers
        {
            //Construction: buildingType, constructionIndex, buildingTime, remainingTime, storageIncrease, position.x, position.y
            ReadNextLine();
            if (currentLine != "###Numerics###")
            {
                var lineRemovableTimer = currentLine.Split(","[0]); //reads the line, values separated by ","

                var timerIndex = int.Parse(lineRemovableTimer[0]);
                var remainingTime = int.Parse(lineRemovableTimer[1]);

                var RemovableTimer = Instantiate(RemovableTimerPf, new Vector3(), Quaternion.identity);

                ProcessRemovableTimer(RemovableTimer, timerIndex, remainingTime);
            }
        }
    }

    protected void ProcessPositionalStructure(GameObject structure, GameObject grass, string structureClass,
        int structureIndex, int grassType, int id, int level, ShopCategoryType categoryType,
        bool isProductionBuilding = false, float amountOfPower = 0f,
        EntityType entityType = EntityType.Any) //string buildingTag,
    {
        var sSel = structure.GetComponent<StructureSelector>();
        var powerStorage = structure.GetComponent<PowerStorage>();
        var trainingCamp = structure.GetComponent<TrainingCamp>();

        sSel.structureIndex = structureIndex; //unique int to pair buildings and the grass underneath
        sSel.grassType = grassType;
        sSel.inConstruction = false;

        sSel.Id = id;
        sSel.Level = level;
        sSel.CategoryType = categoryType;

        if (powerStorage != null)
        {
            powerStorage.CurrentAmountOfPower = amountOfPower;
            powerStorage.Initialize();
        }

        if (trainingCamp) trainingCamp.Initialize();

        var initializer = structure.GetComponent<IInitializer>();

        if (initializer != null) initializer.Initialize();


        var gSel = grass.GetComponent<GrassSelector>();
        gSel.StructureIndex = structureIndex;
        gSel.structureClass = structureClass;

        gSel.EntityType = entityType;

        gSel.ObjTransform = structure.transform;

        sSel.battleMap = isBattleMap;

        grass.transform.parent = structure.transform;

        switch (structureClass)
        {
            case "Building":
                structure.transform.parent = GroupBuildings.transform;
                break;
            case "Weapon":
                structure.transform.parent = GroupWeapons.transform;
                break;
        }

        if (!isBattleMap)
            if (isProductionBuilding)
            {
                sSel.isProductionBuilding = true;
                sSel.LateRegisterAsProductionBuilding();
            }

        LoadedGrass.Add(grass);
        LoadedBuildings.Add(sSel); //the list is sorted and then used to pair the buildings and the grass by index
        //all grass is recorded in the same list, for both buildings and constructions; after they are sorted by index, 
        //the first batch goes to finished buildings, the rest goes to underconstruction
        AdjustStructureZ(structure, 1, 2);
    }

    protected void ProcessGridBasedStructure(GameObject structure, GameObject grass, string structureClass,
        StructureType structureType, int structureIndex, int iRow, int jCol, int id, int level,
        ShopCategoryType categoryType) // int removeTime, int goldPrice, int manaPrice, int goldGain, int manaGain, 
    {
        var sSel = structure.GetComponent<StructureSelector>();

        sSel.structureType = structureType;
        sSel.structureIndex = structureIndex;
        sSel.iRow = iRow;
        sSel.jCol = jCol;

        sSel.battleMap = isBattleMap;

        sSel.Id = id;
        sSel.Level = level;
        sSel.CategoryType = categoryType;

        currentStructure = structure;

        var gSel = grass.GetComponent<GrassSelector>();
        gSel.StructureIndex = structureIndex;
        gSel.structureClass = structureClass;
        currentGrass = grass;

        AdjustStructureZ(currentStructure, 1, 2);
        currentGrass.transform.parent = currentStructure.transform;

        switch (structureClass)
        {
            case "StoneWall":
                structure.transform.parent = GroupWalls.transform;
                break;
            case "WoodFence":
                structure.transform.parent = GroupWalls.transform;
                break;
            case "Ambient":
                structure.transform.parent = GroupAmbients.transform;
                break;
        }
    }

    private void ProcessDamageBar(GameObject damageBar, int grassIndex)
    {
        ((Selector)damageBar.GetComponent("Selector")).index = grassIndex;
        ((Selector)damageBar.GetComponent("Selector")).isSelected = false;
        //damageBar.transform.SetParent(GroupDamageBars.transform);
        LoadedDamageBars.Add(damageBar);
    }


    protected void ProcessConstruction(GameObject structure, GameObject construction, GameObject grass,
        string structureClass, StructureType structureType, int structureIndex, int grassType,
        int buildingTime, int remainingTime, int storageAdd, int iRow, int jCol)
    {
        var cSel = construction.GetComponent<ConstructionSelector>();


        cSel.structureClass = structureClass;
        cSel.StructureType = structureType;
        cSel.constructionIndex = structureIndex;
        cSel.buildingTime = buildingTime;

        cSel.battleMap = isBattleMap;
        cSel.grassType = grassType;
        construction.GetComponentInChildren<Slider>().value = 1 - (float)remainingTime / buildingTime;
        cSel.remainingTime = remainingTime;

        cSel.storageAdd = storageAdd;
        cSel.iRow = iRow;
        cSel.jCol = jCol;

        cSel.isSelected = false;

        if (isBattleMap)
            for (var i = 0; i < 3; i++)
                construction.transform.GetChild(0).transform.GetChild(i).gameObject
                    .SetActive(false); //1 deactivate finish now button/progress bar display

        LoadedConstructions.Add(construction);

        AdjustStructureZ(construction, 1, 3);

        cSel.DetermineParentGroup();

        grass.transform.parent = construction.transform;
        grass.GetComponent<GrassSelector>().structureClass = structureClass;
        grass.GetComponent<GrassSelector>().StructureIndex = structureIndex;
        structure.transform.parent = construction.transform;
        structure.SetActive(false);
        construction.transform.parent = GroupConstructions.transform;

        var sSel = structure.GetComponent<StructureSelector>();

        sSel.structureIndex = structureIndex;
        sSel.grassType = grassType;
        sSel.battleMap = isBattleMap;
        //((StructureSelector)buildingSelector).isSelected = false;	

        if (isBattleMap && structureClass != "Ambient") //&& structureType!="Bomb"
        {
            LoadedGrass.Add(grass);
            LoadedBuildings.Add(sSel);
        }

        AdjustStructureZ(structure, 1, 2);
    }

    protected void ProcessRemovable(GameObject removable, GameObject grass, string removableType,
        int removableIndex, int inRemoval, int removeTime, int iColumn,
        int jRow) // int removeTime, int goldPrice, int manaPrice, int goldGain, int manaGain, 
    {
        var rSel = removable.GetComponent<RemovableSelector>();

        rSel.removableIndex = removableIndex;
        rSel.iColumn = iColumn;
        rSel.jRow = jRow;

        //if(isBattleMap)
        rSel.battleMap = isBattleMap;

        if (inRemoval == 1)
        {
            inRemovalList.Add(removable); //prepare to attach timers
            rSel.removeTime = removeTime;
            rSel.inRemoval = 1;
            //records the removeTime, only for removables already inRemoval
        }

        currentRemovable = removable;

        var gSel = grass.GetComponent<GrassSelector>();
        gSel.StructureIndex = removableIndex;

        currentGrass = grass;
        currentGrass.transform.parent = currentRemovable.transform;
        currentRemovable.transform.parent = GroupRemovables.transform;
    }

    protected void ProcessRemovableTimer(GameObject removableTimer, int timerIndex, int remainingTime)
    {
        var rtSel = removableTimer.GetComponent<RemovableTimerSelector>();
        rtSel.removableIndex = timerIndex;
        rtSel.remainingTime = remainingTime;
        rtSel.inRemovalB = true;

        rtSel.battleMap = isBattleMap;

        if (isBattleMap)
            removableTimer.transform.GetChild(0).transform.GetChild(1).gameObject
                .SetActive(false); //deactivate finish now button

        for (var i = 0; i < inRemovalList.Count; i++)
            if (inRemovalList[i].GetComponent<RemovableSelector>().removableIndex == timerIndex)
            {
                //passes the removeTime, only for removables already inRemoval
                rtSel.removeTime = inRemovalList[i].GetComponent<RemovableSelector>().removeTime;
                removableTimer.transform.position = inRemovalList[i].transform.position;
                removableTimer.transform.parent = inRemovalList[i].transform;
                break;
            }
    }

    protected void LateCalculateElapsedTime()
    {
        StartCoroutine("CalculateElapsedTime");
    }

    private IEnumerator CalculateElapsedTime() //protected void
    {
        yield return new WaitForSeconds(2);

        timeDifference = loadDateTime.Subtract(saveDateTime);

        //everything converted to minutes
        var elapsedTime = timeDifference.Days * 24 * 60 + timeDifference.Hours * 60 + timeDifference.Minutes;

        if (!isBattleMap)
            ((ResourceGenerator)resourceGenerator).FastPaceEconomyAll(elapsedTime);


        var constructionsInProgress = GameObject.FindGameObjectsWithTag("Construction");

        for (var i = 0; i < constructionsInProgress.Length; i++)
        {
            var cSel = constructionsInProgress[i]
                .GetComponent<ConstructionSelector>(); //GetComponent ("ConstructionSelector")

            var buildingTime = cSel.buildingTime;
            var remainingTime = cSel.remainingTime;

            Component slider = (Slider)constructionsInProgress[i].GetComponentInChildren(typeof(Slider));

            //Finish constructions of any kind if elapsed time>construction time
            if (elapsedTime >= remainingTime)
            {
                if (cSel.StructureType == StructureType.Forge || cSel.StructureType == StructureType.Generator ||
                    cSel.StructureType == StructureType.PowerPlant)
                {
                    var timeSpan = elapsedTime - remainingTime;
                    cSel.finishedOffLine = true;
                    cSel.elapsedTime = timeSpan;

                    //print(timeSpan.ToString());
                }

                ((Slider)slider.GetComponent("Slider")).value = 1; //(UISlider)slider.GetComponent ("UISlider"))
                cSel.progCounter = 1.0f;

                //print("elapsedTime-remainingTime = " + (elapsedTime-remainingTime).ToString());
            }
            else //everything under 1 minute will appear as finished at reload - int approximation, not an error
                //Update progress bars for constructions which are not finished
            {
                ((Slider)slider.GetComponent("Slider")).value += elapsedTime / (float)buildingTime;
            }
        }

        var removableTimers = GameObject.FindGameObjectsWithTag("RemovableTimer");

        for (var i = 0; i < removableTimers.Length; i++)
        {
            var rtSel = removableTimers[i].GetComponent<RemovableTimerSelector>();

            var removeTime = rtSel.removeTime;
            var remainingTime = rtSel.remainingTime;

            var slider = removableTimers[i].GetComponentInChildren(typeof(Slider));

            if (elapsedTime >= remainingTime)
            {
                ((Slider)slider.GetComponent("Slider")).value = 1.0f;
                rtSel.progCounter = 1.0f;
            }
            else
            {
                //everything under 1 minute will appear as finished at reload - int approximation, not an error
                ((Slider)slider.GetComponent("Slider")).value += elapsedTime / (float)removeTime;
            }
        }

        //Calculate the progession in unit construction que
        if (!isBattleMap)
        {
            var substractTime = elapsedTime;

            //Units

            var queList = new List<Vector4>(); //qIndex, objIndex, trainingIndex

            queList = ((UnitProc)unitProc).queList;
            queList.Sort(delegate(Vector4 v1, Vector4 v2) { return v1.x.CompareTo(v2.x); });

            var numberOfUnits = GameUnitsSettings_Handler.s.unitsNo;
            var trainingTimes = new int[numberOfUnits];
            trainingTimes = ((UnitProc)unitProc).trainingTimes;

            int currentTrainingTime;

            for (var i = 0; i < queList.Count; i++)
                if (substractTime > 0)
                {
                    currentTrainingTime = trainingTimes[(int)queList[i].y];
                    var trainingIndex = (int)queList[i].z;

                    while (trainingIndex > 0)
                        if (substractTime > currentTrainingTime)
                        {
                            substractTime -= currentTrainingTime;


                            //TODO : fix the commented line
                            var ind = ((Stats)stats).ExistingUnits.FindIndex(x => x.id == (int)queList[i].y);
                            if (ind != -1)
                                ((Stats)stats).ExistingUnits[ind].count++;
                            else
                                ((Stats)stats).ExistingUnits.Add(new ExistedUnit
                                {
                                    id = (int)queList[i].y,
                                    count = 1,
                                    level = (int)queList[i].w
                                });


                            trainingIndex--;
                            ((UnitProc)unitProc).timeRemaining -= currentTrainingTime;
                            queList[i] = new Vector3(queList[i].x, queList[i].y, trainingIndex);
                        }
                        else
                        {
                            ((UnitProc)unitProc).currentTrainingTime = currentTrainingTime;
                            ((UnitProc)unitProc).currentSlidVal += substractTime / (float)currentTrainingTime;

                            if (currentTrainingTime - substractTime > 0)
                                ((UnitProc)unitProc).timeRemaining = currentTrainingTime - substractTime;
                            else
                                ((UnitProc)unitProc).timeRemaining = 1;

                            queList[i] = new Vector3(queList[i].x, queList[i].y, trainingIndex);
                            substractTime = 0;
                            break;
                        }
                }
                else
                {
                    break;
                }

            var allZero = true;

            for (var i = 0; i < queList.Count; i++)
                if ((int)queList[i].z != 0)
                {
                    allZero = false;
                    break;
                }

            if (allZero)
            {
                ((UnitProc)unitProc).queList.Clear();
                UnitProc.SetActive(false);
            }
            else
            {
                ((UnitProc)unitProc).queList = queList;
                //((UnitProc)unitProc).SortList();//irrelevant - our qlist is already sorted
            }

            //Buildings

            //((ResourceGenerator)resourceGenerator).InitializeEconomy();

            StartCoroutine("LateUpdateEconomy");
            StartCoroutine("LateUpdateUI"); //some data reaches stats with latency
        }
    }

    private IEnumerator LateUpdateUI()
    {
        yield return new WaitForSeconds(0.50f);
        //((Stats)stats).ApplyMaxCaps ();
        ((Stats)stats)
            .UpdateUI(); //one-time interface update; called only after changes - once a second because of production
        ((Stats)stats).UpdateUnitsNo();
    }

    private IEnumerator LateUpdateEconomy()
    {
        yield return new WaitForSeconds(2.00f);

        foreach (var eB in existingEconomyBuildings)
            for (var i = 0; i < ((ResourceGenerator)resourceGenerator).ExistingEconomyBuildings.Count; i++)
                if (((ResourceGenerator)resourceGenerator).ExistingEconomyBuildings[i].structureIndex ==
                    eB.structureIndex)
                {
                    ((ResourceGenerator)resourceGenerator).ExistingEconomyBuildings[i].ModifyGoldAmount(eB.storedGold);
                    ((ResourceGenerator)resourceGenerator).ExistingEconomyBuildings[i].ModifyManaAmount(eB.storedMana);
                    break;
                }
    }

    private void InstantiateObjects()
    {
        foreach (Transform child in GroupBuildings.transform)
            if (child.tag == "Structure")
                Destroy(child.gameObject);
        ;

        foreach (Transform child in GroupWeapons.transform)
            if (child.tag == "Structure")
                Destroy(child.gameObject);


        InstantiatePositionalStructures();
        InstantiateGridStructures();

        InstantiateConstructions();
        InstantiateRemovables();
        InstantiateRemovableTimers();

        if (!isBattleMap)
            UpdateButtons();
    }

    private void UpdateButtons()
    {
        buildingCreator.UpdateButtons();
        wallCreator.UpdateButtons();
        weaponCreator.UpdateButtons();
        ambientCreator.UpdateButtons();
    }

    private void ClearLists()
    {
        LoadedBuildings.Clear();
        LoadedConstructions.Clear();
        LoadedGrass.Clear();
        //LoadedCannons.Clear ();
        LoadedDamageBars.Clear();
    }

    protected void LoadGame() //string savefile
    {
        
        if (isFileSave)
        {
            MessageController.Instance.DisplayMessage("Loading from LocalFile...");
        }
        else
        {
            savefileLines = savefile.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            lineIndex = 0;

            MessageController.Instance.DisplayMessage("Loading from PlayerPrefs...");
        }

        if (!isBattleMap)
            ((Stats)stats).gameWasLoaded = true;

        ClearLists();

        currentLine = "";

        while
            (currentLine !=
             "###PosStruct###") //goes through the headers, without doing anything, till it finds the ###Buildings### header
            ReadNextLine();

        InstantiateObjects();
        if (!isBattleMap)
            ((Stats)stats).removablesCreated = true;

        ReadNextLine();
        if (!isBattleMap)
        {
            var structureIndexes = currentLine.Split(","[0]);
            buildingCreator.structureIndex = int.Parse(structureIndexes[0]);
            wallCreator.structureIndex = int.Parse(structureIndexes[1]);
            weaponCreator.structureIndex = int.Parse(structureIndexes[2]);
        }

        //UNITS
        ReadNextLine(); //#Add verification for empty que

        if (!isBattleMap)
        {
            UnitProc.SetActive(true);

            var currentUnitinProgress = currentLine.Split(","[0]);

            ((UnitProc)unitProc).currentSlidVal = float.Parse(currentUnitinProgress[0], CultureInfo.InvariantCulture);
            ((UnitProc)unitProc).currentTrainingTime =
                int.Parse(currentUnitinProgress[1], CultureInfo.InvariantCulture);
        }

        ReadNextLine(); //load training times
        if (!isBattleMap)
        {
            var trainingTimes = currentLine.Split(","[0]);

            for (var i = 0; i < trainingTimes.Length; i++)
            {
                var value = int.Parse(trainingTimes[i]);
                ((UnitProc)unitProc).trainingTimes[i] = value;
                ((MenuUnit)menuUnit).trainingTimes[i] =
                    value; //info is loaded late from xml; user could finish all units with one crystal
            }
        }

        ReadNextLine(); //load population weights
        if (!isBattleMap)
        {
            var populationWeights = currentLine.Split(","[0]);
            for (var i = 0; i < populationWeights.Length; i++)
            {
                var value = int.Parse(populationWeights[i]);
                ((UnitProc)unitProc).sizePerUnit[i] = value;
                ((MenuUnit)menuUnit).sizePerUnit[i] = value; //info is loaded late from xml;
                ((Stats)stats).sizePerUnit[i] = value;
            }
        }

        ReadNextLine(); //load existing units

        if (!isBattleMap)
        {
            ((Stats)stats).ExistingUnits = JsonConvert.DeserializeObject<List<ExistedUnit>>(currentLine);

            ((UnitProc)unitProc).queList.Clear();


            while (currentLine != "###Economy###")
            {
                ReadNextLine();
                if (currentLine != "###Economy###")
                {
                    var unitQue = currentLine.Split(","[0]);

                    if (currentLine != "###Economy###")
                        ((UnitProc)unitProc).queList.Add(new Vector4(
                            float.Parse(unitQue[0], CultureInfo.InvariantCulture),
                            float.Parse(unitQue[1], CultureInfo.InvariantCulture),
                            float.Parse(unitQue[2], CultureInfo.InvariantCulture),
                            float.Parse(unitQue[3], CultureInfo.InvariantCulture)));
                }
            }

            //((UnitProc)unitProc).start = true;
            ((UnitProc)unitProc).SortList();
            ((MenuUnit)menuUnit).unitProc = (UnitProc)unitProc;
        }
        else
        {
            while (currentLine != "###Economy###")
                ReadNextLine();
        }

        if (!isBattleMap)
            while (currentLine != "###Stats###")
            {
                ReadNextLine();
                if (currentLine != "###Stats###")
                {
                    var economyParams = currentLine.Split(","[0]);

                    var eB = EconomyBuildings.AddComponent<EconomyBuilding>(); //search				
                    eB.structureIndex = int.Parse(economyParams[0]);
                    eB.storedGold = int.Parse(economyParams[1]);
                    eB.storedMana = int.Parse(economyParams[2]);
                    existingEconomyBuildings.Add(eB);
                }
            }
        else
            while (currentLine != "###Stats###")
                ReadNextLine();

        //Stats: experience,builder,occupiedBuilder,gold,mana,crystal,,maxStorageGold,maxStroageMana,maxCrystals,forgeRates,generatorRates
        ReadNextLine();

        var statsTx = currentLine.Split(","[0]);

        if (!isBattleMap)
        {
            ((Stats)stats).level = int.Parse(statsTx[0]);
            ((Stats)stats).townHallLevel = int.Parse(statsTx[1]);
            ((Stats)stats).structureIndex = int.Parse(statsTx[2]);
            ((Stats)stats).experience = int.Parse(statsTx[3]);
            ((Stats)stats).maxExperience = int.Parse(statsTx[4]);
            ((Stats)stats).builders = int.Parse(statsTx[5]);
            ((Stats)stats).occupiedBuilders = int.Parse(statsTx[6]);

            ((Stats)stats).occupiedHousing = int.Parse(statsTx[7]);
            ((Stats)stats).maxHousing = int.Parse(statsTx[8]);

            ((Stats)stats).gold = int.Parse(statsTx[9]);
            ((Stats)stats).mana = int.Parse(statsTx[10]);
            ((Stats)stats).crystals = int.Parse(statsTx[11]);

            ((Stats)stats).maxGold = int.Parse(statsTx[12]);
            ((Stats)stats).maxMana = int.Parse(statsTx[13]);
            ((Stats)stats).maxCrystals = int.Parse(statsTx[14]);
        }


        var tutorialCitySeen = bool.Parse(statsTx[15]); //Convert.ToBoolean(statsTx [11]);
        if (Stats.Instance.ShowTutorial && tutorialCitySeen && _ghostHelper) _ghostHelper.StopTutorial();

        if (!isBattleMap)
            ((Stats)stats).TutorialCitySeen = tutorialCitySeen;

        if (!isBattleMap)
        {
            var tutorialBattleSeen = bool.Parse(statsTx[16]);
            ((Stats)stats).tutorialBattleSeen = tutorialBattleSeen;

            var removablesCreated = bool.Parse(statsTx[17]);
            ((Stats)stats).removablesCreated = removablesCreated;

            bool
                soundOn = bool.Parse(statsTx[18]),
                ambientOn = bool.Parse(statsTx[19]), //ambient save/load next release
                musicOn = bool.Parse(statsTx[20]);

            ((Stats)stats).power = int.Parse(statsTx[21]); // POWER
            ((Stats)stats).supplies = int.Parse(statsTx[22]); // SUPPLIES

            var data = Player.Instance.GetPlayer();
            data.SettingsData.IsSoundsEnabled = soundOn;
            data.SettingsData.IsAmbientEnabled = ambientOn;
            data.SettingsData.IsMusicEnabled = musicOn;
            SettingsPanelController.Instance.UpdateSettings();


            ((Stats)stats).UpdateUI();
        }

        ReadNextLine();

        saveDateTime = DateTime.Parse(currentLine, new CultureInfo(cultureFormat, false));

        ReadNextLine();
        ReadNextLine();
        global::MenuUnit.Instance.UnitsInfo = JsonConvert.DeserializeObject<List<UnitInfo>>(currentLine);


        // LateCalculateElapsedTime();

        if (isFileSave)
            sReader.Close();

        MessageController.Instance.DisplayMessage("Game loaded.");
    }
}