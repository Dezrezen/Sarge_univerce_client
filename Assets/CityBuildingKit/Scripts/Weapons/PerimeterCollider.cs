using CityBuildingKit.Scripts.UIControllersAndData.StructureType;
using UnityEngine;

public class PerimeterCollider : MonoBehaviour
{
    //used to color the grass red when in collision

    public int collisionCounter; //keeps track of how many other grass patches this one is overlapping

    public bool
        inCollision;

    private TurretControllerBase turretController;

    private void Start()
    {
        turretController = transform.parent.GetComponent<TurretControllerBase>();
    }

    private void OnTriggerEnter(Collider collider) //OnCollisionEnter(Collision collision)
    {
        if (collider.gameObject.tag == "Unit")
        {
            collisionCounter++;
            turretController.AddTarget(collider.gameObject);
            turretController.fire = true;
            inCollision = true;
        }
    }

    private void OnTriggerExit(Collider collider) //OnCollisionEnter(Collision collision)
    {
        if (collider.gameObject.tag == "Unit")
        {
            collisionCounter--;

            turretController.RemoveTarget(collider.gameObject);

            if (collisionCounter == 0)
            {
                inCollision = false;

                if (transform.parent.GetComponent<StructureSelector>().structureType == StructureType.Catapult)
                    transform.parent.GetComponent<WeaponAnimationController>().StopAnimations();
            }
        }
    }
}