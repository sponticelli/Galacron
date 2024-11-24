using Nexus.Sequences;

namespace Galacron.Sequences
{
    public class LevelIncrementStep : BaseStepWithContext
    {
        public override void StartStep()
        {
            base.StartStep();
            context.GetData<GameData>().IncrementLevel();
            Complete();
            Finish();
        }
    }
}