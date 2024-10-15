using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CityBuildingKit.Scripts.UIControllersAndData;

public class TurretControllerSimple : TurretControllerBase, IInitializer{		//cannons are instantiated in the middle of each building - simplified situation
	
	

    void Update()
	{
		if (fire) 
		{
			if (targetList.Count == 0) 
			{
				fire = false;
				return;
			}
			Aim (); 
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= fireRate) {
				UpdateTarget ();
				elapsedTime = 0.0f;				             
				LaunchProjectile ();
			}
		} 
		else 
		{
			if(_structureSelector.Id != -1)
				Search ();
		}
    }

    public void Initialize()
    {
	    InitializeComponents();
    }
}

