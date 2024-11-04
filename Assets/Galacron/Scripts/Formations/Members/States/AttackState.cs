using System;
using UnityEngine;

namespace Galacron.Formations
{
    /// <summary>
    /// State for attack movement
    /// </summary>
    public class AttackState : IMemberState
    {
        private readonly IFormation formation;
        private readonly FormationSlot slot;
        private readonly PathConfig pathConfig;
        private float progress;
        private Vector2[] pathPoints;
        private bool returning;

        public AttackState(IFormation formation, FormationSlot slot, PathConfig pathConfig)
        {
            this.formation = formation;
            this.slot = slot;
            this.pathConfig = pathConfig;
        }

        public void Enter(IMember member)
        {
            progress = 0f;
            returning = false;

            // Set up attack path
            var startPos = member.GetCurrentPosition();
            var targetPos = ((Formation)formation).GetSlotWorldPosition(slot);

            if (pathConfig.pathType == PathType.Bezier)
            {
                pathPoints = new Vector2[pathConfig.controlPoints.Length];
                Array.Copy(pathConfig.controlPoints, pathPoints, pathConfig.controlPoints.Length);
                pathPoints[0] = startPos;
                pathPoints[pathPoints.Length - 1] = targetPos;
            }
            else
            {
                pathPoints = new[] { startPos, targetPos };
            }

            if (member is IAttackingMember attackingMember)
            {
                attackingMember.StartAttack();
            }
        }

        public void Update(IMember member, float deltaTime)
        {
            progress += deltaTime / pathConfig.duration;

            if (!returning && progress >= 0.5f)
            {
                // Switch to return path at halfway point
                returning = true;
                progress = 0f;
                
                // Reverse path points for return
                var startPos = member.GetCurrentPosition();
                var targetPos = ((Formation)formation).GetSlotWorldPosition(slot);
                
                if (pathConfig.pathType == PathType.Bezier)
                {
                    pathPoints = new Vector2[pathConfig.controlPoints.Length];
                    Array.Copy(pathConfig.controlPoints, pathPoints, pathConfig.controlPoints.Length);
                    pathPoints[0] = startPos;
                    pathPoints[pathPoints.Length - 1] = targetPos;
                }
                else
                {
                    pathPoints = new[] { startPos, targetPos };
                }
            }

            progress = Mathf.Clamp01(progress);
            float curveProgress = pathConfig.speedCurve.Evaluate(progress);

            Vector2 newPosition = pathPoints[0];
            switch (pathConfig.pathType)
            {
                case PathType.Linear:
                    newPosition = Vector2.Lerp(pathPoints[0], pathPoints[1], curveProgress);
                    break;
                case PathType.Bezier:
                    newPosition = EvaluateBezier(pathPoints, curveProgress);
                    break;
            }

            member.SetFormationPosition(newPosition);

            if (returning && progress >= 1f)
            {
                member.SetState(new IdleState(formation, slot));
                if (member is IAttackingMember attackingMember)
                {
                    attackingMember.EndAttack();
                }
            }
        }

        public void Exit(IMember member)
        {
            member.SetFormationPosition(((Formation)formation).GetSlotWorldPosition(slot));
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