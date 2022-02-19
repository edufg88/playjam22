using UnityEngine;

namespace KayakGame
{
    public class Croc : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float timeToFlip;
        [SerializeField] private Vector2 audioInterval = new Vector2(3, 8);
        [SerializeField] private AudioSource audioSource;

        private float timeAcc;
        private float audioTimer;


        private void Update()
        {
            timeAcc += Time.deltaTime;
            if (timeAcc > timeToFlip)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                timeAcc = 0f;
            }
            transform.position += transform.rotation * Vector3.left * Mathf.Sign(transform.localScale.x) * speed * Time.deltaTime;

            audioTimer -= Time.deltaTime;
            if (audioTimer < 0)
            {
                audioTimer = Random.Range(audioInterval.x, audioInterval.y);
                audioSource.Play();
            }
        }
    }
}

