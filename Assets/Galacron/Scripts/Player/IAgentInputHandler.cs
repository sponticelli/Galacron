using Galacron.Inputs;

namespace Galacron.Player
{
    public interface IAgentInputHandler
    {
        event InputEvent<float> OnHorizontalMovement;
        event InputEvent OnFirePressed;
        event InputEvent OnFireReleased;
        
        float HorizontalMovement { get; }
    }
}