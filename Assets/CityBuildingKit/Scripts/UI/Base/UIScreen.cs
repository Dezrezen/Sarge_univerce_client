using CityBuildingKit.Scripts.Interface.UI;
using UnityEngine;

namespace UI.Base
{
    public abstract class UIScreen : MonoBehaviour
    {
        protected IInterfaceSwitcher InterfaceSwitcher;

        protected virtual void Start()
        {
            SubscribeForEvents();
        }
        
        protected void OnDestroy()
        {
            UnSubscribeFromEvents();
        }

        public void SetInterfaceSwitcher(IInterfaceSwitcher interfaceSwitcher)
        {
            InterfaceSwitcher = interfaceSwitcher;
        }

        public virtual void ShowScreen()
        {
            gameObject.SetActive(true);
        }

        public virtual void HideScreen()
        {
            gameObject.SetActive(false);
        }

        protected abstract void SubscribeForEvents();
        protected abstract void UnSubscribeFromEvents();
    }
}