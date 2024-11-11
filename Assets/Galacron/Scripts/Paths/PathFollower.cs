using System;
using UnityEngine;

namespace Galacron.Paths
{
    public class PathFollower : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform objectToMove;
        [SerializeField] private PathBase currentPath;
        
        [Header("Movement Settings")]
        [SerializeField] private float baseSpeed = 2f;
        [SerializeField] private float reachDistance = 0.4f;
        [SerializeField] private bool lookAtDirection = true;
        [SerializeField] private float rotationSpeed = 10f;
        
        private float currentDistance;
        private float waitTimeRemaining;
        private bool isPaused;
        
        public event Action OnPathCompleted;
        public event Action<float> OnProgressChanged;
        
        public float Progress => currentPath != null ? 
            currentDistance / currentPath.GetTotalLength() : 0f;
            
        public bool IsMoving => currentPath != null && !isPaused;
        
        private void Start()
        {
            if (objectToMove == null)
            {
                objectToMove = transform;
            }
        }
        
        public void SetPath(IPath path)
        {
            currentPath = path as PathBase;
            currentDistance = 0f;
            waitTimeRemaining = 0f;
            isPaused = false;
            
            if (path != null)
            {
                OnProgressChanged?.Invoke(0f);
            }
        }
        
        public void Pause() => isPaused = true;
        public void Resume() => isPaused = false;
        
        private void Update()
        {
            if (currentPath == null || isPaused) return;
            
            // Handle waiting
            if (waitTimeRemaining > 0)
            {
                waitTimeRemaining -= Time.deltaTime;
                return;
            }
            
            // Move along path
            float totalLength = currentPath.GetTotalLength();
            if (totalLength <= 0)
            {
                Debug.LogWarning("Path has zero length", this);
                return;
            }
            
            if (currentDistance >= totalLength)
            {
                if (currentPath.IsLooped)
                {
                    currentDistance = 0f;
                }
                else
                {
                    CompletePathing();
                    return;
                }
            }
            
            // Get current point and move towards it
            Vector3 targetPoint = currentPath.GetPointAtDistance(currentDistance);
            
            // Validate target point
            if (float.IsNaN(targetPoint.x) || float.IsNaN(targetPoint.y) || float.IsNaN(targetPoint.z))
            {
                Debug.LogError($"Invalid path point received at distance {currentDistance}", this);
                CompletePathing();
                return;
            }
            
            float speedMultiplier = Mathf.Max(0.1f, currentPath.GetSpeedMultiplierAtDistance(currentDistance));
            float currentSpeed = baseSpeed * speedMultiplier;
            
            Vector3 moveDirection = (targetPoint - objectToMove.position);
            float distance = moveDirection.magnitude;
            
            // Only normalize if distance is not zero to avoid NaN
            if (distance > 0.001f)
            {
                moveDirection /= distance;
            }
            else
            {
                // Skip this frame if too close to target
                currentDistance += currentSpeed * Time.deltaTime;
                OnProgressChanged?.Invoke(Progress);
                return;
            }
            
            // Move
            Vector3 newPosition = Vector3.MoveTowards(
                objectToMove.position,
                targetPoint,
                currentSpeed * Time.deltaTime
            );
            
            // Validate new position before applying
            if (!float.IsNaN(newPosition.x) && !float.IsNaN(newPosition.y) && !float.IsNaN(newPosition.z))
            {
                objectToMove.position = newPosition;
            }
            else
            {
                Debug.LogError("Invalid position calculated", this);
                CompletePathing();
                return;
            }
            
            // Update distance
            currentDistance += currentSpeed * Time.deltaTime;
            
            // Handle rotation
            if (lookAtDirection && moveDirection != Vector3.zero)
            {
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                objectToMove.rotation = Quaternion.Lerp(
                    objectToMove.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
            
            OnProgressChanged?.Invoke(Progress);
        }
        
        private void CompletePathing()
        {
            OnPathCompleted?.Invoke();
            currentPath = null;
            currentDistance = 0f;
        }

        private void OnDisable()
        {
            // Clean up when disabled
            currentPath = null;
            currentDistance = 0f;
            waitTimeRemaining = 0f;
            isPaused = false;
        }
    }
}