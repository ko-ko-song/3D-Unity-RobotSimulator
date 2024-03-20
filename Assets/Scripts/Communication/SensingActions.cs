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

        GameObject elevator = GameObject.Find(elevatorID);

        if (elevator == null)
        {
            Debug.Log("�ش� id�� elevator door�� ã�� �� ����");
            return;
        }
        
        if (!elevator.transform.Find("left_door").GetComponent<Door>().isOpend)
        {
            return;
        }


        IEnumerator coroutine = closeElevatorDoor(sensorActuatorModule, actionProtocolInstance, elevator, functionArgs);

        StartCoroutine(coroutine);
    }

    private IEnumerator closeElevatorDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, GameObject elevator, List<string> functionArgs)
    {

        if (actionProtocolInstance.getProtocolType().Equals("result"))
        {
            sensorActuatorModule.sendMessgae(actionProtocolInstance.getResultMessage(3));
        }
        
        Transform left_door_transform = null;
        Transform right_door_transform = null;
        Transform frontDoorTransform = elevator.transform.Find("front_door");
        Transform backDoorTransform = elevator.transform.Find("back_door");

        if(frontDoorTransform){
            if(elevator.transform.position.y <1.5 ){
                left_door_transform = frontDoorTransform.Find("left_door");
                right_door_transform = frontDoorTransform.Find("right_door");    
        Debug.Log(left_door_transform);

            }
            else{
                left_door_transform = backDoorTransform.Find("left_door");
                right_door_transform = backDoorTransform.Find("right_door");    
        Debug.Log(left_door_transform);

            }
        }else{
            left_door_transform = elevator.transform.Find("left_door");
            right_door_transform = elevator.transform.Find("right_door");
        Debug.Log(left_door_transform);

        }
        Renderer leftDoorRenderer = left_door_transform.GetComponent<Renderer>();
        Bounds bounds = leftDoorRenderer.bounds;
        Vector3 leftDoorActualSize = bounds.size;

        float actualSizeX = leftDoorActualSize.x;
        float actualSizeZ = leftDoorActualSize.z;

        float closingRatio = actualSizeX > actualSizeZ ? actualSizeX / doorOpenTime : actualSizeZ / doorOpenTime;
        float elapsedTime = 0f;

        while (elapsedTime <= doorClosingTime)
        {
            if (actualSizeX > actualSizeZ)
            {
                if (left_door_transform.position.x > right_door_transform.position.x)
                {
                    left_door_transform.position = new Vector3(left_door_transform.position.x - closingRatio * Time.deltaTime, left_door_transform.position.y, left_door_transform.position.z);
                    right_door_transform.position = new Vector3(right_door_transform.position.x + closingRatio * Time.deltaTime, right_door_transform.position.y, right_door_transform.position.z);
                }
                else
                {
                    left_door_transform.position = new Vector3(left_door_transform.position.x + closingRatio * Time.deltaTime, left_door_transform.position.y, left_door_transform.position.z);
                    right_door_transform.position = new Vector3(right_door_transform.position.x - closingRatio * Time.deltaTime, right_door_transform.position.y, right_door_transform.position.z);
                }

            }
            else
            {
                if (left_door_transform.position.z > right_door_transform.position.z)
                {
                    left_door_transform.position = new Vector3(left_door_transform.position.x, left_door_transform.position.y, left_door_transform.position.z - closingRatio * Time.deltaTime);
                    right_door_transform.position = new Vector3(right_door_transform.position.x, right_door_transform.position.y, right_door_transform.position.z + closingRatio * Time.deltaTime);
                }
                else
                {
                    left_door_transform.position = new Vector3(left_door_transform.position.x, left_door_transform.position.y, left_door_transform.position.z + closingRatio * Time.deltaTime);
                    right_door_transform.position = new Vector3(right_door_transform.position.x, right_door_transform.position.y, right_door_transform.position.z - closingRatio * Time.deltaTime);
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (actionProtocolInstance.getProtocolType().Equals("result"))
        {
            sensorActuatorModule.sendMessgae(actionProtocolInstance.getResultMessage(2));
        }

        left_door_transform.GetComponent<Door>().isOpend = false;
        right_door_transform.GetComponent<Door>().isOpend = false;

    }

    private void RequestOpenElevatorDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs)
    {
        string elevatorID = functionArgs[0];

        GameObject elevator = GameObject.Find(elevatorID);

        if (elevator == null)
        {
            Debug.Log("�ش� id�� elevator door�� ã�� �� ����");
            return;
        }


        IEnumerator coroutine = openElevatorDoor(sensorActuatorModule, actionProtocolInstance, elevator, functionArgs);

        StartCoroutine(coroutine);
    }

    private IEnumerator openElevatorDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, GameObject elevator, List<string> functionArgs)
    {
        if (actionProtocolInstance.getProtocolType().Equals("result"))
        {
            sensorActuatorModule.sendMessgae(actionProtocolInstance.getResultMessage(1));
        }

        Transform left_door_transform = null;
        Transform right_door_transform = null;
        Transform frontDoorTransform = elevator.transform.Find("front_door");
        Transform backDoorTransform = elevator.transform.Find("back_door");

        if(frontDoorTransform){
            if(elevator.transform.position.y <1.5 ){
                left_door_transform = frontDoorTransform.Find("left_door");
                right_door_transform = frontDoorTransform.Find("right_door");    
            }
            else{
                left_door_transform = backDoorTransform.Find("left_door");
                right_door_transform = backDoorTransform.Find("right_door");    
            }
        }else{
            left_door_transform = elevator.transform.Find("left_door");
            right_door_transform = elevator.transform.Find("right_door");
        }
        Vector3 left_door_originalPosition = new Vector3(left_door_transform.GetComponent<Door>().originalPositionX, left_door_transform.position.y, left_door_transform.GetComponent<Door>().originalPositionZ);
        Vector3 right_door_originalPosition = new Vector3(right_door_transform.GetComponent<Door>().originalPositionX, right_door_transform.position.y, right_door_transform.GetComponent<Door>().originalPositionZ);

        Renderer leftDoorRenderer = left_door_transform.GetComponent<Renderer>();

        Bounds bounds = leftDoorRenderer.bounds;
        Vector3 leftDoorActualSize = bounds.size;

        float actualSizeX = leftDoorActualSize.x;
        float actualSizeZ = leftDoorActualSize.z;

        float openingRatio = actualSizeX > actualSizeZ ? actualSizeX / doorOpenTime : actualSizeZ / doorOpenTime;
        float elapsedTime = 0.0f;

        while (elapsedTime <= doorOpenTime)
        {
            if (actualSizeX > actualSizeZ)
            {
                if (left_door_transform.position.x > right_door_transform.position.x)
                {
                    left_door_transform.position = new Vector3(left_door_transform.position.x + openingRatio * Time.deltaTime, left_door_transform.position.y, left_door_transform.position.z);
                    right_door_transform.position = new Vector3(right_door_transform.position.x - openingRatio * Time.deltaTime, right_door_transform.position.y, right_door_transform.position.z);
                }
                else
                {
                    left_door_transform.position = new Vector3(left_door_transform.position.x - openingRatio * Time.deltaTime, left_door_transform.position.y, left_door_transform.position.z);
                    right_door_transform.position = new Vector3(right_door_transform.position.x + openingRatio * Time.deltaTime, right_door_transform.position.y, right_door_transform.position.z);
                }

            }
            else
            {
                if (left_door_transform.position.z > right_door_transform.position.z)
                {
                    left_door_transform.position = new Vector3(left_door_transform.position.x, left_door_transform.position.y, left_door_transform.position.z + openingRatio * Time.deltaTime);
                    right_door_transform.position = new Vector3(right_door_transform.position.x, right_door_transform.position.y, right_door_transform.position.z - openingRatio * Time.deltaTime);
                }
                else
                {
                    left_door_transform.position = new Vector3(left_door_transform.position.x, left_door_transform.position.y, left_door_transform.position.z - openingRatio * Time.deltaTime);
                    right_door_transform.position = new Vector3(right_door_transform.position.x, right_door_transform.position.y, right_door_transform.position.z + openingRatio * Time.deltaTime);
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (actionProtocolInstance.getProtocolType().Equals("result"))
        {
            sensorActuatorModule.sendMessgae(actionProtocolInstance.getResultMessage(0));
        }


        left_door_transform.GetComponent<Door>().isOpend = true;
        right_door_transform.GetComponent<Door>().isOpend = true;

    }


    private void RequestMoveElevator(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs)
    {
        string elevatorID = functionArgs[0];
        string goalFloor = functionArgs[1];

        GameObject elevator = GameObject.Find(elevatorID);

        IEnumerator coroutine = moveElevator(sensorActuatorModule, actionProtocolInstance, elevator, goalFloor);
        StartCoroutine(coroutine);
    }
    private IEnumerator moveElevator(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, GameObject elevator, string floor)
    {
        if (elevator == null)
        {
            Debug.Log("can't find elevator object");
            yield break;
        }
        
        float goalHeight = (float.Parse(floor)) * distanceBetweenFloors;
        Vector3 targetPosition = new Vector3(elevator.transform.position.x, goalHeight, elevator.transform.position.z);

        if (Mathf.Abs(elevator.transform.position.y - goalHeight) < 0.1f)
        {
            if (actionProtocolInstance.getProtocolType().Equals("result"))
            {
                sensorActuatorModule.sendMessgae(actionProtocolInstance.getResultMessage(Int32.Parse(floor)));
            }
            yield break;
        }
        
        while (Vector3.Distance(elevator.transform.position, targetPosition) > 0.001f)
        {
            elevator.transform.position = Vector3.MoveTowards(elevator.transform.position, targetPosition, elevatorMovingSpeed * Time.deltaTime);
            yield return null;
        }
        
        
        if (actionProtocolInstance.getProtocolType().Equals("result"))
        {
            sensorActuatorModule.sendMessgae(actionProtocolInstance.getResultMessage(Int32.Parse(floor)));
        }

    }
    //private IEnumerator moveElevator(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, GameObject elevator, string floor)
    //{
    //    if (elevator == null)
    //    {
    //        Debug.Log("can't find elevator object");
    //        yield break;
    //    }

    //    float goalHeight = (float.Parse(floor) - 1) * distanceBetweenFloors;

    //    if (Mathf.Abs(elevator.transform.position.y - goalHeight) < 0.1f)
    //    {
    //        if (actionProtocolInstance.getProtocolType().Equals("result"))
    //        {
    //            sensorActuatorModule.sendMessgae(actionProtocolInstance.getResultMessage(Int32.Parse(floor)));
    //        }
    //        yield break;
    //    }

    //    bool isUpDirection = goalHeight > elevator.transform.position.y ? true : false;

    //    while (Mathf.Abs(elevator.transform.position.y - goalHeight) > 0.002f)
    //    {
    //        if (isUpDirection)
    //        {
    //            elevator.transform.position = new Vector3(elevator.transform.position.x, elevator.transform.position.y + elevatorMovingSpeed * Time.deltaTime, elevator.transform.position.z);
    //        }
    //        else
    //        {
    //            elevator.transform.position = new Vector3(elevator.transform.position.x, elevator.transform.position.y - elevatorMovingSpeed * Time.deltaTime, elevator.transform.position.z);
    //        }
    //        yield return null;
    //    }

    //    if (actionProtocolInstance.getProtocolType().Equals("result"))
    //    {
    //        sensorActuatorModule.sendMessgae(actionProtocolInstance.getResultMessage(Int32.Parse(floor)));
    //    }

    //}

    private void RequestOpenDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs)
    {
        string doorID = functionArgs[0];

        GameObject door = GameObject.Find(doorID);

        if (door == null)
        {
            Debug.Log("�ش� id�� door�� ã�� �� ����");
            return;
        }


        IEnumerator coroutine = openDoor(sensorActuatorModule, actionProtocolInstance, door, functionArgs);

        StartCoroutine(coroutine);
    }

    private IEnumerator openDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, GameObject door, List<string> functionArgs)
    {

        Transform left_door_transform = door.transform.Find("left_door");
        Transform right_door_transform = door.transform.Find("right_door");

        Vector3 left_door_originalPosition = new Vector3(left_door_transform.GetComponent<Door>().originalPositionX, left_door_transform.position.y, left_door_transform.GetComponent<Door>().originalPositionZ);
        Vector3 right_door_originalPosition = new Vector3(right_door_transform.GetComponent<Door>().originalPositionX, right_door_transform.position.y, right_door_transform.GetComponent<Door>().originalPositionZ);

        Renderer leftDoorRenderer = left_door_transform.GetComponent<Renderer>();

        Bounds bounds = leftDoorRenderer.bounds;
        Vector3 leftDoorActualSize = bounds.size;

        float actualSizeX = leftDoorActualSize.x;
        float actualSizeZ = leftDoorActualSize.z;

        float openingRatio = actualSizeX > actualSizeZ ? actualSizeX / doorOpenTime : actualSizeZ / doorOpenTime;
        float elapsedTime = 0.0f;
        
        while (elapsedTime <= doorOpenTime)
        {
            if (actualSizeX > actualSizeZ)
            {
                if (left_door_transform.position.x > right_door_transform.position.x)
                {
                    left_door_transform.position = new Vector3(left_door_transform.position.x + openingRatio * Time.deltaTime, left_door_transform.position.y, left_door_transform.position.z);
                    right_door_transform.position = new Vector3(right_door_transform.position.x - openingRatio * Time.deltaTime, right_door_transform.position.y, right_door_transform.position.z);
                }
                else
                {
                    left_door_transform.position = new Vector3(left_door_transform.position.x - openingRatio * Time.deltaTime, left_door_transform.position.y, left_door_transform.position.z);
                    right_door_transform.position = new Vector3(right_door_transform.position.x + openingRatio * Time.deltaTime, right_door_transform.position.y, right_door_transform.position.z);
                }

            }
            else
            {
                if (left_door_transform.position.z > right_door_transform.position.z)
                {
                    left_door_transform.position = new Vector3(left_door_transform.position.x, left_door_transform.position.y, left_door_transform.position.z + openingRatio * Time.deltaTime);
                    right_door_transform.position = new Vector3(right_door_transform.position.x, right_door_transform.position.y, right_door_transform.position.z - openingRatio * Time.deltaTime);
                }
                else
                {
                    left_door_transform.position = new Vector3(left_door_transform.position.x, left_door_transform.position.y, left_door_transform.position.z - openingRatio * Time.deltaTime);
                    right_door_transform.position = new Vector3(right_door_transform.position.x, right_door_transform.position.y, right_door_transform.position.z + openingRatio * Time.deltaTime);
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (actionProtocolInstance.getProtocolType().Equals("result"))
        {
            sensorActuatorModule.sendMessgae(actionProtocolInstance.getResultMessage(0));
        }

        left_door_transform.GetComponent<Door>().isOpend = true;
        right_door_transform.GetComponent<Door>().isOpend = true;
    }

    private void RequestCloseDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, List<string> functionArgs)
    {
        string doorID = functionArgs[0];

        GameObject door = GameObject.Find(doorID);

        if (door == null)
        {
            Debug.Log("�ش� id�� elevator door�� ã�� �� ����");
            return;
        }
        
        IEnumerator coroutine = closeDoor(sensorActuatorModule, actionProtocolInstance, door, functionArgs);

        StartCoroutine(coroutine);
    }

    private IEnumerator closeDoor(SensorActuatorModule sensorActuatorModule, ActionProtocolInstance actionProtocolInstance, GameObject door, List<string> functionArgs)
    {
        

        Transform left_door_transform = door.transform.Find("left_door");
        Transform right_door_transform = door.transform.Find("right_door");

        Renderer leftDoorRenderer = left_door_transform.GetComponent<Renderer>();
        Bounds bounds = leftDoorRenderer.bounds;
        Vector3 leftDoorActualSize = bounds.size;

        float actualSizeX = leftDoorActualSize.x;
        float actualSizeZ = leftDoorActualSize.z;

        float elapsedTime = 0.0f;

        float closingRatio = actualSizeX > actualSizeZ ? actualSizeX / doorOpenTime : actualSizeZ / doorOpenTime;

        while (elapsedTime <= doorClosingTime)
        {
            if (actualSizeX > actualSizeZ)
            {
                if (left_door_transform.position.x > right_door_transform.position.x)
                {
                    left_door_transform.position = new Vector3(left_door_transform.position.x - closingRatio * Time.deltaTime, left_door_transform.position.y, left_door_transform.position.z);
                    right_door_transform.position = new Vector3(right_door_transform.position.x + closingRatio * Time.deltaTime, right_door_transform.position.y, right_door_transform.position.z);
                }
                else
                {
                    left_door_transform.position = new Vector3(left_door_transform.position.x + closingRatio * Time.deltaTime, left_door_transform.position.y, left_door_transform.position.z);
                    right_door_transform.position = new Vector3(right_door_transform.position.x - closingRatio * Time.deltaTime, right_door_transform.position.y, right_door_transform.position.z);
                }

            }
            else
            {
                if (left_door_transform.position.z > right_door_transform.position.z)
                {
                    left_door_transform.position = new Vector3(left_door_transform.position.x, left_door_transform.position.y, left_door_transform.position.z - closingRatio * Time.deltaTime);
                    right_door_transform.position = new Vector3(right_door_transform.position.x, right_door_transform.position.y, right_door_transform.position.z + closingRatio * Time.deltaTime);
                }
                else
                {
                    left_door_transform.position = new Vector3(left_door_transform.position.x, left_door_transform.position.y, left_door_transform.position.z + closingRatio * Time.deltaTime);
                    right_door_transform.position = new Vector3(right_door_transform.position.x, right_door_transform.position.y, right_door_transform.position.z - closingRatio * Time.deltaTime);
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        if (actionProtocolInstance.getProtocolType().Equals("result"))
        {
            sensorActuatorModule.sendMessgae(actionProtocolInstance.getResultMessage(2));
        }

        left_door_transform.GetComponent<Door>().isOpend = false;
        right_door_transform.GetComponent<Door>().isOpend = false;
       
    }
}






