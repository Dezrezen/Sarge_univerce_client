using UI.Base;

namespace CityBuildingKit.Scripts.Interface.UI
{
    public interface IInterfaceSwitcher
    {
        void ShowPanel<T>() where T : UIScreen;
        void ClosePanel<T>() where T : UIScreen;
        bool SwitchPanel<T>() where T : UIScreen;
        void HideAllPanels();
        void ShowModalPanel<T>() where T : UIScreen;
        void SwitchModalPanel<T>() where T : UIScreen;
        void CloseModalPanel<T>() where T : UIScreen;
        void CloseModalScreen();
    }
}