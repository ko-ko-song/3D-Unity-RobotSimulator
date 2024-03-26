using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Elevator : MonoBehaviour
{
    
    public enum DoorDirection
    {
        Front,
        Back
    }
    public enum ElevatorType{
        oneDoor,
        twoDoor
    }

    public Transform frontDoorTransform;
    public Transform backDoorTransform;
    public List<DoorDirection> doorDirectionByFloors = new List<DoorDirection>();

    public ElevatorState state = ElevatorState.IDLE;
    public float elevatorMovingTimePerFloor = 10f;
    public float distanceBetweenFloors = 2.0f;
    public float doorOpenTime = 2.0f;
    public float doorClosingTime = 2.0f;    
    public ElevatorType elevatorType = ElevatorType.oneDoor;

    public void Start(){
        if(frontDoorTransform == null)
            frontDoorTransform = transform.Find("front_door");
        if(backDoorTransform == null)
            backDoorTransform = transform.Find("back_door");

        Door frontDoor = frontDoorTransform.GetComponent<Door>();
        if(frontDoor == null){
            frontDoor = frontDoorTransform.gameObject.AddComponent<Door>();
            frontDoor.doorOpenTime = this.doorOpenTime;
            frontDoor.doorClosingTime = this.doorClosingTime;
        }

        Door backDoor = backDoorTransform.GetComponent<Door>();
        if(backDoor == null){
            backDoor = backDoorTransform.gameObject.AddComponent<Door>();
            backDoor.doorOpenTime = this.doorOpenTime;
            backDoor.doorClosingTime = this.doorClosingTime;
        }

    }

    public void MoveElevator(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, int floor){
        IEnumerator coroutine = MoveElevatorCoroutine(sensorActuatorModule, actionProtocolInstance, floor);
        StartCoroutine(coroutine);
    }

    public void OpenElevatorDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs){
        int floor = Mathf.RoundToInt(transform.position.y / distanceBetweenFloors);

        DoorDirection direction = DoorDirection.Front;
        Debug.Log(doorDirectionByFloors.Count);
        Debug.Log(floor);

        if(doorDirectionByFloors.Count >= floor)
            direction = doorDirectionByFloors[floor];
        
        Door door = null;
        if(direction == DoorDirection.Back)
            door = backDoorTransform.GetComponent<Door>();
        else 
            door = frontDoorTransform.GetComponent<Door>();
        
        door.OpenDoor(sensorActuatorModule, actionProtocolInstance, null);
    }


    public void CloseElevatorDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs){
        int floor = Mathf.RoundToInt(transform.position.y / distanceBetweenFloors);

        DoorDirection direction = DoorDirection.Front;
        if(doorDirectionByFloors.Count >= floor)
            direction = doorDirectionByFloors[floor];
        
        Door door = null;
        if(direction == DoorDirection.Back)
            door = backDoorTransform.GetComponent<Door>();
        else 
            door = frontDoorTransform.GetComponent<Door>();
        
        door.CloseDoor(sensorActuatorModule, actionProtocolInstance, null);
    }

    private IEnumerator MoveElevatorCoroutine(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, int floor)
    {
        
        float goalHeight = floor * distanceBetweenFloors;
        Vector3 targetPosition = new Vector3(transform.position.x, goalHeight, transform.position.z);

        if (Mathf.Abs(transform.position.y - goalHeight) < 0.1f)
        {
            if (actionProtocolInstance.getProtocolType().Equals("result"))
            {
                sensorActuatorModule.sendMessgae(actionProtocolInstance.getResultMessage(floor));
            }
            yield break;
        }
        
        while (Vector3.Distance(transform.position, targetPosition) > 0.001f)
        {
            float elevatorMovingSpeed = goalHeight / elevatorMovingTimePerFloor;
            
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, elevatorMovingSpeed * Time.deltaTime);
            yield return null;
        }
        
        if (actionProtocolInstance.getProtocolType().Equals("result"))
        {
            sensorActuatorModule.sendMessgae(actionProtocolInstance.getResultMessage(floor));
        }

    }
}
