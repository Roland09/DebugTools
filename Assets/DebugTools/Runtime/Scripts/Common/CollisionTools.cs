using System;
using UnityEngine;

namespace Rowlan.DebugTools
{
    public class CollisionTools
    {
        /// <summary>
        /// Check if two spheres intersct
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="radius"></param>
        /// <param name="other"></param>
        /// <param name="otherRadius"></param>
        /// <returns></returns>
        public static bool SphereIntersect(Vector3 sphere, float radius, Vector3 other, float otherRadius)
        {
            // we are using multiplications because it's faster than calling Math.pow
            var distance = Math.Sqrt((sphere.x - other.x) * (sphere.x - other.x) +
                                     (sphere.y - other.y) * (sphere.y - other.y) +
                                     (sphere.z - other.z) * (sphere.z - other.z));
            return distance < (radius + otherRadius);
        }
    }
}