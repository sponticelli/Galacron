using System.Collections.Generic;
using UnityEngine;

namespace Galacron.Paths
{
    /// <summary>
    /// Base interface for all path types
    /// </summary>
    public interface IPath
    {
        IReadOnlyList<Vector3> GetWorldSpacePoints();
        bool IsLooped { get; }
        float GetTotalLength();
        Vector3 GetPointAtDistance(float distance);
        float GetSpeedMultiplierAtDistance(float distance);
        Color GizmoColor { get; }
    }
}