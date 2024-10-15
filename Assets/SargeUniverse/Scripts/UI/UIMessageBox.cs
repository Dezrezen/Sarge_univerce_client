using System;
using System.Collections.Generic;
using SargeUniverse.Scripts.Model.UI;
using SargeUniverse.Scripts.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SargeUniverse.Scripts.UI
{
    public class UIMessageBox : MonoBehaviour
    {
        [SerializeField] private TMP_Text _messageText = null;
        [SerializeField] private MessageBoxButton _messageBoxButtonPrefab = null;
        [SerializeField] private Transform _buttonsGroup = null;
        private List<MessageBoxButton> _buttons = new();

        private void Clear()
        {
            foreach (var button in _buttons)
            {
                Destroy(button.gameObject);
            }
            _buttons.Clear();
        }

        private MessageBoxButton CreateButton(string label = "Ok")
        {
            var button = Instantiate(_messageBoxButtonPrefab, _buttonsGroup);
            button.Init(label);
            return button;
        }
        
        public void Show(string message, MessageBoxButtonData data)
        {
            _messageText.text = message;

            var button = CreateButton(data.labelText);
            button.AddClickAction(data.callback);
            button.AddClickAction(Hide);
            _buttons.Add(button);
            
            gameObject.SetActive(true);
        }

        public void Show(string message, List<MessageBoxButtonData> dataList)
        {
            _messageText.text = message;

            foreach (var data in dataList)
            {
                var button = CreateButton(data.labelText);
                button.AddClickAction(data.callback);
                button.AddClickAction(Hide);
                _buttons.Add(button);
            }
            
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            Clear();
        }
    }
}