using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE.SortBots {
	public class SortBot : ATEObject
	{
        public float moveSpeed = 0.1f;
        public float turnSpeed = 1.0f;
        public float castDist = 1.0f;


        // Cache capsule cast info
        private Vector3 castPoint1;
        private Vector3 castPoint2;
        private float castRadius;

        private bool isTurningAround = false;
        private float amountToTurn = 0;
        private bool turnNegative;


        private void Awake()
        {
            // Cache anything that can be reused for CapsuleCollider
            CapsuleCollider col = GetComponent<CapsuleCollider>();
            Vector3 halfHeight = Vector3.up * ((col.height / 2) - col.radius);

            castPoint1 = col.center + halfHeight;
            castPoint2 = col.center - halfHeight;
            castRadius = col.radius;
        }


        protected override void OnEventsRegister()
        {
            base.OnEventsRegister();
            GameEvents.instance.timedUpdate += OnTimedUpdate;
        }

        protected override void OnEventsUnregister()
        {
            base.OnEventsUnregister();
            GameEvents.instance.timedUpdate -= OnTimedUpdate;
        }


        private void OnTimedUpdate ()
        {
            if (isTurningAround)
            {
                Turn();
                return;
            }

            RaycastHit hit;
            Vector3 p1 = transform.position + castPoint1;
            Vector3 p2 = transform.position + castPoint2;

            if (Physics.CapsuleCast(p1, p2, castRadius, transform.forward, out hit, castDist))
                BumpCollider(hit.collider);
            else
                MoveForward();
        }


        private void MoveForward()
        {
            transform.position += transform.forward * moveSpeed;
        }
        
        private void BumpCollider(Collider theCol)
        {
            isTurningAround = true;
            turnNegative = Random.Range(0, 2) == 0;
            amountToTurn = 180;
        }

        private void Turn()
        {
            float turnAmount = Mathf.Min (turnSpeed, amountToTurn);
            transform.Rotate(new Vector3(0, turnAmount * (turnNegative ? -1 : 1), 0));

            amountToTurn -= turnAmount;
            if (amountToTurn <= 0)
                isTurningAround = false;
        }

    }
}
