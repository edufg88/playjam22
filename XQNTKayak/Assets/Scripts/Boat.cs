using System.Collections;
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
        
        private Vector2 forwardVector
        {
            get => transform.up;
            set => StartCoroutine(LerpForward(value, 1f));
        }

        private void Awake()
        {
            direction = Vector2.up;
        }

        private void Update()
        {
            transform.position += new Vector3(direction.x, direction.y) * speed * Time.deltaTime;
        }

        private IEnumerator LerpForward(Vector2 target, float duration)
        {
            var dir = forwardVector;
            for (var t = 0f; t < 1f; t += Time.fixedDeltaTime / duration)
            {
                transform.up = Vector2.Lerp(dir, target, t);
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator LerpDirectionCoroutine(Vector2 target, float duration)
        {
            var dir = direction;
            forwardVector = target;
            for (var t = 0f; t < 1f; t += Time.fixedDeltaTime / duration)
            {
                direction = Vector2.Lerp(dir, target, t);
                yield return new WaitForEndOfFrame();
            }
        }

        private void AddAngularSpeed(float value)
        {
            angularSpeed += value;
            var target = Quaternion.Euler(0f, 0f, angularSpeed) * Vector2.up;
            StartCoroutine(LerpDirectionCoroutine(target, 3f));
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