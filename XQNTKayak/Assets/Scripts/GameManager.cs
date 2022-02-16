using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KayakGame
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] IngameUI gameUI;
        
        private float secondsSinceStart;
        private float distanceTraveled;
        private int coinsCollected;
        private int score;

        private void Start()
        {
            SoundManager.Instance.PlayMusic();
        }

        private void Update()
        {
            UpdateScoreValues();
            gameUI.UpdateValues(secondsSinceStart, distanceTraveled, coinsCollected, score);
        }
        
        private void UpdateScoreValues()
        {
            secondsSinceStart = Time.timeSinceLevelLoad;
            score = (int)secondsSinceStart + (int)distanceTraveled + coinsCollected * 100;
        }

        public void OnGameOver()
        {
            StartCoroutine(GameOverCoroutine());
        }

        public void OnCoinCollected()
        {
            coinsCollected++;
        }

        private IEnumerator GameOverCoroutine()
        {
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}