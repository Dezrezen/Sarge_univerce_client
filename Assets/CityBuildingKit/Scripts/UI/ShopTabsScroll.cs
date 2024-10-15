using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShopTabsScroll : MonoBehaviour
    {
        [SerializeField] private ShopTabButton[] _tabButtons = null;
        [SerializeField] private ScrollRect _scroll = null;

        [SerializeField]
        private List<RectTransform> _scrollItems = new();

        private void Start()
        {
            for (var i = 0; i < _tabButtons.Length; i++)
            {
                _tabButtons[i].Init(i, OnTabClick);
            }
        }

        public void AddScrollPosition(RectTransform itemToScroll)
        {
            _scrollItems.Add(itemToScroll);
        }
        
        private void OnTabClick(int tabIndex)
        {
            for (var i = 0; i < _tabButtons.Length; i++)
            {
                if (i == tabIndex)
                {
                    _tabButtons[i].SetTabActive();
                    StartCoroutine(ScrollViewFocusFunctions.FocusOnItemCoroutine(_scroll, _scrollItems[i], 7f));
                }
                else
                {
                    _tabButtons[i].SetTabInactive();
                }
            }
        }
    }
}