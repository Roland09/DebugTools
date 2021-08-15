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

        void OnDrawGizmos()
        {
            Color prevHandlesColor = Handles.color;
            Color prevGizmosColor = Gizmos.color;
            {

                if (positionAndCenter)
                    DrawPositionAndCenter();

                if (meshRendererAABB)
                    DrawMeshRendererAABB();

                if (meshRendererNonAABB)
                    DrawMeshRendererNonAABB();

                if (extents)
                    DrawExtents();
            }
            Handles.color = prevHandlesColor;
            Gizmos.color = prevGizmosColor;
        }

        private void DrawPositionAndCenter()
        {
            #region bounds center
            // world
            MeshRenderer meshRenderer = transform.GetComponent<MeshRenderer>();

            // bounds center
            Bounds bounds = meshRenderer.bounds;
            Vector3 position = bounds.center;

            // center
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(bounds.center, 0.2f);

            #endregion bounds center

            #region transform position

            // transform position
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.2f);

            #endregion transform position

        }

        private void DrawMeshRendererAABB()
        {
            // world bounds
            MeshRenderer meshRenderer = transform.GetComponent<MeshRenderer>();
            Vector3 meshRendererSize = meshRenderer.bounds.size;

            Vector3 position = meshRenderer.bounds.center;

            // filled cube
            Gizmos.color = new Color(0, 1, 0, 0.1f);
            Gizmos.DrawCube(position, meshRendererSize);

            // outer wireframe of the filled cube
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(position, meshRendererSize);

        }


        private void DrawExtents()
        {
            Handles.color = Color.white;

            float lineThickness = 2f;
            float radius = 0.2f;

            Matrix4x4 prevMatrix = Gizmos.matrix;
            {
                // local
                MeshFilter meshFilter = transform.GetComponent<MeshFilter>();
                Vector3 meshFilterSize = meshFilter.sharedMesh.bounds.size;

                // world
                MeshRenderer meshRenderer = transform.GetComponent<MeshRenderer>();

                // bounds
                Bounds bounds = meshRenderer.bounds;
                Vector3 position = bounds.center;

                // axis aligned extents
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(position + Vector3.right * bounds.extents.x, radius);
                Handles.DrawLine(position, position + Vector3.right * bounds.extents.x);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(position + Vector3.up * bounds.extents.y, radius);
                Handles.DrawLine(position, position + Vector3.up * bounds.extents.y);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(position + Vector3.forward * bounds.extents.z, radius);
                Handles.DrawLine(position, position + Vector3.forward * bounds.extents.z);

                // calculate offsets
                Vector3 offsetRight = transform.right * meshFilterSize.x * 0.5f * transform.lossyScale.x;
                Vector3 positionRight = position + offsetRight;

                Vector3 offsetUp = transform.up * meshFilterSize.y * 0.5f * transform.lossyScale.y;
                Vector3 positionUp = position + offsetUp;

                Vector3 offsetForward = transform.forward * meshFilterSize.z * 0.5f * transform.lossyScale.z;
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
            Gizmos.matrix = prevMatrix;

        }

        private void DrawMeshRendererNonAABB()
        {
            float lineThickness = 2f;

            Matrix4x4 prevMatrix = Gizmos.matrix;
            {
                Handles.color = Color.magenta;

                // local
                MeshFilter meshFilter = transform.GetComponent<MeshFilter>();
                Vector3 meshFilterSize = meshFilter.sharedMesh.bounds.size;

                // world
                MeshRenderer meshRenderer = transform.GetComponent<MeshRenderer>();

                // bounds
                Bounds bounds = meshRenderer.bounds;
                Vector3 position = bounds.center;

                // calculate offsets
                Vector3 offsetRight = transform.right * meshFilterSize.x * 0.5f * transform.lossyScale.x;
                Vector3 positionRight = position + offsetRight;

                Vector3 offsetUp = transform.up * meshFilterSize.y * 0.5f * transform.lossyScale.y;
                Vector3 positionUp = position + offsetUp;

                Vector3 offsetForward = transform.forward * meshFilterSize.z * 0.5f * transform.lossyScale.z;
                Vector3 positionForward = position + offsetForward;

                #region draw lines
                Vector3 p1 = position + transform.right * meshFilterSize.x * 0.5f * transform.lossyScale.x + offsetForward + offsetUp;
                Vector3 p2 = position - transform.right * meshFilterSize.x * 0.5f * transform.lossyScale.x + offsetForward + offsetUp;
                Handles.DrawLine(p1, p2, lineThickness);

                Vector3 p3 = position + transform.right * meshFilterSize.x * 0.5f * transform.lossyScale.x + offsetForward - offsetUp;
                Vector3 p4 = position - transform.right * meshFilterSize.x * 0.5f * transform.lossyScale.x + offsetForward - offsetUp;
                Handles.DrawLine(p3, p4, lineThickness);

                Vector3 p5 = position + transform.right * meshFilterSize.x * 0.5f * transform.lossyScale.x - offsetForward - offsetUp;
                Vector3 p6 = position - transform.right * meshFilterSize.x * 0.5f * transform.lossyScale.x - offsetForward - offsetUp;
                Handles.DrawLine(p5, p6, lineThickness);

                Vector3 p7 = position + transform.right * meshFilterSize.x * 0.5f * transform.lossyScale.x - offsetForward + offsetUp;
                Vector3 p8 = position - transform.right * meshFilterSize.x * 0.5f * transform.lossyScale.x - offsetForward + offsetUp;
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
            Gizmos.matrix = prevMatrix;
        }
#endif
    }
}