using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Data.Cricket;
using ConferencePlanner.GraphQL.Sessions;
using HotChocolate.Subscriptions;

namespace ConferencePlanner.GraphQL.Cricket
{
    [QueryType]
    public static class OptionPriceQueries
    {
        public static async Task<decimal> GetOptionPriceAsync(
        MatchStatsData matchStatsData,
        CancellationToken cancellationToken)
        {
            if (matchStatsData.BallNo < 0)
            {
                return 0;
            }
            double price = 133.22;

            return (decimal)price;
        }
        public static async Task<decimal> GetMyPrice([Service]  ITopicEventSender eventSender, CancellationToken cancellationToken)
        {
            SqlDataFetch.FetchData(eventSender, cancellationToken);
            //if (ball < 0)
            //{
            //    return 0;
            //}

            return 1.0m;
        }

        [GraphQLName("optionPriceRate")]
        public static async Task<decimal> GetOptionPriceRateAsync(
            [GraphQLNonNullType] MatchStatsData1 matchStatsData,
             ITopicEventSender eventSender,CancellationToken cancellationToken)
        {
            if (matchStatsData.BallNo < 0)
            {
                return 0;
            }

            //await eventSender.SendAsync(
            //                $"OnBallChanged_{matchStatsData.BallNo}",
            //                matchStatsData.Strikeprice,
            //                cancellationToken);

            await eventSender.SendAsync(
                            nameof(OptionPriceSubscriptions.OnBallChangedAsync),
                            matchStatsData.BallNo,
                            cancellationToken);

            return await Task.FromResult(matchStatsData.BallNo);
        }
    }


    public class MatchStatsData1
    {
        public int BallNo { get; set; }
        public int Strikeprice { get; set; }
    }
}
