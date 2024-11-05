using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Galacron.Formations
{
    public class FormationPattern : MonoBehaviour, IFormationPattern
    {
        private FormationConfig config;
        private List<FormationSlot> slots = new List<FormationSlot>();
        private bool isInitialized;

        public FormationConfig Config => config;
        public List<FormationSlot> Slots => slots;
        public Vector2 BasePosition { get; private set; }

        public void Initialize(FormationConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            this.config = config;
            InitializeSlots();
            isInitialized = true;
        }

        private void InitializeSlots()
        {
            slots.Clear();
            BasePosition = transform.position;

            if (config.slotPositions == null || config.slotPositions.Length == 0)
            {
                Debug.LogError("Formation config has no slot positions defined");
                return;
            }

            for (int i = 0; i < config.slotPositions.Length; i++)
            {
                var slot = new FormationSlot
                {
                    localPosition = config.slotPositions[i] * config.spacing,
                    index = i,
                    isOccupied = false,
                    arrivalDelay = i * config.arrivalDelayBetweenMembers
                };
                slots.Add(slot);
            }

            Debug.Log($"Initialized {slots.Count} formation slots");
        }

        public FormationSlot GetAvailableSlot()
        {
            EnsureInitialized();
            return slots.FirstOrDefault(s => !s.isOccupied);
        }

        public void AssignSlot(FormationSlot slot, IMember member)
        {
            EnsureInitialized();
            
            if (slot == null)
                throw new ArgumentNullException(nameof(slot));
                
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            if (!slots.Contains(slot))
            {
                Debug.LogError("Attempted to assign slot that doesn't belong to this formation");
                return;
            }

            slot.isOccupied = true;
            slot.occupant = member;
        }

        public void ReleaseSlot(FormationSlot slot)
        {
            EnsureInitialized();
            
            if (slot == null)
                throw new ArgumentNullException(nameof(slot));

            if (!slots.Contains(slot))
            {
                Debug.LogError("Attempted to release slot that doesn't belong to this formation");
                return;
            }

            slot.isOccupied = false;
            slot.occupant = null;
        }

        public Vector2 GetSlotWorldPosition(FormationSlot slot)
        {
            EnsureInitialized();
            
            if (slot == null)
                throw new ArgumentNullException(nameof(slot));

            return transform.TransformPoint(slot.localPosition);
        }

        public FormationSlot FindSlotByMember(IMember member)
        {
            EnsureInitialized();
            
            if (member == null)
                throw new ArgumentNullException(nameof(member));
                
            return slots.Find(s => s.occupant == member);
        }

        public bool IsComplete()
        {
            EnsureInitialized();
            return slots.All(s => s.isOccupied);
        }

        private void EnsureInitialized()
        {
            if (!isInitialized)
                throw new InvalidOperationException("FormationPattern not initialized");
        }
    }
}