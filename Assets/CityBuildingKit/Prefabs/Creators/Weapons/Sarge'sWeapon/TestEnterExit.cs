using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnterExit : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.LogError("collider.gameObject.tag");
    }

    void OnTriggerExit2D(Collider2D collider)//OnCollisionEnter(Collision collision)
    {
        Debug.LogError("collider.gameObject.tag");
    }
}
