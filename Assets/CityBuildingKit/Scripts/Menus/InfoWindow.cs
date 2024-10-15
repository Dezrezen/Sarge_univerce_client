using Assets.Scripts.UIControllersAndData.Images;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Menus
{
    public class InfoWindow : MonoBehaviour
    {
        public static InfoWindow Instance;
        [SerializeField] private GameObject _infoWindow;
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _infoText;
        [SerializeField] private Image _img;

        private void Awake()
        {
            Instance = this;
            _img.enabled = false;
        }

        public void SetInfo(BaseStoreItemData building)
        {
            _titleText.text = ((INamed)building).GetName() + "(" + ((ILevel)building).GetLevel() + " lvl)";

            if (_img)
            {
                _img.enabled = !string.IsNullOrEmpty(building.IdOfBigIcon);
                _img.sprite = ImageControler.GetImage(building.IdOfBigIcon);
            }

            _infoText.text = building.Description;

            SetActiveInfoWindow(true);
        }

        public void SetActiveInfoWindow(bool active)
        {
            _infoWindow.SetActive(active);
        }
    }
}