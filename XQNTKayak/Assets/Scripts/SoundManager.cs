using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KayakGame
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource riverSource;
        [SerializeField] private AudioClip music;
        [SerializeField] private AudioClip river;
        [SerializeField] private AudioClip boatBoost;
        [SerializeField] private AudioClip boatCrash;
        [SerializeField] private AudioClip boatRow;
        [SerializeField] private AudioClip collectCoin;
        [SerializeField] private AudioClip uiButton;

        public static SoundManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void PlayMusic()
        {
            musicSource.clip = music;
            musicSource.Play();
            riverSource.clip = river;
            riverSource.Play();
        }

        public void PlayBoatBoost()
        {
            sfxSource.PlayOneShot(boatBoost);
        }

        public void PlayBoatCrash()
        {
            sfxSource.PlayOneShot(boatCrash);
        }

        public void PlayBoatRow()
        {
            sfxSource.PlayOneShot(boatRow);
        }

        public void PlayCollectCoin()
        {
            sfxSource.PlayOneShot(collectCoin);
        }

        public void PlayUIButton()
        {
            sfxSource.PlayOneShot(uiButton);
        }
    }
}