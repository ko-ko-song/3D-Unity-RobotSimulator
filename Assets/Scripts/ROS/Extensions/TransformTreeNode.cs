using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Robotics.Core;
using Unity.Robotics.UrdfImporter;
using RosMessageTypes.Geometry;

/// <summary>
///     This script provides a tf node class.
/// </summary>
class TransformTreeNode
{
    public readonly GameObject SceneObject;
    public readonly List<TransformTreeNode> Children;
    public Transform Transform => SceneObject.transform;
    public string name;
    public bool IsALeafNode => Children.Count == 0;

    private string tf_prefix;

    public TransformTreeNode(GameObject sceneObject, string tf_prefix)
    {
        SceneObject = sceneObject;
        name = SceneObject.name;
        this.tf_prefix = tf_prefix;
        Children = new List<TransformTreeNode>();
        PopulateChildNodes(this, tf_prefix);
    }

    public static TransformStampedMsg ToTransformStamped(TransformTreeNode node, string tf_prefix)
    {
        return node.Transform.ToROSTransformStamped(Clock.time, tf_prefix);
    }

    static void PopulateChildNodes(TransformTreeNode tfNode, string tf_prefix)
    {
        var parentTransform = tfNode.Transform;
        for (var childIndex = 0; childIndex < parentTransform.childCount; ++childIndex)
        {
            var childTransform = parentTransform.GetChild(childIndex);
            var childGO = childTransform.gameObject;

            // If game object has a URDFLink attached, it's a link in the transform tree
            if (childGO.TryGetComponent(out UrdfLink _))
            {
                var childNode = new TransformTreeNode(childGO, tf_prefix);
                tfNode.Children.Add(childNode);
            }
        }
    }
}
