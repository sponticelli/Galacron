using System;

namespace Galacron.Actors
{
    [Serializable]
    public class FireSettings
    {
        public bool canFire = true;
        public float minFireRate = 0.5f;
        public float maxFireRate = 1f;
        public float minPrecision = 0.1f;
        public float maxPrecision = 0.5f;

        public float GetFireRate()
        {
            return UnityEngine.Random.Range(minFireRate, maxFireRate);
        }

        public float GetPrecision()
        {
            return UnityEngine.Random.Range(minPrecision, maxPrecision);
        }
    }
}