using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE.SortBots
{
	public class SortBot : ATEObject
	{
        public float moveSpeed = 0.1f;
        public float castDist = 1.0f;

        public float turnSpeed = 1.0f;
        public float minTurnAngle = 0;
        public float maxTurnAngle = 90;
        public float turnaroundVariance = 15;
        public int minTicksBetweenTurn = 10;
        public int maxTicksBetweenTurn = 100;

        public GameObject heldBoxLocation;
        public GameObject placeBoxLocation;


        // Cache capsule cast info
        private Vector3 castPoint1;
        private Vector3 castPoint2;
        private float castRadius;

        private bool isTurningAround;
        private float amountToTurn;
        private bool turnNegative;
        private int timer_nextTurn;

        private BotBox heldBox;


        private void Awake()
        {
            // Cache anything that can be reused for CapsuleCollider
            CapsuleCollider col = GetComponent<CapsuleCollider>();
            Vector3 halfHeight = Vector3.up * ((col.height / 2) - col.radius);

            castPoint1 = col.center + halfHeight;
            castPoint2 = col.center - halfHeight;
            castRadius = col.radius;

            isTurningAround = false;
            amountToTurn = 0;
            timer_nextTurn = Random.Range(minTicksBetweenTurn, maxTicksBetweenTurn);
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
            Turn();
            if (isTurningAround)
                return;

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
            // Apply turning around
            isTurningAround = true;
            turnNegative = Random.Range(0, 2) == 0;
            amountToTurn = Random.Range(180 - turnaroundVariance, 180 + turnaroundVariance);

            // Pick up or set down box
            BotBox asBox = (BotBox)theCol.gameObject.GetComponent<BotBox>();
            if (asBox != null)
            {
                if (heldBox == null)
                    HoldBox(asBox);
                else if (asBox.boxType == heldBox.boxType)
                    PlaceBox();
            }
        }

        private void Turn()
        {
            timer_nextTurn--;

            if (amountToTurn <= 0)
                return;
            if (!isTurningAround && timer_nextTurn > 0)
                return;

            float turnAmount = Mathf.Min (turnSpeed, amountToTurn);
            amountToTurn -= turnAmount;
            transform.Rotate(new Vector3(0, turnAmount * (turnNegative ? -1 : 1), 0));

            if (amountToTurn <= 0)
            {
                isTurningAround = false;
                timer_nextTurn = Random.Range(minTicksBetweenTurn, maxTicksBetweenTurn);
                amountToTurn = Random.Range(minTurnAngle, maxTurnAngle);
                turnNegative = Random.Range(0, 2) == 0;
            }
        }


        public void HoldBox(BotBox box)
        {
            heldBox = box;
            heldBox.transform.parent = heldBoxLocation.transform;
            heldBox.transform.position = heldBoxLocation.transform.position;
            heldBox.transform.rotation = heldBoxLocation.transform.rotation;
        }

        public void PlaceBox()
        {
            heldBox.transform.parent = null;
            heldBox.transform.position = placeBoxLocation.transform.position;
            heldBox = null;
        }

    }
}
