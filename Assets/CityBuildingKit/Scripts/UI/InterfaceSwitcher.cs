using System.Collections.Generic;
using System.Linq;
using CityBuildingKit.Scripts.Interface.UI;
using UI.Base;
using UnityEngine;

namespace UI
{
    public class InterfaceSwitcher : MonoBehaviour, IInterfaceSwitcher
    {
        [SerializeField] protected List<UIScreen> screens = new();
        private bool _modalScreenActive = false;

        protected void Start()
        {
            screens.ForEach((screen) => screen.SetInterfaceSwitcher(this));
        }

        public virtual void ShowPanel<T>() where T : UIScreen
        {
            var screen = screens.FirstOrDefault(s => s is T);
            screen?.ShowScreen();
        }
        
        public virtual void ClosePanel<T>() where T : UIScreen
        {
            var screen = screens.FirstOrDefault(s => s is T);
            screen?.HideScreen();
        }
        
        public virtual bool SwitchPanel<T>() where T : UIScreen
        {
            if (_modalScreenActive) return false;
            
            HideAllPanels();
            var screen = screens.FirstOrDefault(s => s is T);
            screen?.ShowScreen();
            return true;
        }

        public virtual void HideAllPanels()
        {
            foreach (var screen in screens)
            {
                screen.HideScreen();
            }
        }

        public virtual void ShowModalPanel<T>() where T : UIScreen
        {
            var screen = screens.FirstOrDefault(s => s is T);
            screen?.ShowScreen();
            _modalScreenActive = true;
        }
        
        public virtual void SwitchModalPanel<T>() where T : UIScreen
        {
            var screen = screens.FirstOrDefault(s => s is T);
            screen?.ShowScreen();
            _modalScreenActive = true;
        }

        public virtual void CloseModalPanel<T>() where T : UIScreen
        {
            var screen = screens.FirstOrDefault(s => s is T);
            screen?.HideScreen();
            _modalScreenActive = false;
        }

        public void CloseModalScreen()
        {
            _modalScreenActive = false;
        }
    }
}