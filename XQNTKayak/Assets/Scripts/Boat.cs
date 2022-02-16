using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace KayakGame
{
    public class Boat : MonoBehaviour
    {
        [SerializeField] private BoatUI ui;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer wrongDirSpriteRenderer;
        [SerializeField] private Animator rightPaddleAnimator;
        [SerializeField] private Animator leftPaddleAnimator;
        [SerializeField] private GameObject playerRendererParent;
        [SerializeField] private ParticleSystem trailParticles;
        [SerializeField] private ParticleSystem destructionParticles;
        [SerializeField] private ParticleSystem coinCollectParticlesPrefab;
        [SerializeField] private ParticleSystem leftPaddleParticles;
        [SerializeField] private ParticleSystem rightPaddleParticles;
        [SerializeField] private float speed;
        [SerializeField] private float turboSpeed;
        [SerializeField] private float angularSpeedOnPaddle;
        [SerializeField] private float timeToTurbo;
        [SerializeField] private float turboDuration;
        [SerializeField] private float turboCooldownDuration;
        [SerializeField] private float timeToReposition;
        [SerializeField] private Color turboTrailColor;
        [SerializeField] private UnityEvent onCollisionWithObstacle;
        [SerializeField] private UnityEvent onCollisionWithCoin;

        private float currentSpeed;
        private float leftPaddleStartTime = float.MinValue;
        private float rightPaddleStartTime = float.MaxValue;
        private float turboProgressAcc = 0f;
        private float turboCooldownProgressAcc = 0f;
        private float negativeYAcc = 0f;
        private bool cooldown = false;
        private Coroutine turboCoroutine = null;
        private Vector2 direction;
        private float angularSpeed;
        private Color trailInitialColor;
        private float[] paddleTorqueValues = { -500f, -400f, -300f, 300f, 400f, 500f };
        private float[] paddleForceValues = { -400f, -300f, -200, 200, 300f, 400f };
        private bool dead = false;

        private float GetRandomPaddleTorque() => paddleTorqueValues[Random.Range(0, paddleTorqueValues.Length)];
        private float GetRandomPaddleForce() => paddleForceValues[Random.Range(0, paddleForceValues.Length)];

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
            if (dead)
            {
                return;
            }
            UpdateTurbo();
            CheckYDirection();
            transform.position += currentSpeed * Time.deltaTime * new Vector3(direction.x, direction.y);
        }

        private void CheckYDirection()
        {
            if (direction.y >= 0f)
            {
                negativeYAcc = 0f;
                return;
            }
            negativeYAcc += Time.deltaTime;
            if (negativeYAcc > timeToReposition)
            {
                StartCoroutine(RepositionBoatCoroutine());
            }
        }

        private IEnumerator RepositionBoatCoroutine()
        {
            dead = true;
            wrongDirSpriteRenderer.gameObject.SetActive(true);
            yield return LerpDirectionCoroutine(Vector2.up, 3f);
            angularSpeed = 0f;
            wrongDirSpriteRenderer.gameObject.SetActive(false);
            dead = false;
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
            if (dead)
            {
                return;
            }
            leftPaddleAnimator.Play("PaddleMoveStart");
            leftPaddleStartTime = Time.timeSinceLevelLoad;
        }

        public void OnRightPaddleMoveStart()
        {
            if (dead)
            {
                return;
            }            
            rightPaddleAnimator.Play("PaddleMoveStart");
            rightPaddleStartTime = Time.timeSinceLevelLoad;
        }

        public void OnLeftPaddleMovePerformed()
        {
            if (dead)
            {
                return;
            }
            SoundManager.Instance.PlayBoatRow();
            leftPaddleAnimator.Play("PaddleMovePerform");
            leftPaddleParticles.Play();
            AddAngularSpeed(-angularSpeedOnPaddle);
            PerformTurboIfReady();
        }

        public void OnRightPaddleMovePerformed()
        {
            if (dead)
            {
                return;
            }
            SoundManager.Instance.PlayBoatRow();
            rightPaddleAnimator.Play("PaddleMovePerform");
            rightPaddleParticles.Play();
            AddAngularSpeed(angularSpeedOnPaddle);
            PerformTurboIfReady();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Obstacle"))
            {
                OnCollisionWithObstacle(collision);
            }
            else if (collision.CompareTag("Coin"))
            {
                OnCollisionWithCoin(collision); 
            }
        }

        private void OnCollisionWithObstacle(Collider2D collision)
        {
            SoundManager.Instance.PlayBoatCrash();
            dead = true;
            ui.gameObject.SetActive(false);
            spriteRenderer.enabled = false;
            playerRendererParent.SetActive(false);
            leftPaddleAnimator.enabled = false;
            rightPaddleAnimator.enabled = false;
            var leftPaddleRB = leftPaddleAnimator.GetComponent<Rigidbody2D>();
            var rightPaddleRB = rightPaddleAnimator.GetComponent<Rigidbody2D>();
            leftPaddleRB.WakeUp();
            leftPaddleRB.AddRelativeForce(Vector2.left * GetRandomPaddleForce());
            leftPaddleRB.AddTorque(GetRandomPaddleTorque());
            rightPaddleRB.WakeUp();
            rightPaddleRB.AddRelativeForce(Vector2.right * GetRandomPaddleForce());
            rightPaddleRB.AddTorque(GetRandomPaddleTorque());
            trailParticles.Stop();
            destructionParticles.Play();
            onCollisionWithObstacle?.Invoke();            
        }

        private void OnCollisionWithCoin(Collider2D collision)
        {
            SoundManager.Instance.PlayCollectCoin();
            var particles = Instantiate(coinCollectParticlesPrefab, collision.transform.position, Quaternion.identity);
            Destroy(particles.gameObject, 5f);
            collision.gameObject.SetActive(false);
            onCollisionWithCoin?.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, direction * 10f);
        }
    }
}