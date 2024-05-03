using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Float = RosMessageTypes.Std.Float64Msg;
using RosMessageTypes.Trajectory;
using Unity.Robotics.UrdfImporter;

public class JointTrajectorySubscriber : MonoBehaviour
{

    [SerializeField]
    GameObject rootObject;
    
    // Articulation Bodies
    private Dictionary<string, ArticulationBody> jointArticaultionBodies;

    ROSConnection m_Ros;

    public string topicName = "joint_group_effort_controller/joint_trajectory";
    private UrdfJoint[] jointChain;

    void Start()
    {
        Namespace ns = gameObject.transform.root.GetComponent<Namespace>();
        if(ns != null && ns.useNamespace)
            topicName = ns.namesapce + "/" +topicName;
        // Get ROS connection static instance

        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();

        // Create array for articulation bodies of each joint
        jointArticaultionBodies = new Dictionary<string, ArticulationBody>();

        ArticulationBody[] articulationBodies = rootObject.GetComponentsInChildren<ArticulationBody>();
    
        if(articulationBodies == null){
            Debug.LogWarning("can't find articulationBodies");
        }


        foreach(ArticulationBody ab in articulationBodies){
            if(ab.gameObject.GetComponent<UrdfJoint>()){
                string jointName = ab.gameObject.GetComponent<UrdfJoint>().jointName;
                jointArticaultionBodies.Add(jointName, ab);
            }
        }

        m_Ros.Subscribe<JointTrajectoryMsg>(topicName, UpdateTrajectory);
    }

    public void UpdateTrajectory(JointTrajectoryMsg jointTrajectoryMsg){

        for(int i=0; i<jointTrajectoryMsg.joint_names.Length; i++){
            string jointName = jointTrajectoryMsg.joint_names[i];
            double position = jointTrajectoryMsg.points[0].positions[i];
            ArticulationBody joint = jointArticaultionBodies[jointName];

            var angle = (float)position * Mathf.Rad2Deg;
            var jointXDrive = joint.xDrive;
            jointXDrive.target = angle;
            jointArticaultionBodies[jointName].xDrive = jointXDrive;

        }


    }
}
