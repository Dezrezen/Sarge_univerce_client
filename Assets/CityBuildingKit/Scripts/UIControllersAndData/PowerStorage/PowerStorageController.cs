using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerStorageController : MonoBehaviour
{
    public static PowerStorageController Instance;
    public List<PowerStorage> ListOfPowerStorages { get; set; } = new();

    private void Awake()
    {
        Instance = this;
    }

    public void AddToList(PowerStorage powerStorage)
    {
        if (!ListOfPowerStorages.Contains(powerStorage)) ListOfPowerStorages.Add(powerStorage);
    }

    public void RemoveFromList(PowerStorage powerStorage)
    {
        ListOfPowerStorages.Remove(powerStorage);
    }

    public PowerStorage GetPowerStorageByUniqId(string id)
    {
        if (ListOfPowerStorages.Count > 0) return ListOfPowerStorages.FirstOrDefault(x => x.UniqGuid == id);
        return null;
    }

    public float CalculateTotalAmountOfPower()
    {
        var total = 0f;
        foreach (var powerStorage in ListOfPowerStorages) total += powerStorage.CurrentAmountOfPower;
        return total;
    }

    public PowerStorage FindAnyNotFulledStorage()
    {
        return ListOfPowerStorages.FirstOrDefault(x => x.IsStorageAvailable());
    }
}