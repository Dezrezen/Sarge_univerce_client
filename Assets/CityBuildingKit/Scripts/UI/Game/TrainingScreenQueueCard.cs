using System;
using TMPro;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    public class TrainingScreenQueueCard : UnitCard
    {
        [SerializeField] private Button removeButton;
        [SerializeField] private TMP_Text amountLabel;
        
        public void SetCardInfo(int amount, Action callback)
        {
            amountLabel.text = amount.ToString();
            removeButton.onClick.AddListener(callback.Invoke);
        }

        public void UpdateAmount(int amount)
        {
            amountLabel.text = amount.ToString();
        }
    }
}