using System.Collections;
using System.Linq;
using Assets.Scripts.Menus;
using Assets.Scripts.UIControllersAndData;
using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData.Models;
using CityBuildingKit.Scripts.Utils;
using UIControllersAndData;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UnityEngine;
using UnityEngine.Events;

public class StructureSelector : BaseSelector
{
    [SerializeField] private bool isPowerPlant;

    //attached to each building as an invisible 2dtoolkit button

    private int _level = -1;

    public ShopCategoryType CategoryType { get; set; } = ShopCategoryType.None;

    public int Level
    {
        get => _level;
        set
        {
            _level = value;
            UpdateStructureLevelEvent.Invoke();
        }
    }

    public int Id { get; set; } = -1;

    public UnityEvent UpdateStructureLevelEvent { get; set; } = new();

    public UnityEvent FinishBuildingEvent { get; set; } = new();
    public UnityEvent OkEvent { get; set; } = new();

    public BaseStoreItemData StructureData { get; set; }

    public GrassSelector StructureGrassSelector { get; set; }

    private void Start()
    {
        InitializeComponents();
        InitializeSpecificComponents();

        BroadcastMessage("IsObjectStructure", SendMessageOptions.DontRequireReceiver);

        StructureData = ShopData.Instance.GetStructureData(Id, CategoryType, Level);

        StructureGrassSelector = GetComponentInChildren<GrassSelector>();
    }

    public void DeSelect()
    {
        if (structureClass == "Weapon" && alphaTween)
            alphaTween.FadeAlpha(false, 1);

        DeSelectAction.Invoke();
    }

    public void ReSelect()
    {
        if (!CameraController.Instance.enabled) return;

        if (((Relay)relay).delay || ((Relay)relay).pauseInput) return;

        ((StructureTween)scaleTween).Tween();

        if (((Relay)relay).currentAlphaTween != null)
        {
            if (((Relay)relay).currentAlphaTween.inTransition) //force fade even if in transition
                ((Relay)relay).currentAlphaTween.CancelTransition();

            ((Relay)relay).currentAlphaTween.FadeAlpha(false, 1);
            ((Relay)relay).currentAlphaTween = null;
        }

        ReSelectAction.Invoke();

        //TODO: rework this adding of listeners. To fix it, mechanism of building selection should be reworked first
        if (MoveBuildingPanelController.Instance.LastMoveAction != null)
            MoveBuildingPanelController.Instance.MoveButton.onClick.RemoveListener(MoveBuildingPanelController.Instance
                .LastMoveAction);

        MoveBuildingPanelController.Instance.LastMoveAction = () =>
        {
            ((StructureCreator)structureCreator).ActivateMovingPad();
        };
        MoveBuildingPanelController.Instance.MoveButton.onClick.AddListener(MoveBuildingPanelController.Instance
            .LastMoveAction);


        if (MoveBuildingPanelController.Instance.LastOkAction != null)
            MoveBuildingPanelController.Instance.OkButton.onClick.RemoveListener(MoveBuildingPanelController.Instance
                .LastOkAction);

        if (MoveBuildingPanelController.Instance.UpgradeBuildingAction != null)
            MoveBuildingPanelController.Instance.UpgradeStructureButton.onClick.RemoveListener(
                MoveBuildingPanelController.Instance.UpgradeBuildingAction);

        if (MoveBuildingPanelController.Instance.InfoBuildingAction != null)
            MoveBuildingPanelController.Instance.InfoStructureButton.onClick.RemoveListener(
                MoveBuildingPanelController.Instance.InfoBuildingAction);

        if (MoveBuildingPanelController.Instance.TrainUnitAction != null)
            MoveBuildingPanelController.Instance.TrainUnitButton.onClick.RemoveListener(
                MoveBuildingPanelController.Instance.TrainUnitAction);

        MoveBuildingPanelController.Instance.LastOkAction = () =>
        {
            ((StructureCreator)structureCreator).OK();
            OkEvent.Invoke();
        };
        MoveBuildingPanelController.Instance.OkButton.onClick.AddListener(MoveBuildingPanelController.Instance
            .LastOkAction);


        if (structureClass == "Weapon")
            if (alphaTween)
            {
                alphaTween.FadeAlpha(true, 1);
                ((Relay)relay).currentAlphaTween = alphaTween;
            }

        ((SoundFX)soundFX).Click();

        if (!battleMap)
            if (!((StructureCreator)structureCreator).isReselect &&
                !((Relay)relay).pauseInput)
            {
                if (messageNotification != null && messageNotification.isReady && !isPowerPlant)
                {
                    messageNotification.FadeOut();
                    ResourceGenerator.Harvest(structureIndex);
                    messageNotification.isReady = false;
                    return;
                }

                if (isPowerPlant && PowerStorageController.Instance.FindAnyNotFulledStorage() &&
                    messageNotification != null && messageNotification.isReady)
                {
                    messageNotification.FadeOut();
                    ResourceGenerator.Harvest(structureIndex);
                    messageNotification.isReady = false;
                    return;
                }


                ((BaseCreator)structureCreator).isReselect = true;
                var childrenNo = gameObject.transform.childCount; //grass was parented last
                ((BaseCreator)structureCreator).OnReselect(gameObject,
                    gameObject.transform.GetChild(childrenNo - 1).gameObject, structureType.ToString());
            }

        if (!battleMap) CheckLevels();
    }

