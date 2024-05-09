using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DemoGame
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] AudioSource backgroundMusic;
        [SerializeField] AudioSource effectSound;
        [SerializeField] Player player;

        private void Start()
        {
            if (backgroundMusic == null)
            {
                throw new System.Exception("Background Music not attached");
            }

            if(player == null)
            {
                player = FindAnyObjectByType<Player>(); 
            }

        }

        public void PlaySoundTrack()
        {
            backgroundMusic.Play();
        }
        public void StopSoundTrack()
        {
            backgroundMusic.Stop();
        }

        public IEnumerator PlaySubmitPapers()
        {
            effectSound.Play();
            player.StopPlayingSubmitSounds();
            yield return  new WaitForSeconds(2);
        }


    }
}
