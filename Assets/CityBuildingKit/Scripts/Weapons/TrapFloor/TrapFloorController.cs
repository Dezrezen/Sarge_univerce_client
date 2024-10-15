using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData.Models;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Unit;
using UnityEngine;

public class TrapFloorController : MonoBehaviour
{
    [SerializeField] private StructureSelector _structureSelector;
    [SerializeField] private float defaultWidthOfRangeOfDetection = 240f;
    [SerializeField] private float defaultHeightOfRangeOfDetection = 116.8f;
    [SerializeField] private RectTransform _mask;
    private int amountOfFilledSlots;

    private int amountOfSlots;
    private Selector selector;

    // Start is called before the first frame update
    private void Awake()
    {
        if (_structureSelector)
            _structureSelector.UpdateStructureLevelEvent.AddListener(UpdateStructureLevelHandler);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Unit"))
        {
            selector = collider.GetComponent<Selector>();
            if (selector)
            {
                var unit = ShopData.Instance.GetStructureData(selector.ID, ShopCategoryType.Unit, selector.Level);

                if (unit != null)
                    if (amountOfFilledSlots < amountOfSlots)
                        amountOfFilledSlots += ((UnitCategory)unit).GetAmountOfSlots();

                //TODO:
                //STOP an unit.
                // When the unit falls into the trap there is an animation of floor
                // tiles being uncovered from grass and breaking, revealing the hole.
                // Then the unit disappears and the floor tiles
                // come back upon the hole and are covered with grass.
                // The trapfloor is hidden again until the next unit triggers it.
            }
        }
    }

    private void UpdateStructureLevelHandler()
    {
        SetRangeOfDetection();
    }

    private void SetRangeOfDetection()
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
                if (_mask) _mask.localScale = transform.localScale;
            }

            if (data != null && data is IAmountOfSlots) amountOfSlots = ((IAmountOfSlots)data).GetAmountOfSlots();
        }
    }
}