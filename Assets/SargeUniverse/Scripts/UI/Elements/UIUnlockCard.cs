using System;
using System.Linq;
using Controller;
using SargeUniverse.Scripts.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SargeUniverse.Scripts.UI
{
    public class UIUnlockCard : MonoBehaviour, IDisposable
    {
        [SerializeField] private Image _buildingImage = null;
        [SerializeField] private TMP_Text _amountText = null;
        [SerializeField] private GameObject _newCard = null;

        public void InitCard(BuildingID buildingId, int amount = 1, bool isNew = false)
        {
            var sprite = GameConfig.instance.BuildingConfig.GetBuildingData(buildingId).playerLevelSprites.First();
            _buildingImage.sprite = sprite;
            _amountText.text = "x" + amount;
            _amountText.gameObject.SetActive(amount > 1);
            _newCard.SetActive(isNew);
        }


        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}