using System;
using System.Collections;
using System.IO;
using System.Text;
using Assets.Scripts.Menus;
using UnityEngine;

public class SaveLoadBattle : SaveLoadBase
{
    public string //http://citybuildingkit.com/costin/admin.php
        serverAddress = "https://citybuildingkit.com/UA/pvp-server-sync-v6/",
        filesAddress = "get_match.php", //get_match.php?license=U125UNITYDEMO
        matchAddress = "finish_match.php",
        license = "U125UNITYDEMO", //"costin/finish_match.php"
        myMapIDCode;

    public string[] campaignNo;

    public GameObject
        GoHomeBt;

    private readonly string
        //filePath,
        //local - saves the map id from server IlXcUovzS6
        attackExt = "_results"; //the server avoids loading own map; if there is no own map, an error is displayed in console

    private readonly string
        //filePath,
        //local - saves the map id from server IlXcUovzS6
        battleSaveFile =
            "BattleMap"; //the server avoids loading own map; if there is no own map, an error is displayed in console

    private readonly string
        //filePath,
        //local - saves the map id from server IlXcUovzS6
        fileExt = ".txt"; //the server avoids loading own map; if there is no own map, an error is displayed in console

    private readonly string
        //filePath,
        myMapID = "MyMapID"; //the server avoids loading own map; if there is no own map, an error is displayed in console

    private readonly string
        //filePath,
        //local - saves the map id from server IlXcUovzS6
        //_attack
        nullMapIDCode =
            "0000000000"; //the server avoids loading own map; if there is no own map, an error is displayed in console

    private string
        //filePath,
        fileNameLocal =
            "LocalSave"; //the server avoids loading own map; if there is no own map, an error is displayed in console

    private string
        //filePath,
        //local - saves the map id from server IlXcUovzS6
        myUseridFile, //gfdfghjke.txt
        battleMapID; //the server avoids loading own map; if there is no own map, an error is displayed in console

    private bool oneLoad = true;
    private Component statsBattle; //removableCreator, 
    private WWW w2;

    // Use this for initialization
    private void Start()
    {
        isBattleMap = true;
        InitializeComponents();

        statsBattle = GameObject.Find("StatsBattle").GetComponent<StatsBattle>();
        GroupDamageBars = GameObject.Find("GroupDamageBars");
        filePath = Application.persistentDataPath + "/"; //other platforms

        LoadBattleGame();
    }

    private bool CheckServerSaveFile() //LOCAL recording of a previous save on server
    {
        var serverSaveExists =
            false; //checks if the mapcode was saved locally, not if it is still available on server, to avoid attacking your own city

#if !UNITY_WEBPLAYER
        serverSaveExists = File.Exists(filePath + myMapID + fileExt);
#endif

#if UNITY_WEBPLAYER
		serverSaveExists = PlayerPrefs.HasKey("mapid");
#endif

        return serverSaveExists;
    }

    public void ReadMyMapID()
    {
#if !UNITY_WEBPLAYER
        var sReader = new StreamReader(filePath + myMapID + fileExt);
        myMapIDCode = sReader.ReadLine();
#endif

#if UNITY_WEBPLAYER
		myMapIDCode = PlayerPrefs.GetString("mapid");
#endif
    }

    private void SaveBattleMap() //saves a copy of the server map locally
    {
#if !UNITY_WEBPLAYER
        var sWriter = new StreamWriter(filePath + battleSaveFile + fileExt);
        sWriter.Write(w2.text);
        sWriter.Flush();
        sWriter.Close();
#endif

#if UNITY_WEBPLAYER
		PlayerPrefs.SetString("battlesave", w2.text);
		PlayerPrefs.Save ();
#endif

        //LoadMap();
    }

    public void SaveAttack()
    {
        StartCoroutine("SendAttackToServer");
    }

