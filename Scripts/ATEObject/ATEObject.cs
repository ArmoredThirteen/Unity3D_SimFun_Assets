using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE
{
    public class ATEObject : MonoBehaviour
    {
        protected void Start()
        {
            OnEventsRegister();
        }

        protected void OnDestroy()
        {
            OnEventsUnregister();
        }


        protected virtual void OnEventsRegister() { }
        protected virtual void OnEventsUnregister() { }

    }
}
