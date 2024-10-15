using UnityEngine;
using System.Collections;
using CityBuildingKit.Scripts.UIControllersAndData.StructureType;
using UnityEngine.Events;

public class BaseSelector : MonoBehaviour {//attached to each building as an invisible 2dtoolkit button
	
	public bool 		
		inConstruction = true,//only for load/save
		isProductionBuilding = false,
		battleMap = false;
	
	public int 	
		productionListIndex,
		grassType,				//for saving/loading without saving grass any more
		structureIndex = -1,	//unique building ID for harvesting
		iRow,jCol;				//the gridmanager node coordinates, smaller to store than x,y positions

	public string
		structureClass;		//Building,Wall,Weapon,Ambient

	public StructureType
		structureType;		//Forge,Generator
	
	protected GameObject EconomyBuildings;
	protected Component
	structureCreator, relay, helios, soundFX, scaleTween;

	public AlphaTween alphaTween;

	public MessageNotification messageNotification;

	public MessageNotification MessageNotification
	{
		get => messageNotification;
		set => messageNotification = value;
	}

	private ResourceGenerator _resourceGenerator;

	public ResourceGenerator ResourceGenerator
	{
		get => _resourceGenerator;
		set => _resourceGenerator = value;
	}

	private EconomyBuilding _economyBuilding;

	public EconomyBuilding EconomyBuilding
	{
		get => _economyBuilding;
		set => _economyBuilding = value;
	}

	//public List<ExistedUnit> ExistingUnits { get; set; } = new();

	public UnityEvent DeSelectAction
	{
		get;
		set;
	} = new();

	public UnityEvent ReSelectAction
	{
		get;
		set;
	} = new();
	
	protected void InitializeComponents()
	{
		scaleTween = GetComponent<StructureTween> ();

		soundFX = GameObject.Find("SoundFX").GetComponent<SoundFX>();
		relay = GameObject.Find("Relay").GetComponent<Relay>();

		if (battleMap) 
		{				
			helios = GameObject.Find ("Helios").GetComponent<Helios> ();			
		} 
		else
			ResourceGenerator = GameObject.Find ("ResourceGenerator").GetComponent<ResourceGenerator> ();	

		EconomyBuildings = GameObject.Find ("EconomyBuildings");
	}

	protected void InitializeSpecificComponents()
	{
		if (battleMap)
			return;
		
		switch (structureClass) //gameObject.tag
		{
		case "Building":
			structureCreator = GameObject.Find("BuildingCreator").GetComponent<StructureCreator>();
			break;
		case "StoneWall":
			structureCreator = GameObject.Find("WallCreator").GetComponent<StructureCreator>();
			break;	
		case "WoodFence":
			structureCreator = GameObject.Find("WallCreator").GetComponent<StructureCreator>();
			break;	
		case "Weapon":
			structureCreator = GameObject.Find("WeaponCreator").GetComponent<StructureCreator>();
			break;	
		case "Ambient":
			structureCreator = GameObject.Find ("AmbientCreator").GetComponent<StructureCreator> ();
			break;	

		}

	}

}
