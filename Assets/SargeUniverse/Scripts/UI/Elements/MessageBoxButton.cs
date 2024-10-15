using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SargeUniverse.Scripts.UI.Elements
{
    public class MessageBoxButton : MonoBehaviour
    {
        [SerializeField] private Button _button = null;
        [SerializeField] private TMP_Text _buttonLabel = null;

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void Init(string text)
        {
            _buttonLabel.text = text;
        }

        public void AddClickAction(Action callback)
        {
            if (callback != null)
            {
                _button.onClick.AddListener(callback.Invoke);
            }
        }
    }
}