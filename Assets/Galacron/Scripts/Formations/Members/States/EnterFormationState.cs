using UnityEngine;

namespace Galacron.Formations
{
    /// <summary>
    /// State for handling formation entry movement
    /// </summary>
    public class EnterFormationState : IMemberState
    {
        private readonly IFormation formation;
        private readonly FormationSlot targetSlot;
        private readonly PathConfig pathConfig;
        private float progress;
        private Vector2 startPosition;
        private Vector2[] pathPoints;

        public EnterFormationState(IFormation formation, FormationSlot slot, PathConfig pathConfig)
        {
            this.formation = formation;
            this.targetSlot = slot;
            this.pathConfig = pathConfig;
        }

        public void Enter(IMember member)
        {
            progress = 0f;
            startPosition = member.GetCurrentPosition();

            // Generate path points based on path type
            switch (pathConfig.pathType)
            {
                case PathType.Linear:
                    pathPoints = new[] { startPosition, ((Formation)formation).GetSlotWorldPosition(targetSlot) };
                    break;
                case PathType.Bezier:
                    pathPoints = pathConfig.controlPoints;
                    pathPoints[0] = startPosition;
                    pathPoints[pathPoints.Length - 1] = ((Formation)formation).GetSlotWorldPosition(targetSlot);
                    break;
                case PathType.CircularArc:
                    // Implementation for circular arc path
                    break;
            }
        }

        public void Update(IMember member, float deltaTime)
        {
            // Wait for arrival delay
            if (targetSlot.arrivalDelay > 0)
            {
                targetSlot.arrivalDelay -= deltaTime;
                return;
            }

            progress += deltaTime / pathConfig.duration;
            progress = Mathf.Clamp01(progress);

            // Get position along path based on path type
            Vector2 newPosition = pathPoints[0];
            float curveProgress = pathConfig.speedCurve.Evaluate(progress);

            switch (pathConfig.pathType)
            {
                case PathType.Linear:
                    newPosition = Vector2.Lerp(pathPoints[0], pathPoints[1], curveProgress);
                    break;
                case PathType.Bezier:
                    newPosition = EvaluateBezier(pathPoints, curveProgress);
                    break;
                case PathType.CircularArc:
                    // Implementation for circular arc path
                    break;
            }

            member.SetFormationPosition(newPosition);

            // Transition to idle state when reaching destination
            if (progress >= 1f)
            {
                member.SetState(new IdleState(formation, targetSlot));
            }
        }

        public void Exit(IMember member)
        {
            // Ensure final position is exact
            member.SetFormationPosition(((Formation)formation).GetSlotWorldPosition(targetSlot));
        }

        private Vector2 EvaluateBezier(Vector2[] points, float t)
        {
            if (points.Length == 2) return Vector2.Lerp(points[0], points[1], t);

            var newPoints = new Vector2[points.Length - 1];
            for (int i = 0; i < points.Length - 1; i++)
            {
                newPoints[i] = Vector2.Lerp(points[i], points[i + 1], t);
            }

            return EvaluateBezier(newPoints, t);
        }
    }
}