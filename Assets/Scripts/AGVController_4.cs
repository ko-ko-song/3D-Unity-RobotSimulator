using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using Unity.Robotics.UrdfImporter.Control;

namespace RosSharp.Control
{
    public class AGVController_4 : MonoBehaviour
    {
        public GameObject front_left_wheel;
        public GameObject front_right_wheel;
        public GameObject rear_left_wheel;
        public GameObject rear_right_wheel;
        public ControlMode mode = ControlMode.ROS;
        public string topicName = "cmd_vel";

        private ArticulationBody f_l_wA;
        private ArticulationBody f_r_wA;
        private ArticulationBody r_l_wA;
        private ArticulationBody r_r_wA;

        public float maxLinearSpeed = 1f; //  m/s
        public float maxRotationalSpeed = 2f;//
        public float wheelRadius = 0.1651f; //meters
        public float trackWidth = 0.5708f; // meters Distance between tyres
        public float forceLimit = 10000000;
        public float damping = 10000;

        public float stiffness = 10;
        
        //실제 로봇과의 회전 속도를 맞추기 위해 TrackWidth에 곱하는 값 
        //허스키의 경우 2정도 default = 1;
        public float wheelSeparationMultiplier = 1;
        
        private float calibratedTrackWidth;

        public float ROSTimeout = 0.5f;
        private float lastCmdReceived = 0f;

        ROSConnection ros;
        private RotationDirection direction;
        private float rosLinear = 0f;
        private float rosAngular = 0f;

        void Start()
        {
            calibratedTrackWidth = trackWidth * wheelSeparationMultiplier;

            f_l_wA = front_left_wheel.GetComponent<ArticulationBody>();
            f_r_wA = front_right_wheel.GetComponent<ArticulationBody>();
            r_l_wA = rear_left_wheel.GetComponent<ArticulationBody>();
            r_r_wA = rear_right_wheel.GetComponent<ArticulationBody>();
            SetParameters(f_l_wA);
            SetParameters(f_r_wA);
            SetParameters(r_l_wA);
            SetParameters(r_r_wA);
            ros = ROSConnection.GetOrCreateInstance();
            ros.Subscribe<TwistMsg>(topicName, ReceiveROSCmd);

        }

        void ReceiveROSCmd(TwistMsg cmdVel)
        {
            rosLinear = (float)cmdVel.linear.x;
            rosAngular = (float)cmdVel.angular.z;
            lastCmdReceived = Time.time;
        }

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

        private void SetParameters(ArticulationBody joint)
        {
            ArticulationDrive drive = joint.xDrive;
            drive.forceLimit = forceLimit;
            drive.damping = damping;
            drive.stiffness = stiffness;
            joint.xDrive = drive;
        }

        private void SetSpeed(ArticulationBody joint, float wheelSpeed = float.NaN)
        {
            ArticulationDrive drive = joint.xDrive;
            if (float.IsNaN(wheelSpeed))
            {
                drive.targetVelocity = ((2 * maxLinearSpeed) / wheelRadius) * Mathf.Rad2Deg * (int)direction;
            }
            else
            {
                drive.targetVelocity = wheelSpeed;
            }
            joint.xDrive = drive;
        }
        
        private void KeyBoardUpdate()
        {
            float moveDirection = Input.GetAxis("Vertical");
            float inputSpeed;
            float inputRotationSpeed;
            if (moveDirection > 0)
            {
                inputSpeed = maxLinearSpeed;
            }
            else if (moveDirection < 0)
            {
                inputSpeed = maxLinearSpeed * -1;
            }
            else
            {
                inputSpeed = 0;
            }

            float turnDirction = Input.GetAxis("Horizontal");
            if (turnDirction > 0)
            {
                inputRotationSpeed = maxRotationalSpeed;
            }
            else if (turnDirction < 0)
            {
                inputRotationSpeed = maxRotationalSpeed * -1;
            }
            else
            {
                inputRotationSpeed = 0;
            }
            RobotInput(inputSpeed, inputRotationSpeed);
        }


        private void ROSUpdate()
        {
            if (Time.time - lastCmdReceived > ROSTimeout)
            {
                rosLinear = 0f;
                rosAngular = 0f;
            }
            RobotInput(rosLinear, -rosAngular);
        }

        private void RobotInput(float linearVelocity, float angularVelocity) // m/s and rad/s
        {

            if (linearVelocity > maxLinearSpeed)
            {
                linearVelocity = maxLinearSpeed;
            }
            if (angularVelocity > maxRotationalSpeed)
            {
                angularVelocity = maxRotationalSpeed;
            }

            float rightWheelSpeed = (linearVelocity - angularVelocity * calibratedTrackWidth / 2) / wheelRadius;
            float leftWheelSpeed = (linearVelocity + angularVelocity * calibratedTrackWidth / 2) / wheelRadius;

            float rightWheelSpeedDeg = rightWheelSpeed * Mathf.Rad2Deg;
            float leftWheelRotationDeg = leftWheelSpeed * Mathf.Rad2Deg;
            
            SetSpeed(f_l_wA, leftWheelRotationDeg);
            SetSpeed(f_r_wA, rightWheelSpeedDeg);
            SetSpeed(r_l_wA, leftWheelRotationDeg);
            SetSpeed(r_r_wA, rightWheelSpeedDeg);
        }
    }
}
