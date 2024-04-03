using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Elevator : MonoBehaviour
{
    public float elevatorMovingTimePerFloor = 10f;
    public float distanceBetweenFloors = 2.0f;
    public float doorOpenTime = 2.0f;
    public float doorClosingTime = 2.0f;    
    public ElevatorState state = ElevatorState.IDLE;

    public GameObject[] gobjsInElevator;

    public virtual void Start(){
    }

    public abstract void OpenElevatorDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs);

    public abstract void CloseElevatorDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs);

    public void MoveElevator(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, int floor){
        IEnumerator coroutine = MoveElevatorCoroutine(sensorActuatorModule, actionProtocolInstance, floor);
        StartCoroutine(coroutine);
    }

    private IEnumerator MoveElevatorCoroutine(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, int floor)
    {
        if(state != ElevatorState.IDLE)
            yield break;

        float goalHeight = floor * distanceBetweenFloors;
        Vector3 targetPosition = new Vector3(transform.position.x, goalHeight, transform.position.z);

        float diffHeight = goalHeight - transform.position.y;

        if (Mathf.Abs(diffHeight) < 0.1f)
        {
            if (actionProtocolInstance.getProtocolType().Equals("result"))
            {
                sensorActuatorModule.sendMessgae(actionProtocolInstance.getResultMessage(floor));
            }
            yield break;
        }
        
        // foreach (var obj in hashsetLiftingObjects)
		// {
            
			// var articulationBodies = test.GetComponentsInChildren<ArticulationBody>();
			// foreach (var articulationBody in articulationBodies)
			// {
			// 	if (articulationBody.isRoot)
			// 	{
			// 		var a = articulationBody.transform;
            //         a.SetParent(transform);
			// 		break;
			// 	}
			// }
		// }

        if(diffHeight > 0)
            state = ElevatorState.GoingUp;
        else
            state = ElevatorState.GoingDown;

        while (Vector3.Distance(transform.position, targetPosition) > 0.001f)
        {
            float elevatorMovingSpeed = distanceBetweenFloors / elevatorMovingTimePerFloor;
            
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, elevatorMovingSpeed * Time.deltaTime);
            yield return null;
        }
        
        if (actionProtocolInstance.getProtocolType().Equals("result"))
        {
            sensorActuatorModule.sendMessgae(actionProtocolInstance.getResultMessage(floor));
        }
        state = ElevatorState.IDLE;

    }

    
    public void setElevatorStateDoorOpend(){
        state = ElevatorState.DoorOpend;
    }

    public void setElevatorStateIDLE(){
        state = ElevatorState.IDLE;
    }
}
