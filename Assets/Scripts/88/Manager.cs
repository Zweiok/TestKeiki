using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game_88
{
    public class Manager : MonoBehaviour
    {
        [SerializeField] Crocodile crocodile;
        [SerializeField] Brush brush;
        [SerializeField] List<AudioClip> countSounds = new List<AudioClip>();
        [SerializeField] AudioSource audio;
        [SerializeField] AudioClip doneSound;
        [SerializeField] AudioClip taskSound;
        [SerializeField] AudioClip incorrectSound;

        int idleTime = 0;

        private void Start()
        {
            StartCoroutine(IdleController());
        }


        /// <summary>
        /// Check user idle
        /// </summary>
        /// <returns></returns>
        IEnumerator IdleController()
        {
            while (audio.isPlaying)
            {
                yield return new WaitForSeconds(1);
            }

            while (true)
            {
                idleTime++;
                yield return new WaitForSeconds(1);

                if (idleTime > 0 && crocodile.clearedTeeth < 10) {
                    if (idleTime % 10 == 0)
                    {
                        brush.StartCoroutine(brush.LightUp(5));
                    }
                    else if (idleTime % 5 == 0)
                    {
                        audio.clip = taskSound;
                        audio.Play();
                    }
                    else if (idleTime % 4 == 0)
                    {
                        crocodile.SetAnimation("Sad_Open_mouth", false);
                    }
                }
            }
        }
        
        public void PlayTaskSound()
        {
            if (!audio.isPlaying)
            {
                audio.clip = taskSound;
                audio.Play();
            }
        }

        public void IncorrectSound()
        {
            if (!audio.isPlaying)
            {
                audio.clip = incorrectSound;
                audio.Play();
            }
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                idleTime = 0;
            }
        }


        /// <summary>
        /// Check cleaned teeth
        /// </summary>
        /// <param name="id">Tooth id</param>
        public void ToothCleaned(int id)
        {
            if (!audio.isPlaying || crocodile.clearedTeeth == 10) {
                audio.clip = countSounds[crocodile.clearedTeeth - 1];
                audio.Play();
            }

            if(crocodile.clearedTeeth == 10)
            {
                audio.clip = doneSound;
                audio.Play();
            }
        }

        

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void UnpauseGame()
        {
            Time.timeScale = 1;
        }
    }
}