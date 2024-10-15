using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Menus;
using Assets.Scripts.UIControllersAndData.Store;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Unit;
using UnityEngine;
using UnityEngine.UI;

public class MenuArmyBattle : MonoBehaviour
{
    private const int
        zeroZ = 0;

    public static MenuArmyBattle instance;

    public Camera
        mainCamera; //for some reason the SpriteLightKitCam is creating an input problem sometimes - its camera is recorded as Camera.main 

    public Button[] minusBt;

    public GameObject
        spawnPointStar0,
        spawnPointStarI,
        spawnPointStarII,
        spawnPointStarIII, //prefabs for the stars you click on the edge of the map
        deployBt,
        closeBt; //we need to deactivate this button at the end of the battle		

    public Vector3 spawnPoint = new(0, 0, zeroZ); //cycles in the list and spreads the units on the map

    private readonly float deployTime = 0.1f;
    private readonly List<Vector3> spawnPointList = new();
    private readonly List<GameObject> starList = new();
    private readonly float touchTime = 0.1f;
    private bool deploying;
    private float deployTimer;
    private GameObject GroupUnits;
    private Component helios, statsBattle, soundFx, relay;
    private int spawnIndex, unitTypeIndex;
    private float speedModifier = 0.2f, touchTimer;
    private int unitsNo => ShopData.Instance.UnitCategoryData.category.Count;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GroupUnits = GameObject.Find("GroupUnits");

