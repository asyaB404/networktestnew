using System;
using UnityEngine;
using UnityEngine.Events;

namespace GamePlay.Coins
{
    public class MovingController : MonoBehaviour
    {
        [SerializeField] private int priority = -128;
        [SerializeField] private float speed;
        [SerializeField] private Transform endPoint;
        [SerializeField] private Vector3 endPosition;
        [SerializeField] private bool isActive;
        [SerializeField] private Coin coin;
        public event Action OnComplete;

        private void FixedUpdate()
        {
            if (!isActive) return;
            if (endPoint) endPosition = endPoint.transform.localPosition;
            var moveDistance = speed * Time.fixedDeltaTime;
            transform.Translate((endPosition - transform.localPosition).normalized * moveDistance);
            var isAtEndPos = Vector2.Distance(transform.localPosition, endPosition) < moveDistance;
            if (isAtEndPos)
            {
                CompleteImmediate();
            }
        }

        public void CompleteImmediate()
        {
            transform.localPosition = endPosition;
            OnComplete?.Invoke();
            OnComplete = null;
            isActive = false;
            endPoint = null;
            priority = -128;
        }


        public bool MoveTo(Vector3 pos, float newSpeed, int newPriority = 0, Action onComplete = null)
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

        public bool MoveTo(Transform newEndPoint, float newSpeed, int newPriority = 0, Action onComplete = null)
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