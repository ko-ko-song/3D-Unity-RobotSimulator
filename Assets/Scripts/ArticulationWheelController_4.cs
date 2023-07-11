using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.UrdfImporter.Control;
using RosMessageTypes.Geometry;

using UnityEngine;

public enum ControlMode { Keyboard, ROS };

/// <summary>
///     This script converts linear velocity and 
///     angular velocity to joint velocities for
///     differential drive robot.
/// </summary>
public class ArticulationWheelController_4 : MonoBehaviour
{
    public ControlMode mode = ControlMode.ROS;

    public GameObject front_left_wheel;
    public GameObject front_right_wheel;
    public GameObject rear_left_wheel;
    public GameObject rear_right_wheel;

    public float wheelTrackLength;
    public float wheelRadius;
    private float vRight;
    private float vLeft;

    private ArticulationBody f_l_wA;
    private ArticulationBody f_r_wA;
    private ArticulationBody r_l_wA;
    private ArticulationBody r_r_wA;

    public float maxLinearSpeed = 2; //  m/s
    public float maxRotationalSpeed = 1;//
    public float trackWidth = 0.288f; // meters Distance between tyres
    public float forceLimit = 10;
    public float damping = 10;
    public float stiffness = 10;

    public float ROSTimeout = 0.5f;
    private float lastCmdReceived = 0f;

    ROSConnection ros;
    private RotationDirection direction;
    public float rosLinear = 0f;
    public float rosAngular = 0f;
   

    public float keyboardLinearSpped;
    public float keyboardRotationSpped;


    void Start() 
    {
        f_l_wA = front_left_wheel.GetComponent<ArticulationBody>();
        f_r_wA = front_right_wheel.GetComponent<ArticulationBody>();
        r_l_wA = rear_left_wheel.GetComponent<ArticulationBody>();
        r_r_wA = rear_right_wheel.GetComponent<ArticulationBody>();
        SetParameters(f_l_wA);
        SetParameters(f_r_wA);
        SetParameters(r_l_wA);
        SetParameters(r_r_wA);
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<TwistMsg>("cmd_vel", ReceiveROSCmd);
    }

    void Update() {}


    void FixedUpdate()
    {
        if (mode == ControlMode.Keyboard)
        {
            KeyBoardUpdate();
        }
        else if (mode == ControlMode.ROS)
        {
            ROSUpdate();
        }
    }
    void ReceiveROSCmd(TwistMsg cmdVel)
    {
        rosLinear = (float)cmdVel.linear.x;
        rosAngular = (float)cmdVel.angular.z;
        lastCmdReceived = Time.time;
    }


    private void SetParameters(ArticulationBody joint)
    {
        ArticulationDrive drive = joint.xDrive;
        drive.forceLimit = forceLimit;
        drive.damping = damping;
        drive.stiffness = stiffness;
        joint.xDrive = drive;
    }


    private void KeyBoardUpdate()
    {
        float moveDirection = Input.GetAxis("Vertical");
        float inputSpeed;
        float inputRotationSpeed;
        if (moveDirection > 0)
        {
            inputSpeed = keyboardLinearSpped;
        }
        else if (moveDirection < 0)
        {
            inputSpeed = keyboardLinearSpped * -1;
        }
        else
        {
            inputSpeed = 0;
        }

        float turnDirction = Input.GetAxis("Horizontal");
        if (turnDirction > 0)
        {
            inputRotationSpeed = keyboardRotationSpped;
        }
        else if (turnDirction < 0)
        {
            inputRotationSpeed = keyboardRotationSpped * -1;
        }
        else
        {
            inputRotationSpeed = 0;
        }
        //RobotInput(inputSpeed, inputRotationSpeed);
        SetRobotVelocity(inputSpeed, inputRotationSpeed);
    }


    private void ROSUpdate()
    {
        if (Time.time - lastCmdReceived > ROSTimeout)
        {
            rosLinear = 0f;
            rosAngular = 0f;
        }
        //RobotInput(rosLinear, -rosAngular);
        SetRobotVelocity(rosLinear, -rosAngular);
    }

    //private void RobotInput(float speed, float rotSpeed) // m/s and rad/s
    //{
    //    if (speed > maxLinearSpeed)
    //    {
    //        speed = maxLinearSpeed;
    //    }
    //    if (rotSpeed > maxRotationalSpeed)
    //    {
    //        rotSpeed = maxRotationalSpeed;
    //    }
    //    float wheel1Rotation = (speed / wheelRadius);
    //    float wheel2Rotation = wheel1Rotation;
    //    float wheelSpeedDiff = ((rotSpeed * trackWidth) / wheelRadius);
    //    if (rotSpeed != 0)
    //    {
    //        wheel1Rotation = (wheel1Rotation + (wheelSpeedDiff / 1)) * Mathf.Rad2Deg;
    //        wheel2Rotation = (wheel2Rotation - (wheelSpeedDiff / 1)) * Mathf.Rad2Deg;
    //    }
    //    else
    //    {
    //        wheel1Rotation *= Mathf.Rad2Deg;
    //        wheel2Rotation *= Mathf.Rad2Deg;
    //    }
    //    SetSpeed(wA1, wheel1Rotation);
    //    SetSpeed(wA2, wheel2Rotation);
    //}












    public void SetRobotVelocity(float targetLinearSpeed, float targetAngularSpeed)
    {
        // Stop the wheel if target velocity is 0
        if (targetLinearSpeed == 0 && targetAngularSpeed == 0)
        {
            StopWheel(f_l_wA);
            StopWheel(f_r_wA);
            StopWheel(r_l_wA);
            StopWheel(r_r_wA);
        }
        else
        {

            // Convert from linear x and angular z velocity to wheel speed
            vRight = -targetAngularSpeed*(wheelTrackLength/2) + targetLinearSpeed;
            vLeft = targetAngularSpeed*(wheelTrackLength/2) + targetLinearSpeed;

            SetWheelVelocity(f_l_wA, vLeft / wheelRadius * Mathf.Rad2Deg);
            SetWheelVelocity(f_r_wA, vRight / wheelRadius * Mathf.Rad2Deg);
            SetWheelVelocity(r_l_wA, vLeft / wheelRadius * Mathf.Rad2Deg);
            SetWheelVelocity(r_r_wA, vRight / wheelRadius * Mathf.Rad2Deg);
        }
    }

    private void SetWheelVelocity(ArticulationBody wheel, float jointVelocity)
    {
        ArticulationDrive drive = wheel.xDrive;
        drive.target = drive.target + jointVelocity * Time.fixedDeltaTime;
        wheel.xDrive = drive;
    }

    private void StopWheel(ArticulationBody wheel)
    {
        // Set desired angle as current angle to stop the wheel
        ArticulationDrive drive = wheel.xDrive;
        drive.target = wheel.jointPosition[0] * Mathf.Rad2Deg;
        wheel.xDrive = drive;
    }
}
