using UnityEngine;

namespace Galacron.Formations
{
    /// <summary>
    /// Defines a slot position within a formation
    /// </summary>
    [System.Serializable]
    public class FormationSlot
    {
        public Vector2 localPosition;
        public int index;
        public bool isOccupied;
        public IMember occupant;
        public float arrivalDelay;
    }
}