    private IEnumerator SendAttackToServer()
    {
        MessageController.Instance.DisplayMessage("Uploading battle results.");

        byte[] levelData;

        levelData = Encoding.ASCII.GetBytes("###StartofFile###\n" + // gold,mana,buildingsDestroyed,unitsLost
                                            ((StatsBattle)statsBattle).gold + "," +
                                            ((StatsBattle)statsBattle).mana + "," +
                                            ((StatsBattle)statsBattle).buildingsDestroyed + "," +
                                            ((StatsBattle)statsBattle).unitsLost +
                                            "\n###EndofFile###");

        var form = new WWWForm();

        form.AddField("savefile", "file");

        form.AddBinaryData("savefile", levelData, battleMapID + attackExt, "text/xml"); //file

        //change the url to the url of the php file
        var w = new WWW(serverAddress + filesAddress + "?mapid=" + battleMapID + attackExt + "&license=" + license,
            form); //myUseridFile 

        yield return w;
        if (w.error != null)
        {
            print("error");
            print(w.error);
            MessageController.Instance.DisplayMessage("Network error.");
        }
        else
        {
            var ready = false;
#if !UNITY_WEBPLAYER
            ready = w.uploadProgress == 1 && w.isDone;
#endif

#if UNITY_WEBPLAYER
			ready = w.isDone;
#endif

            //this part validates the upload, by waiting 5 seconds then trying to retrieve it from the web
            if (ready) //w.uploadProgress == 1 && w.isDone
            {
                yield return new WaitForSeconds(5);
                //change the url to the url of the folder you want it the levels to be stored, the one you specified in the php file
                var w2 = new WWW(serverAddress + filesAddress + "?get_user_map=1&mapid=" + battleMapID + attackExt +
                                 "&license=" + license); //returns a specific map

                yield return w2;
                if (w2.error != null)
                {
                    print("error 2");
                    print(w2.error);
                    MessageController.Instance.DisplayMessage("Attack file check error.");
                }
                else
                {
                    //then if the retrieval was successful, validate its content to ensure the level file integrity is intact
                    if (w2.text != null && w2.text != "")
                    {
                        if (w2.text.Contains("###StartofFile###") && w2.text.Contains("###EndofFile###"))
                        {
                            //and finally announce that everything went well
                            print("Received Verification File " + battleMapID + attackExt + " Contents are: \n\n" +
                                  w2.text); //file
                            MessageController.Instance.DisplayMessage("Uploaded attack results to " + battleMapID +
                                                                      attackExt);
                        }
                        else
                        {
                            print("Level File " + battleMapID + attackExt + " is Invalid"); //file
                            print("Received Verification File " + battleMapID + attackExt + " Contents are: \n\n" +
                                  w2.text); //file although incorrect, prints the content of the retrieved file
                            MessageController.Instance.DisplayMessage("Uploaded attack failed.");
                        }
                    }
                    else
                    {
                        print("Level File " + battleMapID + attackExt + " is Empty"); //file
                        MessageController.Instance.DisplayMessage("Attack file is empty?");
                    }
                }
            }
        }

        GoHomeBt.SetActive(true); //display go home button regardles of upload success, so the user can go back to hometown/exit the game
    }

    public void LoadMapFromServer()
    {
        StartCoroutine("DownloadBattleMap"); //force the local level save before this

        MessageController.Instance.DisplayMessage("Downloading random map.");
    }

    private IEnumerator DownloadBattleMap()
    {
        if (CheckServerSaveFile())
            ReadMyMapID();
        else
            myMapIDCode = nullMapIDCode;

        //loads a map other than the user map; if there is only one map on the server - your own, loads your own map

        var campaignLevel = ((TransData)transData).campaignLevel;

        if (campaignLevel == -1)
        {
            // w2 = new WWW(
            // serverAddress + filesAddress + "?get_random_map=1&mapid=" + myMapIDCode + "&license=" +
            // license); //mapid with the get_random_map to prevent the user's map from being downloaded by accident
            // Debug.Log(serverAddress + filesAddress + "?get_random_map=1&mapid=" + myMapIDCode + "&license=" + license);


            //var tFullPathName = "Assets/CityBuildingKit/Resources/LocalSave.txt";
            // var tFullPathName =
            // "/Users/byelik/Documents/Git/Unity/CBK/CBK_Unity/CBK_UnityPorject/Assets/CityBuildingKit/Resources/LocalSave.txt";
            // var tURIBuilder = new UriBuilder(tFullPathName);
            // tURIBuilder.Scheme = "file";

            w2 = new WWW(
                "https://bitbucket.org/!api/2.0/snippets/the-sarge-universe/GzzyKE/e01e04364b1a9fd226816bd2b56ba78a9df8df3d/files/TestBattle");
        }
        else
        {
            battleMapID = "0000camp" + campaignNo[campaignLevel];
            w2 = new WWW(serverAddress + matchAddress + "?get_user_map=1&mapid=" + battleMapID + "&license=" + license);
            // Debug.Log(serverAddress + matchAddress + "?get_user_map=1&mapid=" + battleMapID + "&license=" + license);
        }


        //w2 = new WWW(serverAddress + matchAddress + "?get_user_map=1&mapid=" + myMapIDCode+"&license="+license);//download own map reference - no mapid, so it doesn't work here
        yield return w2;

        if (w2.error != null)
        {
            print("Server load error" + w2.error);
            MessageController.Instance.DisplayMessage("Map download error.");
        }

        else
        {
            //then if the retrieval was successful, validate its content to ensure the level file integrity is intact
            if (w2.text != null && w2.text != "")
            {
                if (w2.text.Contains("###StartofFile###") && w2.text.Contains("###EndofFile###"))
                {
                    var temp = w2.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                    if (campaignLevel == -1) battleMapID = temp[1]; //skip ###StartofFile### then read ID

                    print("Downloaded File " + battleMapID + " Contents are: \n\n" + w2.text);

                    SaveBattleMap();
                    sReader = new StreamReader(filePath + battleSaveFile + fileExt);

                    StartCoroutine("LateLoadGame");
                    MessageController.Instance.DisplayMessage("Map downloaded.");
                }

                else
                {
                    print("Random Level File is Invalid. Contents are: \n\n" + w2.text);
                    //although incorrect, prints the content of the retrieved file
                    MessageController.Instance.DisplayMessage(
                        "License code not added in Map01\nor downloaded file corrupted.");

                    MessageController.Instance.DisplayMessage("If you have your own save file on server\n" +
                                                              "delete local files or change your 10 character ID in:\n" +
                                                              "C:/Users/user/AppData/LocalLow/\n" +
                                                              "AStarterKits/StrategyKit/MyMapID.txt");
                }
            }
            else
            {
                print("Random Level File is Empty");
                MessageController.Instance.DisplayMessage("Downloaded file empty?");
            }
        }
    }

