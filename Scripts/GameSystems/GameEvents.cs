using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE
{
    public class GameEvents : MonoBehaviour
    {
        public static GameEvents instance;


        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                GameObject.Destroy(this.gameObject);
        }


        public event Action timedUpdate;
        public void InvokeTimedUpdate()
        {
            if (timedUpdate != null)
                timedUpdate ();
        }

    }
}
