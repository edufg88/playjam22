using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PathCreation;

namespace KayakGame
{
    public class RiverCreator : MonoBehaviour
    {
        [SerializeField] private float width = 4f;
        [SerializeField] private float lenght = 20f;
        [SerializeField] private Vector2 distanceYBetweenWaypoints = new Vector2(3, 6);
        [SerializeField] private float maxDistanceToOrigin = 5f;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float originY;
        [SerializeField]private float flowSpeed = 2;

        [SerializeField] EdgeCollider2D edgeCollider2DLeft;
        [SerializeField] EdgeCollider2D edgeCollider2DRight;

        private PathCreator pathCreator;

        private LinkedList<Vector3> waypoints;

        private Vector2[] riverLeftEdges;
        private Vector2[] riverRightEdges;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Mesh mesh;

        private float currentLenght;
        private float textureOffset;


        private void Start()
        {
            Initialize();
            CreateRiver();
        }

        private void Update()
        {
            UpdateOrigin();
            GeneratePath();
            GenerateRiver();
        }

        private void Initialize ()
        {
            TryGetComponent(out pathCreator);
            mainCamera = Camera.main;

            TryGetComponent(out meshRenderer);
            if (TryGetComponent(out meshFilter))
            {
                meshFilter.sharedMesh = mesh = new Mesh();
            }
        }

        private void CreateRiver()
        {
            waypoints = new LinkedList<Vector3>();
        }

        private void UpdateOrigin()
        {
            originY = mainCamera.transform.position.y;
        }

        private void GeneratePath()
        {
            bool update = false;

            if (waypoints.Count == 0) 
            {
                update = true;
            } 
            else if (waypoints.Count > 0)
            {
                var firstPositionY = waypoints.First.Value.y;
                var topBorder = originY + (lenght * 0.5f);
                var requiredLenght = topBorder - firstPositionY;

                update = requiredLenght > 0;
            }

            if (update)
            {
                if (currentLenght < lenght)
                {
                    AddWaypoint();
                }
                else if (currentLenght > lenght)
                {
                    RemoveWaypoint();
                }
            }
        }

        private void GenerateRiver ()
        {
            if (waypoints.Count < 2)
                return;

            var riverAspecrRatio = lenght / width;

            // Mesh renderer
            Material riverMaterial = meshRenderer.sharedMaterials[0];
            riverMaterial.mainTextureScale = new Vector3(1, lenght / width);
            var offset = Mathf.Repeat(Time.time * flowSpeed, 1);
            riverMaterial.mainTextureOffset = new Vector2(0, offset);

            // Mesh
            pathCreator.bezierPath = new BezierPath(waypoints, false, PathSpace.xy);
            var vertexPath = pathCreator.path;

            Vector3[] verts = new Vector3[vertexPath.NumPoints * 2];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];

            int numTris = 2 * (vertexPath.NumPoints - 1);
            int[] roadTriangles = new int[numTris * 3];

            int vertIndex = 0;
            int triIndex = 0;

            // Vertices for the top of the road are layed out:
            // 0  1
            // 2  3
            int[] triangleMap = { 0, 2, 1, 1, 2, 3 };

            riverLeftEdges = new Vector2[vertexPath.NumPoints];
            riverRightEdges = new Vector2[vertexPath.NumPoints];

            for (int i = 0; i < vertexPath.NumPoints; i++)
            {
                Vector3 localUp = Vector3.Cross(vertexPath.GetTangent(i), vertexPath.GetNormal(i));
                Vector3 localRight = vertexPath.GetNormal(i);

                // Find position to left and right of current path vertex
                Vector3 vertSideA = vertexPath.GetPoint(i) - localRight * Mathf.Abs(width);
                Vector3 vertSideB = vertexPath.GetPoint(i) + localRight * Mathf.Abs(width);

                riverLeftEdges[i] = vertSideA;
                riverRightEdges[i] = vertSideB;

                // Add top of road vertices
                verts[vertIndex + 0] = vertSideA;
                verts[vertIndex + 1] = vertSideB;

                // Set uv on y axis to path time (0 at start of path, up to 1 at end of path)
                uvs[vertIndex + 0] = new Vector2(0, vertexPath.times[i]);
                uvs[vertIndex + 1] = new Vector2(1, vertexPath.times[i]);
        
                // Top of road normals
                normals[vertIndex + 0] = localUp;
                normals[vertIndex + 1] = localUp;

                // Set triangle indices
                if (i < vertexPath.NumPoints - 1)
                {
                    for (int j = 0; j < triangleMap.Length; j++)
                    {
                        roadTriangles[triIndex + j] = (vertIndex + triangleMap[j]) % verts.Length;
                    }
                }

                vertIndex += 2;
                triIndex += 6;
            }

            mesh.Clear();
            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.subMeshCount = 1;
            mesh.SetTriangles(roadTriangles, 0);
            mesh.RecalculateBounds();

            edgeCollider2DLeft.points = riverLeftEdges;
            edgeCollider2DRight.points = riverRightEdges;
        }

        private void AddWaypoint ()
        {
            float firstPositionY;

            if (waypoints.Count == 0)
            {
                firstPositionY = originY - (lenght * 0.5f);
            }
            else
            {
                firstPositionY = waypoints.First.Value.y;
            }

            var shift = Random.Range(distanceYBetweenWaypoints.x, distanceYBetweenWaypoints.y);
            var positionY = firstPositionY + shift;
            var positionX = Random.Range(-maxDistanceToOrigin, maxDistanceToOrigin);

            waypoints.AddFirst(new Vector3(positionX, positionY, 0));
            currentLenght += shift;
        }

        private void RemoveWaypoint ()
        {
            var lastWaypoint = waypoints.Last;
            waypoints.RemoveLast();
            var shift = waypoints.Last.Value.y - lastWaypoint.Value.y;
            currentLenght -= shift;
        }
    }

}
