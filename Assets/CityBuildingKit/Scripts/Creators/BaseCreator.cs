using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Menus;
using Assets.Scripts.UIControllersAndData;
using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.NewUI;
using CityBuildingKit.Scripts.UIControllersAndData;
using CityBuildingKit.Scripts.UIControllersAndData.Models;
using JetBrains.Annotations;
using UIControllersAndData;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UIControllersAndData.Store.ShopItems;
using UnityEngine;

/// <summary>
/// </summary>
public class BaseCreator : MonoBehaviour
{
    public static BaseCreator Instance;


    private bool isUpgradeBuilding;


    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (MovingPad.activeSelf)
        {
            if (((Relay)relay).delay)
                return;


#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL
            if (Input.GetMouseButtonDown(0))
            {
                var layer_mask = LayerMask.GetMask("Grass");
                var hit = new RaycastHit();
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100, layer_mask))
                    if (hit.transform.parent.gameObject == selectedGrass.gameObject)
                    {
                        CameraController.Instance.enabled = false;
                        dragPhase = true;
                        return;
                    }
            }

            if (Input.GetMouseButtonUp(0))
            {
                CameraController.Instance.enabled = true;
                dragPhase = false;
            }

            if (Input.GetMouseButton(0) && dragPhase && !isUpgradeBuilding) MouseTouchMove();

#elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    int layer_mask = LayerMask.GetMask("Grass");
                    RaycastHit hit = new RaycastHit();
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    if(Physics.Raycast(ray, out hit, 100, layer_mask))
                    {
                        if(hit.transform.parent.gameObject == selectedGrass.gameObject)
                        {
                            CameraController.Instance.enabled = false;
                            dragPhase = true;
                            return;
                        }
                    }
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    CameraController.Instance.enabled = true;
                    dragPhase = false;
                }

                if (touch.phase == TouchPhase.Moved && dragPhase)
                {
                    MouseTouchMove();
                }
            }
