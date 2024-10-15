using UnityEngine;

public class PerimeterCollider2D : MonoBehaviour
{
    [SerializeField] private TurretControllerBase turretController;

    private void OnTriggerEnter2D(Collider2D collider) //OnCollisionEnter(Collision collision)
    {
        if (collider.gameObject.tag == "Unit")
        {
            turretController.AddTarget(collider.gameObject);
            turretController.fire = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider) //OnCollisionEnter(Collision collision)
    {
        if (collider.gameObject.tag == "Unit") turretController.RemoveTarget(collider.gameObject);
        //TODO: NEED TO CHECK THIS
        // if (transform.parent.GetComponent<StructureSelector>().structureType == StructureType.Catapult)
        //     transform.parent.GetComponent<WeaponAnimationController>().StopAnimations();
    }
}