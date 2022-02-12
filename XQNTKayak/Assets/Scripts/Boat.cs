using UnityEngine;

namespace KayakGame
{
    public class Boat : MonoBehaviour
    {
        [SerializeField] private float speed;

        private Vector2 direction;
        private float angularSpeed;

        private void Awake()
        {
            direction = Vector2.up;
        }

        private void Update()
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }
}