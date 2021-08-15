using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace Rowlan.DebugTools
{
    [ExecuteInEditMode]
    public class DebugBounds : MonoBehaviour
    {
#if UNITY_EDITOR
        [Tooltip("Transform position and bounds center")]
        public bool positionAndCenter;

        [Tooltip("Axis aligned bounding box of the mesh renderer")]
        public bool meshRendererAABB;

        [Tooltip("Non axis aligned bounding box of the mesh renderer")]
        public bool meshRendererNonAABB;

        [Tooltip("Axis aligned as wire spheres, an non axis aligned extents as filled spheres")]
        public bool extents;


        private Bounds zeroBounds = new Bounds(Vector3.zero, Vector3.zero);

        void OnDrawGizmos()
        {
            Bounds localBounds;
            Bounds worldBounds;

            if (!GetBounds( out localBounds, out worldBounds))
            {
                Debug.LogError("Can't get bounds from " + transform.name);
                return;
            }

            Color prevHandlesColor = Handles.color;
            Color prevGizmosColor = Gizmos.color;
            {

                if (positionAndCenter)
                    DrawPositionAndCenter( (Bounds) localBounds, (Bounds) worldBounds);

                if (meshRendererAABB)
                    DrawMeshRendererAABB((Bounds) localBounds, (Bounds) worldBounds);

                if (meshRendererNonAABB)
                    DrawMeshRendererNonAABB((Bounds) localBounds, (Bounds) worldBounds);

                if (extents)
                    DrawExtents((Bounds) localBounds, (Bounds) worldBounds);
            }
            Handles.color = prevHandlesColor;
            Gizmos.color = prevGizmosColor;
        }

        private bool GetBounds( out Bounds localBounds, out Bounds worldBounds) {

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            if ( meshFilter && meshRenderer)
            {
                localBounds = meshFilter.sharedMesh.bounds;
                worldBounds = meshRenderer.bounds;

                return true;
            }
            else
            {

                SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

                if (skinnedMeshRenderer)
                {
                    localBounds = skinnedMeshRenderer.sharedMesh.bounds;
                    worldBounds = skinnedMeshRenderer.bounds;

                    return true;
                }

            }

            localBounds = zeroBounds;
            worldBounds = zeroBounds;

            return false;
        }

        private void DrawPositionAndCenter(Bounds localBounds, Bounds worldBounds)
        {
            #region bounds center

            // center
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(worldBounds.center, 0.2f);

            #endregion bounds center

            #region transform position

            // transform position
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.2f);

            #endregion transform position

        }


        private void DrawExtents(Bounds localBounds, Bounds worldBounds)
        {
            Handles.color = Color.white;

            float radius = 0.2f;

            #region local to world

            Vector3 worldCenter = transform.TransformPoint(localBounds.center);

            Gizmos.color = Color.red;
            Vector3 worldRight = transform.TransformPoint(localBounds.center + Vector3.right * localBounds.extents.x);
            Gizmos.DrawSphere(worldRight, radius);
            Handles.DrawLine(worldCenter, worldRight);

            Gizmos.color = Color.green;
            Vector3 worldUp = transform.TransformPoint(localBounds.center + Vector3.up * localBounds.extents.y);
            Gizmos.DrawSphere(worldUp, radius);
            Handles.DrawLine(worldCenter, worldUp);

            Gizmos.color = Color.blue;
            Vector3 worldForward = transform.TransformPoint(localBounds.center + Vector3.forward * localBounds.extents.z);
            Gizmos.DrawSphere(worldForward, radius);
            Handles.DrawLine(worldCenter, worldForward);

            #endregion local to world

            #region world bounds extents

            Gizmos.color = Color.red;
            Vector3 worldExtentsRight = worldBounds.center + Vector3.right * worldBounds.extents.x;
            Gizmos.DrawWireSphere(worldExtentsRight, radius);
            Handles.DrawLine(worldCenter, worldExtentsRight);

            Gizmos.color = Color.green;
            Vector3 worldExtentsUp = worldBounds.center + Vector3.up * worldBounds.extents.y;
            Gizmos.DrawWireSphere(worldExtentsUp, radius);
            Handles.DrawLine(worldCenter, worldExtentsUp);

            Gizmos.color = Color.blue;
            Vector3 worldExtentsForward = worldBounds.center + Vector3.forward * worldBounds.extents.z;
            Gizmos.DrawWireSphere(worldExtentsForward, radius);
            Handles.DrawLine(worldCenter, worldExtentsForward);

            #endregion world bounds extents

        }


        private void DrawMeshRendererAABB(Bounds localBounds, Bounds worldBounds)
        {
            Vector3 rendererSize = worldBounds.size;
            Vector3 position = worldBounds.center;

            // filled cube
            Gizmos.color = new Color(0, 1, 0, 0.1f);
            Gizmos.DrawCube(position, rendererSize);

            // outer wireframe of the filled cube
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(position, rendererSize);

        }

        private void DrawMeshRendererNonAABB(Bounds localBounds, Bounds worldBounds)
        {

            float half = 0.5f;
            float sphereSize = 0.1f;
            float lineThickness = 2f;
            Gizmos.color = Color.magenta;
            Handles.color = Color.magenta;

            Vector3 p0 = transform.TransformPoint(localBounds.center + Vector3.right * localBounds.extents.x + Vector3.up * localBounds.extents.y + Vector3.forward * localBounds.size.z * half);
            Vector3 p1 = transform.TransformPoint(localBounds.center + Vector3.right * localBounds.extents.x + Vector3.up * localBounds.extents.y - Vector3.forward * localBounds.size.z * half);
            Vector3 p2 = transform.TransformPoint(localBounds.center - Vector3.right * localBounds.extents.x + Vector3.up * localBounds.extents.y + Vector3.forward * localBounds.size.z * half);
            Vector3 p3 = transform.TransformPoint(localBounds.center - Vector3.right * localBounds.extents.x + Vector3.up * localBounds.extents.y - Vector3.forward * localBounds.size.z * half);
            // same for negative y
            Vector3 p4 = transform.TransformPoint(localBounds.center + Vector3.right * localBounds.extents.x - Vector3.up * localBounds.extents.y + Vector3.forward * localBounds.size.z * half);
            Vector3 p5 = transform.TransformPoint(localBounds.center + Vector3.right * localBounds.extents.x - Vector3.up * localBounds.extents.y - Vector3.forward * localBounds.size.z * half);
            Vector3 p6 = transform.TransformPoint(localBounds.center - Vector3.right * localBounds.extents.x - Vector3.up * localBounds.extents.y + Vector3.forward * localBounds.size.z * half);
            Vector3 p7 = transform.TransformPoint(localBounds.center - Vector3.right * localBounds.extents.x - Vector3.up * localBounds.extents.y - Vector3.forward * localBounds.size.z * half);

            // draw spheres
            Gizmos.DrawSphere(p0, sphereSize);
            Gizmos.DrawSphere( p1, sphereSize);
            Gizmos.DrawSphere( p2, sphereSize);
            Gizmos.DrawSphere( p3, sphereSize);
            Gizmos.DrawSphere( p4, sphereSize);
            Gizmos.DrawSphere( p5, sphereSize);
            Gizmos.DrawSphere( p6, sphereSize);
            Gizmos.DrawSphere( p7, sphereSize);

            // draw line: horizontal top
            Handles.DrawLine(p0, p1, lineThickness);
            Handles.DrawLine(p2, p3, lineThickness);
            Handles.DrawLine(p0, p2, lineThickness);
            Handles.DrawLine(p1, p3, lineThickness);

            // draw line: horizontal bottom
            Handles.DrawLine(p4, p5, lineThickness);
            Handles.DrawLine(p6, p7, lineThickness);
            Handles.DrawLine(p4, p6, lineThickness);
            Handles.DrawLine(p5, p7, lineThickness);

            // draw line: vertical
            Handles.DrawLine(p0, p4, lineThickness);
            Handles.DrawLine(p1, p5, lineThickness);
            Handles.DrawLine(p2, p6, lineThickness);
            Handles.DrawLine(p3, p7, lineThickness);

        }
#endif
    }
}