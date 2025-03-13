using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Sessions;
using HotChocolate.Subscriptions;
using Npgsql;
using System.Text.Json;
using System.Threading;

namespace ConferencePlanner.GraphQL.Cricket
{
    public class SqlDataFetch
    {
        public async static void FetchData(ITopicEventSender eventSender, CancellationToken cancellationToken)
        {
            // Connection string to your PostgreSQL database
            string connString = "Host=127.0.0.1;Username=graphql_workshop;Password=secret";

            // SQL query to fetch id and data
            string query = "SELECT \"Id\", \"Data\" FROM public.\"WebhookData\" where  \"Id\" < 50 order by \"Id\" asc;";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        Random random = new Random();
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string data = reader.GetString(1);
                            if (data.Contains("title"))
                            {
                                Root response = JsonSerializer.Deserialize<Root>(data);
                                if (response.response.match_id == 80660)
                                {
                                    MatchInning matchInning = new MatchInning()
                                    {
                                        overs = response.response.live.live_score.overs,
                                        runs = response.response.live.live_score.runs,
                                        wicket = response.response.live.live_score.wickets
                                    };
                                    int delay = random.Next(2000, 6000);
                                    Thread.Sleep(delay);

                                    await eventSender.SendAsync(
                                             nameof(OptionPriceByRunSubscriptions.OnBallChangedByRunAsync),
                                             matchInning,
                                             cancellationToken);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //[SubscriptionType]
    //public static class OptionPriceNewBallSubscriptions
    //{
    //    [Subscribe]
    //    [Topic]
    //    public static async Task<decimal> OnBallNewBallChangedAsync(
    //        [EventMessage] int ballNo,
    //        CancellationToken cancellationToken)
    //    {
    //        SqlDataFetch.FetchData();

    //        return ballNo * 4;
    //    }
    //}

    public class MatchInning
    {
        public int wicket { get; set; }
        public int runs { get; set; }
        public double overs { get; set; }
    }
    public class Root
    {
        public Response response { get; set; }
    }
    public class Response
    {
        public int match_id { get; set; }
        public Live live { get; set; }
    }
    public class Live
    {
        public LiveScore live_score { get; set; }
    }
    public class LiveScore
    {
        public int runs { get; set; }
        public double overs { get; set; }
        public int wickets { get; set; }

    }
}
