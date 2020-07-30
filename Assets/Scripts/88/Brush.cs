using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game_88
{
    public class Brush : MonoBehaviour
    {
        [SerializeField] Vector2 tapOffset;

        SpriteRenderer sprite;

        Vector3 startPos;

        private void Start()
        {
            startPos = transform.position;
            sprite = GetComponent<SpriteRenderer>();
        }


        /// <summary>
        /// Changing brush position
        /// </summary>
        void OnMouseDrag()
        {
            if (!EventSystem.current.IsPointerOverGameObject()) {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = transform.position.z;
                transform.position = pos + (Vector3)tapOffset;
            }
        }

        private void OnMouseUp()
        {
            transform.position = startPos;
        }

        /// <summary>
        /// Check collision with teeth and clean
        /// </summary>
        void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("tooth"))
            {
                Tooth tooth = other.GetComponent<Tooth>();

                tooth.needToClean -= 1;

                if (tooth.needToClean == 0) {
                    tooth.CleanTooth();
                }
                if(tooth.needToClean < 0)
                {
                    tooth.ToothWasCleaned();
                }
            }
        }


        /// <summary>
        /// Light up when user in idle
        /// </summary>
        public IEnumerator LightUp(int times)
        {
            Color baseColor = sprite.color;
            Color changedColor = sprite.color;

            int currTime = 0;
            float mult = .3f;
            while (currTime < times) {
                yield return null;

                if (changedColor.b <= .7f)
                    mult = .3f;
                if (changedColor.b >= 1)
                    mult = -.3f;

                changedColor.b += Time.deltaTime * mult;

                sprite.color = changedColor;

                currTime++;
            }
            sprite.color = baseColor;
        }
    }
}