    private void CheckLevels()
    {
        var levels = ShopData.Instance.GetLevels(Id, CategoryType);

        var nextLevel = levels.Count == Level + 1 ? Level + 1 : Level;
        var building = levels?.Find(x => ((ILevel)x).GetLevel() == nextLevel);

        var currentBuilding = levels?.Find(x => ((ILevel)x).GetLevel() == Level);

        var requiredHQLevelForUpgrade = ((IRequiredHQLevel)currentBuilding).GetRequiredHQLevel();
        if (building != null && levels.Count > 1 && levels.Count > Level)
        {
            if (Stats.Instance.CurrentHqLevel >= requiredHQLevelForUpgrade)
            {
                MoveBuildingPanelController.Instance.UpgradeStructureButton.interactable = true;
            }
            else
            {
                MessageController.Instance.DisplayMessage("First, UPGRADE YouR HQ to Required LeveL: " +
                                                          requiredHQLevelForUpgrade +
                                                          "; current level is: " + Stats.Instance.CurrentHqLevel);

                MoveBuildingPanelController.Instance.UpgradeStructureButton.interactable = false;
            }
        }
        else
        {
            MoveBuildingPanelController.Instance.UpgradeStructureButton.interactable = false;
        }

        if (building != null && Stats.Instance.IsHqExist)
        {
            var toLevel = nextLevel;
            var price = building.Price;
            var currencyType = building.Currency;
            var structureName = ((INamed)building).GetName();

            if (!inConstruction)
            {
                MoveBuildingPanelController.Instance.InfoBuildingAction = () =>
                {
                    var buildingInfo = levels?.FirstOrDefault(x => ((ILevel)x).GetLevel() == Level);
                    InfoWindow.Instance.SetInfo(buildingInfo);
                };
                MoveBuildingPanelController.Instance.InfoStructureButton.onClick.AddListener(MoveBuildingPanelController
                    .Instance.InfoBuildingAction);


                var isBuildingToTrainUnit = ArmyUtil.IsBuildingToTrainUnit(
                    ((IStructure)building).GetStructureType());

                if (isBuildingToTrainUnit)
                {
                    MoveBuildingPanelController.Instance.TrainUnitButton.gameObject.SetActive(true);
                    MoveBuildingPanelController.Instance.TrainUnitAction = () =>
                    {
                        var buildingInfo = levels?.FirstOrDefault(x => ((ILevel)x).GetLevel() == Level);
                        //TODO: add here -some dialog - show something with an info 

                        MoveBuildingPanelController.Instance.TrainUnitButton.gameObject.SetActive(false);
                        ((StructureCreator)structureCreator).OK();
                    };
                    MoveBuildingPanelController.Instance.TrainUnitButton.onClick.AddListener(MoveBuildingPanelController
                        .Instance.TrainUnitAction);
                }
            }


            MoveBuildingPanelController.Instance.UpgradeBuildingAction = () =>
                {
                    UpgradeBuilding(structureName, toLevel, price, currencyType, building);
                }
                ;
            MoveBuildingPanelController.Instance.UpgradeStructureButton.onClick.AddListener(MoveBuildingPanelController
                .Instance.UpgradeBuildingAction);


            MoveBuildingPanelController.Instance.UpgradeStructureButton.gameObject.SetActive(true);
            MoveBuildingPanelController.Instance.InfoStructureButton.gameObject.SetActive(true);
            if (currentBuilding is IMovable)
            {
                MoveBuildingPanelController.Instance.MoveButton.interactable = ((IMovable)currentBuilding).IsMovable();
                MoveBuildingPanelController.Instance.MoveButton.gameObject.SetActive(true);
            }
        }
    }


