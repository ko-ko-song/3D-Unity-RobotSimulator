using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using System;
using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;

namespace freight
{
    public static class TransformExtensions
    {

        public static TransformMsg ToROSTransform(this Transform tfUnity)
        {
            return new TransformMsg(
                // Using vector/quaternion To<>() because Transform.To<>() doesn't use localPosition/localRotation
                tfUnity.localPosition.To<FLU>(),
                tfUnity.localRotation.To<FLU>());
        }

        public static TransformStampedMsg ToROSTransformStamped(this Transform tfUnity, double timeStamp)
        {
            return new TransformStampedMsg(
                new HeaderMsg((uint)Math.Floor(Clock.time), new TimeStamp(timeStamp), tfUnity.parent.gameObject.name),


                tfUnity.gameObject.name,
                tfUnity.ToROSTransform());
        }
    }

}
