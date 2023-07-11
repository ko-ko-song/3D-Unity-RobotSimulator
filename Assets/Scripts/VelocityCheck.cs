using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityCheck : MonoBehaviour
{

    ArticulationBody ab;
    public Vector3 velocity;
    public Vector3 angularVelocity;

    public float speed;
    public float angularSpeed;

    public float maxSpeed;
    public float maxAngularSpeed;

    Quaternion previousRotation;
    float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        ab = gameObject.GetComponent<ArticulationBody>();
        maxSpeed = ab.maxLinearVelocity;
        maxAngularSpeed = ab.maxAngularVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        velocity = new Vector3(Mathf.Round(ab.velocity.x*10)*0.1f, Mathf.Round(ab.velocity.y * 10) * 0.1f, Mathf.Round(ab.velocity.z * 10) * 0.1f);
        angularVelocity = new Vector3(Mathf.Round(ab.angularVelocity.x * 10) * 0.1f, Mathf.Round(ab.angularVelocity.y * 10) * 0.1f, Mathf.Round(ab.angularVelocity.z * 10) * 0.1f);

        speed = Mathf.Round(velocity.magnitude*10)*0.1f;
        angularSpeed = Mathf.Round(angularVelocity.magnitude*10)*0.1f;

    }

    

}
