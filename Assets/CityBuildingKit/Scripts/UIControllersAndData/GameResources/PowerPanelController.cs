/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Assets.Scripts.UIControllersAndData.GameResources
{
	public class PowerPanelController:MonoBehaviour
	{
		// [FormerlySerializedAs("_currentGoldCountField")] [SerializeField] private Text _currentPowerCountField;
		// [FormerlySerializedAs("_maxGoldCountField")] [SerializeField] private Text _maxPowerCountField;
		// [FormerlySerializedAs("_goldSlider")] [SerializeField] private Slider _powerSlider;
		
		[FormerlySerializedAs("_currentGoldCountField")] [SerializeField] private TMP_Text _currentPowerCountField;
		[FormerlySerializedAs("_maxGoldCountField")] [SerializeField] private TMP_Text _maxPowerCountField;
		[FormerlySerializedAs("_goldSlider")] [SerializeField] private Slider _powerSlider;

		private void Awake()
		{
			Player.Player.Instance.PlayerEvt.AddListener(Display);
		}

		private void Display(PlayerData data)
		{
			if (data.PlayerResources.Power.MaxValue == 0)
			{
				return;
			}

			if (_currentPowerCountField != null)
				_currentPowerCountField.text = data.PlayerResources.Power.CurrentValue.ToString();
			if (_maxPowerCountField != null) _maxPowerCountField.text = data.PlayerResources.Power.MaxValue.ToString();
			if (_powerSlider != null)
				_powerSlider.value =
					(float)data.PlayerResources.Power.CurrentValue / data.PlayerResources.Power.MaxValue;
		}
	}
}
