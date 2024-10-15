using System.Collections.Generic;
using System.Xml;
using Assets.Scripts.Menus;
using Assets.Scripts.UIControllersAndData.Images;
using UIControllersAndData.Store;
using UnityEngine;
using UnityEngine.UI;

public class RemovableCreator : MonoBehaviour
{
    public int removableIndex = -1; //increments to give removables and grass patches an unique ID number

    public int[] removeTimes;

    public bool battleMap;


    public GameObject
        RemovableSelectedPanel,
        RemovableTimerPf,
        GrassRemovablePf,
        GroupRemovables;

    public Text removePriceLb;
    public Image currencyIco; //0 gold 1 mana
    public string[] currencyIcoNames = new string[2];

    public TextAsset RemovablesXML; //variables for loading building characteristics from XML

    private readonly int
        grassZ = 2;

    private readonly List<Dictionary<string, string>> removables = new();

    private readonly int
        zeroZ = 0;

    private int
        currentSelection;

    private Dictionary<string, string> dictionary;

    private int numberOfRemovables;

    private Component relay, stats;
    private GameObject[] RemovablesPf;

    private GameObject
        selectedRemovable,
        selectedRemovableGrass,
        selectedRemovablePad,
        selectedRemovableTimer;

    // Use this for initialization
    private void Start()
    {
        //Getting removables from SO Removables.asset-
        numberOfRemovables = OtherGameData.Instance.Removables.Category.Count;
        RemovablesPf = new GameObject[numberOfRemovables];
        removeTimes = new int[numberOfRemovables];
        RemovablesPf = OtherGameData.Instance.Removables.Category.ToArray();

        relay = GameObject.Find("Relay").GetComponent<Relay>();

        if (!battleMap)
        {
            stats = GameObject.Find("Stats").GetComponent<Stats>();
            removableIndex = ((Stats)stats).structureIndex;

            if (!((Stats)stats).removablesCreated)
            {
                //InitializeRemovables();
            }
        }

        GetRemovablesXML();
        RecordRemoveTime(); //make sure this happens before game load if automatic
    }

    private void GetRemovablesXML() //reads buildings XML
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(RemovablesXML.text);
        var removablesList = xmlDoc.GetElementsByTagName("Removable");

