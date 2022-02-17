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
        [SerializeField] private GameObject[] decorationPrefabs;
        [SerializeField] private RiverCreator riverCreator;
        [SerializeField] private Camera camera;
        [SerializeField] private Vector2 spawnInterval = new Vector2(1, 4);

        private List<GameObject> objectQueue = new List<GameObject>();
        private float spawnAcc = 0f;
        private float nextSpawnIn = 0f;

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
            nextSpawnIn = Random.Range(spawnInterval.x, spawnInterval.y);
        }

        private void Update()
        {
            spawnAcc += Time.deltaTime;
            if (spawnAcc > nextSpawnIn)
            {
                SpawnLoop();
                DecorationLoop();
                spawnAcc = 0f;
                nextSpawnIn = Random.Range(spawnInterval.x, spawnInterval.y);
            }
            CleanQueue();

#if UNITY_EDITOR
            var p = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
            var d = riverCreator.GetRiverDirectionOnPoint(p);
            Debug.DrawRay(camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f)), d * 2f, Color.magenta, 1f);
#endif
        }

        private void DecorationLoop()
        {
            var spawnPosition = camera.ViewportToWorldPoint(new Vector3(0.5f, 1.2f, 0));
            spawnPosition.z = 0;
            var closestPosition = riverCreator.GetClosestPointOnRiver(spawnPosition);
            var riverDirectionInPosition = riverCreator.GetRiverDirectionOnPoint(closestPosition);
            var rotatedDirection = Quaternion.Euler(0f, 0f, Mathf.Sign(Random.Range(-1f, 1f)) * 90f) * riverDirectionInPosition;
            var spawnRadius = riverCreator.Width * Random.Range(1.5f, 4.5f);
            closestPosition += rotatedDirection * spawnRadius;
            var instance = Instantiate(decorationPrefabs[Random.Range(0, decorationPrefabs.Length)], closestPosition, Quaternion.identity);
            instance.transform.localScale = Vector3.one * Random.Range(0.75f, 1.25f);
            objectQueue.Add(instance);
        }

        private void SpawnLoop ()
        {
            var randomObject = Random.Range(0, 3);
            switch (randomObject)
            {
                case 0:
                    SpawnCoin();
                    break;
                case 1:
                    SpawnLog();
                    break;
                case 2:
                    SpawnCroc();
                    break;
            }
        }

        private Vector3 GetNextSpawnPositionInRiver()
        {
            Vector3 spawnPosition = camera.ViewportToWorldPoint(new Vector3(0.5f, 1.2f, 0));
            spawnPosition.z = 0;
            var spawnRadius = riverCreator.Width * 0.8f; // Using 0.8f instead of 1 to prevent objects from spawning at river edges
            var closestPosition = riverCreator.GetClosestPointOnRiver(spawnPosition);
            var inCircle = Random.insideUnitCircle * spawnRadius;
            closestPosition += new Vector3(inCircle.x, inCircle.y, 0);
            return closestPosition;
        }

        private void SpawnObjectInRiver (GameObject spawnedObject)
        {
            var closestPosition = GetNextSpawnPositionInRiver();
            var randomRotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));
            objectQueue.Add(Instantiate(spawnedObject, closestPosition, randomRotation));
        }

        public void SpawnCoin()
        {
            SpawnObjectInRiver(coinPrefab);
        }

        public void SpawnLog()
        {
            SpawnObjectInRiver(woodlogPrefab);
        }

        public void SpawnCroc()
        {
            var closestPosition = GetNextSpawnPositionInRiver();
            var riverDirectionInPosition = riverCreator.GetRiverDirectionOnPoint(closestPosition);
            var rotatedDirection = Quaternion.Euler(0f, 0f, Mathf.Sign(Random.Range(-1f, 1f)) * 90f) * riverDirectionInPosition;
            var rotationAngle = Vector3.Angle(Vector3.left, rotatedDirection);
            var rotation = Quaternion.Euler(0f, 0f, rotationAngle);            
            var instance = Instantiate(crocodilePrefab, closestPosition, rotation);
            if (rotationAngle > 90f || rotationAngle < -90f)
            {
                var scale = instance.transform.localScale;
                scale.y *= -1f;
                instance.transform.localScale = scale;
            }
            objectQueue.Add(instance);
        }

        private void CleanQueue()
        {
            var count = objectQueue.Count;
            for (var i = 0; i < count - 10; ++i)
            {
                Destroy(objectQueue[i]);
                objectQueue.RemoveAt(i);
            }
        }
    }

}

