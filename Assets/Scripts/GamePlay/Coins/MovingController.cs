using System;
using UnityEngine;
using UnityEngine.Events;

namespace GamePlay.Coins
{
    public class MovingController : MonoBehaviour
    {
        [SerializeField] private int priority;
        [SerializeField] private float speed;
        [SerializeField] private Transform endPoint;
        [SerializeField] private Vector2 endPosition;
        [SerializeField] private bool isActive;
        [SerializeField] private Coin coin;
        public event UnityAction OnComplete;

        private void FixedUpdate()
        {
            if (!isActive) return;
            if (endPoint) endPosition = endPoint.transform.localPosition;
            transform.localPosition =
                Vector2.MoveTowards(transform.localPosition, endPosition, speed * Time.fixedDeltaTime);
            var isAtEndPos = Vector2.Distance(transform.localPosition, endPosition) < 0.01f;
            if (isAtEndPos)
            {
                OnComplete?.Invoke();
                OnComplete = null;
                isActive = false;
                endPoint = null;
            }
        }

        public bool MoveTo(Vector2 pos, float newSpeed, int newPriority = 0, UnityAction onComplete = null)
        {
            if (priority > newPriority)
            {
                return false;
            }

            OnComplete = onComplete;
            isActive = true;
            endPosition = pos;
            speed = newSpeed;
            priority = newPriority;
            return true;
        }

        public bool MoveTo(Transform newEndPoint, float newSpeed, int newPriority = 0, UnityAction onComplete = null)
        {
            if (priority > newPriority)
            {
                return false;
            }

            OnComplete = onComplete;
            isActive = true;
            endPoint = newEndPoint;
            speed = newSpeed;
            priority = newPriority;
            return true;
        }
    }
}