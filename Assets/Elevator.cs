using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        other.transform.parent = transform;
    }

    void OnTriggerExit(Collider other)
    {
        other.transform.parent = null;
    }

}
