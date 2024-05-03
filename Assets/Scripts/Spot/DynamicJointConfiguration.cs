using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DynamicJointConfiguration : MonoBehaviour
{
    public float stiffness = 20000;
    public float damping = 1000;
    public float forceLimit = 10000;
    public bool assignToAllChildren = true;
    public int robotChainLength = 0;
    public int defDyanmicVal = 100;

    public GameObject robotRoot;

    private ArticulationBody[] articulationChain;

    void Start()
    {
        // Get non-fixed joints
        articulationChain = robotRoot.GetComponentsInChildren<ArticulationBody>();
        articulationChain = articulationChain.Where(
            joint => joint.jointType != ArticulationJointType.FixedJoint
        ).ToArray();

        // Joint length to assign
        int assignLength = articulationChain.Length;
        if (!assignToAllChildren)
            assignLength = robotChainLength;

        InvokeRepeating("DynamicConfigureJoints", 1f, 1f);


    }



    public void DynamicConfigureJoints(){
        int assignLength = articulationChain.Length;
        if (!assignToAllChildren)
            assignLength = robotChainLength;
            
        for (int i = 0; i < assignLength; i++)
        {
            ArticulationBody joint = articulationChain[i];
            ArticulationDrive drive = joint.xDrive;

            joint.jointFriction = defDyanmicVal;
            joint.angularDamping = defDyanmicVal;

            drive.stiffness = stiffness;
            drive.damping = damping;
            drive.forceLimit = forceLimit;
            joint.xDrive = drive;
        }
    }
}
