using Nexus.Sequences;
using TMPro;
using UnityEngine;

namespace Galacron.Sequences
{
    public class SetLevelTextStep : BaseStepWithContext
    {
        [Header("Target")]
        [SerializeField] private TMP_Text targetText;
        
        [Header("Text")]
        [SerializeField] private string levelText = "LEVEL {0}";
        
        
        public override void StartStep()
        {
            base.StartStep();
            targetText.text = string.Format(levelText, context.GetData<GameData>().CurrentLevel);
            Complete();
            Finish();
        }
    }
}