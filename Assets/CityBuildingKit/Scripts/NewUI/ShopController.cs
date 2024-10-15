using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData.StructureType;
using UIControllersAndData;
using UIControllersAndData.Models;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Buildings;
using UIControllersAndData.Store.Categories.Military;
using UIControllersAndData.Store.Categories.Weapon;
using UIControllersAndData.Store.ShopItems;
using UIControllersAndData.Store.ShopItems.Building;
using UnityEngine;
using UnityEngine.UI;

namespace CityBuildingKit.Scripts.NewUI
{
    public class ShopController : MonoBehaviour
    {
        public static ShopController Instance;

        [SerializeField] private Text _buildingsField;
        [SerializeField] private Slider _buildingsSlider;
        [SerializeField] private int max = 100;

        [SerializeField] private Text _shieldField;
        [SerializeField] private Slider _shieldSlider;
        [SerializeField] private int maxShields = 100;


        [SerializeField] private Text _creditsField;
        [SerializeField] private Slider _creditsSlider;
        [SerializeField] private int maxCredits = 100;


        [SerializeField] private BuildingItem _buildingItem;
        [SerializeField] private BuildingItem _weaponItem;
        [SerializeField] private BuildingItem _armyItem;
        [SerializeField] private StructureCreator _buildingCreator;
        [SerializeField] private StructureCreator _weaponCreator;
        [SerializeField] private RectTransform _content;
        [SerializeField] private RectTransform _decorationsContent;
        [SerializeField] private Button _closeButton;

        [SerializeField] private ShopInnerTab _resourcesTab; //Buildings
        [SerializeField] private ShopInnerTab _armyTab; //Army
        [SerializeField] private ShopInnerTab _defenseTab; //Weapon

        private int currentBuildingsValue;
        private int currentCreditsValue;
        private int currentShieldsValue;

        public List<BaseShopItem> ListOfItemsInCategory { get; } = new();


        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        private void Start()
        {
            _buildingsSlider.maxValue = max;
            _shieldSlider.maxValue = maxShields;
            _creditsSlider.maxValue = maxCredits;
        }

        public void UpdateBuildingsUI()
        {
            if (currentBuildingsValue < max)
            {
                currentBuildingsValue++;
                _buildingsSlider.value = currentBuildingsValue;
                _buildingsField.text = currentBuildingsValue + "/" + max;
            }
        }

        public void UpdateShieldsUI()
        {
            if (currentShieldsValue < maxShields)
            {
                currentShieldsValue++;
                _shieldSlider.value = currentShieldsValue;
                _shieldField.text = currentShieldsValue + "/" + maxShields;
            }
        }

        public void UpdateCreditsUI()
        {
            if (currentCreditsValue < maxCredits)
            {
                currentCreditsValue++;
                _creditsSlider.value = currentCreditsValue;
                _creditsField.text = currentCreditsValue + "/" + maxCredits;
            }
        }

        /// <summary>
        ///     Clears a shop category
        /// </summary>
        /// <param name="shopCategoryType"></param>
        private void ClearShopCategory(RectTransform content, ShopCategoryType shopCategoryType = ShopCategoryType.None)
        {
            _resourcesTab.ItemToScroll = new RectTransform();
            foreach (Transform child in content.transform) Destroy(child.gameObject);
            ;
        }

        public void OnStoreCategoryHandler(int categoryIndex)
        {
            ClearShopCategory(_decorationsContent);
            ClearShopCategory(_content);
            ListOfItemsInCategory.Clear();

            IEnumerable<DrawCategoryData> drawCategoryData;
            object category;
            switch (categoryIndex)
            {
                case 0:
                    category = ShopData.Instance.ArmyCategoryData;
                    var armyCategory = (ArmyCategoryData)category;
                    var armyCategories = armyCategory.category.Select(item =>
                    {
                        var baseData = item.levels[0];
                        return new DrawCategoryData { BaseItemData = baseData, Id = item, Name = item };
                    });
                    DrawCategory(armyCategories, ShopCategoryType.Army);
                    
                    
                    category = ShopData.Instance.BuildingsCategoryData;
                    var buildingCategory = (BuildingsCategoryData)category;
                    var buildingsCategories = buildingCategory.category.Select(item =>
                    {
                        var baseData = item.levels[0];
                        return new DrawCategoryData { BaseItemData = baseData, Id = item, Name = item };
                    });
                    DrawCategory(buildingsCategories, ShopCategoryType.Buildings);


                    category = ShopData.Instance.WeaponCategoryData;
                    var weaponCategory = (WeaponCategoryData)category;
                    var weaponCategories = weaponCategory.category.Select(item =>
                    {
                        var baseData = item.levels[0];
                        return new DrawCategoryData { BaseItemData = baseData, Id = item, Name = item };
                    });
                    DrawCategory(weaponCategories, ShopCategoryType.Weapon);

                    break;

                // case 1:
                //     category = ShopData.Instance.ArmyCategoryData;
                //     var armyCategory = (ArmyCategoryData)category;
                //     var armyCategories = armyCategory.category.Select(item =>
                //     {
                //         var baseData = item.levels[0];
                //         return new DrawCategoryData { BaseItemData = baseData, Id = item, Name = item };
                //     });
                //     DrawCategory(armyCategories, ShopCategoryType.Army);
                //     break;
            }
        }

