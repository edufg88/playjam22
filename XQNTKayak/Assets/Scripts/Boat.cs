using UnityEngine;

namespace KayakGame
{
    public class Boat : MonoBehaviour
    {
        [SerializeField] private Animator rightPaddleAnimator;
        [SerializeField] private Animator leftPaddleAnimator;
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

        public void OnLeftPaddleMoveStart()
        {
            leftPaddleAnimator.Play("PaddleMoveStart");
        }

        public void OnRightPaddleMoveStart()
        {
            rightPaddleAnimator.Play("PaddleMoveStart");
        }

        public void OnLeftPaddleMovePerformed()
        {
            leftPaddleAnimator.Play("PaddleMovePerform");
        }

        public void OnRightPaddleMovePerformed()
        {
            rightPaddleAnimator.Play("PaddleMovePerform");
        }
    }
}