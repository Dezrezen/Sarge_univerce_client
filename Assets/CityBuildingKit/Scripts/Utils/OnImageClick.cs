using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CityBuildingKit.Scripts.Utils
{
    public class OnImageClick : MonoBehaviour, IPointerClickHandler
    {
        public Action onClick;
        private bool _interactable = true;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_interactable)
            {
                onClick?.Invoke();
            }
        }

        public void SetInteractable(bool value)
        {
            _interactable = value;
        }
    }
}