        private void DrawCategory(IEnumerable<DrawCategoryData> items, ShopCategoryType shopCategoryType)
        {
            // ClearShopCategory(_content, shopCategoryType);
            // ClearShopCategory(_decorationsContent, shopCategoryType);
            // ListOfItemsInCategory.Clear();

            foreach (var data in items)
            {
                //TODO: maybe check if we should display SupplyStorage and use some array of available items
                if (((IStructure)data.BaseItemData).GetStructureType() == StructureType.HomeBase) continue;
                BaseShopItem cell;
                cell = CreateCell(shopCategoryType, data);
                if (cell != null && data.BaseItemData != null)
                {
                    AddListener(cell, shopCategoryType, data);
                    switch (shopCategoryType)
                    {
                        case ShopCategoryType.Buildings:
                        case ShopCategoryType.Army:
                        case ShopCategoryType.Weapon:
                            _buildingCreator.ConfigureQuantityForItem(data.Id.GetId());
                            break;
                    }
                }
            }
        }

        private BaseShopItem CreateCell(ShopCategoryType shopCategoryType, DrawCategoryData data)
        {
            BaseShopItem cell = null;

            if (shopCategoryType == ShopCategoryType.Store &&
                TransData.Instance.ListOfIdSoldItems.Contains(data.Id.GetId())) return null;

            switch (shopCategoryType)
            {
                case ShopCategoryType.Buildings:
                    cell = Instantiate(_buildingItem, _content.transform);
                    cell.Initialize(data, shopCategoryType);
                    ListOfItemsInCategory.Add(cell);
                    cell.name = ((INamed)cell.ItemData).GetName();
                    if (_resourcesTab && !_resourcesTab.ItemToScroll) _resourcesTab.ItemToScroll = cell.RectTransform;
                    break;
                case ShopCategoryType.Weapon:
                    cell = Instantiate(_weaponItem, _content.transform);
                    cell.Initialize(data, shopCategoryType);
                    ListOfItemsInCategory.Add(cell);
                    cell.name = ((INamed)cell.ItemData).GetName();
                    if (_defenseTab && !_defenseTab.ItemToScroll) _defenseTab.ItemToScroll = cell.RectTransform;
                    break;
                case ShopCategoryType.Army:
                    cell = Instantiate(_armyItem, _content.transform);
                    cell.Initialize(data, shopCategoryType);
                    ListOfItemsInCategory.Add(cell);
                    cell.name = ((INamed)cell.ItemData).GetName();
                    if (_armyTab && !_armyTab.ItemToScroll) _armyTab.ItemToScroll = cell.RectTransform;
                    break;
            }

            return cell;
        }

        private void AddListener(BaseShopItem cell, ShopCategoryType shopCategoryType, DrawCategoryData itemData)
        {
            cell.OnClick += () =>
            {
                switch (shopCategoryType)
                {
                    case ShopCategoryType.Store:
                        Store.Instance.BuyStoreItem(itemData);
                        break;
                    case ShopCategoryType.Buildings:
                    case ShopCategoryType.Army:
                        _buildingCreator.BuyStoreItem(itemData, shopCategoryType,
                            () => { _closeButton.onClick.Invoke(); });
                        break;
                    case ShopCategoryType.Weapon:
                        _weaponCreator.BuyStoreItem(itemData, shopCategoryType,
                            () => { _closeButton.onClick.Invoke(); });
                        break;
                }
            };
        }

        public void OnCloseShop()
        {
            ClearShopCategory(_decorationsContent);
            ClearShopCategory(_content);
        }
    }
}