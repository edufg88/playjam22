using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KayakGame
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] IngameUI gameUI;
        [SerializeField] Boat boat;

        private float secondsSinceStart;
        private float distanceTraveled;
        private int coinsCollected;
        private int score;

        private bool gameOver = false;

        private void Start()
        {
            SoundManager.Instance.PlayMusic();
        }

        private void OnEnable()
        {
            boat.DistanceAlongRiverTraveledEvent += OnDistanceAlongRiverTraveled;
        }

        private void OnDisable()
        {
            boat.DistanceAlongRiverTraveledEvent -= OnDistanceAlongRiverTraveled;
        }

        private void OnDistanceAlongRiverTraveled(float distance)
        {
            distanceTraveled += distance;
        }

        private void Update()
        {
            if (!gameOver)
            {
                UpdateScoreValues();
                gameUI.UpdateValues(secondsSinceStart, distanceTraveled, coinsCollected, score);
            }
        }
        
        private void UpdateScoreValues()
        {
            secondsSinceStart = Time.timeSinceLevelLoad;
            score = 
                ((int)secondsSinceStart * 10) + 
                ((int)(distanceTraveled * 10f)) + 
                (coinsCollected * 100);
        }

        public void OnGameOver()
        {
            gameOver = true;
            StartCoroutine(GameOverCoroutine());
        }

        public void OnCoinCollected()
        {
            coinsCollected++;
        }

        public void OnDistanceTraveled(float distance)
        {
            distanceTraveled += distance;
        }

        private IEnumerator GameOverCoroutine()
        {
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        } 
    }
}