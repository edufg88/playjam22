using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayakGame
{
    public class CameraObstacle : MonoBehaviour
    {
        private const float EXTRA_MARGIN = 50f;
        public enum Border 
        {
            BottomBorder,
            TopBorder
        }

        [SerializeField] private Border border;

        private Camera camera;
        private EdgeCollider2D edgeCollider2D;

        private void Awake()
        {
            camera = Camera.main;
            TryGetComponent(out edgeCollider2D);
        }

        private void Update()
        {
            UpdatePosition();
        }

        private void UpdatePosition ()
        {
            Vector2 cornerA;
            Vector2 cornerB;

            if (border == Border.TopBorder)
            {
                cornerA = camera.ViewportToWorldPoint(new Vector3(0, 1, 0)) + new Vector3(-EXTRA_MARGIN, 0, 0);
                cornerB = camera.ViewportToWorldPoint(new Vector3(1, 1, 0)) + new Vector3(EXTRA_MARGIN, 0, 0);
            }
            else
            {
                cornerA = camera.ViewportToWorldPoint(new Vector3(0, 0, 0)) + new Vector3(-EXTRA_MARGIN, 0, 0); ;
                cornerB = camera.ViewportToWorldPoint(new Vector3(1, 0, 0)) + new Vector3(EXTRA_MARGIN, 0, 0);
            }

            edgeCollider2D.SetPoints(new List<Vector2>() { cornerA, cornerB });
        }
    }

}
