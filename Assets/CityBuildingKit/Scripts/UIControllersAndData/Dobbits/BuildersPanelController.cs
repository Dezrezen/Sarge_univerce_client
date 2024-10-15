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

namespace Assets.Scripts.UIControllersAndData.Builder
{
    public class BuildersPanelController : MonoBehaviour
    {
        // [FormerlySerializedAs("_dobbitField")] [SerializeField]
        // private Text _builderField;
        
        [SerializeField]
        private TMP_Text _builderField;

        // [FormerlySerializedAs("_dobbitSlider")] [SerializeField]
        // private Slider _builderSlider;


        /// <summary>
        ///     Called before Start
        /// </summary>
        private void Awake()
        {
            Player.Player.Instance.PlayerEvt.AddListener(Display);
        }

        private void Display(PlayerData data)
        {
            if (data.PlayerResources.Builder.MaxValue == 0) return;
            // _builderSlider.value = 1 - data.PlayerResources.Builder.CurrentValue /
            //     (float)data.PlayerResources.Builder.MaxValue;
            if (_builderField != null)
                _builderField.text = data.PlayerResources.Builder.MaxValue - data.PlayerResources.Builder.CurrentValue +
                                     " / " + data.PlayerResources.Builder.MaxValue;
        }
    }
}