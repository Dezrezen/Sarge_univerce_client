﻿using UnityEngine;

namespace UI
{
    public class ShopTabsGroup : MonoBehaviour
    {
        [SerializeField] private ShopTabButton[] _tabButtons = null;
        [SerializeField] private GameObject[] _tabPanels = null;

        private void Start()
        {
            for (var i = 0; i < _tabButtons.Length; i++)
            {
                _tabButtons[i].Init(i, OnTabClick);
            }
        }

        private void OnTabClick(int tabIndex)
        {
            for (var i = 0; i < _tabButtons.Length; i++)
            {
                if (i == tabIndex)
                {
                    _tabButtons[i].SetTabActive();
                    _tabPanels[i]?.SetActive(true);
                }
                else
                {
                    _tabButtons[i].SetTabInactive();
                    _tabPanels[i]?.SetActive(false);
                }
            }
        }
    }
}