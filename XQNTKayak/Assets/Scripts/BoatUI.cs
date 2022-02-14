using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KayakGame
{
    public class BoatUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup turboBarGroup;
        [SerializeField] private Image turboBarProgress;
        [SerializeField] private Image turboCooldownProgress;
        [SerializeField] private Color progressColor;
        [SerializeField] private Color progressCompleteColor;

        private bool isTurboBarVisible => turboBarGroup.alpha == 1f;

        private void Awake()
        {
            turboBarProgress.color = progressColor;
            turboCooldownProgress.gameObject.SetActive(false);
        }

        private void ShowProgress(bool show)
        {
            turboBarGroup.alpha = show ? 1f : 0f;
        }

        public void UpdateTurboProgress(float progress)
        {
            if (!isTurboBarVisible)
            {
                ShowProgress(true);
            }            
            if (progress >= 1f)
            {
                progress = 1f;
                turboBarProgress.color = progressCompleteColor;
            }
            turboBarProgress.fillAmount = progress;
        }

        public void UpdateCooldownProgress(float progress)
        {
            Debug.Log(progress);
            turboCooldownProgress.fillAmount = Mathf.Min(progress, 1f);
        }

        public void OnTurboStop(bool intoCooldown)
        {
            turboBarProgress.fillAmount = 0f;
            turboBarProgress.color = progressColor;
            turboBarProgress.gameObject.SetActive(!intoCooldown);            
            turboCooldownProgress.gameObject.SetActive(intoCooldown);
            if (!intoCooldown)
            {
                ShowProgress(false);
            }
        }

        public void OnTurboCooldownStop()
        {
            turboCooldownProgress.fillAmount = 0f;
            turboBarProgress.gameObject.SetActive(true);
            ShowProgress(false);
        }
    }
}

