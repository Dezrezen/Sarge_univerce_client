using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterPathFinder : MonoBehaviour
{
    private Component helios;

    public ArrayList pathArray;

    private int selectedTarget; //finds the path to the target

    private Vector3 startPos, endPos;
    private List<Vector2> targetList = new();
    public Node startNode { get; set; }
    public Node goalNode { get; set; }

    public List<Vector2> AITargetVectorsO { get; set; } = new();

    private void Start()
    {
        helios = GameObject.Find("Helios").GetComponent<Helios>();
        pathArray = new ArrayList();
    }

    private void OnDrawGizmos()
    {
        if (pathArray == null)
            return;

        if (pathArray.Count > 0)
        {
            var index = 1;
            foreach (Node node in pathArray)
                if (index < pathArray.Count)
                {
                    var nextNode = (Node)pathArray[index];
                    Debug.DrawLine(node.position, nextNode.position, Color.green);
                    index++;
                }
        }
    }

    public void FindPath()
    {
        startPos = transform.position;
        if (targetList.Count == 0) targetList = AITargetVectorsO;
        FindNextFree();
    }

    public void ResetPath()
    {
        targetList.Clear();
        AITargetVectorsO.Clear();
    }

    private void FindNextFree() //since the corners are free, but might be inaccessible 
    {
        GetSurroundIndex(); //some of the units may have died; when surrounding the building, this unit's order might be different
        if (selectedTarget >= targetList.Count)
            FindNextFree();
        
        endPos = targetList[selectedTarget]; // of aiTargets
        //Assign StartNode and Goal Node
        startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(startPos)));
        goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(endPos)));
        pathArray = AStar.FindPath(startNode, goalNode);
    }

    private void GetSurroundIndex()
    {
        var currentIndex = ((Helios)helios).surroundIndex[0];
        selectedTarget = currentIndex;

        if (currentIndex < targetList.Count - 1)
            ((Helios)helios).surroundIndex[0]++;
        else
            ((Helios)helios).surroundIndex[0] = 0;
    }
}