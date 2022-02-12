using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayakGame
{
    public class WaveDrawer : MonoBehaviour
    {
        [SerializeField] private GameObject waves;
        [SerializeField] private GameObject wave;

        private void Start()
        {
            for (int i = 0; i < 1000; i++)
            {
                var newWave = Instantiate(wave, waves.transform);
                newWave.transform.position += Vector3.up * 2 * i; 
            }
        }
    }
}
