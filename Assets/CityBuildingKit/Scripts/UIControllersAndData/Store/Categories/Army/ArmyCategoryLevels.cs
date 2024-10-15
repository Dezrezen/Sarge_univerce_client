/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2019.
 * All rights reserved.
 */

using System.Collections.Generic;
using UIControllersAndData.Models;

namespace UIControllersAndData.Store.Categories.Military
{
	[System.Serializable]
	public class ArmyCategoryLevels: INamed, IId
	{
		public string name;
		public int id;
		public List<ArmyCategory> levels;

		public string GetName()
		{
			return name;
		}

		public int GetId()
		{
			return id;
		}
	
	}
}
