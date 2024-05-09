using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace DemoGame
{
    public class GameManager : Singleton<GameManager>
    {
        public static int MoneyValue = 5;
        public TextMeshProUGUI MoneyCounter;
        public AudioManager audioManager;
        public Player player;
        private bool isPaused = false;
        private int _moneyCounter;

        void Start()
        {
            _moneyCounter = AppConstants.initial_Money;
            MoneyCounter.text = AppConstants.initial_Money.ToString();
            if (player == null)
            {
                player = FindAnyObjectByType<Player>();
            }
            if (audioManager == null)
            {
                audioManager = GetComponent<AudioManager>();
            }
        }
        public void IncreaseMoney()
        {
            _moneyCounter += MoneyValue;
            PlayerPrefs.SetInt(AppConstants.tag_Currency, _moneyCounter);
            MoneyCounter.text = PlayerPrefs.GetInt(AppConstants.tag_Currency).ToString();

        }
        private void Update()
        {
            if (player.SubmitSounds())
            {
                StartCoroutine(audioManager.PlaySubmitPapers());

            }
        }

    }

}
