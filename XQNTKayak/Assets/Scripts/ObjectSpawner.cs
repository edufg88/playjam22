using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayakGame
{
    public class ObjectSpawner : MonoBehaviour
    {
        public static ObjectSpawner Instance;

        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private GameObject woodlogPrefab;
        [SerializeField] private GameObject crocodilePrefab;
        [SerializeField] private RiverCreator riverCreator;
        [SerializeField] private Camera camera;

        [SerializeField] private Vector2 spawnInterval = new Vector2(1, 4);

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            } 
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InvokeRepeating("SpawnLoop", 0, Random.Range(spawnInterval.x, spawnInterval.y));
        }

        private void SpawnLoop ()
        {
            Vector3 spawnPosition = camera.ViewportToWorldPoint(new Vector3(0.5f, 1.2f, 0));
            spawnPosition.z = 0;

            var randomObject = Random.Range(0, 3);
            var spawnRadius = riverCreator.Width * 0.8f; // Using 0.8f instead of 1 to prevent objects from spawning at river edges

            switch (randomObject)
            {
                case 0:
                    SpawnCoin(spawnPosition, spawnRadius);
                    break;
                case 1:
                    SpawnLog(spawnPosition, spawnRadius);
                    break;
                case 2:
                    SpawnCroc(spawnPosition, spawnRadius);
                    break;
            }
        }


        private void SpawnObjectInRiver (GameObject spawnedObject, Vector3 position, float spawnRadius)
        {
            var closestPosition = riverCreator.GetClosestPointOnRiver(position);
            var inCircle = Random.insideUnitCircle * spawnRadius;
            closestPosition += new Vector3(inCircle.x, inCircle.y, 0);

            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));

            Instantiate<GameObject>(spawnedObject, closestPosition, randomRotation);
        }
        public void SpawnCoin(Vector3 position, float spawnRadius)
        {
            SpawnObjectInRiver(coinPrefab, position, spawnRadius);
        }

        public void SpawnLog(Vector3 position, float spawnRadius)
        {
            SpawnObjectInRiver(woodlogPrefab, position, spawnRadius);
        }

        public void SpawnCroc(Vector3 position, float spawnRadius)
        {
            SpawnObjectInRiver(crocodilePrefab, position, spawnRadius);
        }
    }

}