    private void UpgradeBuilding(string structureName, int toLevel, int price, CurrencyType currencyType,
        BaseStoreItemData building)
    {
        ((StructureCreator)structureCreator).UpgradeBuilding(Id, structureName, Level, toLevel, price,
            currencyType, CategoryType, (StructureCreator)structureCreator, building.TimeToBuild);
    }

    public void LateRegisterAsProductionBuilding()
    {
        StartCoroutine("RegisterAsProductionBuilding");
    }

    private IEnumerator RegisterAsProductionBuilding() //private IEnumerator 
    {
        yield return new WaitForSeconds(0.5f);

        for (var i = 0; i < ResourceGenerator.basicEconomyValues.Length; i++)
            if (ResourceGenerator.basicEconomyValues[i].StructureType == structureType)
            {
                CopyBasicValues(ResourceGenerator.basicEconomyValues[i]);
                break;
            }
    }

    private void CopyBasicValues(EconomyBuilding basicValuesEB)
    {
        //EconomyBuilding myEconomyParams = new EconomyBuilding();

        var EBArray = EconomyBuildings.GetComponentsInChildren<EconomyBuilding>();

        var buildingRegistered = false;

        if (EBArray.Length != 0)
            foreach (var eb in EBArray)
                if (eb.structureIndex == structureIndex)
                {
                    eb.structureIndex = structureIndex;
                    eb.ProdPerHour = basicValuesEB.ProdPerHour;
                    eb.StoreCap = basicValuesEB.StoreCap;
                    eb.StructureType = structureType;
                    eb.ProdType = basicValuesEB.ProdType;
                    eb.StoreType = basicValuesEB.StoreType;
                    eb.StoreResource = basicValuesEB.StoreResource;

                    ResourceGenerator.index++;
                    productionListIndex = ResourceGenerator.index;
                    ResourceGenerator.ExistingEconomyBuildings.Add(eb);
                    RegisterNotification();

                    buildingRegistered = true;
                }

        if (!buildingRegistered)
        {
            var eb = EconomyBuildings.AddComponent<EconomyBuilding>();

            eb.structureIndex = structureIndex;
            eb.ProdPerHour = basicValuesEB.ProdPerHour;
            eb.StoreCap = basicValuesEB.StoreCap;
            eb.StructureType = structureType;
            eb.ProdType = basicValuesEB.ProdType;
            eb.StoreType = basicValuesEB.StoreType;
            eb.StoreResource = basicValuesEB.StoreResource;

            ResourceGenerator.index++;
            productionListIndex = ResourceGenerator.index;
            ResourceGenerator.ExistingEconomyBuildings.Add(eb);

            EconomyBuilding = eb;

            RegisterNotification();
        }
    }

    private void RegisterNotification()
    {
        var m = GetComponent<MessageNotification>();
        m.structureIndex = structureIndex;
        ResourceGenerator.RegisterMessageNotification(m);
    }

    public void LateCalculateElapsedProduction(int elapsedTime)
    {
        StartCoroutine(CalculateElapsedProduction(elapsedTime));
    }

    private IEnumerator CalculateElapsedProduction(int elapsedTime)
    {
        yield return new WaitForSeconds(1.0f);

        ResourceGenerator.FastPaceProductionIndividual(productionListIndex, elapsedTime);
    }
}