using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game_89
{
    [ExecuteInEditMode]
    public class Crocodile : MonoBehaviour
    {
        [SerializeField] BezierCurve spline;
        
        [Range(0, 1f)]
        [SerializeField] float speed = 1;

        public float pos = 0;

        public const float endPos = .98f;

        public SkeletonAnimation skeletonAnimation;

        Vector3 tapPos;

        string defaultAnimation = "Idle";

        string currentAnim = "Idle";

        bool dragged = false;

        [SerializeField] float maxTapDistance = 10;

        int tapCount = 0;

        [SerializeField] Manager manager;

        public bool isEndGame = false;

        MeshRenderer mesh;

        private void Start()
        {
            mesh = GetComponent<MeshRenderer>();

            skeletonAnimation.state.Complete += BackToDefaultAnim;
        }

        private void Update()
        {
            if (pos < endPos)
            {
                Vector3 direction = spline.GetPointAt(pos + .01f) - spline.GetPointAt(pos);
                Quaternion q = Quaternion.LookRotation(transform.forward, direction);
                transform.rotation = Quaternion.Euler(0, 0, q.z * 180) * Quaternion.EulerRotation(0, 0, -90);
                transform.position = spline.GetPointAt(pos);
            }
            else
            {
                if(!isEndGame)
                {
                    StartCoroutine(EndGameScenario());
                }
                isEndGame = true;
            }
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
            manager.missTapCount = 0;
            tapPos = Input.mousePosition;
            dragged = false;
        }

        void OnMouseUp()
        {
            if(Vector3.Distance(tapPos, Input.mousePosition) < 1)
            {
                SetAnimation("Tap", false);
                tapCount++;

                if(tapCount % 4 == 0)
                {
                    manager.PlaySound(Manager.SoundType.task);
                } else if(tapCount % 2 == 0)
                {
                    manager.PlaySound(Manager.SoundType.incorrect);
                }
            }
            else if(dragged)
            {
                tapCount = 0;
                defaultAnimation = "Idle";
                SetAnimation("Idle", true);
            }
        }

        void OnMouseDrag()
        {
            if (Vector3.Distance(spline.GetPointAt(pos), Camera.main.ScreenToWorldPoint(Input.mousePosition)) < maxTapDistance 
                &&  (spline.GetPointAt(pos + .01f) - Camera.main.ScreenToWorldPoint(Input.mousePosition)).magnitude <
                    (spline.GetPointAt(pos) - Camera.main.ScreenToWorldPoint(Input.mousePosition)).magnitude)
            {
                if(!currentAnim.Equals("Walk"))
                {
                    SetAnimation("Walk", true);
                }

                if (pos < endPos) {
                    pos += Time.deltaTime * speed;
                }
            }
            dragged = true;
        }

        IEnumerator EndGameScenario()
        {
            manager.PlaySound(Manager.SoundType.greatJob);

            while(mesh.material.GetFloat("_Alpha") > 0)
            {
                transform.position += -transform.right * Time.deltaTime * 2;
                mesh.material.SetFloat("_Alpha", mesh.material.GetFloat("_Alpha") - Time.deltaTime / 2);
                yield return null;
            }
        }
    }
} 