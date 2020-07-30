using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game_88
{
    
    public class Tooth : MonoBehaviour
    {
        public int id;

        public int needToClean = 3;

        public delegate void ToothCleaned(int id);
        public event ToothCleaned onCleaned;

        public delegate void WasCleaned(int count);
        public event WasCleaned wasCleaned;

        public void ToothWasCleaned()
        {
            wasCleaned?.Invoke(Mathf.Abs(needToClean));
        }

        public void CleanTooth()
        {
            onCleaned?.Invoke(id);
        }
    }
}