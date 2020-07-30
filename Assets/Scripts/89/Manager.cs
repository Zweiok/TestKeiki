using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game_89
{
    public class Manager : MonoBehaviour
    {
        public enum SoundType {task, incorrect, greatJob };

        [SerializeField] Crocodile crocodile;
        [SerializeField] AudioSource audio;

        [SerializeField] AudioClip incorrectSound;
        [SerializeField] AudioClip taskSound;
        [SerializeField] AudioClip greatJobSound;
        
        public int missTapCount = 0;


        /// <summary>
        /// user idle time
        /// </summary>
        int idleTime = 0;


        /// <summary>
        /// time from last 4 missed taps
        /// </summary>
        float fewTapsTime = 0;
        
        float disableSoundTime = 0;

        bool soundIsDisabled = false;

        Coroutine disableSoundCoroutine;

        [SerializeField] SpriteRenderer trackSprite;

        Coroutine trackLightUpCoroutine;

        private void Start()
        {
            StartCoroutine(IdleController());
        }

        public void PlaySound(SoundType type)
        {
            if(!audio.isPlaying && !soundIsDisabled)
            {
                switch(type)
                {
                    case SoundType.incorrect:
                        audio.clip = incorrectSound;
                        break;
                    case SoundType.task:
                        audio.clip = taskSound;
                        break;
                    case SoundType.greatJob:
                        audio.clip = greatJobSound;
                        break;
                }
                audio.Play();
            }
        }

        private void Update()
        {
            if(Input.GetMouseButton(0))
            {
                idleTime = 0;
            }

            if(Input.GetMouseButtonDown(0))
            {
                idleTime = 0;

                missTapCount++;

                if(trackLightUpCoroutine != null)
                {
                    trackSprite.color = Color.white;
                    StopCoroutine(trackLightUpCoroutine);
                }

                if(missTapCount % 4 == 0)
                {
                    PlaySound(SoundType.incorrect);
                    if (Time.timeSinceLevelLoad - fewTapsTime < 3)
                    {
                        disableSoundTime = 4;
                        if (disableSoundCoroutine == null)
                        {
                            disableSoundCoroutine = StartCoroutine(DisableSound());
                        } 
                    }
                    fewTapsTime = Time.timeSinceLevelLoad;
                }
            }
        }

        IEnumerator IdleController()
        {
            while (audio.isPlaying)
            {
                yield return new WaitForSeconds(1);
            }

            while (!crocodile.isEndGame)
            {
                idleTime++;
                yield return new WaitForSeconds(1);

                if (idleTime > 0)
                {
                    if (idleTime % 7 == 0)
                    {
                        if (trackLightUpCoroutine == null)
                        {
                            trackLightUpCoroutine = StartCoroutine(TrackLightUp());
                        }
                    }
                    else if (idleTime % 5 == 0)
                    {
                        PlaySound(SoundType.task);
                    }
                    else if (idleTime % 4 == 0)
                    {
                        crocodile.SetAnimation("Tap", false);
                    }
                }
            }
        }

        IEnumerator DisableSound()
        {
            soundIsDisabled = true;
            while (disableSoundTime > 0)
            {
                yield return new WaitForSeconds(1);
                disableSoundTime--;
            }
            soundIsDisabled = false;
            disableSoundCoroutine = null;
        }

        IEnumerator TrackLightUp()
        {
            Color trackColor = trackSprite.color;
            float mult = .3f;
            while (true) {
                yield return null;

                if (trackColor.r <= .7f || trackColor.g <= .7f || trackColor.b <= .7f)
                    mult = .3f;
                if (trackColor.r >= 1 || trackColor.g >= 1 || trackColor.b >= 1)
                    mult = -.3f;

                trackColor.r = trackColor.g = trackColor.b += Time.deltaTime * mult;
                trackSprite.color = trackColor;
            }
        }
    }
}