        foreach (XmlNode removableInfo in removablesList)
        {
            var removablesContent = removableInfo.ChildNodes;
            dictionary = new Dictionary<string, string>();

            foreach (XmlNode removableItems in removablesContent) // levels itens nodes.
            {
                /*
                    <Name>ClamA</Name>	
                    <RemoveTime>5</RemoveTime>
                    <Currency>Gold</Currency>				<!-- Save as 0 Gold 1 Mana -->
                    <RemovePrice>50</RemovePrice>
                    <XpAward>2</XpAward>
                */
                if (removableItems.Name == "Name")
                    dictionary.Add("Name", removableItems.InnerText); // put this in the dictionary.
                if (removableItems.Name == "RemoveTime") dictionary.Add("RemoveTime", removableItems.InnerText);
                if (removableItems.Name == "Currency") dictionary.Add("Currency", removableItems.InnerText);
                if (removableItems.Name == "RemovePrice") dictionary.Add("RemovePrice", removableItems.InnerText);
                if (removableItems.Name == "XpAward") dictionary.Add("XpAward", removableItems.InnerText);
            }

            removables.Add(dictionary);
        }
    }

    private void RecordRemoveTime()
    {
        for (var i = 0; i < removables.Count; i++) removeTimes[i] = int.Parse(removables[i]["RemoveTime"]);
    }

    public void InitializeRemovables()
    {
        ((Stats)stats).removablesCreated = true;
        ((Stats)stats).gameWasLoaded = true; //do not allow game loads after Removable generation

        var nodes = GridManager.instance.nodes;

        for (var i = 2; i < nodes.GetLength(0) - 2; i++)
        for (var j = 2; j < nodes.GetLength(1) - 2; j++)
        {
            var k = 0;
            k = Random.Range(0, 10); //about 1 in 10 probability to instantiate something

            if (k == 0)
            {
                currentSelection = Random.Range(0, numberOfRemovables);

                Vector3
                    nodePos = nodes[i, j].position,
                    removablePos = new(nodePos.x, nodePos.y, zeroZ),
                    grassPos = new(nodePos.x, nodePos.y, grassZ);

                var removable = Instantiate(RemovablesPf[currentSelection], removablePos, Quaternion.identity);
                var removableGrass = Instantiate(GrassRemovablePf, grassPos, Quaternion.identity);
                selectedRemovable = removable;
                selectedRemovableGrass = removableGrass;

                PlaceObject(currentSelection, i, j);
            }
        }

        ((Stats)stats).removablesCreated = true;
        RemovableSelectedPanel.transform.SetAsLastSibling();
    }

    public void OnSelect(GameObject selectedRem)
    {
        selectedRemovable = selectedRem;
        SelectRemovable();
    }

    public void OK() //ok remove
    {
        Component remSelector = selectedRemovable.GetComponent<RemovableSelector>();

        var removableType = ((RemovableSelector)remSelector).removableType;

        TypeToSelectionIndex(removableType);

        bool
            enoughtMoney = false,
            builderAvailable = ((Stats)stats).builders > ((Stats)stats).occupiedBuilders;

        switch (removables[currentSelection]["Currency"])
        {
            case "Gold":
                enoughtMoney = ((Stats)stats).gold >= int.Parse(removables[currentSelection]["RemovePrice"]);
                break;
            case "Mana":
                enoughtMoney = ((Stats)stats).mana >= int.Parse(removables[currentSelection]["RemovePrice"]);
                break;
        }

        if (enoughtMoney && builderAvailable)
        {
            switch (removables[currentSelection]["Currency"])
            {
                case "Gold":
                    ((Stats)stats).SubstractResources(int.Parse(removables[currentSelection]["RemovePrice"]), 0, 0, 0,
                        0);
                    break;
                case "Mana":
                    ((Stats)stats).SubstractResources(0, int.Parse(removables[currentSelection]["RemovePrice"]), 0, 0,
                        0);
                    break;
            }

            ((Stats)stats).occupiedBuilders++;

            ((Stats)stats).UpdateUI();
        }
        else
        {
            if (!enoughtMoney)
                switch (removables[currentSelection]["Currency"])
                {
                    case "Gold":
                        MessageController.Instance.DisplayMessage("Not enough Gold.");
                        break;
                    case "Mana":
                        MessageController.Instance.DisplayMessage("Not enough Mana.");
                        break;
                }

            if (!builderAvailable)
                MessageController.Instance.DisplayMessage("You need a builder for this job.");

            return;
        }

        var RemovableTimer = Instantiate(RemovableTimerPf, selectedRemovable.transform.position, Quaternion.identity);
        selectedRemovableTimer = RemovableTimer;
        SelectRemovableTimer();
        //selectedRemovable.GetComponent<RemovableSelector> ().inRemoval = true;
        Deselect();
    }

    private void SelectRemovableTimer()
    {
        Component
            remTimer = selectedRemovableTimer.GetComponent<RemovableTimerSelector>(),
            remSelector = selectedRemovable.GetComponent<RemovableSelector>();

        var remType = ((RemovableSelector)remSelector).removableType;

        TypeToSelectionIndex(remType);

        ((RemovableTimerSelector)remTimer).removeTime = int.Parse(removables[currentSelection]["RemoveTime"]);

        ((RemovableTimerSelector)remTimer).xpAward = int.Parse(removables[currentSelection]["XpAward"]);
        ((RemovableTimerSelector)remTimer).removableName = removables[currentSelection]["Name"];
        ((RemovableTimerSelector)remTimer).crystalAward = GenerateReward();
        ((RemovableTimerSelector)remTimer).removableIndex = ((RemovableSelector)remSelector).removableIndex;
        ((RemovableSelector)remSelector).inRemoval = 1; //0 for false, 1 for true
        ((RemovableTimerSelector)remTimer).inRemovalB = true;

        selectedRemovableTimer.transform.SetParent(selectedRemovable.transform);
        selectedRemovableTimer.transform.position = selectedRemovable.transform.position;
        selectedRemovableTimer.GetComponent<RemovableTimerSelector>().isSelected = false;
    }

    private int GenerateReward()
    {
        int winProbability = 0, xpAward = 0;

        winProbability = Random.Range(0, 2);
        if (winProbability != 0)
            xpAward = Random.Range(1, 5);

        return xpAward;
    }


    public void Cancel()
    {
        Deselect();
    }

    private void Deselect()
    {
        RemovableSelectedPanel.SetActive(false);
        selectedRemovable.GetComponent<RemovableSelector>().isSelected = false;
        ((Relay)relay).pauseInput = false;
    }

    private void SelectRemovable() //string tag
    {
        RemovableSelectedPanel.SetActive(true);

        Component remSelector = selectedRemovable.GetComponent<RemovableSelector>();

        var removableType = ((RemovableSelector)remSelector).removableType;

        TypeToSelectionIndex(removableType);

        RemovableSelectedPanel.transform.position = selectedRemovable.transform.position;
        removePriceLb.text = removables[currentSelection]["RemovePrice"];

        //if(removables [currentSelection] ["Currency"]=="Gold")

        switch (removables[currentSelection]["Currency"])
        {
            case "Gold":
                currencyIco.sprite =
                    ImageControler.GetImage(currencyIcoNames[0]); //248 237 93 ->  0.97 0.92 0.36  248/255 etc
                removePriceLb.color = new Color(1, 1, 0);
                break;
            case "Mana":
                currencyIco.sprite = ImageControler.GetImage(currencyIcoNames[1]);
                removePriceLb.color = new Color(0, 1, 1);
                break;
        }

        ((Relay)relay).pauseInput = true;
    }

    private void TypeToSelectionIndex(string type)
    {
        //"ClamA","ClamB","ClamC","TreeA","TreeB","TreeC","TreeD"
        switch (type) //converts the type to the dictionary index
        {
            case "ClamA":
                currentSelection = 0;
                break;
            case "ClamB":
                currentSelection = 1;
                break;
            case "ClamC":
                currentSelection = 2;
                break;
            case "TreeA":
                currentSelection = 3;
                break;
            case "TreeB":
                currentSelection = 4;
                break;
            case "TreeC":
                currentSelection = 5;
                break;
            case "TreeD":
                currentSelection = 6;
                break;
        }
    }


    private void PlaceObject(int currentSelection, int i, int j) //string tag, 
    {
        removableIndex++;
        ((Stats)stats).structureIndex++;

        Component
            removableSelector = selectedRemovable.GetComponent<RemovableSelector>(),
            grassSelector = selectedRemovableGrass.GetComponent<GrassSelector>();

        ((RemovableSelector)removableSelector).removableIndex = removableIndex;
        ((RemovableSelector)removableSelector).iColumn = i;
        ((RemovableSelector)removableSelector).jRow = j;

        ((GrassSelector)grassSelector).StructureIndex = removableIndex;

        selectedRemovable.transform.parent = GroupRemovables.transform;
        selectedRemovableGrass.transform.parent = selectedRemovable.transform;
    }
}