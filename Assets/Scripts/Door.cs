using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float originalPositionX = 0.0f;
    public float originalPositionZ = 0.0f;
    public bool isOpend = false;
    // Start is called before the first frame update
    void Start()
    {
        originalPositionX = transform.position.x;
        originalPositionZ = transform.position.z;
    }
    
    
}
