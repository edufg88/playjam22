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
        [SerializeField] private float minTimeToTurbo;
        [SerializeField] private float turboDuration;
        [SerializeField] private float turboCooldownDuration;
        [SerializeField] private float timeToReposition;
        [SerializeField] private Color turboTrailColor;
        [SerializeField] private UnityEvent onCollisionWithObstacle;
        [SerializeField] private UnityEvent onCollisionWithCoin;
        [SerializeField] private RiverCreator riverCreator;

        public delegate void DistanceAlongRiverTraveledDelegate(float distance);
        public DistanceAlongRiverTraveledDelegate DistanceAlongRiverTraveledEvent;

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
        private float distanceTraveled = 0;

        private float GetRandomPaddleTorque() => paddleTorqueValues[Random.Range(0, paddleTorqueValues.Length)];
        private float GetRandomPaddleForce() => paddleForceValues[Random.Range(0, paddleForceValues.Length)];

        private Vector2 forwardVector
        {
            get => transform.up;
            set => transform.up = value;
        }

        private void Awake()
        {
            direction = Vector2.up;
            currentSpeed = speed;
            trailInitialColor = trailParticles.main.startColor.color;
        }

        private void Start()
        {
            TeleportToStartPosition();
        }

        private void Update()
        {
            if (dead)
            {
                return;
            }
            UpdateTurbo();
            CheckYDirection();
            UpdateMovement();
        }

        private void TeleportToStartPosition ()
        {
            transform.position = riverCreator.GetClosestPointOnRiver(transform.position);
        }

        private void UpdateMovement()
        {
            var previousPathPosition = riverCreator.GetClosestPointOnRiver(transform.position);
            transform.position += currentSpeed * Time.deltaTime * new Vector3(direction.x, direction.y);
            var currentPathPosition = riverCreator.GetClosestPointOnRiver(transform.position);
            var distanceTraveled = Vector3.Distance(currentPathPosition, previousPathPosition);
            DistanceAlongRiverTraveledEvent?.Invoke(distanceTraveled);
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
            yield return LerpDirectionCoroutine(Quaternion.identity, 3f);
            angularSpeed = 0f;
            wrongDirSpriteRenderer.gameObject.SetActive(false);
            dead = false;
        }

        private IEnumerator LerpForward(Quaternion targetRotation, float duration)
        {
            var rotation = Quaternion.FromToRotation(Vector3.up, forwardVector);
            for (var t = 0f; t < 1f; t += Time.fixedDeltaTime / duration)
            {
                forwardVector = Quaternion.Slerp(rotation, targetRotation, t) * Vector3.up;
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator LerpDirectionCoroutine(Quaternion targetRotation, float duration)
        {
            var rotation = Quaternion.FromToRotation(Vector3.up, direction);
            StartCoroutine(LerpForward(targetRotation, 1f));
            for (var t = 0f; t < 1f; t += Time.fixedDeltaTime / duration)
            {
                direction = Quaternion.Lerp(rotation, targetRotation, t) * Vector3.up;
                yield return new WaitForEndOfFrame();
            }
        }

        private void AddAngularSpeed(float value)
        {
            angularSpeed += value;
            if (angularSpeed > 360f)
            {
                angularSpeed -= 360f;
            }
            if (angularSpeed < -360f)
            {
                angularSpeed += 360f;
            }
            var target = Quaternion.Euler(0f, 0f, angularSpeed) * Vector2.up;
            StartCoroutine(LerpDirectionCoroutine(Quaternion.Euler(0f, 0f, angularSpeed), 2f));
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
                ui.UpdateTurboProgress(turboProgressAcc / timeToTurbo, 1);
            }
        }

        private void PerformTurboIfReady()
        {
            if (cooldown)
            {
                return;
            }
            if (turboProgressAcc >= minTimeToTurbo)
            {
                if (turboCoroutine == null)
                {
                    var turboPower = Mathf.Clamp(turboProgressAcc / timeToTurbo, 0, 1);
                    currentSpeed = Mathf.Lerp(speed, turboSpeed, turboPower);
                    turboCoroutine = StartCoroutine(StopTurboAndCooldownCoroutine(turboPower));
                }
            }
            else if (turboProgressAcc > 0f)
            {
                CancelTurbo(false);
            }
        }

        private IEnumerator StopTurboAndCooldownCoroutine(float turboPower)
        {
            var psMain = trailParticles.main;
            psMain.startColor = turboTrailColor;
            leftPaddleStartTime = float.MinValue;
            rightPaddleStartTime = float.MaxValue;
            yield return new WaitForSeconds(turboDuration * turboPower);
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
            if (dead)
            {
                return;
            }
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