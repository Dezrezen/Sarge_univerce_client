using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Assets.Scripts.Menus;
using Newtonsoft.Json;
using UnityEngine;

//																	## This is for local saving / loading ##  
/*
public struct ConStruct
{
	public string buildingTag;
	public int 
	structureIndex,
	buildingTime,
	remainingTime,
	storageIncrease;
}
*/
public class SaveLoadMap : SaveLoadBase
{
    public static SaveLoadMap Instance;

    private readonly string
        fileExt = ".txt";

    private readonly string
        fileNameAttack = "ServerAttack";

    //private List<ConStruct[]> conStruct = new List<ConStruct[]>();


    //protected string filePath;
    private readonly string
        fileNameLocal = "LocalSave";

    private readonly string
        fileNameServer = "ServerSave";

    private readonly List<GameObject>
        //buildings and weapons can have different base sizes
        gridStructures = new(); //grid

    private readonly List<GameObject>
        positionalStructures = new(); //grid

    //PlayerPrefs save variables
    private readonly List<string> savefileStrings = new();

    private Component relay;

    /* 	
        //save file data structure
        Grass: grassType, grassIndex, position.x, position.y
        Buildings: buildingType, structureIndex, position.x, position.y
        Construction: buildingType, constructionIndex, buildingTime, remainingTime, storageIncrease, position.x, position.y
        Removables: removableType, removableIndex, removeTime, inRemoval, goldPrice, manaPrice, goldGain, manaGain,  position.x,position.Y
        Units: currentSlidVal, currentTrainingTime
        Units: trainingTimes, existingUnits, battleUnits
        Units: queList = qIndex, objIndex, trainingIndex
        Stats: experience,builder,occupiedBuilders,currentPopulation,maxPopulation,gold,mana,crystals,maxStorageGold,maxStorageMana,maxCrystals,productionRateGold,productionRateMana,tutorialCitySeen,tutorialBattleSeen,removablesCreated,soundOn,musicOn
    */

    //public GameObject 
    //DamagePanel;

    private GameObject[] RemovableTimers; //, Buildings

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        InitializeComponents();

        relay = GameObject.Find("Relay").GetComponent<Relay>();

        filePath = Application.persistentDataPath + "/"; //other platforms'=