        helios = GameObject.Find("Helios").GetComponent<Helios>();
        statsBattle = GameObject.Find("StatsBattle").GetComponent<StatsBattle>();
        soundFx = GameObject.Find("SoundFX").GetComponent<SoundFX>();
        relay = GameObject.Find("Relay").GetComponent<Relay>();
    }

    private void Update()
    {
        if (deploying)
        {
            deployTimer += Time.deltaTime;

            if (deployTimer > deployTime)
            {
                DeployInSequence();
                deployTimer = 0;
            }
        }


        touchTimer += Time.deltaTime;

        if (touchTimer > touchTime)
        {
            touchTimer = 0;
            if (Input.GetMouseButton(0))
            {
                if (Input.mousePosition.y > 130)
                    RecordSpawnPoint();
            }
            else if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).position.y > 130)
                    RecordSpawnPoint();
            }
        }
    }

    private void Delay() //to prevent button commands from interfering with sensitive areas/buttons underneath
    {
        ((Relay)relay).DelayInput();
    }

    private void RecordSpawnPoint()
    {
        if (!((Helios)helios).networkLoadReady || ((Relay)relay).delay)
            return;

        if (deploying)
        {
            MessageController.Instance.DisplayMessage("Deployment in progress. Please wait.");
            return;
        }

        var gridPos = new Vector3(0, 0, 0);

        // Generate a plane that intersects the transform's position with an upwards normal.
        var playerPlane = new Plane(Vector3.back, new Vector3(0, 0, 0)); //transform.position + 

        // Generate a ray from the cursor position

        Ray RayCast;

        if (Input.touchCount > 0)
            RayCast = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
        else
            RayCast = mainCamera.ScreenPointToRay(Input.mousePosition); // Camera.main.

        // Determine the point where the cursor ray intersects the plane.
        float HitDist = 0;

        // If the ray is parallel to the plane, Raycast will return false.
        if (playerPlane.Raycast(RayCast, out HitDist)) //playerPlane.Raycast
        {
            // Get the point along the ray that hits the calculated distance.
            var RayHitPoint = RayCast.GetPoint(HitDist);
            var indexCell = GridManager.instance.GetGridIndex(RayHitPoint);
            var col = GridManager.instance.GetColumn(indexCell);
            var row = GridManager.instance.GetRow(indexCell);

            if (row > 0 && row < 34 && col > 0 && col < 34) //force click inside map
                if (!GridManager.instance.nodes[row, col].isObstacle)
                {
                    gridPos = GridManager.instance.nodes[row, col].position;
                    CreateStar(gridPos);
                }
        }
    }

    private void CreateStar(Vector3 gridPos)
    {
        ((SoundFX)soundFx).SoldierPlace();
        var currentGroup = -1;//((Helios)helios).instantiationGroupIndex;

        var starPos = gridPos + new Vector3(0, 0, -3);

        switch (currentGroup)
        {
            case -1:
                var StarO = Instantiate(spawnPointStar0, starPos, Quaternion.identity);
                break;
            case 0:
                var StarI = Instantiate(spawnPointStarI, starPos, Quaternion.identity);
                break;
            case 1:
                var StarII = Instantiate(spawnPointStarII, starPos, Quaternion.identity);
                break;
            case 2:
                var StarIII = Instantiate(spawnPointStarIII, starPos, Quaternion.identity);
                break;
            case 3:
                MessageController.Instance.DisplayMessage("You already deployed all 4 squads.");
                break;
        }

        spawnPointList.Add(gridPos);

        var stars = GameObject.FindGameObjectsWithTag("Star");

        for (var i = 0; i < stars.Length; i++)
            if (stars[i].GetComponent<Selector>().isSelected)
            {
                starList.Add(stars[i]);
                stars[i].GetComponent<Selector>().isSelected = false;
                break;
            }
    }

    private void DestroyStars()
    {
        for (var i = 0; i < starList.Count; i++) ((Star)starList[i].GetComponent("Star")).die = true;
        starList.Clear();
        spawnPointList.Clear();
    }


    public void CommitAll(int i, bool all)
    {
        if (deploying)
        {
            MessageController.Instance.DisplayMessage("Deployment in progress. Please wait.");
            return;
        }

        Delay(); //brief delay to prevent stars from appearing under the menus

        if (all)
        {
            if (((StatsBattle)statsBattle).AvailableUnits[i].count > 0)
            {
                if (((StatsBattle)statsBattle).DeployedUnits[i].count == 0) minusBt[i].gameObject.SetActive(true);

                ((StatsBattle)statsBattle).DeployedUnits[i].count += ((StatsBattle)statsBattle).AvailableUnits[i].count;
                ((StatsBattle)statsBattle).DeployedUnits[i].id = ((StatsBattle)statsBattle).AvailableUnits[i].id;
                ((StatsBattle)statsBattle).DeployedUnits[i].level = ((StatsBattle)statsBattle).AvailableUnits[i].level;
                ((StatsBattle)statsBattle).AvailableUnits[i].count = 0;
            }
        }
        else
        {
            if (((StatsBattle)statsBattle).DeployedUnits[i].count > 0)
            {
                ((StatsBattle)statsBattle).AvailableUnits[i].count += ((StatsBattle)statsBattle).DeployedUnits[i].count;
                ((StatsBattle)statsBattle).DeployedUnits[i].count = 0;

                minusBt[i].gameObject.SetActive(false);
            }
        }
    }

    public void Commit(int i)
    {
        if (deploying)
        {
            MessageController.Instance.DisplayMessage("Deployment in progress. Please wait.");
            return;
        }

        Delay(); //brief delay to prevent stars from appearing under the menus
        if (((StatsBattle)statsBattle).AvailableUnits[i].count > 0)
        {
            if (((StatsBattle)statsBattle).DeployedUnits[i].count == 0) minusBt[i].gameObject.SetActive(true);

            ((StatsBattle)statsBattle).AvailableUnits[i].count--;
            ((StatsBattle)statsBattle).DeployedUnits[i].count++;
            ((StatsBattle)statsBattle).DeployedUnits[i].id = ((StatsBattle)statsBattle).AvailableUnits[i].id;
            ((StatsBattle)statsBattle).DeployedUnits[i].level = ((StatsBattle)statsBattle).AvailableUnits[i].level;
        }
    }

    public void Cancel(int i)
    {
        if (deploying)
        {
            MessageController.Instance.DisplayMessage("Deployment in progress. Please wait.");
            return;
        }

        Delay(); //brief delay to prevent stars from appearing under the menus
        if (((StatsBattle)statsBattle).DeployedUnits[i].count > 0)
        {
            if (((StatsBattle)statsBattle).DeployedUnits[i].count == 1) minusBt[i].gameObject.SetActive(false);
            ((StatsBattle)statsBattle).DeployedUnits[i].count--;
            ((StatsBattle)statsBattle).AvailableUnits[i].count++;
        }
    }


    private void DeployInSequence()
    {
        for (var i = unitTypeIndex; i < unitsNo; i++) //unitTypeIndex
        {
            var index = ((StatsBattle)statsBattle).DeployedUnits[i].count;
            if (index > 0)
            {
                unitTypeIndex = i; //to avoid starting from 0 each time
                spawnPoint = spawnPointList[spawnIndex];

                if (spawnIndex < spawnPointList.Count - 1) //distributes the units to all spawn points
                    spawnIndex++;
                else
                    spawnIndex = 0;

                ((StatsBattle)statsBattle).DeployedUnits[i].count--; //units are deployed one by one
                InstantiateUnit(((StatsBattle)statsBattle).DeployedUnits[i].id, speedModifier);

                speedModifier += 0.2f;
                

                break;
            }

            if (i == unitsNo - 1) //finished deploying
            {
                ((StatsBattle)statsBattle).UpdateUnitsNo(); //update group units	

                // var deployedIndex = ((Helios)helios).instantiationGroupIndex; //the index we are deploying now
                ((Helios)helios).ProcessUnitGroup();

                DestroyStars();
                deploying = false;
                ((Relay)relay).deploying = false;
                closeBt.SetActive(true);
                break;
            }
        }
    }

    public void DeployUnits()
    {
        if (deploying)
        {
            MessageController.Instance.DisplayMessage("Deployment in progress. Please wait.");
            return;
        }

        Delay(); //brief delay to prevent stars from appearing under the menus or select building target at the same time
        if (starList.Count == 0)
        {
            MessageController.Instance.DisplayMessage("Select the location on the edge of the map.");
            return; //insert message
        }

        // if (((Helios)helios).instantiationGroupIndex >= 3) //already deployed all 4 groups
        // {
            // MessageController.Instance.DisplayMessage("You already deployed all 4 squads.");
            // return;
        // }

        if (!((Helios)helios).networkLoadReady) //map not loaded yet - don't deploy
        {
            MessageController.Instance.DisplayMessage("Map is not loaded or internet connection failed.");
            return; //insert messages
        }

        var someUnitSelected = false;

        
        // ((StatsBattle)statsBattle).DeployedUnits[0].id = 3;
        // ((StatsBattle)statsBattle).DeployedUnits[0].level = 1;
        // ((StatsBattle)statsBattle).DeployedUnits[0].count = 1;
        
        ((StatsBattle)statsBattle).DeployedUnits[1].id = 2;
        ((StatsBattle)statsBattle).DeployedUnits[1].level = 1;
        ((StatsBattle)statsBattle).DeployedUnits[1].count = 2;


        for (var i = 0; i < unitsNo; i++)
            if (((StatsBattle)statsBattle).DeployedUnits[i].count != 0)
            {
                someUnitSelected = true;
                break;
            }

        if (!someUnitSelected) //user has not selected any unit to deploy
        {
            MessageController.Instance.DisplayMessage("Assign units to the squad.");
            return;
        }

        // ((Helios)helios).instantiationGroupIndex++;

        // if (((Helios)helios).instantiationGroupIndex != 0)
            // ((Helios)helios).selectedGroupIndex++;

        ((Relay)relay).deploying = true;

        closeBt.SetActive(false);
        spawnIndex = 0;
        unitTypeIndex = 0;
        speedModifier = 0.2f; //puts some distance between units while walking
        deploying = true;
    }

    private void InstantiateUnit(int index, float speedModifier)
    {
        var level = MenuArmy.Instance.ExistingBattleUnits.Find(x => x.id == index).level;

        var levels = ShopData.Instance.GetLevels(index, ShopCategoryType.Unit);
        if (levels == null) return;
        var unit = levels?.FirstOrDefault(x => ((ILevel)x).GetLevel() == level);
        if (unit == null) return;

        if (((UnitCategory)unit).asset)
        {
            Instantiate(((UnitCategory)unit).asset, spawnPoint, Quaternion.identity);
            ProcessUnit(speedModifier, unit);
        }
        else
        {
            Debug.LogError("Asset is null: " + ((UnitCategory)unit).name);
        }
    }

    private void ProcessUnit(float speedModifier, BaseStoreItemData unit)
    {
        var unitType = "Unit";

        var units = GameObject.FindGameObjectsWithTag(unitType);
        for (var i = 0; i < units.Length; i++)
            if (((Selector)units[i].GetComponent("Selector")).isSelected)
            {
                units[i].transform.parent = GroupUnits.transform;
                var fightController = units[i].GetComponent<FighterController>();
                fightController.UnitData = (UnitCategory)unit;
                fightController.speed += speedModifier;
                // fightController.assignedToGroup = ((Helios)helios).selectedGroupIndex;
                if (unit != null)
                {
                    var selector = (Selector)units[i].GetComponent("Selector");
                    if (selector)
                    {
                        selector.ID = ((UnitCategory)unit).GetId();
                        selector.Level = ((UnitCategory)unit).GetLevel();
                    }

                    fightController.Life = unit.HP;
                    fightController.Shooter.fireRate = ((UnitCategory)unit).fireRate;
                    fightController.DamagePoints = ((UnitCategory)unit).damagePoints;

                    var fighterRadius = units[i].GetComponentInChildren<FighterRadius>();

                    fighterRadius.SetRadius((UnitCategory)unit);
                }


                ((Helios)helios)._allUnits.Add(units[i]);
                ((Helios)helios).instantiationUnitIndex++;
                ((Helios)helios).DeployUnit(units[i]);
                units[i].GetComponent<Selector>().index = ((Helios)helios).instantiationUnitIndex;
                ((Selector)units[i].GetComponent("Selector")).isSelected = false;
                break;
            }
    }
}