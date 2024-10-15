using Assets.Scripts.UIControllersAndData.Store;
using UnityEngine;

public class GrassSelector : MonoBehaviour
{
    public bool
        inCollision,
        isDestroyed; //the building on top has already been destroyed

    public int
        grassType;

    public string structureClass;

    public int StructureIndex { get; set; } = -1;

    public EntityType EntityType { get; set; }

    public Transform ObjTransform { get; set; }
}