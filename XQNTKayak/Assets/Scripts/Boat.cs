using UnityEngine;

namespace KayakGame
{
    public class Boat : MonoBehaviour
    {
        [SerializeField] private Animator rightPaddleAnimator;
        [SerializeField] private Animator leftPaddleAnimator;
        [SerializeField] private float speed;
        [SerializeField] private float angularSpeedOnPaddle;

        private Vector2 direction;
        private float angularSpeed;
        
        private void Awake()
        {
            direction = Vector2.up;
        }

        private void Update()
        {
            transform.position += new Vector3(direction.x, direction.y) * speed * Time.deltaTime;
        }

        private void AddAngularSpeed(float value)
        {
            angularSpeed += value;
            direction = Quaternion.Euler(0f, 0f, angularSpeed) * Vector2.up;
            transform.up = direction;
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
            AddAngularSpeed(-angularSpeedOnPaddle);
        }

        public void OnRightPaddleMovePerformed()
        {
            rightPaddleAnimator.Play("PaddleMovePerform");
            AddAngularSpeed(angularSpeedOnPaddle);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, direction * 10f);
        }
    }
}