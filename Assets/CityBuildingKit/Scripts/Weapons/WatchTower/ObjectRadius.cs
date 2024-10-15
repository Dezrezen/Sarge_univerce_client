using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData.Models;
using JetBrains.Annotations;
using UIControllersAndData;
using UIControllersAndData.Store;
using UnityEngine;

public class ObjectRadius : MonoBehaviour
{
    [SerializeField] private StructureSelector _structureSelector;

    [SerializeField] private float defaultWidthOfRangeOfDetection = 240f;
    [SerializeField] private float defaultHeightOfRangeOfDetection = 116.8f;

    [SerializeField] private SpriteRenderer _rangeSprite;
    [SerializeField] private RectTransform _mask;

    private void Awake()
    {
        _structureSelector.UpdateStructureLevelEvent.AddListener(UpdateStructureLevelHandler);
    }

    [UsedImplicitly]
    public void OnObjectClick()
    {
        if (_structureSelector)
        {
            var data = ShopData.Instance.GetStructureData(_structureSelector.Id, ShopCategoryType.Weapon,
                _structureSelector.Level);

            if (data != null && data is IRange)
            {
                var width = defaultWidthOfRangeOfDetection * ((IRange)data).GetRange();
                var height = defaultHeightOfRangeOfDetection * ((IRange)data).GetRange();
                transform.localScale = new Vector3(width, height, 1);
                _rangeSprite.enabled = true;

                if (_mask)
                {
                    _mask.localScale = transform.localScale;
                    _mask.gameObject.SetActive(true);
                }

                MoveBuildingPanelController.Instance.OkButton.onClick.AddListener(Deselect);
            }
        }
    }

    private void Deselect()
    {
        _rangeSprite.enabled = false;
        if (_mask) _mask.gameObject.SetActive(false);
        MoveBuildingPanelController.Instance.OkButton.onClick.RemoveListener(Deselect);
    }

    private void UpdateStructureLevelHandler()
    {
        OnObjectClick();
    }
}