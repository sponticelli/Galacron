using UnityEngine;

namespace Galacron.Actors
{
    public class Formation : MonoBehaviour
    {
        [SerializeField] private FormationLayout layout;
        [SerializeField] private FormationRegistry registry;
        [SerializeField] private AbstractFormationDiveController diveController;
    
        private void Awake()
        {
            if (layout==null) layout = GetComponent<FormationLayout>();
            if (registry==null) registry = GetComponent<FormationRegistry>();
            if (diveController) diveController = GetComponent<AbstractFormationDiveController>();
        }

        // Public API methods that delegate to the appropriate component
        public Vector3 GetPosition(int index) => layout.GetPosition(index);
        
        public int GetTotalMembers() => registry.GetMembers().Count;
    
        public void RegisterMember(FormationActor actor, int positionIndex)
        {
            registry.RegisterMember(actor, positionIndex);
        }
    
        public void OnMemberDestroyed(FormationActor actor)
        {
            registry.UnregisterMember(actor, true);
        }
    
        public void SetTotalMembers(int count)
        {
            registry.SetExpectedMembers(count);
        }
    
        public void OnSpawnComplete()
        {
            diveController.EnableDiving();
        }
    
        public void OnDiveComplete(int actorId)
        {
            diveController.OnDiveComplete(actorId);
        }
    }
}