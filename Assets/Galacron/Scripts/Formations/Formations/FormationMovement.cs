using System;
using UnityEngine;

namespace Galacron.Formations
{
    public class FormationMovement : MonoBehaviour, IFormationMovement
    {
        private FormationConfig config;
        private IFormationPattern pattern;
        private float idleTime;
        private bool isInitialized;

        public void Initialize(FormationConfig config, IFormationPattern pattern)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
                
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            this.config = config;
            this.pattern = pattern;
            isInitialized = true;
        }

        public void UpdateMovement()
        {
            EnsureInitialized();
            UpdateIdleAnimation();
        }

        private void UpdateIdleAnimation()
        {
            idleTime += Time.deltaTime;
            var offset = Mathf.Sin(idleTime * config.idleFrequency) * config.idleAmplitude;

            foreach (var slot in pattern.Slots)
            {
                if (!slot.isOccupied || slot.occupant == null) continue;

                var worldPos = pattern.GetSlotWorldPosition(slot);
                worldPos.y += offset;
                slot.occupant.SetFormationPosition(worldPos);
            }
        }

        private void EnsureInitialized()
        {
            if (!isInitialized)
                throw new InvalidOperationException("FormationMovement not initialized");
        }
    }

}