using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayakGame
{
    public class Cloud : MonoBehaviour
    {
        [SerializeField] private float speed;
        private Vector3 direction;

        private void Start()
        {
            direction = transform.position.x > 0 ? Vector3.left : Vector3.right;
        }

        private void Update()
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}

