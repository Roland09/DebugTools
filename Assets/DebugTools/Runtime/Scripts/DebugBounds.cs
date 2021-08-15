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
                if(skinnedMeshRenderer)
                {
                    localBounds = skinnedMeshRenderer.localBounds;
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

        private void DrawExtents(Bounds localBounds, Bounds worldBounds)
        {
            Handles.color = Color.white;

            float lineThickness = 2f;
            float radius = 0.2f;

            // local
            Vector3 localBoundsSize = localBounds.size;

            // world
            Vector3 position = worldBounds.center;

            {
                // axis aligned extents
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(position + Vector3.right * worldBounds.extents.x, radius);
                Handles.DrawLine(position, position + Vector3.right * worldBounds.extents.x);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(position + Vector3.up * worldBounds.extents.y, radius);
                Handles.DrawLine(position, position + Vector3.up * worldBounds.extents.y);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(position + Vector3.forward * worldBounds.extents.z, radius);
                Handles.DrawLine(position, position + Vector3.forward * worldBounds.extents.z);

                // calculate offsets
                Vector3 offsetRight = transform.right * localBoundsSize.x * 0.5f * transform.lossyScale.x;
                Vector3 positionRight = position + offsetRight;

                Vector3 offsetUp = transform.up * localBoundsSize.y * 0.5f * transform.lossyScale.y;
                Vector3 positionUp = position + offsetUp;

                Vector3 offsetForward = transform.forward * localBoundsSize.z * 0.5f * transform.lossyScale.z;
                Vector3 positionForward = position + offsetForward;

                #region draw gizmos and handles

                Gizmos.color = Color.red;
                Handles.color = Gizmos.color;

                Handles.DrawLine(position, positionRight, lineThickness);
                Gizmos.DrawSphere(positionRight, radius);

                Gizmos.color = Color.green;
                Handles.color = Gizmos.color;

                Handles.DrawLine(position, positionUp, lineThickness);
                Gizmos.DrawSphere(positionUp, radius);

                Gizmos.color = Color.blue;
                Handles.color = Gizmos.color;

                Handles.DrawLine(position, positionForward, lineThickness);
                Gizmos.DrawSphere(positionForward, radius);

                #endregion draw gizmos and handles

            }
        }

        private void DrawMeshRendererNonAABB(Bounds localBounds, Bounds worldBounds)
        {
            // local
            Vector3 localBoundsSize = localBounds.size;

            // world
            Vector3 position = worldBounds.center;

            float lineThickness = 2f;

            {
                Handles.color = Color.magenta;

                // calculate offsets
                Vector3 offsetRight = transform.right * localBoundsSize.x * 0.5f * transform.lossyScale.x;
                Vector3 positionRight = position + offsetRight;

                Vector3 offsetUp = transform.up * localBoundsSize.y * 0.5f * transform.lossyScale.y;
                Vector3 positionUp = position + offsetUp;

                Vector3 offsetForward = transform.forward * localBoundsSize.z * 0.5f * transform.lossyScale.z;
                Vector3 positionForward = position + offsetForward;

                #region draw lines
                Vector3 p1 = position + transform.right * localBoundsSize.x * 0.5f * transform.lossyScale.x + offsetForward + offsetUp;
                Vector3 p2 = position - transform.right * localBoundsSize.x * 0.5f * transform.lossyScale.x + offsetForward + offsetUp;
                Handles.DrawLine(p1, p2, lineThickness);

                Vector3 p3 = position + transform.right * localBoundsSize.x * 0.5f * transform.lossyScale.x + offsetForward - offsetUp;
                Vector3 p4 = position - transform.right * localBoundsSize.x * 0.5f * transform.lossyScale.x + offsetForward - offsetUp;
                Handles.DrawLine(p3, p4, lineThickness);

                Vector3 p5 = position + transform.right * localBoundsSize.x * 0.5f * transform.lossyScale.x - offsetForward - offsetUp;
                Vector3 p6 = position - transform.right * localBoundsSize.x * 0.5f * transform.lossyScale.x - offsetForward - offsetUp;
                Handles.DrawLine(p5, p6, lineThickness);

                Vector3 p7 = position + transform.right * localBoundsSize.x * 0.5f * transform.lossyScale.x - offsetForward + offsetUp;
                Vector3 p8 = position - transform.right * localBoundsSize.x * 0.5f * transform.lossyScale.x - offsetForward + offsetUp;
                Handles.DrawLine(p7, p8, lineThickness);
                #endregion draw lines

                #region draw cube

                // vertical sides
                Handles.DrawLine(p1, p3, lineThickness);
                Handles.DrawLine(p2, p4, lineThickness);
                Handles.DrawLine(p5, p7, lineThickness);
                Handles.DrawLine(p6, p8, lineThickness);

                // horizontal sides
                Handles.DrawLine(p1, p7, lineThickness);
                Handles.DrawLine(p2, p8, lineThickness);
                Handles.DrawLine(p3, p5, lineThickness);
                Handles.DrawLine(p4, p6, lineThickness);
                #endregion draw cube

            }
        }
#endif
    }
}