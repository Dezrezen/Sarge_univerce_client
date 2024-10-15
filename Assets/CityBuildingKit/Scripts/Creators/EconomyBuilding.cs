using Assets.Scripts.UIControllersAndData.Store;
using CityBuildingKit.Scripts.UIControllersAndData.StructureType;
using UIControllersAndData.GameResources;
using UnityEngine;

public class EconomyBuilding : MonoBehaviour
{
    public int structureIndex, ProdPerHour, StoreCap;
    public StoreType StoreType;

    public GameResourceType StoreResource;
    
    public StructureType StructureType;
    
    public GameResourceType ProdType;

    public float storedGold, storedMana, storedSoldiers, storedPower, storedSupplies;

    public void ModifyGoldAmount(float f)
    {
        storedGold += f;
    }

    public void ModifyManaAmount(float f)
    {
        storedMana += f;
    }

    public void ModifyPowerAmount(float f)
    {
        storedPower += f;
    }

    public void ModifySuppliesAmount(float f)
    {
        storedSupplies += f;
    }
}