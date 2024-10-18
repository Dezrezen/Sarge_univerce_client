using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShopTabButton : MonoBehaviour
    {
        [SerializeField] private Button _activeTabButton = null;
        [SerializeField] private Button _inactiveTabButton = null;

        private int _tabIndex = 0;

        public void Init(int index, Action<int> callback)
        {
            _tabIndex = index;
            
            _activeTabButton.onClick.AddListener(() => callback.Invoke(_tabIndex));
            _inactiveTabButton.onClick.AddListener(() => callback.Invoke(_tabIndex));
        }

        public void SetTabActive()
        {
            _activeTabButton.gameObject.SetActive(true);
            _inactiveTabButton.gameObject.SetActive(false);
        }

        public void SetTabInactive()
        {
            _inactiveTabButton.gameObject.SetActive(true);
            _activeTabButton.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _activeTabButton.onClick.RemoveAllListeners();
            _inactiveTabButton.onClick.RemoveAllListeners();
        }

    }
}