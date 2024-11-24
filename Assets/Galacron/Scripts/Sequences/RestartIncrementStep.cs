using Nexus.Sequences;

namespace Galacron.Sequences
{
    public class RestartIncrementStep : BaseStepWithContext
    {
        public override void StartStep()
        {
            base.StartStep();
            context.GetData<GameData>().IncrementRestarts();
            Complete();
            Finish();
        }
    }
}