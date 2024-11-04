using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nexus.Core.Services;
using UnityEngine;

namespace Galacron.Formations
{
    [ServiceImplementation]
    public class FormationService : MonoBehaviour, IFormationService
    {
        private List<IFormation> _activeFormations;
        public UnityEngine.Events.UnityEvent onAllFormationsDefeated;
    
        public void RegisterFormation(IFormation formation)
        {
            Debug.Log("Registering formation");
            _activeFormations.Add(formation);
            
            Debug.Log("Formations registered: " + _activeFormations.Count);
        }
        
        public void UnregisterFormation(IFormation formation)
        {
            _activeFormations.Remove(formation);
        }
        
        public bool AssignToFormation(IMember member)
        {
            // Logic to determine which formation should receive this member
            var targetFormation = SelectAppropriateFormation();
            if (targetFormation == null)
            {
                Debug.LogWarning("No formation available to assign member to");
                return false;
            }
            
            targetFormation.AddMember(member);
            member.OnFormationAssigned(targetFormation);
            return true;
        }
        
        protected void Update()
        {
            if (_activeFormations.Count == 0)
            {
                return;
            }
            
            // Remove defeated formations
            foreach (var formation in _activeFormations.Where(formation => formation.IsDefeated()))
            {
                formation.Disband();
            }
            _activeFormations.RemoveAll(formation => formation.IsDefeated());
            
            if (_activeFormations.Count == 0)
            {
                onAllFormationsDefeated?.Invoke();
                return;
            }
            
            
            foreach (var formation in _activeFormations)
            {
                formation.UpdateFormation();
            }
        }
        

        protected virtual IFormation SelectAppropriateFormation()
        {
            // Search for the first incomplete  aformation
            var incompleteFormation = _activeFormations.FirstOrDefault(formation => !formation.IsComplete() && !formation.IsDefeated());
            return incompleteFormation;
        }

        public Task InitializeAsync()
        {
            _activeFormations = new List<IFormation>();
            IsInitialized = true;
            return Task.CompletedTask;
        }

        public Task WaitForInitialization()
        {
            return Task.CompletedTask;
        }

        public bool IsInitialized { get; private set; }
    }
}