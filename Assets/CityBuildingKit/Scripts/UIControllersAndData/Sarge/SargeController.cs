using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData.Models;
using JetBrains.Annotations;
using UIControllersAndData.Store;
using UIControllersAndData.Store.Categories.Unit;
using UnityEngine;

public class SargeController : MonoBehaviour
{
    public static SargeController Instance;

    private UnitCategoryLevels sargeLevelsData;

    private bool isSpecialAbilityAlreadyUsed = false;

    private float durationOfSargeSpecialAbility;
    private int currentHp;
    
    
    void Awake()
    {
        Instance = this;
        
        
        var sargeCategory = ShopData.Instance.UnitCategoryData;
        UnitCategory sarge = sargeCategory.category.Find((building) => building.levels[0].GetUnit() == GameUnit.Sarge).levels[0];
        if (sarge != null)
        {
            durationOfSargeSpecialAbility = ((IDurationOfSargeSpecialAbility)sarge).GetDurationOfSargeSpecialAbility();
            
        }
    }

    [UsedImplicitly]
    public void ActiveSpecialAbility()
    {
        isSpecialAbilityAlreadyUsed = true;
        //TODO: increase the sarge
        // speedup regeneration of the health - currentHp += 30%;
        
        //use 3 grenades and attack a nearest structure
        //After dealing special attack - increased dmg - X%
        //A group of X Riflemen summoned to help him out - - as soon as the players presses the special ability button.

    }
}