        //LoadFromLocal ();	//automatic local load at startup - for local testing; for multiplayer, 
        //the user must both upload/download from server to receive the attack results
    }

    private void OnApplicationQuit() //autosave
    {
        //if(!((Relay)relay).pauseInput)//check if the user is doing something, like moving buildings


        //SaveGameLocalFile();
    }


    public void SaveGamePlayerPrefs()
    {
        isFileSave = false;
        // SaveGame();
    }

    public void SaveGameLocalFile()
    {
        isFileSave = true;
        // SaveGame();
    }

    private void WriteNextLine(string newLine)
    {
        if (isFileSave)
            sWriter.WriteLine(newLine);
        else
            savefileStrings.Add(newLine);
    }


    private void SaveGame()
    {
        ((Stats)stats).gameWasLoaded = true; //saving the game also prevents the user from loading on top

        if (isFileSave)
        {
            print("Save Game Path on your system: " + filePath);
            sWriter = new StreamWriter(filePath + fileNameLocal + fileExt);
        }
        else
        {
            print("Saving to PlayerPrefs.\n" +
                  "PlayerPrefs are stored in HKEY_CURRENT_USER/SOFTWARE/StartupKits/StrategyKit (Run regedit)");
            savefileStrings.Clear();
        }

        // ReadStructures();

        WriteNextLine("###StartofFile###");

        WriteNextLine("###PosStruct###");

        for (var i = 0; i < positionalStructures.Count; i++)
        {
            var sSel = positionalStructures[i].GetComponent("StructureSelector");


            var powerStorage = positionalStructures[i].GetComponent<PowerStorage>();
            var powerStorageData = powerStorage ? "," + powerStorage.CurrentAmountOfPower : "";

            WriteNextLine(((StructureSelector)sSel).structureClass + "," +
                          ((StructureSelector)sSel).structureType + "," +
                          ((StructureSelector)sSel).structureIndex + "," +
                          ((StructureSelector)sSel).grassType + "," +
                          positionalStructures[i].transform.position.x + "," +
                          positionalStructures[i].transform.position.y + "," +
                          ((StructureSelector)sSel).Id + "," +
                          ((StructureSelector)sSel).Level + "," +
                          ((StructureSelector)sSel).CategoryType + powerStorageData
            );
        }

        WriteNextLine("###GridStruct###");

        for (var i = 0; i < gridStructures.Count; i++)
        {
            var sSel = gridStructures[i].GetComponent("StructureSelector");

            WriteNextLine(((StructureSelector)sSel).structureClass + "," +
                          ((StructureSelector)sSel).structureType + "," +
                          ((StructureSelector)sSel).structureIndex + "," +
                          ((StructureSelector)sSel).iRow + "," +
                          ((StructureSelector)sSel).jCol + "," +
                          ((StructureSelector)sSel).Id + "," +
                          ((StructureSelector)sSel).Level + "," +
                          ((StructureSelector)sSel).CategoryType
            );
        }

        WriteNextLine("###Construction###");
        for (var i = 0; i < Construction.Length; i++)
        {
            var cSel = Construction[i].GetComponent("ConstructionSelector");
            WriteNextLine(((ConstructionSelector)cSel).structureClass + "," +
                          ((ConstructionSelector)cSel).StructureType + "," +
                          ((ConstructionSelector)cSel).constructionIndex + "," +
                          ((ConstructionSelector)cSel).grassType + "," +
                          ((ConstructionSelector)cSel).buildingTime + "," +
                          ((ConstructionSelector)cSel).remainingTime + "," +
                          ((ConstructionSelector)cSel).storageAdd + "," +
                          ((ConstructionSelector)cSel).iRow + "," +
                          ((ConstructionSelector)cSel).jCol + "," +
                          ((ConstructionSelector)cSel).Id + "," +
                          ((ConstructionSelector)cSel).Level + "," +
                          ((ConstructionSelector)cSel).CategoryType + "," +
                          Construction[i].transform.position.x + "," +
                          Construction[i].transform.position.y
            );
        }

        WriteNextLine("###Removables###");

        //NO NEED TO SAVE THIS SINCE IT WILL BE RECOVERED FROM RemovableCreator AT LOAD
        //int[] removeTimeArray = ((RemovableCreator)removableCreator).removeTimeArray;
        //WriteNextLine (string.Join(",", Array.ConvertAll(removeTimeArray, x => x.ToString())));  // array conversion to csv

        for (var i = 0; i < Removables.Length; i++)
        {
            var rSel = Removables[i].GetComponent("RemovableSelector");

            WriteNextLine(((RemovableSelector)rSel).removableType + "," +
                          ((RemovableSelector)rSel).removableIndex + "," +
                          ((RemovableSelector)rSel).inRemoval + "," +
                          ((RemovableSelector)rSel).iColumn + "," +
                          ((RemovableSelector)rSel).jRow
            );
        }

        WriteNextLine("###RemovableTimers###");

        for (var i = 0; i < RemovableTimers.Length; i++)
        {
            var rTimer = RemovableTimers[i].GetComponent("RemovableTimerSelector");

            WriteNextLine(((RemovableTimerSelector)rTimer).removableIndex + "," +
                          ((RemovableTimerSelector)rTimer).remainingTime);
        }

        WriteNextLine("###Numerics###"); //the unique id for buildings/grass patches

        WriteNextLine(buildingCreator.structureIndex + "," + wallCreator.structureIndex + "," +
                      weaponCreator.structureIndex);
        WriteNextLine(((UnitProc)unitProc).currentSlidVal.ToString("0.00") + "," +
                      ((UnitProc)unitProc).currentTrainingTime);

        var trainingTimes =
            new int[Stats.Instance.ExistingUnits.Count]; //an array that holds training times from all units - 
        //at first load, the XML will not have been read 
        var sizePerUnit = new int[Stats.Instance.ExistingUnits.Count];


        trainingTimes =
            ((UnitProc)unitProc).trainingTimes; //replace our empty array with the xml values, already in unitproc
        sizePerUnit = ((UnitProc)unitProc).sizePerUnit;

        WriteNextLine(string.Join(",", new List<int>(trainingTimes).ConvertAll(i => i.ToString()).ToArray()));
        WriteNextLine(string.Join(",", new List<int>(sizePerUnit).ConvertAll(i => i.ToString()).ToArray()));

        var serializedExistedUnits = JsonConvert.SerializeObject(Stats.Instance.ExistingUnits);
        WriteNextLine(string.Join(",", serializedExistedUnits));


        var queList = new List<Vector4>();
        queList = ((UnitProc)unitProc).queList;

        for (var i = 0; i < queList.Count; i++) WriteNextLine(queList[i].ToString().Trim(')', '('));

        WriteNextLine("###Economy###");

        existingEconomyBuildings.Clear();
        existingEconomyBuildings = ((ResourceGenerator)resourceGenerator).ExistingEconomyBuildings;

        for (var i = 0; i < existingEconomyBuildings.Count; i++)
            WriteNextLine(existingEconomyBuildings[i].structureIndex + "," +
                          (int)existingEconomyBuildings[i].storedGold + "," +
                          (int)existingEconomyBuildings[i].storedMana);


        WriteNextLine("###Stats###");

        WriteNextLine(
            ((Stats)stats).level + "," +
            ((Stats)stats).townHallLevel + "," +
            ((Stats)stats).structureIndex + "," +
            ((Stats)stats).experience + "," +
            ((Stats)stats).maxExperience + "," +
            ((Stats)stats).builders + "," +
            ((Stats)stats).occupiedBuilders + "," +
            ((Stats)stats).occupiedHousing + "," +
            ((Stats)stats).maxHousing + "," +
            ((Stats)stats).gold + "," +
            ((Stats)stats).mana + "," +
            ((Stats)stats).crystals + "," +
            ((Stats)stats).maxGold + "," +
            ((Stats)stats).maxMana + "," +
            ((Stats)stats).maxCrystals + "," +
            ((Stats)stats).TutorialCitySeen + "," +
            ((Stats)stats).tutorialBattleSeen + "," +
            ((Stats)stats).removablesCreated + "," +
            ((SoundFX)soundFX).soundOn + "," +
            ((SoundFX)soundFX).ambientOn + "," +
            ((SoundFX)soundFX).musicOn + "," +
            ((Stats)stats).power + "," +
            ((Stats)stats).supplies
        );


        var cultureInfo = new CultureInfo(cultureFormat);
        WriteNextLine(DateTime.Now.ToString(cultureInfo));

        WriteNextLine("###UnitsInfo###");
        var serializedUnitsInfo = JsonConvert.SerializeObject(global::MenuUnit.Instance.UnitsInfo);
        WriteNextLine(string.Join(",", serializedUnitsInfo));

        WriteNextLine("###EndofFile###");

        if (isFileSave)
        {
            sWriter.Flush();
            sWriter.Close();
        }
        else
        {
            savefile = "";

            for (var i = 0; i < savefileStrings.Count; i++) savefile += savefileStrings[i] + "\n";

            PlayerPrefs.SetString("savefile", savefile);
            PlayerPrefs.Save();
        }

        existingBuildings =
            new int[GameUnitsSettings_Handler.s.buildingTypesNo]; //reset for next save - remove if automatic
        MessageController.Instance.DisplayMessage("Game saved.");
    }

    private void ReadStructures() //reads all buildings/grass/under construction
    {
        Construction = GameObject.FindGameObjectsWithTag("Construction"); //find all buildings under construction
        Removables = GameObject.FindGameObjectsWithTag("Removable");
        RemovableTimers = GameObject.FindGameObjectsWithTag("RemovableTimer");
        //Grass = GameObject.FindGameObjectsWithTag ("Grass");//finds all patches of grass from underneath the buildings

        var structures = GameObject.FindGameObjectsWithTag("Structure");
        ClearStructureLists();

        foreach (var structure in structures)
        {
            var structureClass = structure.GetComponent<StructureSelector>().structureClass;

            switch (structureClass)
            {
                case "Building":
                    positionalStructures.Add(structure);
                    break;
                case "Weapon":
                    positionalStructures.Add(structure);
                    break;

                case "StoneWall":
                    gridStructures.Add(structure);
                    break;
                case "WoodFence":
                    gridStructures.Add(structure);
                    break;
                case "Ambient":
                    gridStructures.Add(structure);
                    break;
            }
        }
    }

    private void ClearStructureLists()
    {
        positionalStructures.Clear();
        gridStructures.Clear();
    }

    private bool CheckLocalSaveFile()
    {
        MessageController.Instance.DisplayMessage("Checking for local save file...");
        var localSaveExists = File.Exists(filePath + fileNameLocal + fileExt);
        return localSaveExists;
    }

    private bool CheckPlayerPrefsSaveFile()
    {
        MessageController.Instance.DisplayMessage("Checking for PlayerPrefs save file...");
        var localSaveExists = PlayerPrefs.HasKey("savefile");
        return localSaveExists;
    }

    private bool CheckLoadOnce()
    {
        var gameWasLoaded = ((Stats)stats).gameWasLoaded;

        if (gameWasLoaded) //prevents loading twice, since there are no safeties and the procedure should be automated at startup, not button triggered
        {
            MessageController.Instance.DisplayMessage("Only one load per session is allowed. Canceling...");
            MessageController.Instance.DisplayMessage(
                "Unless you just generated the trees - this prevents you from loading houses on top of trees.");
            MessageController.Instance.DisplayMessage(
                "Next time, generate the trees before you do anything else - then save/load the game.");
        }

        return gameWasLoaded;
    }

    public void LoadFromLocalFile()
    {
        if (CheckLoadOnce()) return;

        if (CheckLocalSaveFile())
        {
            MessageController.Instance.DisplayMessage("Local save file found.");
            //StreamReader 
            sReader = new StreamReader(filePath + fileNameLocal + fileExt);

            isFileSave = true; //this is a local file save, not player prefs registry record
            LoadGame();
        }
        else
        {
            MessageController.Instance.DisplayMessage("No local save file found.");
            ((SoundFX)soundFX).ChangeMusic(true); //first run, start music
        }
    }

    public void LoadFromPlayerPrefs()
    {
        if (CheckLoadOnce()) return;

        if (CheckPlayerPrefsSaveFile())
        {
            MessageController.Instance.DisplayMessage("PlayerPrefs save file found.");
            savefile = PlayerPrefs.GetString("savefile");

            isFileSave = false; //this is a player prefs registry record load
            LoadGame();
        }
        else
        {
            MessageController.Instance.DisplayMessage("No local save file found.");
            ((SoundFX)soundFX).ChangeMusic(true); //first run, start music
        }
    }

    public void LoadFromServer()
    {
        if (CheckLoadOnce()) return;

        //StreamReader 
        sReader = new StreamReader(filePath + fileNameServer + fileExt);

        isFileSave = true;
        LoadGame();
    }

    public void LoadPlayerPrefsAttackFromServer()
    {
        var text = PlayerPrefs.GetString(fileNameAttack);
        AttackPlayerPrefsDamage(text);
    }

    public void LoadAttackFromServer()
    {
        //	StreamReader 
        sReader = new StreamReader(filePath + fileNameAttack + fileExt);
        AttackDamage(sReader);
    }

    private void AttackPlayerPrefsDamage(string text)
    {
        var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        currentLine = lines[1]; //skip header

        var losses = currentLine.Split(","[0]);

        var goldLost = int.Parse(losses[0]);
        var manaLost = int.Parse(losses[1]);
        var buildingsLost = int.Parse(losses[2]);
        var unitsLost = int.Parse(losses[3]);

        if (goldLost == 0 && manaLost == 0) return; //the file exists, but has been loaded and reset

        ((Stats)stats).gold -= goldLost;
        ((Stats)stats).mana -= manaLost;
        ((Stats)stats).UpdateUI();
        //menuMain.OnCloseSettings();
        StartCoroutine("ActivateDamagePanel");
    }

    private void AttackDamage(StreamReader sReader)
    {
        currentLine = "";

        currentLine = sReader.ReadLine(); //skip header

        currentLine = sReader.ReadLine(); //read gold/mana

        var losses = currentLine.Split(","[0]);

        var goldLost = int.Parse(losses[0]);
        var manaLost = int.Parse(losses[1]);
        var buildingsLost = int.Parse(losses[2]);
        var unitsLost = int.Parse(losses[3]);

        if (goldLost == 0 && manaLost == 0) return; //the file exists, but has been loaded and reset

        ((Stats)stats).gold -= goldLost;
        ((Stats)stats).mana -= manaLost;
        ((Stats)stats).UpdateUI();
        //menuMain.OnCloseSettings();
        StartCoroutine("ActivateDamagePanel");
    }

    private IEnumerator
        ActivateDamagePanel() //keeps trying to launch the damage panel, waiting fot the user to finish other tasks, if any
    {
        //otherwise the panel is superimposed on other panels	
        yield return new WaitForSeconds(2);
        if (!((Relay)relay).pauseInput)
        {
            //((Relay)relay).pauseInput = true;
        }
        else
        {
            StartCoroutine("ActivateDamagePanel");
        }
    }
    /*
    void OnGUI()
    {
        if (GUI.Button (new Rect (245, Screen.height - 140, 45, 25), "menu")) {
            //I will leave this on manual control; When save/load are automated, include this as well
            menuMain.OnDamage ();
        }
        if (GUI.Button (new Rect (445, Screen.height - 140, 45, 25), "close")) {
            //I will leave this on manual control; When save/load are automated, include this as well
            menuMain.OnCloseDamage ();
        }

    }
    */
}

//2498 lines before refactoring