    private IEnumerator LateLoadGame()
    {
        yield return new WaitForSeconds(0.5f);
        LoadGame();
        BattleMapInstructions();
    }

    private void BattleMapInstructions()
    {
        Helios.Instance.Buildings = LoadedBuildings;
        Helios.Instance.InitializeCurrentDamage();
        Helios.Instance.InitializeDamageBars(LoadedDamageBars);
        var tempGridManager = GameObject.Find("GridManager");
        tempGridManager.GetComponent<GridManager>().UpdateObstacles();
        Helios.Instance.NetworkLoadReady();
        ((SoundFX)soundFX).BattleMapSpecific();
    }

    public void LoadBattleGame()
    {
        if (!oneLoad)
            return; //prevents loading twice, since there are no safeties and the procedure should be automated at startup, not button triggered
        oneLoad = false;

        LoadMapFromServer();
    }

    private void LoadMap()
    {
#if !UNITY_WEBPLAYER
        isFileSave = true;
        LoadGame();
#endif

#if UNITY_WEBPLAYER
		isFileSave = false;
		LoadGameMapPlayerPrefs();
#endif
    }

    private bool CheckPlayerPrefsSaveFile()
    {
        // loads the battle map from the playerprefs save - could be loaded from w2.text directly	//	PlayerPrefs.SetString("battlesave", w2.text);

        MessageController.Instance.DisplayMessage("Checking for PlayerPrefs battle map save file...");
        var localSaveExists = PlayerPrefs.HasKey("battlesave");
        return localSaveExists;
    }

    private void InstantiateObjects()
    {
        InstantiateConstructions();
        InstantiateRemovables();
        InstantiateRemovableTimers();
    }

    private void ProcessZ()
    {
        for (var i = 0; i < LoadedBuildings.Count; i++)
        {
            var pivotPos = LoadedBuildings[i].transform.GetChild(1).position; //pivot
            var spritesPos = LoadedBuildings[i].transform.GetChild(2).position; //sprites

            var correctiony = 10 / (pivotPos.y + 3300); //ex: fg 10 = 0.1   bg 20 = 0.05  
            //all y values must be positive, so we add the grid origin y 3207 +100 to avoid divide by 0; 
            //otherwise depth glitches around y 0

            LoadedBuildings[i].transform.GetChild(2).position =
                new Vector3(spritesPos.x, spritesPos.y, zeroZ - correctiony); //	transform.GetChild(2).position   
        }

        for (var i = 0; i < LoadedConstructions.Count; i++)
        {
            var pivotPos = LoadedConstructions[i].transform.GetChild(1).position; //pivot
            var pos = LoadedConstructions[i].transform.GetChild(3).position; //sprite

            var correctiony = 10 / (pivotPos.y + 3300); //ex: fg 10 = 0.1   bg 20 = 0.05  
            //all y values must be positive, so we add the grid origin y 3207 +100 to avoid divide by 0; 
            //otherwise depth glitches around y 0

            LoadedConstructions[i].transform.GetChild(3).position = new Vector3(pos.x, pos.y, zeroZ - correctiony);
        }
    }
}