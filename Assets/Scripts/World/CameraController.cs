using UnityEngine;

namespace TopDownSurvival.World
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target settings")]
        [SerializeField] private Transform target;
        [SerializeField] private float smoothSpeed = 0.125f;
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

        private void LateUpdate()
        {
            if (target == null)
            {
                // Try to find the player target if not set
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
                else
                {
                    return;
                }
            }

            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
