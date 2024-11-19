using UnityEngine;

namespace Galacron.Actors
{
    public interface IFormationLayout 
    {
        Vector3 GetPosition(int index);
        void ReorderPoints(int oldIndex, int newIndex);
        int PointCount { get; }
    }
}