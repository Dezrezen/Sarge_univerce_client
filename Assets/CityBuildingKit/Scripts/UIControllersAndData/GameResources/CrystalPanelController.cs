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
	public class CrystalPanelController:MonoBehaviour 
	{
		// [SerializeField] private Text _currenCrystalCountField;
		[SerializeField] private TMP_Text _currenCrystalCountField;
		[SerializeField] private Slider _crystalSlider;

		private void Awake()
		{
			Player.Player.Instance.PlayerEvt.AddListener(Display);
		}

		private void Display(PlayerData data)
		{
			if (data.PlayerResources.Crystal.MaxValue == 0)
			{
				return;
			}

			if (_currenCrystalCountField != null)
				_currenCrystalCountField.text = data.PlayerResources.Crystal.CurrentValue.ToString();
			if (_crystalSlider != null)
				_crystalSlider.value = (float)data.PlayerResources.Crystal.CurrentValue /
				                       data.PlayerResources.Crystal.MaxValue;
		}
	}
}
