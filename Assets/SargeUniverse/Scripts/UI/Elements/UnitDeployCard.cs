using System;
using Controller;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SargeUniverse.Scripts.UI
{
    public class UnitDeployCard : UnitCard
    {
        [SerializeField] private TMP_Text _amountText = null;
        [SerializeField] private Button _cardButton = null;

        private UnitID _id = UnitID.rifleman;
        private int _amount = 1;

        private void OnDestroy()
        {
            _cardButton.onClick.RemoveListener(CardClick);
        }

        public void InitCard(UnitID id, int level, int amount)
        {
            _id = id;
            _amount = amount;
            
            SetLevel(level.ToString());
            _amountText.text = "x" + amount;

            SetCardImage(GameConfig.instance.UnitsConfig.GetUnitData(id).image);
            
            _cardButton.onClick.AddListener(CardClick);
        }

        private void CardClick()
        {
            BattleControl.Instance.SetDeployUnit(_id, UpdateCard);
        }

        private void UpdateCard(UnitID id)
        {
            if (id != _id)
            {
                return;
            }
            
            _amount -= 1;
            if (_amount == 0)
            {
                BattleControl.Instance.SetDeployUnit(UnitID.empty, null);
                Destroy(gameObject);
            }
            else
            {
                _amountText.text = "x" + _amount;
            }
        }
    }
}