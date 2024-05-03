using UnityEngine;
using System.Linq;

// Set the joint values to change how it moves
public class InitializeJointAttributes : MonoBehaviour
{
    // Play with setting these variables
    // to find the best spot behavior
    public float stiffness = 20000;
    public float damping = 1000;
    public float forceLimit = 10000;
    public int dynamicVal = 100;
    public bool assignToAllChildren = true;
    public int robotChainLength = 0;

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

        // Setting stiffness, damping and force limit
        int defDyanmicVal = 100;
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
