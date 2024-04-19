using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorTwoDoor : Elevator
{
    public enum DoorDirection
    {
        Front,
        Back
    }
    public Transform frontDoorTransform;
    public Transform backDoorTransform;
    public List<DoorDirection> doorDirectionByFloors = new List<DoorDirection>();

    public override void Start(){
        base.Start();

        if(frontDoorTransform == null)
            frontDoorTransform = transform.Find("front_door");
        if(backDoorTransform == null)
            backDoorTransform = transform.Find("back_door");

        if(frontDoorTransform != null){
            Door frontDoor = frontDoorTransform.GetComponent<Door>();
            if(frontDoor == null){
                frontDoor = frontDoorTransform.gameObject.AddComponent<Door>();
                frontDoor.doorOpenTime = this.doorOpenTime;
                frontDoor.doorClosingTime = this.doorClosingTime;
            }
        }

        if(backDoorTransform != null ){
            Door backDoor = backDoorTransform.GetComponent<Door>();
            if(backDoor == null){
                backDoor = backDoorTransform.gameObject.AddComponent<Door>();
                backDoor.doorOpenTime = this.doorOpenTime;
                backDoor.doorClosingTime = this.doorClosingTime;
            }
        }
    }

    public override void OpenElevatorDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs){

        int floor = Mathf.RoundToInt(transform.position.y / distanceBetweenFloors);
        DoorDirection direction = DoorDirection.Front;

        if(doorDirectionByFloors.Count > floor)
            direction = doorDirectionByFloors[floor];
        
        Door door = null;
        if(direction == DoorDirection.Back)
            door = backDoorTransform.GetComponent<Door>();
        else 
            door = frontDoorTransform.GetComponent<Door>();
        state = ElevatorState.DoorOpening;        
        door.OpenDoor(sensorActuatorModule, actionProtocolInstance, null, setElevatorStateDoorOpend);
    }


    public override void CloseElevatorDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs){
        int floor = Mathf.RoundToInt(transform.position.y / distanceBetweenFloors);

        DoorDirection direction = DoorDirection.Front;
        if(doorDirectionByFloors.Count > floor)
            direction = doorDirectionByFloors[floor];
        
        Door door = null;
        if(direction == DoorDirection.Back)
            door = backDoorTransform.GetComponent<Door>();
        else 
            door = frontDoorTransform.GetComponent<Door>();

        state = ElevatorState.DoorClosing;        
        door.CloseDoor(sensorActuatorModule, actionProtocolInstance, null, setElevatorStateIDLE);
    }


}
