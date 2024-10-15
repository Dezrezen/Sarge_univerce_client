using System.Collections.Generic;
using Assets.Scripts.UIControllersAndData.Store;
using UnityEngine;

public class BarrackController : MonoBehaviour
{

    [SerializeField]
    private int _maxAmountOfTypesOfUnitsToTrain;
    
    public static BarrackController Instance;

    private List<GameUnit> _listOfUnitTypesInTraining = new ();

    public List<GameUnit> ListOfUnitTypesInTraining
    {
        get => _listOfUnitTypesInTraining;
        set => _listOfUnitTypesInTraining = value;
    }


    private void Awake()
    {
        Instance = this;
    }

    public bool IsAbleToTrainAnyTypeOfUnits()
    {
        return _listOfUnitTypesInTraining.Count < _maxAmountOfTypesOfUnitsToTrain;
    }
}
