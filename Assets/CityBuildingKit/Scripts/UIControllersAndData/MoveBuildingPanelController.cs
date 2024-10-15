/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UIControllersAndData
{
    public class MoveBuildingPanelController : MonoBehaviour
    {
        public static MoveBuildingPanelController Instance;
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _moveButton;

        [FormerlySerializedAs("_upgradeBuildingButton")] [SerializeField]
        private Button _upgradeStructureButton;

        [SerializeField] private Button _infoButton;
        [SerializeField] private Button _trainUnitButton;
        [SerializeField] private GameObject _panel;

        public UnityAction UpgradeBuildingAction { get; set; }
        public UnityAction InfoBuildingAction { get; set; }
        public UnityAction TrainUnitAction { get; set; }
        public UnityAction LastOkAction { get; set; }
        public UnityAction LastMoveAction { get; set; }

        public Button UpgradeStructureButton => _upgradeStructureButton;
        public Button InfoStructureButton => _infoButton;
        public Button MoveButton => _moveButton;

        public Button OkButton => _okButton;
        public Button TrainUnitButton => _trainUnitButton;


        public GameObject Panel => _panel;

        private void Awake()
        {
            Instance = this;
        }

        public void Show(bool value)
        {
            Panel.SetActive(value);
            if (!value)
            {
                UpgradeStructureButton.gameObject.SetActive(value);
                InfoStructureButton.gameObject.SetActive(value);
                MoveButton.interactable = true;
                MoveButton.gameObject.SetActive(false);
                TrainUnitButton.gameObject.SetActive(value);
            }
        }
    }
}