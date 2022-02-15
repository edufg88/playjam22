using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayakGame
{
    public class IngameUI : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI timeLabel;
        [SerializeField] private TMPro.TextMeshProUGUI distanceLabel;
        [SerializeField] private TMPro.TextMeshProUGUI coinsLabel;
        [SerializeField] private TMPro.TextMeshProUGUI scoreLabel;

        public void UpdateValues(float time, float distance, int coins, int score)
        {
            var timespan = TimeSpan.FromSeconds(time);
            timeLabel.text = timespan.ToString(@"hh\:mm\:ss\:fff");
            distanceLabel.text = distance.ToString();
            coinsLabel.text = coins.ToString();
            scoreLabel.text = score.ToString();
        }
    }
}