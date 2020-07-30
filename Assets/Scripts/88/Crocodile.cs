using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System.Linq;
using Spine;

namespace Game_88
{
    public class Crocodile : MonoBehaviour
    {
        string currentAnim = "Sit_Open_mouth";

        public SkeletonAnimation skeletonAnimation;

        public int clearedTeeth = 0;

        [SerializeField] List<Tooth> teeth = new List<Tooth>();
        [SerializeField] Manager manager;

        [SpineSlot]
        public string eyeSlot;

        [SpineAttachment(currentSkinOnly: true)]
        public string eyeClosed;


        /// <summary>
        /// scenario followed by a crocodile
        /// </summary>
        Coroutine currScenario;

        string defaultAnimation = "Sit_Open_mouth";

        private void Start()
        {
            foreach(Tooth tooth in teeth)
            {
                tooth.onCleaned += CleanTeeth;
                tooth.onCleaned += manager.ToothCleaned;
                tooth.wasCleaned += ToothWasCleaned;
            }

            skeletonAnimation.state.Complete += BackToDefaultAnim;
        }


        /// <summary>
        /// Check how much times tooth was cleaned
        /// </summary>
        /// <param name="count"></param>
        public void ToothWasCleaned(int count)
        {

            if (count % 12 == 0)
            {
                manager.PlayTaskSound();
            } else if (count % 6 == 0)
            {
                manager.IncorrectSound();
            }
            else if(count % 3 == 0)
            {
                if (currScenario != null)
                {
                    StopCoroutine(currScenario);
                }
                SetAnimation("Sad_Open_mouth", false);
            }
        }
        
        public void CleanTeeth(int id)
        {
            skeletonAnimation.skeleton.SetAttachment(id.ToString(), null);
            clearedTeeth++;

            if (currScenario == null) {
                Debug.Log(clearedTeeth);
                currScenario = StartCoroutine(ToothCleanScenario());
            }

            if(clearedTeeth >= 10)
            {
                if (currScenario != null)
                {
                    StopCoroutine(currScenario);
                }
                defaultAnimation = "Idle";
                SetAnimation("Close_mouth", false);
            }
        }
        
        /// <summary>
        /// Scenario for crocodile when tooth creaned
        /// </summary>
        /// <returns></returns>
        IEnumerator ToothCleanScenario()
        {
            skeletonAnimation.state.OnComplete(null);
            skeletonAnimation.state.ClearTracks();

            skeletonAnimation.skeleton.SetAttachment(eyeSlot, eyeClosed);

            yield return new WaitForSeconds(.6f);

            skeletonAnimation.skeleton.SetAttachment(eyeSlot, null);

            currScenario = null;

            skeletonAnimation.state.OnComplete(null);

            SetAnimation("Sit_Open_mouth_hand", false);
        }
        
        public void SetAnimation(string animName, bool loop)
        {
            if (animName == currentAnim || string.IsNullOrEmpty(animName))
                return;

            skeletonAnimation.state.SetAnimation(0, animName, loop);
            currentAnim = animName;
        }

        public void BackToDefaultAnim(TrackEntry trackEntry)
        {
            SetAnimation(defaultAnimation, true);
        }

        void OnMouseDown()
        {
            if (currentAnim == "Sit_Open_mouth_hand" 
                || currentAnim == "Sad_Open_mouth" 
                || currentAnim == "Sit_Open_mouth")
            {
                SetAnimation("Tap", false);
            }
        }
    }
    
}