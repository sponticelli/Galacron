using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Galacron.Formations
{
    /// <summary>
    /// Basic formation implementation
    /// </summary>
    public class Formation : MonoBehaviour, IFormation
    {
        [SerializeField] private FormationConfig config;
        [SerializeField]    
        private List<FormationSlot> slots = new List<FormationSlot>();
        private List<IMember> members = new List<IMember>();
        private Vector2 basePosition;
        private float idleTime;
        private float attackTimer;

        
        private bool _isDefeated;
        public FormationConfig Config
        {
            get => config;
            set => config = value;
        }

        private void Start()
        {
            InitializeSlots();
        }

        private void InitializeSlots()
        {
            Debug.Log("Initializing formation slots");
            slots.Clear();
            basePosition = transform.position;

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
        }

        public void AddMember(IMember member)
        {
            // Find first available slot
            var slot = slots.FirstOrDefault(s => !s.isOccupied);
            if (slot == null)
            {
                Debug.LogWarning("No available slots in formation");
                return;
            }

            // Assign slot
            slot.isOccupied = true;
            slot.occupant = member;
            members.Add(member);

            // Create enter formation state
            member.SetFormationPosition(slot.localPosition);
            var enterState = new EnterFormationState(this, slot, config.entryPath);
            member.SetState(enterState);
        }

        public void RemoveMember(IMember member)
        {
            var slot = slots.Find(s => s.occupant == member);
            if (slot != null)
            {
                slot.isOccupied = false;
                slot.occupant = null;
            }
            members.Remove(member);
            
            if (members.Count == 0)
            {
                _isDefeated = true;
            }
        }

        public void UpdateFormation()
        {
            UpdateIdleMovement();
            UpdateAttackCycle();
        }

        private void UpdateIdleMovement()
        {
            idleTime += Time.deltaTime;
            var offset = Mathf.Sin(idleTime * config.idleFrequency) * config.idleAmplitude;
            
            // Apply gentle floating motion to formation
            foreach (var slot in slots)
            {
                if (!slot.isOccupied) continue;
                
                var worldPos = transform.TransformPoint(slot.localPosition);
                worldPos.y += offset;
                slot.occupant.SetFormationPosition(worldPos);
            }
        }

        private void UpdateAttackCycle()
        {
            attackTimer += Time.deltaTime;
            if (attackTimer < config.attackCooldown) return;

            attackTimer = 0f;
            
            // Count current attackers
            var currentAttackers = members.Count(m => m is IAttackingMember attacking && attacking.IsAttacking);
            
            if (currentAttackers >= config.maxSimultaneousAttackers) return;

            // Select random members for attack
            var availableMembers = members
                .Where(m => m is IAttackingMember attacking && !attacking.IsAttacking)
                .OrderBy(x => UnityEngine.Random.value)
                .Take(config.maxSimultaneousAttackers - currentAttackers);

            foreach (var member in availableMembers)
            {
                if (UnityEngine.Random.value <= config.attackProbability)
                {
                    var slot = slots.Find(s => s.occupant == member);
                    var attackState = new AttackState(this, slot, config.attackPath);
                    member.SetState(attackState);
                }
            }
        }

        public bool IsComplete()
        {
            return members.Count >= slots.Count;
        }

        public bool IsDefeated()
        {
            return _isDefeated;
        }

        public void Disband()
        {
            members.Clear();
            slots.Clear();
        }

        public Vector2 GetSlotWorldPosition(FormationSlot slot)
        {
            return transform.TransformPoint(slot.localPosition);
        }
    }
}