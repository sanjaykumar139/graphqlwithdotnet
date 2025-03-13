namespace ConferencePlanner.GraphQL.Cricket
{
    public sealed class GameStateChange(int strikePrice, int ballNo)
    {
        public int StrikePrice { get; } = strikePrice;
        public int BallNo { get; } = ballNo;

        public async Task<int> OptionPriceAsync(int strikePrice, int ballNo, CancellationToken cancellationToken)
        {
            return strikePrice * ballNo + 2;
        }
    }
}