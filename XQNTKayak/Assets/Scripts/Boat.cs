using System.Collections;
using UnityEngine;

namespace KayakGame
{
    public class Boat : MonoBehaviour
    {
        [SerializeField] private BoatUI ui;
        [SerializeField] private Animator rightPaddleAnimator;
        [SerializeField] private Animator leftPaddleAnimator;
        [SerializeField] private ParticleSystem trailParticles;
        [SerializeField] private float speed;
        [SerializeField] private float turboSpeed;
        [SerializeField] private float angularSpeedOnPaddle;
        [SerializeField] private float timeToTurbo;
        [SerializeField] private float turboDuration;
        [SerializeField] private float turboCooldownDuration;
        [SerializeField] private Color turboTrailColor;

        private float currentSpeed;
        private float leftPaddleStartTime = float.MinValue;
        private float rightPaddleStartTime = float.MaxValue;
        private float turboProgressAcc = 0f;
        private float turboCooldownProgressAcc = 0f;
        private bool cooldown = false;
        private Coroutine turboCoroutine = null;
        private Vector2 direction;
        private float angularSpeed;
        private Color trailInitialColor;

        private Vector2 forwardVector
        {
            get => transform.up;
            set => StartCoroutine(LerpForward(value, 1f));
        }

        private void Awake()
        {
            direction = Vector2.up;
            currentSpeed = speed;
            trailInitialColor = trailParticles.main.startColor.color;
        }

        private void Update()
        {
            UpdateTurbo();
            transform.position += new Vector3(direction.x, direction.y) * currentSpeed * Time.deltaTime;
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

        private void UpdateTurbo()
        {
            if (cooldown)
            {
                turboCooldownProgressAcc += Time.deltaTime;
                ui.UpdateCooldownProgress(turboCooldownProgressAcc / turboCooldownDuration);
            }
            else if (Mathf.Abs(leftPaddleStartTime - rightPaddleStartTime) < 0.1f)
            {
                turboProgressAcc += Time.deltaTime;
                ui.UpdateTurboProgress(turboProgressAcc / timeToTurbo);
            }
        }

        private void PerformTurboIfReady()
        {
            if (cooldown)
            {
                return;
            }
            if (turboProgressAcc >= timeToTurbo)
            {
                if (turboCoroutine == null)
                {
                    currentSpeed = turboSpeed;
                    turboCoroutine = StartCoroutine(StopTurboAndCooldownCoroutine());
                }
            }
            else if (turboProgressAcc > 0f)
            {
                CancelTurbo(false);
            }
        }

        private IEnumerator StopTurboAndCooldownCoroutine()
        {
            var psMain = trailParticles.main;
            psMain.startColor = turboTrailColor;
            yield return new WaitForSeconds(turboDuration);
            CancelTurbo(true);
            psMain.startColor = trailInitialColor;
            cooldown = true;
            yield return new WaitForSeconds(turboCooldownDuration);
            CancelTurboCooldown();
        }

        private void CancelTurbo(bool intoCooldown)
        {
            currentSpeed = speed;
            turboProgressAcc = 0f;
            turboCooldownProgressAcc = 0f;
            leftPaddleStartTime = float.MinValue;
            rightPaddleStartTime = float.MaxValue;
            turboCoroutine = null;
            ui.OnTurboStop(intoCooldown);
        }

        private void CancelTurboCooldown()
        {
            turboCooldownProgressAcc = 0f;
            cooldown = false;
            ui.OnTurboCooldownStop();
        }

        public void OnLeftPaddleMoveStart()
        {
            leftPaddleAnimator.Play("PaddleMoveStart");
            leftPaddleStartTime = Time.timeSinceLevelLoad;
        }

        public void OnRightPaddleMoveStart()
        {
            rightPaddleAnimator.Play("PaddleMoveStart");
            rightPaddleStartTime = Time.timeSinceLevelLoad;
        }

        public void OnLeftPaddleMovePerformed()
        {
            leftPaddleAnimator.Play("PaddleMovePerform");
            AddAngularSpeed(-angularSpeedOnPaddle);
            PerformTurboIfReady();
        }

        public void OnRightPaddleMovePerformed()
        {
            rightPaddleAnimator.Play("PaddleMovePerform");
            AddAngularSpeed(angularSpeedOnPaddle);
            PerformTurboIfReady();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, direction * 10f);
        }
    }
}