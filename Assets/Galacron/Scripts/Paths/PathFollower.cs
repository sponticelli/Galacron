using System;
using UnityEngine;
using UnityEngine.Events;

namespace Galacron.Paths
{
    public class PathFollower : MonoBehaviour
    {
        [SerializeField] private Transform objectToMove;
        [SerializeField] private Path pathToFollow;
        [SerializeField] private float speed = 2;
        [SerializeField] private float reachDistance = 0.4f;
        [SerializeField] private bool useBezier;
        
        [SerializeField] private UnityEvent OnPathEnd;
        
        private int currentWayPointID = 0;
        private float distance; //current distance to next waypoint
        
        public void SetPath(Path path)
        {
            pathToFollow = path;
            currentWayPointID = 0;
        }
        
        private void Start()
        {
            if (objectToMove == null)
            {
                objectToMove = transform;
            }
        }

        private void Update()
        {
            if (pathToFollow == null)
            {
                return;
            }
            MoveOnPath();
        }
        
        private void MoveOnPath()
        {
            int pathCount = 0;
            if (useBezier)
            {
                MoveOnBezierPath();
                pathCount = pathToFollow.bezierObjList.Count;
            }
            else
            {
                MoveOnStraightPath();
                pathCount = pathToFollow.pathObjList.Count;
            }
            
            if (distance <= reachDistance)
            {
                currentWayPointID++;
            }
            
            if (currentWayPointID >= pathCount)
            {
                currentWayPointID = 0;
                OnPathEnd?.Invoke();
            }
        }
        
        private void MoveOnBezierPath()
        {
            distance = Vector3.Distance(pathToFollow.bezierObjList[currentWayPointID], objectToMove.position);
            objectToMove.position = Vector3.MoveTowards(objectToMove.position, pathToFollow.bezierObjList[currentWayPointID],
                speed * Time.deltaTime);
            
            var direction = pathToFollow.bezierObjList[currentWayPointID] - objectToMove.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            objectToMove.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        private void MoveOnStraightPath()
        {
            distance = Vector3.Distance(pathToFollow.pathObjList[currentWayPointID].position, objectToMove.position);
            objectToMove.position = Vector3.MoveTowards(objectToMove.position,
                pathToFollow.pathObjList[currentWayPointID].position, speed * Time.deltaTime);
            
            var direction = pathToFollow.pathObjList[currentWayPointID].position - objectToMove.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            objectToMove.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}