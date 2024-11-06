using System.Linq;
using UnityEngine;

namespace Galacron.Formations
{
    [CreateAssetMenu(fileName = "FormationTemplate", menuName = "Galacron/Formations/Formation Template")]
    public class FormationTemplate : ScriptableObject
    {
        public string templateName;
        public string description;
        public Vector2[] slotPositions;
        public float spacing = 1f;
        public float arrivalDelayBetweenMembers = 0.2f;
        
        public void ApplyToConfig(FormationConfig config)
        {
            config.slotPositions = slotPositions.ToArray();
            config.spacing = spacing;
            config.arrivalDelayBetweenMembers = arrivalDelayBetweenMembers;
        }

        public void CopyFromConfig(FormationConfig config)
        {
            slotPositions = config.slotPositions.ToArray();
            spacing = config.spacing;
            arrivalDelayBetweenMembers = config.arrivalDelayBetweenMembers;
        }
    }
}