using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rowlan.DebugTools
{
    public class RotationTools
    {
        /// <summary>
        /// Rotate a point around a given pivot
        /// </summary>
        /// <param name="point"></param>
        /// <param name="pivot"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            return rotation * (point - pivot) + pivot;
        }
    }
}