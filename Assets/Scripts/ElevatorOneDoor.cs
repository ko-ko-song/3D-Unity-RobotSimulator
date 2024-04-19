using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorOneDoor : Elevator
{
    public Transform doorTransform;

    public override void Start(){
        base.Start();

        if(doorTransform == null)
            doorTransform = transform.Find("door");

        if(doorTransform != null){
            Door door = doorTransform.GetComponent<Door>();
            if(door == null){
                door = doorTransform.gameObject.AddComponent<Door>();
                door.doorOpenTime = this.doorOpenTime;
                door.doorClosingTime = this.doorClosingTime;
            }
        }
    }

    public override void OpenElevatorDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs){
        int floor = Mathf.RoundToInt(transform.position.y / distanceBetweenFloors);
        
        Door door = doorTransform.GetComponent<Door>();
        state = ElevatorState.DoorOpening;        
        door.OpenDoor(sensorActuatorModule, actionProtocolInstance, null, setElevatorStateDoorOpend);
    }


    public override void CloseElevatorDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs){
        int floor = Mathf.RoundToInt(transform.position.y / distanceBetweenFloors);

        Door door = doorTransform.GetComponent<Door>();
        state = ElevatorState.DoorClosing;        
        door.CloseDoor(sensorActuatorModule, actionProtocolInstance, null, setElevatorStateIDLE);
    }
}
