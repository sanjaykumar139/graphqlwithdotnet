using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Data.Cricket;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace ConferencePlanner.GraphQL.Cricket
{
    //[SubscriptionType]
    //public static class OptionPriceSubscriptions
    //{
    //    [Subscribe(With = nameof(SubscribeToOnBallChangedAsync))]
    //    public static GameStateChange OnBallChange(int strikePrice, [EventMessage] int ballNo)
    //    {
    //        return new GameStateChange(strikePrice, ballNo);
    //    }

    //    public static async ValueTask<ISourceStream<int>> SubscribeToOnBallChangedAsync(
    //        int ballNo,
    //        ITopicEventReceiver eventReceiver,CancellationToken cancellationToken)
    //    {
    //        return await eventReceiver.SubscribeAsync<int>($"OnBallChanged_{ballNo}",cancellationToken);
    //    }
    //}


    [SubscriptionType]
    public static class OptionPriceSubscriptions
    {
        [Subscribe]
        [Topic]
        public static async Task<decimal> OnBallChangedAsync(
            [EventMessage] int ballNo,
            CancellationToken cancellationToken)
        {
            if (ballNo == 0) { return 0; }

            return ballNo * 4;
        }
    }

    [SubscriptionType]
    public static class OptionPriceByRunSubscriptions
    {
        [Subscribe]
        [Topic]
        public static async Task<IEnumerable<MatchOptionChainingResponseModel>> OnBallChangedByRunAsync(
            [EventMessage] MatchInning matchInning,
            CancellationToken cancellationToken)
        {
            MatchStatsData matchData = new MatchStatsData()
            {
                BallNo = ConvertOversToBalls(matchInning.overs),
                Inning = "1st Inning",
                Lots = 10,
                ScoreStrike = 10,
                TargetRun = 0,
                TotalRuns = matchInning.runs,
                Wicket = matchInning.wicket,
            };

            IEnumerable<MatchOptionChainingResponseModel> optionChain = OptionPriceType.GetOptionChainingData(matchData);

            return optionChain;
        }

        public static int ConvertOversToBalls(double overs)
        {
            int fullOvers = (int)overs;
            double partialOvers = overs - fullOvers;
            int ballsFromFullOvers = fullOvers * 6;
            int ballsFromPartialOvers = (int)(partialOvers * 10);
            return ballsFromFullOvers + ballsFromPartialOvers;
        }
    }
}