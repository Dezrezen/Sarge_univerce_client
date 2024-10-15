using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class SupplyDropController : MonoBehaviour
{
    public static SupplyDropController Instance;
    [FormerlySerializedAs("listOfSupplyStorages")] public List<SupplyDrop> listOfSupplyDrops = new();


    public List<SupplyDrop> ListOfSupplyDrops
    {
        get => listOfSupplyDrops;
        set => listOfSupplyDrops = value;
    }

    private void Awake()
    {
        Instance = this;
    }

    public void AddToList(SupplyDrop supplyDrop)
    {
        if (!ListOfSupplyDrops.Contains(supplyDrop)) listOfSupplyDrops.Add(supplyDrop);
    }

    public void RemoveFromList(SupplyDrop supplyDrop)
    {
        ListOfSupplyDrops.Remove(supplyDrop);
    }

    public SupplyDrop GetSupplyStorageByUniqId(string id)
    {
        if (ListOfSupplyDrops.Count > 0) return ListOfSupplyDrops.FirstOrDefault(x => x.UniqGuid == id);

        return null;
    }

    public float CalculateTotalAmountOfSupply()
    {
        var total = 0f;
        foreach (var powerStorage in ListOfSupplyDrops) total += powerStorage.CurrentAmountOfSupply;
        return total;
    }

    public SupplyDrop FindAnyNotFulledStorage()
    {
        return ListOfSupplyDrops.FirstOrDefault(x => x.IsSupplyStorageAvailable());
    }
}