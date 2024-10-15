using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Menus;
using Assets.Scripts.UIControllersAndData.Store;
using UIControllersAndData.Store;
using UnityEngine;
using Object = UnityEngine.Object;

public class Barrack : MonoBehaviour
{
 //Test list of available units
 private List<GameUnit> availableUnitTypes = Enum.GetValues(typeof(GameUnit)).Cast<GameUnit>().ToList();//Enumerable.Range(0, Enum.GetNames(typeof(UnitType)).Length).ToList();


 private void Awake()
 {
     var unitCategoryLevels = ShopData.Instance.UnitCategoryData.category;
     var unitCategoryData = unitCategoryLevels.SelectMany(level => level.levels).Where(c => c.level == 1).ToList();
     // var unitCategoryData = unitCategoryLevels.SelectMany(level => level.levels);
     Debug.LogError("unitCategoryData: " + unitCategoryData);

     var filtered = unitCategoryLevels.SelectMany(level => level.levels).Where(c => c.requiredBarracksLevel == 2);
     Debug.LogError("filtered: " + filtered);
     
   //That's a test variant of adding types for training.
   //When mocksups will be ready we will finish it.
   if (BarrackController.Instance.IsAbleToTrainAnyTypeOfUnits())
   {
       BarrackController.Instance.ListOfUnitTypesInTraining.Add(GameUnit.Sarge);
       BarrackController.Instance.ListOfUnitTypesInTraining.Add(GameUnit.Alien);
       BarrackController.Instance.ListOfUnitTypesInTraining.Add(GameUnit.Hydra);
   }

   if (BarrackController.Instance.IsAbleToTrainAnyTypeOfUnits())
   {
       Debug.LogError("availableUnitTypes: " + BarrackController.Instance.IsAbleToTrainAnyTypeOfUnits());    
   }
   else
   {
       // MessageController.Instance.DisplayMessage("You can't train any other types of units");
       foreach (var unitType in BarrackController.Instance.ListOfUnitTypesInTraining)
       {
           Debug.LogError("UNIT that can be trained: " +  unitType);
       }
   }
   

 }
}
