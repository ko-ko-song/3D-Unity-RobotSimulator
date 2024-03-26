using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;

/// <summary>
///     This script subscribes to twist command
///     and use robot controller to 
/// </summary>
public class TwistSubscriber : MonoBehaviour
{
    // ROS Connector
    private ROSConnection ros;
    // Variables required for ROS communication
    public string topicName = "cmd_vel";

    public AGVController_m agvController;
    private float robotLinearSpeed;
    private float robotAngularSpeed;
    
    public float ROSTimeout = 0.5f;
    private float lastCmdReceived = 0f;

    void Start()
    {
        Namespace ns = gameObject.transform.root.GetComponent<Namespace>();
        if(ns != null && ns.useNamespace)
            topicName = ns.namesapce + "/" +topicName;
        // Get ROS connection static instance
        ros = ROSConnection.GetOrCreateInstance();

        robotLinearSpeed = 0f;
        robotAngularSpeed = 0f;
        
        ros.Subscribe<TwistMsg>(topicName, UpdateVelocity);
    }

    private void UpdateVelocity(TwistMsg twist)
    {

        robotLinearSpeed = twist.linear.From<FLU>().z;
        robotAngularSpeed = twist.angular.From<FLU>().y;
        if (Time.time - lastCmdReceived > ROSTimeout)
        {
            robotLinearSpeed = 0f;
            robotAngularSpeed = 0f;
        }
        agvController.SetRobotSpeed(robotLinearSpeed, robotAngularSpeed);
        lastCmdReceived = Time.time;
    }
}

