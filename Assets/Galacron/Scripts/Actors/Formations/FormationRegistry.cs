using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Galacron.Actors
{
    public class FormationRegistry : MonoBehaviour, IFormationRegistry
    {
        [System.Serializable]
        public class FormationMember
        {
            public int positionIndex;
            public FormationActor actor;
        
            public FormationMember(int index, FormationActor member)
            {
                positionIndex = index;
                actor = member;
            }
        }

        [SerializeField] private UnityEvent onAllMembersDestroyed;
    
        private List<FormationMember> members = new List<FormationMember>();
        private int totalExpectedMembers;

        public void SetExpectedMembers(int count)
        {
            totalExpectedMembers = count;
        }

        public void RegisterMember(FormationActor actor, int positionIndex)
        {
            // If it's not already in the list, add it
            if (members.Find(x => x.actor == actor) == null)
            {
                members.Add(new FormationMember(positionIndex, actor));
            }
        }

        
        public void UnregisterMember(FormationActor actor, bool destroyed = false)
        {
            members.RemoveAll(x => x.actor == actor);
        
            if (members.Count == 0 && totalExpectedMembers > 0 && destroyed)
            {
                onAllMembersDestroyed?.Invoke();
            }
        }

        public bool HasMember(FormationActor actor)
        {
            return members.Exists(x => x.actor == actor);
        }

        public List<FormationMember> GetMembers() => members;
    }
}