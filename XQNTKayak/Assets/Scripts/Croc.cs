using UnityEngine;

namespace KayakGame
{
    public class Croc : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float timeToFlip;

        private float timeAcc;
        
        private void Update()
        {
            timeAcc += Time.deltaTime;
            if (timeAcc > timeToFlip)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                timeAcc = 0f;
            }
            transform.position += transform.rotation * Vector3.left * Mathf.Sign(transform.localScale.x) * speed * Time.deltaTime;
        }
    }
}

