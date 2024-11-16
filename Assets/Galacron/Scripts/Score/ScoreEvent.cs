namespace Galacron.Score
{
    public readonly struct ScoreEvent
    {
        public readonly int PreviousScore { get; }
        public readonly int NewScore { get; }
        public readonly int Difference { get; }

        public ScoreEvent(int previousScore, int newScore)
        {
            PreviousScore = previousScore;
            NewScore = newScore;
            Difference = newScore - previousScore;
        }
    }
}