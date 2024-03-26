using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SensingActions : MonoBehaviour
{
    public float doorOpenTime = 2.0f;
    public float doorClosingTime = 2.0f;
    public float elevatorMovingSpeed = 0.2f;
    public float distanceBetweenFloors = 2.0f;


    public void executeAction(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance)
    {
        string functionName = "";
        List<string> functionArgs = new List<string>();
        if (actionProtocolInstance.actionInstance != null)
        {
            functionName = actionProtocolInstance.actionInstance.actionName;
            functionArgs = actionProtocolInstance.actionInstance.actionArgs;
        }

        StringBuilder sb = new StringBuilder();
        foreach (string arg in functionArgs)
        {
            sb.Append(arg);
            sb.Append("\t");
        }
        if (sb.Length != 0)
        {
            Debug.Log("execute action : \n" + functionName);
        }

        //Debug.Log("action : " + functionName + "\t args :   " + sb.ToString());

        switch (functionName)
        {
            case "openDoor":
                RequestOpenDoor(sensorActuatorModule, actionProtocolInstance, functionArgs);
                break;
            case "closeDoor":
                RequestCloseDoor(sensorActuatorModule, actionProtocolInstance, functionArgs);
                break;
            case "openElevatorDoor":
                RequestOpenElevatorDoor(sensorActuatorModule, actionProtocolInstance, functionArgs);
                break;
            case "closeElevatorDoor":
                RequestCloseElevatorDoor(sensorActuatorModule, actionProtocolInstance, functionArgs);
                break;
            case "moveElevator":
                RequestMoveElevator(sensorActuatorModule, actionProtocolInstance, functionArgs);
                break;
            default:
                Debug.Log("funtion name undefined   may be palletizer Enter, Exit" + functionName);
                break;
        }
    }

    private void RequestCloseElevatorDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs)
    {
        string elevatorID = functionArgs[0];
        GameObject elevatorGobj = GameObject.Find(elevatorID);

        if (elevatorGobj == null)
        {
            Debug.Log("�ش� id�� elevator door�� ã�� �� ����");
            return;
        }

        Elevator elevator = elevatorGobj.GetComponent<Elevator>();
        if(elevator != null ){
         elevator.CloseElevatorDoor(sensorActuatorModule, actionProtocolInstance,  functionArgs);
        }
    }

    private void RequestOpenElevatorDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs)
    {
        string elevatorID = functionArgs[0];
        GameObject elevatorGobj = GameObject.Find(elevatorID);

        if (elevatorGobj == null)
        {
            Debug.Log("�ش� id�� elevator door�� ã�� �� ����");
            return;
        }

        Elevator elevator = elevatorGobj.GetComponent<Elevator>();
        if(elevator != null ){
         elevator.OpenElevatorDoor(sensorActuatorModule, actionProtocolInstance,  functionArgs);
        }
    }


    private void RequestMoveElevator(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs)
    {
        string elevatorID = functionArgs[0];
        string goalFloor = functionArgs[1];

        GameObject elevatorGobj = GameObject.Find(elevatorID);
        Elevator elevator = elevatorGobj.GetComponent<Elevator>();
        if(elevator != null ){
         elevator.MoveElevator(sensorActuatorModule, actionProtocolInstance,  Int32.Parse(goalFloor));
        }
    }

    private void RequestOpenDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs)
    {
        string doorID = functionArgs[0];

        GameObject doorGobj = GameObject.Find(doorID);

        if (doorGobj == null)
        {
            Debug.Log("�ش� id�� door�� ã�� �� ����");
            return;
        }
        Door door = doorGobj.GetComponent<Door>();        
        door.OpenDoor(sensorActuatorModule, actionProtocolInstance, functionArgs);
    }

    private void RequestCloseDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs)
    {
        string doorID = functionArgs[0];
        GameObject doorGobj = GameObject.Find(doorID);

        if (doorGobj == null)
        {
            Debug.Log("�ش� id�� elevator door�� ã�� �� ����");
            return;
        }

        Door door = doorGobj.GetComponent<Door>();        
        door.CloseDoor(sensorActuatorModule, actionProtocolInstance, functionArgs);
    }

}






