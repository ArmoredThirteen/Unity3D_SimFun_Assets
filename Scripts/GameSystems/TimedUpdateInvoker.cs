using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE {
    public class TimedUpdateInvoker : MonoBehaviour
    {
        public float updatesPerSecond = 30;

        private float updatesTime;
        private float timer_update;


        private void Awake()
        {
            updatesTime = 1.0f / updatesPerSecond;
            timer_update = 0;
        }

        private void Update()
        {
            if (updatesPerSecond <= 0)
                return;
            
            updatesTime = 1.0f / updatesPerSecond;
            timer_update += Time.deltaTime;

            // Invoke one or more updates
            // Can be multiple if deltaTime is significantly higher than updatesTime
            while (timer_update >= updatesTime)
            {
                timer_update -= updatesTime;
                GameEvents.instance.InvokeTimedUpdate();
            }
        }

	}
}
