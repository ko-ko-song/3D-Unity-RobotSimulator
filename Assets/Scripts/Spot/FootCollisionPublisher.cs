using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using RosMessageTypes.Champ;


// Send the foot collision data to ROS for the spot_controller
public class FootCollisionPublisher : MonoBehaviour
{
    // Name of each Unity Spot foot 
    public static readonly string[] FeetNames =
    {   "rh_foot_link",
        "rf_foot_link",
        "lh_foot_link",
        "lf_foot_link"
    };
    
    public string topicName = "/foot_contacts";

    public GameObject[] foots;

    [SerializeField]
    GameObject m_Spot;
    // ROS Connector
    ROSConnection m_Ros;

    FootCollisionSensor[] m_FootCollisionSensors;

    const int k_NumFeetContacts = 4;
    string m_RobotName = "spot1";

    float publishRate = 10f;

    void Start()
    {
        Namespace ns = gameObject.transform.root.GetComponent<Namespace>();
        if(ns != null && ns.useNamespace)
            topicName = ns.namesapce + "/" +topicName;
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.RegisterPublisher<ContactsStampedMsg>(topicName);

        m_FootCollisionSensors = new FootCollisionSensor[k_NumFeetContacts];

        for (int i = 0; i < k_NumFeetContacts; i++)
        {
            // m_FootCollisionSensors[i] = m_Spot.transform.Find(FeetNames[i]).AddComponent<FootCollisionSensor>();
            m_FootCollisionSensors[i] = foots[i].AddComponent<FootCollisionSensor>();

        }
    
        InvokeRepeating("PublishContactStates", 1f, 1f/publishRate);
    }

    // Send the feet contacts to ROS
    public void PublishContactStates()
    {
        for (int i = 0; i < k_NumFeetContacts; i++)
        {
            ContactsStampedMsg contactStateMsg = new ContactsStampedMsg();

            bool[] contactStates = new bool[k_NumFeetContacts];
            
            for(int j=0; j<contactStates.Length; j++){
                contactStates[j] = m_FootCollisionSensors[j].colliding;

            }
            var header = new HeaderMsg();
            contactStateMsg.header = header;
            contactStateMsg.contacts = contactStates;

            m_Ros.Publish(topicName, contactStateMsg);
        }
    }

}
