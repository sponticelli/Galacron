using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Galacron.Formations
{
    public class Formation : MonoBehaviour, IFormation
    {
        private FormationConfig config;
        private IFormationPattern pattern;
        private IFormationMovement movement;
        private List<IMember> members = new List<IMember>();
        private float attackTimer;
        private bool isInitialized;
        private bool isDefeated;

        public void Initialize(FormationConfig config, IFormationPattern pattern, IFormationMovement movement)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
                
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));
                
            if (movement == null)
                throw new ArgumentNullException(nameof(movement));

            this.config = config;
            this.pattern = pattern;
            this.pattern.Initialize(config);
            this.movement = movement;
            this.movement.Initialize(config, pattern);
            
            isInitialized = true;
            Debug.Log("Formation initialized successfully");
        }


        public void AddMember(IMember member)
        {
            EnsureInitialized();
            
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            var slot = pattern.GetAvailableSlot();
            if (slot == null)
            {
                Debug.LogWarning("No available slots in formation");
                return;
            }

            pattern.AssignSlot(slot, member);
            members.Add(member);

            var enterState = new EnterFormationState(this, slot, config.entryPath);
            member.SetState(enterState);
        }

        public void RemoveMember(IMember member)
        {
            var slot = pattern.Slots.FirstOrDefault(s => s.occupant == member);
            if (slot != null)
            {
                pattern.ReleaseSlot(slot);
            }
            members.Remove(member);

            if (members.Count == 0)
            {
                isDefeated = true;
            }
        }

        public void UpdateFormation()
        {
            movement.UpdateMovement();
            UpdateAttackCycle();
        }

        private void UpdateAttackCycle()
        {
            attackTimer += Time.deltaTime;
            if (attackTimer < config.attackCooldown) return;

            attackTimer = 0f;

            var currentAttackers = members.Count(m => m is IAttackingMember attacking && attacking.IsAttacking);
            if (currentAttackers >= config.maxSimultaneousAttackers) return;

            var availableMembers = members
                .Where(m => m is IAttackingMember attacking && !attacking.IsAttacking)
                .OrderBy(x => Random.value)
                .Take(config.maxSimultaneousAttackers - currentAttackers);

            foreach (var member in availableMembers)
            {
                if (Random.value <= config.attackProbability)
                {
                    var slot = pattern.Slots.Find(s => s.occupant == member);
                    var attackState = new AttackState(this, slot, config.attackPath);
                    member.SetState(attackState);
                }
            }
        }

        public bool IsComplete() => pattern.IsComplete();
        public bool IsDefeated() => isDefeated;

        public void Disband()
        {
            members.Clear();
            foreach (var slot in pattern.Slots)
            {
                pattern.ReleaseSlot(slot);
            }
        }

        public Vector2 GetSlotWorldPosition(FormationSlot slot)
        {
            return pattern.GetSlotWorldPosition(slot);
        }
        
        private void EnsureInitialized()
        {
            if (!isInitialized)
                throw new InvalidOperationException("Formation not initialized");
        }
    }
}