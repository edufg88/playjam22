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

        [SerializeField] EdgeCollider2D edgeCollider2DLeft;
        [SerializeField] EdgeCollider2D edgeCollider2DRight;

        private PathCreation.PathCreator pathCreator;


        private LinkedList<Vector3> waypoints;

        private Vector2[] riverLeftEdges;
        private Vector2[] riverRightEdges;

        private float currentLenght;

        private void Awake()
        {          
            TryGetComponent(out pathCreator);
            mainCamera = Camera.main;
        }

        private void Start()
        {
            CreateRiver();
        }

        private void Update()
        {
            UpdateOrigin();
            GeneratePath();
            GenerateRiver();
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

            pathCreator.bezierPath = new BezierPath(waypoints, false, PathSpace.xy);
            var vertexPath = pathCreator.path;

            riverLeftEdges = new Vector2[vertexPath.NumPoints];
            riverRightEdges = new Vector2[vertexPath.NumPoints];

            for (int i = 0; i < vertexPath.NumPoints; i++)
            {
                Vector3 localRight = vertexPath.GetNormal(i);

                Vector3 vertexLeft = vertexPath.GetPoint(i) - localRight * Mathf.Abs(width);
                Vector3 vertexRight = vertexPath.GetPoint(i) + localRight * Mathf.Abs(width);

                riverLeftEdges[i] = vertexLeft;
                riverRightEdges[i] = vertexRight;
            }

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