#endif
        }

        if (drawingField)
            if (!((Relay)relay).delay && Input.GetMouseButtonUp(0))
                RecordSpawnPoint();
    }


    public void BuyStoreItem(DrawCategoryData data, ShopCategoryType shopCategoryType, Action callback)
    {
        if (data.Id == null) throw new Exception("id is null");

        _currentItemLevel = ((ILevel)data.BaseItemData).GetLevel();
        currentSelection = data.Id.GetId();
        isProductionBuilding = data.BaseItemData is IProdBuilding &&
                               ((IProdBuilding)data.BaseItemData).IsProductionBuilding();
        _currentCategoryType = shopCategoryType;
        Verify(callback);
    }

    public void BuildUpgradeForStructure(int id, int level, ShopCategoryType categoryType, bool needToDeteleStructure)
    {
        needToDeleteBeforeUpgrade = needToDeteleStructure;
        _currentItemLevel = level;
        currentSelection = id;
        _currentCategoryType = categoryType;
        Verify(null, true);
    }

    protected void InitializeComponents()
    {
        transData = GameObject.Find("TransData").GetComponent<TransData>();
        relay = GameObject.Find("Relay").GetComponent<Relay>();
        soundFX = GameObject.Find("SoundFX")
            .GetComponent<SoundFX>(); // //connects to SoundFx - a sound source near the camera
        cameraController =
            GameObject.Find("tk2dCamera").GetComponent<CameraController>(); //to move building and not scroll map
        menuMain = GameObject.Find("Main").GetComponent<MenuMain>();

        if (myTown)
        {
            resourceGenerator = GameObject.Find("ResourceGenerator").GetComponent<ResourceGenerator>();
            stats = GameObject.Find("Stats").GetComponent<Stats>(); //conects to Stats script
            structureIndex = ((Stats)stats).structureIndex;
        }
    }

    protected void Verify(Action callback = null, bool isUpgrade = false)
    {
        isUpgradeBuilding = isUpgrade;
        if (isArray[currentSelection] == 0)
        {
            VerifyConditions(callback, isUpgrade);
        }
        else
        {
            isField = true;
            drawingField = true;
            Delay();
            VerifyConditions(callback, isUpgrade);
        }
    }

    protected void Delay()
    {
        ((Relay)relay).DelayInput();
    }

    private IEnumerator MouseOperations(int index)
    {
        yield return new WaitForSeconds(0.2f);

        if (!((Relay)relay).delay)
            switch (index)
            {
                case 0:
                    //OK ();
                    ((MenuMain)menuMain).OnCloseConfirmationBuilding();
                    break;
                case 1:
                    ((MenuMain)menuMain).OnConfirmationBuilding();
                    break;
            }
    }

    protected void ReadStructures()
    {
        var countOfExisting = 0;
        switch (structureXMLTag)
        {
            case "Building":
                countOfExisting = GameUnitsSettings_Handler.s.resourcesBuildingTypesNo +
                                  GameUnitsSettings_Handler.s.militaryBuildingTypesNo;
                GetBuildingsXML();
                break;
            case "Wall":
                countOfExisting = GameUnitsSettings_Handler.s.wallTypesNo;
                GetWallsXML();
                break;
            case "Weapon":
                countOfExisting = GameUnitsSettings_Handler.s.weaponTypesNo;
                GetWeaponsXML();
                break;
            case "Ambient":
                countOfExisting = GameUnitsSettings_Handler.s.ambientTypesNo;
                GetAmbientXML();
                break;
        }

        //existingStructures = Enumerable.Repeat(0, countOfExisting).ToList();
    }


    protected void GetBuildingsXML() //reads structures from its SO *Will be renamed to GetStructureFromSO()*
    {
        var buildingCategoryLevels = ShopData.Instance.BuildingsCategoryData.category;
        var buildingsCategoryData = buildingCategoryLevels.SelectMany(level => level.levels)
            .Where(c => c.level == _structuressWithLevel).ToList();

        var militaryCategoryLevels = ShopData.Instance.ArmyCategoryData.category;
        var militaryCategoryData = militaryCategoryLevels.SelectMany(level => level.levels)
            .Where(c => c.level == _structuressWithLevel).ToList();

        FillBuildingData(buildingsCategoryData);
        FillBuildingData(militaryCategoryData);
    }

    private void FillBuildingData<T>(List<T> data)
        where T : BaseStoreItemData, INamed, IProdBuilding, IStoreBuilding, IStructure
    {
        foreach (var structureItem in data)
        {
            dictionary = new Dictionary<string, string>();
            dictionary.Add("Name", structureItem.GetName()); // put this in the dictionary.
            dictionary.Add("StructureType", structureItem.GetStructureType().ToString()); // put this in the dictionary.
            dictionary.Add("Description", structureItem.Description);
            dictionary.Add("Currency", structureItem.Currency.ToString());
            dictionary.Add("Price", structureItem.Price.ToString());
            dictionary.Add("ProdType", structureItem.GetProdType().ToString());
            dictionary.Add("ProdPerHour", structureItem.GetProdPerHour().ToString());
            dictionary.Add("StoreType", structureItem.GetStoreType().ToString());
            dictionary.Add("StoreResource", structureItem.GetStoreResource().ToString());
            dictionary.Add("StoreCap", structureItem.GetStoreCap().ToString());

//			if(structureItem.Name == "PopCap"){dictionary.Add("PopCap",structureItem.PopCap.to);}

            dictionary.Add("TimeToBuild", structureItem.TimeToBuild.ToString());
            dictionary.Add("HP", structureItem.HP.ToString());
            dictionary.Add("XpAward", structureItem.XpAward.ToString());
            dictionary.Add("UpRatio", structureItem.UpRatio.ToString());
            structures.Add(dictionary);
        }
    }

    protected void GetWallsXML() //reads structures from its SO *Will be renamed to GetStructureFromSO()*
    {
        var wallCategoryLevels = ShopData.Instance.WallsCategoryData.category;
        var wallCategoryData = wallCategoryLevels.SelectMany(level => level.levels)
            .Where(c => c.level == _structuressWithLevel).ToList();


        foreach (var structureItem in wallCategoryData)
        {
            dictionary = new Dictionary<string, string>();
            dictionary.Add("Name", structureItem.GetName()); // put this in the dictionary.
            dictionary.Add("Currency", structureItem.Currency.ToString());
            dictionary.Add("Price", structureItem.Price.ToString());
            dictionary.Add("TimeToBuild", structureItem.TimeToBuild.ToString());
            dictionary.Add("HP", structureItem.HP.ToString());
            dictionary.Add("XpAward", structureItem.XpAward.ToString());
            dictionary.Add("UpRatio", structureItem.UpRatio.ToString());
            structures.Add(dictionary);
        }
    }

    protected void GetWeaponsXML() //reads structures from its SO *Will be renamed to GetStructureFromSO()*
    {
        var weaponCategoryLevels = ShopData.Instance.WeaponCategoryData.category;
        var weaponCategoryData = weaponCategoryLevels.SelectMany(level => level.levels)
            .Where(c => c.level == _structuressWithLevel).ToList();


        foreach (var structureItem in weaponCategoryData)
        {
            dictionary = new Dictionary<string, string>();
            dictionary.Add("Name", structureItem.GetName()); // put this in the dictionary.
            dictionary.Add("Description", structureItem.Description); // put this in the dictionary.
            dictionary.Add("Currency", structureItem.Currency.ToString());
            dictionary.Add("Price", structureItem.Price.ToString());
            dictionary.Add("TimeToBuild", structureItem.TimeToBuild.ToString());
            dictionary.Add("HP", structureItem.HP.ToString());
            dictionary.Add("Range", structureItem.range.ToString());
            dictionary.Add("FireRate", structureItem.fireRate.ToString());
            dictionary.Add("DamageType", structureItem.damageType.ToString());
            dictionary.Add("TargetType", structureItem.targetType.ToString());
            dictionary.Add("PreferredTarget", structureItem.entityType.ToString());
            dictionary.Add("DamageBonus", structureItem.damageBonus.ToString());
            dictionary.Add("XpAward", structureItem.XpAward.ToString());
            dictionary.Add("UpRatio", structureItem.UpRatio.ToString());
            structures.Add(dictionary);
        }
    }

    protected void GetAmbientXML() //reads structures from its SO *Will be renamed to GetStructureFromSO()*
    {
        var ambientCategoryLevels = ShopData.Instance.AmbientCategoryData.category;
        var ambientCategoryData = ambientCategoryLevels.SelectMany(level => level.levels)
            .Where(c => c.level == _structuressWithLevel).ToList();

        foreach (var structureItem in ambientCategoryData)
        {
            dictionary = new Dictionary<string, string>();
            dictionary.Add("Name", structureItem.GetName()); // put this in the dictionary.
            dictionary.Add("Description", structureItem.Description); // put this in the dictionary.
            dictionary.Add("Currency", structureItem.Currency.ToString());
            dictionary.Add("Price", structureItem.Price.ToString());
            dictionary.Add("TimeToBuild", structureItem.TimeToBuild.ToString());
            dictionary.Add("XpAward", structureItem.XpAward.ToString());
            structures.Add(dictionary);
        }
    }

    private void UpdateStructuresAllowed() //string structureType
    {
        switch (structureXMLTag)
        {
            case "Building":
                allowedStructures = ((Stats)stats).maxBuildingsAllowed;
                break;
            case "Wall":
                allowedStructures = ((Stats)stats).maxWallsAllowed;
                break;
            case "Weapon":
                allowedStructures = ((Stats)stats).maxWeaponsAllowed;
                break;
            case "Ambient":
                allowedStructures = ((Stats)stats).maxAmbientsAllowed;
                break;
        }
    }

    public void UpdateButtons()
    {
        StartCoroutine("UpdateLabelStats");
    }

    protected IEnumerator UpdateLabelStats()
    {
        yield return new WaitForSeconds(xmlLoadDelay);

        UpdateStructuresAllowed();

        bool buildingAllowed;

        for (var i = 0; i < totalStructures; i++)
        {
            // buildingAllowed = allowedStructures[i] - existingStructures[i] > 0;
            var hasMoney = false;
            if (structures[i]["Currency"] == "Gold") //refunds the gold/mana 
            {
                if (((Stats)stats).gold + ((Stats)stats).deltaGoldPlus - ((Stats)stats).deltaGoldMinus >=
                    int.Parse(structures[i]["Price"])) hasMoney = true;
            }
            else if (structures[i]["Currency"] == "Mana")
            {
                if (((Stats)stats).mana + ((Stats)stats).deltaManaPlus - ((Stats)stats).deltaManaMinus >=
                    int.Parse(structures[i]["Price"])) hasMoney = true;
            }
            else if (structures[i]["Currency"] == "Power")
            {
                if (((Stats)stats).power + ((Stats)stats).deltaPowerPlus - ((Stats)stats).deltaPowerMinus >=
                    int.Parse(structures[i]["Price"])) hasMoney = true;
            }
            else if (structures[i]["Currency"] == "Supplies")
            {
                if (((Stats)stats).supplies + ((Stats)stats).deltaSuppliesPlus - ((Stats)stats).deltaSuppliesMinus >=
                    int.Parse(structures[i]["Price"])) hasMoney = true;
            }
            else
            {
                if (((Stats)stats).crystals + ((Stats)stats).deltaCrystalsPlus - ((Stats)stats).deltaCrystalsMinus >=
                    int.Parse(structures[i]["Price"])) hasMoney = true;
            }
        }
    }


    //	xy
    public void MoveNW()
    {
        Move(0);
    } //	-+

    public void MoveNE()
    {
        Move(1);
    } //	++

    public void MoveSE()
    {
        Move(2);
    } //	+-

    public void MoveSW()
    {
        Move(3);
    } //	--


    protected void MovingPadOn() //move pad activated and translated into position
    {
        MovingPad.SetActive(true);

        selectedStructure.transform.parent = MovingPad.transform;
        selectedGrass.transform.parent = MovingPad.transform;

        if (isReselect)
        {
            selectedGrass.transform.position = new Vector3(selectedGrass.transform.position.x,
                selectedGrass.transform.position.y,
                selectedGrass.transform.position.z - 2.0f); //move to front 

            selectedStructure.transform.position = new Vector3(selectedStructure.transform.position.x,
                selectedStructure.transform.position.y,
                selectedStructure.transform.position.z - 6); //move to front
            displacedonZ = true;
        }

        ((CameraController)cameraController).movingBuilding = true;
    }

    protected void Move(int i)
    {
        if (((Relay)relay).pauseMovement || ((Relay)relay).delay) return;

        ((SoundFX)soundFX).Move(); //128x64

        var stepX = (float)gridx / 2;
        var stepY = (float)gridy / 2; //cast float, otherwise  181/2 = 90, and this accumulats a position error;

        switch (i)
        {
            case 0:
                MovingPad.transform.position += new Vector3(-stepX, stepY, 0); //NW	
                break;

            case 1:
                MovingPad.transform.position += new Vector3(stepX, stepY, 0); //NE		
                break;

            case 2:
                MovingPad.transform.position += new Vector3(stepX, -stepY, 0); //SE		
                break;

            case 3:
                MovingPad.transform.position += new Vector3(-stepX, -stepY, 0); //SW		
                break;
        }
    }

    protected void MouseTouchMove()
    {
        touchMoveCounter += Time.deltaTime;

        if (touchMoveCounter > touchMoveTime)
        {
            touchMoveCounter = 0;
            TouchMove();
            if (mouseFollow)
                MouseMove();
        }
    }

    private void TouchMove()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            var touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            if (touchDeltaPosition.x < 0)
            {
                if (touchDeltaPosition.y < 0)
                    MoveSW();
                else if (touchDeltaPosition.y > 0) MoveNW();
            }
            else if (touchDeltaPosition.x > 0)
            {
                if (touchDeltaPosition.y < 0)
                    MoveSE();
                else if (touchDeltaPosition.y > 0) MoveNE();
            }
        }
    }

    public void MouseMove()
    {
        GetMousePosition();
        var deltaPosition = mousePosition - new Vector2(selectedStructure.transform.position.x,
            selectedStructure.transform.position.y);

        if (Mathf.Abs(deltaPosition.x) > gridx || Mathf.Abs(deltaPosition.y) > gridy)
        {
            if (deltaPosition.x < 0)
            {
                if (deltaPosition.y < 0)
                    MoveSW();
                else if (deltaPosition.y > 0) MoveNW();
            }
            else if (deltaPosition.x > 0)
            {
                if (deltaPosition.y < 0)
                    MoveSE();
                else if (deltaPosition.y > 0) MoveNE();
            }
        }
    }

    private void GetMousePosition()
    {
        var gridPos = new Vector3(0, 0, 0);

        // Generate a plane that intersects the transform's position with an upwards normal.
        var playerPlane = new Plane(Vector3.back, new Vector3(0, 0, 0)); //transform.position + 

        // Generate a ray from the cursor position

        Ray RayCast;

        RayCast = Camera.main.ScreenPointToRay(Input.mousePosition);

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

            gridPos = GridManager.instance.nodes[row, col].position;
        }

        mousePosition = gridPos;
    }

    public void AdjustStructureZ(int pivotIndex, int spriteIndex)
    {
        var pivot = selectedStructure.transform.GetChild(pivotIndex);
        var pivotPos = pivot.position; //pivot
        var spritesPos = selectedStructure.transform.GetChild(spriteIndex).position; //sprites
        var correctiony = 10 / (pivotPos.y + 3300); //ex: fg 10 = 0.1   bg 20 = 0.05  
//		all y values must be positive, so we add the grid origin y 3207 +100 to avoid divide by 0; 
//		otherwise depth glitches around y 0
        selectedStructure.transform.GetChild(spriteIndex).position =
            new Vector3(spritesPos.x, spritesPos.y, zeroZ - correctiony); //	transform.GetChild(2).position
    }

    protected void InstantiateStructure(bool isUpgrade = false) //instantiates the building and grass prefabs
    {
        if (isInstant[currentSelection] == 0)
            ((Stats)stats).occupiedBuilders++; //get one builder

        ((Stats)stats).UpdateUI(); //to reflect the free/total builder ratio

        ((Relay)relay).pauseInput = true; //pause all other input - the user starts moving the building

        pivotCorrection = pivotCorrections[currentSelection] == 1;

        var someStructure =
            ShopData.Instance.GetAssetForLevel(currentSelection, _currentCategoryType, _currentItemLevel);
        if (someStructure)
        {
            Vector3 structurePos;
            Vector3 grassPos;
            if (selectedStructure)
            {
                var position = selectedStructure.transform.localPosition;
                structurePos = new Vector3(position.x, position.y, zeroZ);
                grassPos = new Vector3(position.x, position.y, grassZ);
            }
            else
            {
                structurePos = new Vector3(0, 0, zeroZ);
                grassPos = new Vector3(0, 0, grassZ);
            }

            var building =
                ShopData.Instance.GetStructureData(currentSelection, _currentCategoryType, _currentItemLevel);

            var defaultGrassType = 3;
            var grassType = 0;
            if (building is IGrassType)
                grassType = ((IGrassType)building).GetGrassType();
            else
                grassType = defaultGrassType;

            var structure = Instantiate(someStructure, structurePos, Quaternion.identity);
            var grass = Instantiate(GetGrassPrefab(grassType), grassPos,
                Quaternion.identity);


            var structureSelector = structure.GetComponent<StructureSelector>();
            if (structureSelector)
            {
                structureSelector.Id = ((IId)building).GetId();
                structureSelector.Level = _currentItemLevel;
            }

            if (currentSelection == 3 && _currentCategoryType == ShopCategoryType.Weapon)
            {
                var dronePad = structure.GetComponent<DronePad>();
                if (dronePad) dronePad.LaunchDrone(currentSelection, _currentItemLevel, _currentCategoryType);
            }


            selectedStructure = structure;
            selectedGrass = grass;

            SelectStructure(isUpgrade);
        }
        else
        {
            throw new Exception("Can't build a structure, didn't get building");
        }
    }

    protected void
        SelectStructure(
            bool isUpgrade =
                false) //after the grass/building prefabs are instantiated, they must be selected from the existing structures on the map
    {
        MoveBuildingPanelController.Instance.MoveButton.onClick.AddListener(ActivateMovingPad);
        MoveBuildingPanelController.Instance.Show(true);
        MovingPad.SetActive(true);
        MoveBuildingPanelController.Instance.OkButton.onClick.AddListener(OK);

        selectedStructure.GetComponent<StructureSelector>().grassType =
            selectedGrass.GetComponent<GrassSelector>().grassType;

        int posX = 0, posY = 0;

        // if (!isUpgrade)
        {
            if (!isField)
            {
                posX =
                    (int)(Scope.transform.position.x - //calculates the middle of the screen - the Scope position,
                          Scope.transform.position.x %
                          gridx); //and adjusts it to match the grid; the dummy is attached to the 2DToolkit camera
                posY = (int)(Scope.transform.position.y -
                             Scope.transform.position.y % gridy);
            }
            else
            {
                posX = (int)spawnPointList[currentFieldIndex].x;
                posY = (int)spawnPointList[currentFieldIndex].y;
            }
        }


        // if (!isUpgrade)
        {
            var movingPadPosX = selectedStructure.transform.localPosition.x;
            var movingPadPosY = selectedStructure.transform.localPosition.y;
            if (pivotCorrection)
            {
                selectedGrass.transform.position = new Vector3(posX + gridx / 2, posY, grassZ - 2); //grass
                if (isUpgrade)
                {
                    selectedStructure.transform.localPosition =
                        new Vector3(0, 0, zeroZ - 6); //moves the building to position
                    selectedGrass.transform.localPosition = new Vector3(0, 0, grassZ - 2); //grass
                    MovingPad.transform.localPosition = new Vector3(movingPadPosX, movingPadPosY, padZ); //move pad    
                }
                else
                {
                    MovingPad.transform.position = new Vector3(posX + gridx / 2, posY, padZ); //move pad
                    selectedStructure.transform.position =
                        new Vector3(posX + gridx / 2, posY, zeroZ - 6); //moves the building to position
                }
            }

            else
            {
                selectedStructure.transform.position =
                    new Vector3(posX, posY, zeroZ - 6); //the building must appear in front				
                selectedGrass.transform.position = new Vector3(posX, posY, grassZ - 2);
                MovingPad.transform.position = new Vector3(posX, posY, padZ);
            }
        }

        if (!isField)
        {
            selectedStructure.transform.parent =
                MovingPad.transform; //parents the selected building to the arrow moving platform
            selectedGrass.transform.parent = MovingPad.transform; //parents the grass to the move platform

            if (isUpgrade)
            {
                selectedStructure.transform.localPosition =
                    new Vector3(0, 0, selectedStructure.transform.localPosition.z); //moves the building to position
                selectedGrass.transform.localPosition =
                    new Vector3(0, 0, selectedGrass.transform.localPosition.z); //grass   
            }
        }

        ((Relay)relay).pauseInput = true; //pause other input so the user can move the building	
        ((CameraController)cameraController).movingBuilding = true;

        if (isUpgrade) MoveBuildingPanelController.Instance.OkButton.onClick.Invoke();
    }

    public void PlaceStructure()
    {
        Vector3
            grassPos = selectedGrass.transform.position,
            structurePos = selectedStructure.transform.position;
        if (!isReselect)
        {
            ((Stats)stats).structureIndex++;
            structureIndex = ((Stats)stats).structureIndex; //unique number for harvesting

            var sSel = selectedStructure.GetComponent<StructureSelector>();
            var gSel = selectedGrass.GetComponent<GrassSelector>();

            if (gridBased)
                RegisterGridPosition(sSel);

            sSel.structureIndex = structureIndex;
            gSel.StructureIndex = structureIndex; //grassIndex and structureIex are paired

            //instantiates the construction prefab and pass the relevant info;
            if (isInstant[currentSelection] == 0)
            {
                var Construction = Instantiate(constructionPf[constructionTypes[currentSelection]],
                    new Vector3(structurePos.x, structurePos.y, structurePos.z + 6),
                    Quaternion.identity);

                sSel.Id = currentSelection;
                sSel.CategoryType = _currentCategoryType;
                sSel.Level = _currentItemLevel;


                var initializer = selectedStructure.GetComponent<IInitializer>();

                if (initializer != null) initializer.Initialize();


                selectedConstruction = Construction;
                var cSel = selectedConstruction.GetComponent<ConstructionSelector>();
                cSel.constructionIndex = structureIndex;

                cSel.Id = currentSelection;
                cSel.CategoryType = _currentCategoryType;
                cSel.Level = _currentItemLevel;
                cSel.IsProductionBuilding = isProductionBuilding;

                var levels = ShopData.Instance.GetLevels(currentSelection, _currentCategoryType);
                var structure = levels.FirstOrDefault(x => ((ILevel)x).GetLevel() == _currentItemLevel);
                if (structure != null)
                {
                    cSel.buildingTime = structure.TimeToBuild;
                    cSel.StructureType = ((IStructure)structure).GetStructureType();
                    cSel.grassType = gSel.grassType;

                    cSel.ParentGroup = ParentGroup;
                    cSel.structureClass = structureClass;

                    if (structureXMLTag == "Building") cSel.storageAdd = ((IStoreBuilding)structure).GetStoreCap();
                }
            }

            if (gridBased)
            {
                var cSel = selectedConstruction.GetComponent<ConstructionSelector>();
                cSel.iRow = sSel.iRow;
                cSel.jCol = sSel.jCol;
            }
        }
        else
        {
            selectedStructure.GetComponent<StructureSelector>().DeSelect();
        }

        selectedGrass.GetComponentInChildren<GrassCollider>().isMoving = false;
        selectedGrass.GetComponentInChildren<GrassCollider>().enabled = false;

        //--> Reselect
        if (!isReselect)
        {
            if (isInstant[currentSelection] == 0)
            {
                selectedConstruction.transform.SetParent(ParentGroup.transform);
                selectedGrass.transform.SetParent(selectedConstruction.transform);
            }
            else
            {
                selectedGrass.transform.SetParent(selectedStructure.transform);
            }

            selectedGrass.transform.position =
                new Vector3(grassPos.x, grassPos.y, grassPos.z + 2); //cancel the instantiation z correction
            selectedStructure.transform.position =
                new Vector3(structurePos.x, structurePos.y, structurePos.z + 6); //6


            if (isInstant[currentSelection] == 0)
            {
                selectedStructure.transform.SetParent(selectedConstruction.transform);
                selectedStructure.SetActive(false);
                AdjustConstructionZ();
            }
            else
            {
                selectedStructure.transform.SetParent(ParentGroup.transform);
            }
        }
        else if (displacedonZ)
        {
            //send the structures 6 z unit to the background
            selectedGrass.transform.position = new Vector3(grassPos.x, grassPos.y, grassPos.z + 2); //move to back
            selectedStructure.transform.position =
                new Vector3(structurePos.x, structurePos.y, structurePos.z + 6); //6

            selectedStructure.transform.SetParent(ParentGroup.transform);
            selectedGrass.transform.SetParent(selectedStructure.transform);
            displacedonZ = false;
        }

        AdjustStructureZ(1, 2);

        MovingPad.SetActive(false);
        ((CameraController)cameraController).movingBuilding = false;
        MoveBuildingPanelController.Instance.MoveButton.onClick.RemoveAllListeners();
        MoveBuildingPanelController.Instance.OkButton.onClick.RemoveAllListeners();
        MoveBuildingPanelController.Instance.Show(false);

        StartCoroutine("Deselect");
        ((MenuMain)menuMain).waitForPlacement = false;
        Delay(); //delay and pause input = two different things 
    }

    public void PlaceStructureGridInstant()
    {
        if (!isReselect)
        {
            ((Stats)stats).structureIndex++;
            structureIndex =
                ((Stats)stats)
                .structureIndex; //unique number for pairing the buildings and the grass patches underneath

            ((StructureSelector)selectedStructure.GetComponent("StructureSelector")).structureIndex =
                structureIndex;
            ((GrassSelector)selectedGrass.GetComponent("GrassSelector")).StructureIndex = structureIndex;
            PutBackInPlace();
        }
        else if (displacedonZ)
        {
            PutBackInPlace();
            displacedonZ = false;
        }

        selectedStructure.transform.SetParent(ParentGroup.transform);
        selectedGrass.transform.SetParent(selectedStructure.transform);
        var sSel = selectedStructure.GetComponent<StructureSelector>();

        RegisterGridPosition(sSel);

        selectedGrass.GetComponentInChildren<GrassCollider>().isMoving = false;
        selectedGrass.GetComponentInChildren<GrassCollider>().enabled = false;

        AdjustStructureZ(1, 2);
        MovingPad.SetActive(false);
        ((CameraController)cameraController).movingBuilding = false;
        MoveBuildingPanelController.Instance.OkButton.onClick.RemoveAllListeners();
        MoveBuildingPanelController.Instance.MoveButton.onClick.RemoveAllListeners();
        MoveBuildingPanelController.Instance.Show(false);

        if (!isReselect)
        {
            sSel.Id = currentSelection;
            sSel.CategoryType = _currentCategoryType;
            //TODO : set the level of the building, wall, etc
            sSel.Level = _currentItemLevel;
        }

        isReselect = false;
        StartCoroutine("Deselect");
        ((MenuMain)menuMain).waitForPlacement = false;
        Delay(); //delay and pause input = two different things
    }

    private void PutBackInPlace()
    {
        selectedGrass.transform.position = new Vector3(selectedGrass.transform.position.x,
            selectedGrass.transform.position.y,
            selectedGrass.transform.position.z + 2); //move to back

        selectedStructure.transform.position = new Vector3(selectedStructure.transform.position.x,
            selectedStructure.transform.position.y,
            selectedStructure.transform.position.z + 6); //6		
    }

    private void RegisterGridPosition(StructureSelector sSel)
    {
        var indexCell = GridManager.instance.GetGridIndex(selectedStructure.transform.position);

        var row = GridManager.instance.GetRow(indexCell);
        var col = GridManager.instance.GetColumn(indexCell);

        sSel.iRow = row;
        sSel.jCol = col;
    }

    public void CancelObject(bool isUpgrade = false) //cancel construction, or reselect building and destroy/cancel
    {
        var selectedStructureSelector = selectedStructure.GetComponent<StructureSelector>();

        var levels = ShopData.Instance.GetLevels(currentSelection, _currentCategoryType);
        var structure = levels.FirstOrDefault(x => ((ILevel)x).GetLevel() == selectedStructureSelector.Level);

        if (structure == null) return;

        if (!isReselect)
        {
            if (((Stats)stats).occupiedBuilders > 0) ((Stats)stats).occupiedBuilders--; //frees the builder    


            if (structure.Currency == CurrencyType.Gold) //refunds the gold/mana 
                ((Stats)stats).AddResources(structure.Price, 0, 0, 0, 0);
            else if (structure.Currency == CurrencyType.Mana)
                ((Stats)stats).AddResources(0, structure.Price, 0, 0, 0);
            else
                ((Stats)stats).AddResources(0, 0, structure.Price, 0, 0);

            ((Stats)stats).ApplyMaxCaps();
        }

        if (structure is IStoreBuilding && ((IStoreBuilding)structure).GetStoreType() != StoreType.None)
            DecreaseStorage(((IStoreBuilding)structure).GetStoreType().ToString(),
                ((IStoreBuilding)structure).GetStoreCap());

        ((Stats)stats).experience -= structure.XpAward;
        //We comment this line because we don't have PopCap'
//		((Stats)stats).maxHousing -= int.Parse (structures [currentSelection] ["PopCap"]);

        selectedStructureSelector.ResourceGenerator.RemoveMessageNotifications(selectedStructureSelector
            .MessageNotification);
        selectedStructureSelector.ResourceGenerator.RemoveFromExisting(selectedStructureSelector.EconomyBuilding);
        if (structure is IId && structure is IStructure)
            selectedStructureSelector.ResourceGenerator.UpdateBasicValues(((IId)structure).GetId(),
                _currentCategoryType,
                _currentItemLevel, ((IStructure)structure).GetStructureType());


        if (currentSelection == 3 && _currentCategoryType == ShopCategoryType.Weapon)
        {
            var dronePad = selectedStructure.GetComponent<DronePad>();
            if (dronePad) Destroy(dronePad.CreatedDrone);
        }

        Destroy(selectedStructure);
        UpdateExistingStructures(-1,
            structure.MaxCountOfThisItem,
            isUpgrade); //decreases the array which counts how many structures of each type you have 

        Destroy(selectedGrass);

        MovingPad.SetActive(false); //deactivates the arrow building moving platform
        MoveBuildingPanelController.Instance.MoveButton.onClick.RemoveAllListeners();
        MoveBuildingPanelController.Instance.OkButton.onClick.RemoveAllListeners();
        MoveBuildingPanelController.Instance
            .Show(false); //deactivates the buttons move/upgrade/place/cancel, at the bottom of the screen
        ((Relay)relay).pauseInput = false; //while the building is selected, pressing other buttons has no effect
        ((Relay)relay).pauseMovement = false; //the confirmation screen is closed
        if (isReselect) isReselect = false; //end the reselect state	

        ((Stats)stats).UpdateUI();
    }

    private void
        DecreaseStorage(string resType,
            int value) //when a building is reselected and destroyed, the gold/mana storage capacity decrease; 
    {
        if (resType == "Gold")
        {
            ((Stats)stats).maxGold -= value; //the destroyed building storage cap
        }
        else if (resType == "Mana")
        {
            ((Stats)stats).maxMana -= value;
        }
        else if (resType == "Dual") //gold+mana
        {
            ((Stats)stats).maxGold -= value;
            ((Stats)stats).maxMana -= value;
        }

        ((Stats)stats).UpdateUI(); //updates the interface numbers
    }

    //  verifies if the building can be constructed:
    //  exceeds max number of structures / enough gold/mana/free builder to build?
    //  pays the price to Stats; updates the Stats interface numbers
    protected void VerifyConditions(Action callback, bool isUpgrade = false)
    {
        var maxAllowed = 0;

        var levels = ShopData.Instance.GetLevels(currentSelection, _currentCategoryType);
        var building = levels.FirstOrDefault(x => ((ILevel)x).GetLevel() == _currentItemLevel);


        if (building == null) return;

        var structureName = ((INamed)building)?.GetName();
        var maxCount = building.MaxCountOfThisItem;
        var id = ((IId)building).GetId();
        var priceOfStructure = building.Price;
        var xwAward = building.XpAward;
        var currencyType = building.Currency;

        UpdateStructuresAllowed(); //structureXMLTag

        var canBuild = true; //must pass as true through all verifications

        if (!isUpgrade)
            //max allowed structures ok?
            if (existingStructures.ContainsKey(id) && existingStructures[id] >= maxCount) //max already reached
            {
                canBuild = false;
                MessageController.Instance.DisplayMessage("Maximum " + maxCount +
                                                          " structures of type " + structureName + ". " +
                                                          "Upgrade Summoning Circle to build more."); //displays the hint - you can have only 3 structures of this type
            }


        //enough gold?
        if (currencyType == CurrencyType.Gold) //this needs gold
        {
            var existingGold = ((Stats)stats).gold + ((Stats)stats).deltaGoldPlus - ((Stats)stats).deltaGoldMinus;

            if (existingGold < priceOfStructure)
            {
                canBuild = false;
                MessageController.Instance.DisplayMessage("Not enough gold."); //updates hint text
            }
        }
        else if (currencyType == CurrencyType.Mana)
        {
            var existingMana = ((Stats)stats).mana + ((Stats)stats).deltaManaPlus - ((Stats)stats).deltaManaMinus;

            if (existingMana < priceOfStructure)
            {
                canBuild = false;
                MessageController.Instance.DisplayMessage("Not enough mana."); //updates hint text
            }
        }
        else if (currencyType == CurrencyType.Power)
        {
            var existingPower = Stats.Instance.power + Stats.Instance.deltaPowerPlus -
                                Stats.Instance.deltaPowerMinus;
            if (existingPower < priceOfStructure)
            {
                canBuild = false;
                MessageController.Instance.DisplayMessage("Not enough power."); //updates hint text
            }
        }
        else if (currencyType == CurrencyType.Supplies)
        {
            var existingSupplies = Stats.Instance.supplies + Stats.Instance.deltaSuppliesPlus -
                                   Stats.Instance.deltaSuppliesMinus;
            if (existingSupplies < priceOfStructure)
            {
                canBuild = false;
                MessageController.Instance.DisplayMessage("Not enough supplies."); //updates hint text
            }
        }
        else
        {
            var existingCrystals = ((Stats)stats).crystals + ((Stats)stats).deltaCrystalsPlus -
                                   ((Stats)stats).deltaCrystalsMinus;

            if (existingCrystals < priceOfStructure)
            {
                canBuild = false;
                MessageController.Instance.DisplayMessage("Not enough crystals."); //updates hint text
            }
        }

        if (((Stats)stats).occupiedBuilders >= ((Stats)stats).builders) //builder available?
        {
            canBuild = false;
            MessageController.Instance.DisplayMessage("You need more builder.");
        }

        if (canBuild)
        {
            if (callback != null) callback.Invoke();


            ((MenuMain)menuMain).constructionGreenlit = true; //ready to close menus and place the building; 
            //constructionGreenlit bool necessary because the command is sent by pressing the button anyway

            ((Stats)stats).experience += xwAward; //incre	ases Stats experience  // move this to building finished 

            if (((Stats)stats).experience > ((Stats)stats).maxExperience)
            {
                ((Stats)stats).level++;
                ((Stats)stats).experience -= ((Stats)stats).maxExperience;
                ((Stats)stats).maxExperience += 100;
            }

            //pays the gold/mana price to Stats
            if (currencyType == CurrencyType.Gold)
                ((Stats)stats).SubstractResources(priceOfStructure, 0, 0, 0, 0);
            else if (currencyType == CurrencyType.Mana)
                ((Stats)stats).SubstractResources(0, priceOfStructure, 0, 0, 0);
            else if (currencyType == CurrencyType.Power)
                ((Stats)stats).SubstractResources(0, 0, 0, priceOfStructure, 0);
            else if (currencyType == CurrencyType.Supplies)
                ((Stats)stats).SubstractResources(0, 0, 0, 0, priceOfStructure);
            else
                ((Stats)stats).SubstractResources(0, 0, priceOfStructure, 0, 0);

            UpdateButtons(); //payments are made, update all

            ((Stats)stats)
                .UpdateUI(); //tells stats to update the interface - otherwise new numbers are updated but not displayed

            if (!isField || buildingFields)
            {
                if (needToDeleteBeforeUpgrade)
                {
                    CancelObject(isUpgrade);
                    needToDeleteBeforeUpgrade = false;
                }

                UpdateExistingStructures(+1,
                    building.MaxCountOfThisItem,
                    isUpgrade); //an array that keeps track of how many structures of each type exist
                InstantiateStructure(isUpgrade);
            }
        }
        else
        {
            ((MenuMain)menuMain).constructionGreenlit =
                false; //halts construction - the button message is sent anyway, but ignored
        }
    }

    public void ReloadExistingStructures(int index, int maxCount)
    {
        currentSelection = index;
        UpdateExistingStructures(1, maxCount);
    }

    public void ConfigureQuantityForItem(int id)
    {
        var building = GetExistingItem(id);
        var quantity = -1;
        existingStructures.TryGetValue(building.Id, out quantity);
        if (building) building.UpdateQuantity(quantity);
    }

    public BaseShopItem GetExistingItem(int id)
    {
        return ShopController.Instance.ListOfItemsInCategory.Find(x => x.Id == id);
    }

    protected void AddToExistingStructure(int id, int value, int maxCount, bool isUpgrade = false)
    {
        if (isUpgrade) return;
        var val = -1;
        if (existingStructures.TryGetValue(id, out val))
        {
            if (existingStructures[id] < maxCount)
            {
                if (val != -1)
                    existingStructures[id] += value;
            }
            else
            {
                MessageController.Instance.DisplayMessage("Maximum " +
                                                          maxCount); //displays the hint - you can have only 3 structures of this type
            }
        }
        else
        {
            existingStructures.Add(id, value);
        }
    }

    private void UpdateExistingStructures(int value, int maxCount, bool isUpgrade = false) // +1 or -1
    {
        /*
        if you are allowed to have 50 wals/50 wooden fences, you can build any type, and the number 50 is decreased as a hole		
        */
        switch (structureXMLTag)
        {
            case "Building":
                AddToExistingStructure(currentSelection, value, maxCount, isUpgrade);
                var building = ShopController.Instance.ListOfItemsInCategory.Find(x => x.Id == currentSelection);
                if (building) building.UpdateQuantity(existingStructures[currentSelection]);

                break;

            case "Wall":
                //stone walls and wood fences are considered 2 different types, each type can have 50 pieces of any kind at level 1
                if (currentSelection < 3)
                    for (var i = 0; i < 3; i++)
                        existingStructures[i] += value;
                else
                    for (var i = 3; i < existingStructures.Count; i++)
                        existingStructures[i] += value;
                break;

            case "Weapon":
                AddToExistingStructure(currentSelection, value, maxCount, isUpgrade);
                break;
            case "Ambient":
                existingStructures[currentSelection] += value;
                break;
        }
    }

    public void ConstructionFinished(string constructionType) //called by construction selector finish sequence
    {
        switch (constructionType)
        {
            // move this to building finished 

            case "Building":
                /*
                <StoreType>None</StoreType>					<!-- resource stored - none/gold/mana/dual/soldiers-->	
                <StoreCap>0</StoreCap>						<!-- gold/mana/dual/soldiers storage -->			
                */
                ((Stats)stats).occupiedBuilders--; //the builder previously assigned becomes available


                var structureTypeIndex = BuildingTypeToIndex(constructionType);
                var storeCapIncrease = int.Parse(structures[structureTypeIndex]["StoreCap"]);

                if (structures[structureTypeIndex]["StoreType"] == StoreType.None.ToString())
                {
                } //get rid of the none types, most buildings store nothing
                else if (structures[structureTypeIndex]["StoreType"] == "Soldiers")
                {
                    ((Stats)stats).maxHousing += storeCapIncrease;
                }
                else if (structures[structureTypeIndex]["StoreType"] == "Gold")
                {
                    ((Stats)stats).maxGold += storeCapIncrease;
                }
                else if (structures[structureTypeIndex]["StoreType"] == "Mana")
                {
                    ((Stats)stats).maxMana += storeCapIncrease;
                }
                else if (structures[structureTypeIndex]["StoreType"] == "Dual")
                {
                    ((Stats)stats).maxGold += storeCapIncrease;
                    ((Stats)stats).maxMana += storeCapIncrease;
                }

                break;
        }
    }

    private int BuildingTypeToIndex(string constructionType)
    {
        var structureTypeIndex = 0;
        switch (constructionType)
        {
        }

        return structureTypeIndex;
    }

    //receive a Tk2d button message to select an existing building; the button is in the middle of each building prefab and is invisible 

    public void OnReselect(GameObject structure, GameObject grass, string structureType) //string defenseType, 
    {
        selectedStructure = structure;
        selectedGrass = grass;
        GetCurrentSelection(structureType);
        ReselectStructure();
    }

    private void GetCurrentSelection(string structureType)
    {
        //{"Academy","Barrel","Chessboard","Classroom","Forge","Generator","Globe","Summon","Toolhouse","Vault","Workshop"};

        currentSelection = 0;

        foreach (var b in ShopData.Instance.BuildingsCategoryData.category)
            if (b.name.Contains(structureType))
            {
                currentSelection = b.id;
                break;
            }

        foreach (var b in ShopData.Instance.ArmyCategoryData.category)
            if (b.name.Contains(structureType))
            {
                currentSelection = b.id;
                break;
            }

        // switch (structureType) 
        // {
        // case "Toolhouse": 	currentSelection = 0; 	break;
        // case "Forge": 		currentSelection = 1;	break;
        // case "Generator":	currentSelection = 2;	break;
        // case "Vault":		currentSelection = 3;	break;
        // case "Barrel":		currentSelection = 4;	break;
        // case "Summon":		currentSelection = 5;	break;
        // case "Academy":		currentSelection = 6;	break;
        // case "Classroom":	currentSelection = 7;	break;
        // case "Chessboard":	currentSelection = 8;	break;		
        // case "Globe":		currentSelection = 9;	break;		
        // case "Workshop":	currentSelection = 10;	break;
        // case "Tatami":		currentSelection = 11;	break;
        // }
    }

    private void ReselectStructure()
    {
        MoveBuildingPanelController.Instance.Show(true);

        ((Relay)relay).pauseInput = true;

        MovingPad.transform.position =
            new Vector3(selectedStructure.transform.position.x,
                selectedStructure.transform.position.y, padZ);

        selectedGrass.GetComponentInChildren<GrassCollider>().enabled = true;
        selectedGrass.GetComponentInChildren<GrassCollider>().isMoving = true;
    }

    public void Cancel()
    {
        CancelObject();
        Delay();
    }

    public void OK()
    {
        if (((Relay)relay).currentAlphaTween != null)
        {
            if (((Relay)relay).currentAlphaTween.inTransition) //force fade even if in transition
                ((Relay)relay).currentAlphaTween.CancelTransition();

            ((Relay)relay).currentAlphaTween.FadeAlpha(false, 1);
            ((Relay)relay).currentAlphaTween = null;
        }


        Delay();

        inCollision = selectedGrass.GetComponentInChildren<GrassCollider>().inCollision;

        if (!inCollision)
        {
            if (allInstant)
                PlaceStructureGridInstant();
            else
                PlaceStructure();
        }

        ((Relay)relay).pauseMovement = false; //the confirmation screen is closed
        isUpgradeBuilding = false;
    }

    private IEnumerator Deselect()
    {
        yield return new WaitForSeconds(0.3f);
        isReselect = false;
        ((Relay)relay).pauseInput = false; //main menu butons work again
    }

    public void UpgradeBuilding(int id, string structureName, int level, int toLevel, int price,
        CurrencyType currencyType,
        ShopCategoryType categoryType, StructureCreator creator, int timeToBuild)
    {
        UpgradeBuildingWindow.Instance.SetInfo(id, structureName, level, toLevel, price, currencyType, categoryType,
            creator, timeToBuild);
        MoveBuildingPanelController.Instance.UpgradeStructureButton.gameObject.SetActive(false);
        OK();
    }

    private void AdjustConstructionZ()
    {
        var pivotPos = selectedConstruction.transform.GetChild(1).position; //pivot
        var pos = selectedConstruction.transform.GetChild(3).position; //sprites
        var correctiony = 10 / (pivotPos.y + 3300); //ex: fg 10 = 0.1   bg 20 = 0.05  
        //all y values must be positive, so we add the grid origin y 3207 +100 to avoid divide by 0; 
        //otherwise depth glitches around y 0
        selectedConstruction.transform.GetChild(3).position = new Vector3(pos.x, pos.y, zeroZ - correctiony);
    }

    [UsedImplicitly]
    public void ActivateMovingPad() //move pad activated and translated into position
    {
        if (!MovingPad.activeSelf) MovingPadOn();
    }

    private void RecordSpawnPoint()
    {
        var gridPos = new Vector3(0, 0, 0);

        // Generate a plane that intersects the transform's position with an upwards normal.
        var playerPlane = new Plane(Vector3.back, new Vector3(0, 0, 0));

        // Generate a ray from the cursor position

        Ray RayCast;

        if (Input.touchCount > 0)
            RayCast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        else
            RayCast = Camera.main.ScreenPointToRay(Input.mousePosition);

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


            if (!GridManager.instance.nodes[row, col].isObstacle)
            {
                if (!startField)
                {
                    if (!FieldFootstep.activeSelf)
                    {
                        FieldFootstep.SetActive(true);
                        FieldFootstep.GetComponentInChildren<GrassCollider>().collisionCounter = 0;
                        FieldFootstep.GetComponentInChildren<GrassCollider>().inCollision = false;
                    }

                    gridPos = GridManager.instance.nodes[row, col].position;
                    startPosition = gridPos;
                    startCell = indexCell;
                    startCol = col;
                    startRow = row;
                    StartCoroutine(CreateStar(gridPos, row, col));
                    startField = true;
                }
                else if (!endField)
                {
                    gridPos = GridManager.instance.nodes[row, col].position;

                    //don't overlapp || not on the same row/column
                    if (gridPos == startPosition || (startCol != col && startRow != row))
                        return;

                    startCol = 0;
                    startRow = 0;
                    endCell = indexCell;
                    CreateStarArray();
                    starSequencer += 0.1f;
                    StartCoroutine(CreateStar(gridPos, row, col));
                    endField = true;
                }
            }
        }
    }

    private void CreateStarArray()
    {
        int startRow,
            startCol,
            endRow,
            endCol,
            leftRow,
            leftCol,
            rightRow,
            rightCol,
            highCell,
            lowCell,
            highRow,
            highCol,
            lowRow,
            lowCol;

        var gridPos = new Vector3(0, 0, 0);

        if (startCell > endCell)
        {
            highCell = startCell;
            lowCell = endCell;
        }
        else
        {
            highCell = endCell;
            lowCell = startCell;
        }

        startRow = GridManager.instance.GetRow(highCell);
        startCol = GridManager.instance.GetColumn(highCell);

        endRow = GridManager.instance.GetRow(lowCell);
        endCol = GridManager.instance.GetColumn(lowCell);

        leftRow = startRow;
        leftCol = endCol;
        rightRow = endRow;
        rightCol = startCol;

        if (leftRow >= rightRow)
        {
            highRow = leftRow;
            lowRow = rightRow;
        }
        else
        {
            highRow = rightRow;
            lowRow = leftRow;
        }

        if (leftCol >= rightCol)
        {
            highCol = leftCol;
            lowCol = rightCol;
        }
        else
        {
            highCol = rightCol;
            lowCol = leftCol;
        }

        for (var i = highRow; i >= lowRow; i--)
        for (var j = lowCol; j <= highCol; j++)
        {
            gridPos = GridManager.instance.nodes[i, j].position;
            if ((i != startRow || j != startCol) && (i != endRow || j != endCol))
            {
                starSequencer += 0.1f;
                StartCoroutine(CreateStar(gridPos, i, j));
            }
        }

        StartCoroutine(LateOnCreateFields());
    }

    private IEnumerator LateOnCreateFields()
    {
        yield return new WaitForSeconds(starSequencer + 0.2f);
        OnCreateFields();
    }

    private IEnumerator CreateStar(Vector3 gridPos, int iRow, int jCol)
    {
        yield return new WaitForSeconds(starSequencer);
        FieldFootstep.transform.position = new Vector3(gridPos.x, gridPos.y, fieldDetectorZ);

        var Star = Instantiate(spawnPointStar, gridPos, Quaternion.identity);
        spawnPointList.Add(gridPos);

        starList.Add(Star);
        Component sSel = Star.GetComponent<Selector>();
        ((Selector)sSel).iRow = iRow;
        ((Selector)sSel).jCol = jCol;
    }

    public void OnCreateFields()
    {
        CreateFields();
    }

    public void OnCloseFields()
    {
        CloseFields();
    }

    private void CreateFields()
    {
        buildingFields = true;

        for (var i = 0; i < starList.Count; i++)
        {
            currentFieldIndex = i;
            OnFieldBuild();
        }

        DestroyStars();
        CloseFields();
    }


    private void OnFieldBuild()
    {
        Verify();

        if (((MenuMain)menuMain).constructionGreenlit)
            OK();
    }

    private void CloseFields()
    {
        buildingFields = false;
        isField = false;
        drawingField = false;
        if (startField || endField) DestroyStars();
        ((Relay)relay).pauseInput = false;
        ResetFootstepPosition();
        StartCoroutine("DeactivateFootstep");
    }

    private IEnumerator DeactivateFootstep()
    {
        yield return new WaitForSeconds(0.2f);
        FieldFootstep.SetActive(false);
    }

    private void ResetFootstepPosition()
    {
        var gridPos = GridManager.instance.nodes[2, 2].position;
        FieldFootstep.transform.position = new Vector3(gridPos.x, gridPos.y, fieldDetectorZ);
    }


    private void DestroyStars()
    {
        for (var i = 0; i < starList.Count; i++) ((Star)starList[i].GetComponent("Star")).die = true;
        starList.Clear();
        spawnPointList.Clear();
        startField = false;
        endField = false;
        starSequencer = 0.2f;
    }

    private GameObject GetGrassPrefab(int grassType)
    {
        return grassPf.Find(item => item.grassType == grassType).gameObject;
    }

    #region Variables

    protected int
        totalStructures; //will use this for iterations, since the const below is not visible in the inspector; trim excess elements !

    protected const int
        noOfStructures = 20; //number of maximum existing structures ingame, of any kind - buildings, weapons, walls

    protected int _structuressWithLevel = 1; // items with the level - 1

    protected GameObject[] structurePf;

    protected int[]
        constructionTypes,
        grassTypes,
        pivotCorrections; //buildings can have different size grass patches; weapons 2x2,walls 1x1 and removables 1x1 one type each

    public int
        structureIndex =
            -1; //associates the underlying grass with the building on top, so they can be reselected together

    public bool
        inCollision; // prevents placing the structure in inaccessible areas/on top of another building

    public bool
        mouseFollow = true,
        isReselect, //building is under construction or reselected
        myTown = true, //stats game obj does not exist on battle map
        gridBased, //are objects placed by position or by grid Row/Col index
        allInstant;

    public GameObject
        Scope, // a crosshair that follows the middle of the screen, for placing a new building; position is adjusted to exact grid middle point
        ParentGroup, // to keep all structures in one place in the ierarchy, they are parented to this empty object
        MovingPad; // the arrow pad - when created/selected+move, the structures are parented to this object that can move

    public string
        structureXMLTag,
        structureClass; //Building,Wall,Weapon,Ambient

    public GameObject FieldFootstep;

    public TextAsset StructuresXML; //variables for loading building characteristics from XML

    //by correlating structurePrefabs with the associated grass patch, we will be able to use a common formula to instantiate them
    [SerializeField] private List<GrassSelector> grassPf = new();

    public GameObject[] constructionPf =
        new GameObject[10]; //Grass1xWall, Grass1xFence, Grass1xDeco, Grass1xRemovableI, Grass1xRemovableII, Grass1xRemovableIII, Grass2x,Grass3x,Grass4x,Grass5x; separated because removables and walls can be hundreds

    public int[]
        isArray =
            new int[noOfStructures], //set manually 0 false 1 true - placement in array mode rows of flowers, walls
        isInstant =
            new int[noOfStructures]; //set manually 0 false 1 true - no construction sequence - instantiate immediately

    protected ShopCategoryType _currentCategoryType;
    private int _currentItemLevel;

    protected int
        currentSelection,
        gridx = 256, //necessary to adjust the middle screen "target" to the exact grid X position
        gridy = 181, //necessary to adjust the middle screen "target" to the exact grid Y position
        padZ = -3, //moving pad
        zeroZ = 0,
        grassZ = 2;

    protected bool isProductionBuilding;

    protected float
        touchMoveCounter, //drag-moves the buildings in steps
        touchMoveTime = 0.1f,
        xmlLoadDelay = 0.4f; //xml read is slow, some operations must be delayed

    protected Vector2 mousePosition;

    private bool
        dragPhase,
        pivotCorrection,
        displacedonZ; //adjusts the position to match the grid

    private GameObject //make private
        selectedStructure,
        selectedGrass,
        selectedConstruction; //current selected "under construction" prefab

    //private float initSequenceDelay = 0.2f;//necessary to load xml properly


    public List<Dictionary<string, string>> structures = new();
    protected Dictionary<string, string> dictionary;

    public List<int> allowedStructures;

    public Dictionary<int, int> existingStructures = new();
    //Array creator
    //Multiple selection for fields

    public List<GameObject> starList = new(); //make private
    public List<Vector3> spawnPointList = new(); //make private
    public GameObject spawnPointStar;

    private bool //make private
        drawingField,
        startField,
        endField,
        isField,
        buildingFields;

    private float
        starSequencer = 0.2f; //0.2f

    private Vector3 startPosition, endPosition;

    private int
        startCell,
        endCell, //for start/end cells to draw the field
        startRow,
        startCol,
        currentFieldIndex;

    private readonly int
        fieldDetectorZ = 0;

    protected Component stats, soundFX, transData, cameraController, relay, menuMain, resourceGenerator; //protected


    private bool needToDeleteBeforeUpgrade;

    //void Start () {}

    #endregion
}