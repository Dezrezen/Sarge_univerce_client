using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

namespace CityBuildingKit.Scripts.NewUI
{
    public class ShopInnerTab: MonoBehaviour
    {
        [SerializeField] private RectTransform itemToScroll;

        public RectTransform ItemToScroll
        {
            get => itemToScroll;
            set => itemToScroll = value;
        }

        [SerializeField] private ScrollRect scrollRect;

        public void OnInnerTabClick()
        {
            if (itemToScroll && scrollRect)
            {
                StartCoroutine(ScrollViewFocusFunctions.FocusOnItemCoroutine(scrollRect, itemToScroll, 7f));    
            }
            
        }
    }
}