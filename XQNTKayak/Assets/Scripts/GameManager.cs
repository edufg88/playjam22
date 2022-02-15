using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KayakGame
{
    public class GameManager : MonoBehaviour
    {
        public void OnGameOver()
        {
            StartCoroutine(GameOverCoroutine());
        }

        private IEnumerator GameOverCoroutine()
        {
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}