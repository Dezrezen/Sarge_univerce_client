using UnityEngine;

public class SargeSpawnPoint : MonoBehaviour
{
    public bool IsPointAvailable { get; set; } = true;

    [SerializeField] private Transform spawnPoint;

    public Transform SpawnPoint
    {
        get => spawnPoint;
        set => spawnPoint = value;
    }


    public void OnTriggerEnter(Collider collider)
    {
        IsPointAvailable = Check(true, collider);
    }
    
    public void OnTriggerExit(Collider collider)
    {

        IsPointAvailable = Check(false, collider);
        Debug.LogError("Exit IsPointAvailable: " + IsPointAvailable);
    }

    private bool Check(bool isEnter, Collider collider)
    {
        GrassCollider grass = collider.gameObject.GetComponent<GrassCollider>();
        if (grass && grass.IsStructure)
        {
            return isEnter ? false : true;
        }

        return true;
    }
}