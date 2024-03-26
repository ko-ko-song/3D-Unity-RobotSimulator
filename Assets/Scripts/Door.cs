using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    
    public enum DoorState {
        opend,
        opening,
        closed,
        closing
    }
    public DoorState doorState = DoorState.closed;
    public float doorOpenTime = 2.0f;
    public float doorClosingTime = 2.0f;

    public Transform left_door_transform;
    public Transform right_door_transform;

    // Start is called before the first frame update
    void Start()
    {
        if(left_door_transform == null)
            left_door_transform = transform.Find("left_door");
            
        if(right_door_transform == null)
            right_door_transform = transform.Find("right_door");
    }
    
    public void OpenDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs){
        IEnumerator coroutine = OpenDoorCoroutine(sensorActuatorModule, actionProtocolInstance, functionArgs);
        StartCoroutine(coroutine);
    }

    public void CloseDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs)
    {
        IEnumerator coroutine = CloseDoorCoroutine(sensorActuatorModule, actionProtocolInstance, functionArgs);
        StartCoroutine(coroutine);
    }
    
    private IEnumerator OpenDoorCoroutine(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs)
    {
        Renderer leftDoorRenderer = left_door_transform.GetComponent<Renderer>();

        Bounds bounds = leftDoorRenderer.bounds;
        Vector3 leftDoorActualSize = bounds.size;

        float actualSizeX = leftDoorActualSize.x;
        float actualSizeZ = leftDoorActualSize.z;

        float elapsedTime = 0.0f;
        float openingRatio = actualSizeX / transform.localScale.x / doorOpenTime;

        while (elapsedTime <= doorOpenTime)
        {
            if(doorState != DoorState.closed)
                break;

            // if (actualSizeX > actualSizeZ)
            // {
            //     if (left_door_transform.position.x > right_door_transform.position.x)
            //     {
            //         left_door_transform.position = new Vector3(left_door_transform.position.x + openingRatio * Time.deltaTime, left_door_transform.position.y, left_door_transform.position.z);
            //         right_door_transform.position = new Vector3(right_door_transform.position.x - openingRatio * Time.deltaTime, right_door_transform.position.y, right_door_transform.position.z);
            //     }
            //     else
            //     {
                    left_door_transform.localPosition = new Vector3(left_door_transform.localPosition.x - openingRatio * Time.deltaTime, left_door_transform.localPosition.y, left_door_transform.localPosition.z);
                    right_door_transform.localPosition = new Vector3(right_door_transform.localPosition.x + openingRatio * Time.deltaTime, right_door_transform.localPosition.y, right_door_transform.localPosition.z);
            //     }

            // }
            // else
            // {
            //     if (left_door_transform.position.z > right_door_transform.position.z)
            //     {
            //         left_door_transform.position = new Vector3(left_door_transform.position.x, left_door_transform.position.y, left_door_transform.position.z + openingRatio * Time.deltaTime);
            //         right_door_transform.position = new Vector3(right_door_transform.position.x, right_door_transform.position.y, right_door_transform.position.z - openingRatio * Time.deltaTime);
            //     }
            //     else
            //     {
            //         left_door_transform.position = new Vector3(left_door_transform.position.x, left_door_transform.position.y, left_door_transform.position.z - openingRatio * Time.deltaTime);
            //         right_door_transform.position = new Vector3(right_door_transform.position.x, right_door_transform.position.y, right_door_transform.position.z + openingRatio * Time.deltaTime);
            //     }
            // }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        
        if (actionProtocolInstance.getProtocolType().Equals("result"))
        {
            sensorActuatorModule.sendMessgae(actionProtocolInstance.getResultMessage(0));
        }

        this.doorState = DoorState.opend;
        
    }

    private IEnumerator CloseDoorCoroutine(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs)
    {
        Renderer leftDoorRenderer = left_door_transform.GetComponent<Renderer>();
        Bounds bounds = leftDoorRenderer.bounds;
        Vector3 leftDoorActualSize = bounds.size;

        float actualSizeX = leftDoorActualSize.x;
        float actualSizeZ = leftDoorActualSize.z;

        float elapsedTime = 0.0f;

        float  closingRatio = actualSizeX / transform.localScale.x / doorOpenTime;

        while (elapsedTime <= doorClosingTime)
        {
            if(doorState != DoorState.opend)
                break;

            left_door_transform.localPosition = new Vector3(left_door_transform.localPosition.x + closingRatio * Time.deltaTime, left_door_transform.localPosition.y, left_door_transform.localPosition.z);
            right_door_transform.localPosition = new Vector3(right_door_transform.localPosition.x - closingRatio * Time.deltaTime, right_door_transform.localPosition.y, right_door_transform.localPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }


        if (actionProtocolInstance.getProtocolType().Equals("result"))
        {
            sensorActuatorModule.sendMessgae(actionProtocolInstance.getResultMessage(2));
        }

        this.doorState = DoorState.closed;
    }
    
}
