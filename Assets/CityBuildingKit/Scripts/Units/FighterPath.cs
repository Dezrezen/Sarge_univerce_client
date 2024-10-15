using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FighterPath : MonoBehaviour {	//the path soldiers are using to get to their targets; updated in certain circumstances
	public bool bDebug = true;
	public float Radius = 30;//10 larger value to avoid units passing through the exact same point with wrong z
	public List<Vector3> waypoints;
	public FighterPathFinder pathFinder;

	public int RangeOfAttack { get; set; }

	private float debugRangeOfAttack;
	
	void Awake () 
	{
		waypoints = new List<Vector3>();
		waypoints.Add(transform.position);//since the list is empty
		pathFinder = GetComponent<FighterPathFinder>();

		debugRangeOfAttack = (RangeOfAttack * GridManager.instance.gridCellSize);
	}

	public void UpdatePath()
	{
		waypoints.Clear();

		for (int i = 0; i < pathFinder.pathArray.Count; i++) 
		{			
			waypoints.Add(((Node)pathFinder.pathArray[i]).position);			
		}
	}

	public float Length {
		get {
			return waypoints.Count;
		}
	}
	public Vector3 GetPoint(int index) {
		return waypoints[index];
	}

	public Vector3 GetEndPoint() {
		return waypoints[waypoints.Count-1];
	}
	
	void OnDrawGizmos() 
	{

		if (!bDebug) return;
		for (int i = 0; i < waypoints.Count; i++) 
		{
			if (i + 1 < waypoints.Count) 
			{
				Debug.DrawLine(waypoints[i], waypoints[i + 1], Color.red);
				
			} 
			//gridCellSize - 97
			Gizmos.DrawWireSphere( transform.position, debugRangeOfAttack);
		}
	}
}
