/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using Assets.Scripts.UIControllersAndData.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIControllersAndData.GameResources
{
	public class HousingPanelController:MonoBehaviour 
	{
		// [SerializeField] private Text _currentHousingCountField;
		// [SerializeField] private Text _maxHousingCountField;
		// [SerializeField] private Slider _housingSlider;
		// [SerializeField] private Text _allUnits;
		
		[SerializeField] private TMP_Text _currentHousingCountField;
		[SerializeField] private TMP_Text _maxHousingCountField;
		[SerializeField] private Slider _housingSlider;
		[SerializeField] private TMP_Text _allUnits;

		private void Awake()
		{
			Player.Player.Instance.PlayerEvt.AddListener(Display);
		}

		private void Display(PlayerData data)
		{
			if (data.PlayerResources.Housing.MaxValue == 0)
			{
				return;
			}

			if (_currentHousingCountField != null)
				_currentHousingCountField.text = data.PlayerResources.Housing.CurrentValue.ToString();
			if (_maxHousingCountField != null)
				_maxHousingCountField.text = data.PlayerResources.Housing.MaxValue.ToString();
			if (_housingSlider != null)
				_housingSlider.value = (float)data.PlayerResources.Housing.CurrentValue /
				                       data.PlayerResources.Housing.MaxValue;
			if (_allUnits != null) _allUnits.text = data.AllUnits.ToString();
		}
	}
}
