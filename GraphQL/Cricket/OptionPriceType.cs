using ConferencePlanner.GraphQL.Data;
using ConferencePlanner.GraphQL.Data.Cricket;

namespace ConferencePlanner.GraphQL.Cricket
{
    public class OptionPriceType
    {
        public static async Task<List<MatchOptionChainingResponseModel?>> GetOptionPriceAsync(
        MatchStatsData matchStatsData,
        CancellationToken cancellationToken)
        {
            if (matchStatsData.BallNo < 0)
            {
                return null;
            }

            return GetOptionChainingData(matchStatsData);
        }

        public static List<MatchOptionChainingResponseModel> GetOptionChainingData(MatchStatsData cricketMatchData)
        {
            List<MatchOptionChainingResponseModel> cricketDataResponse = new List<MatchOptionChainingResponseModel>();
            int totalRuns = 0; int totalBalls = 120; int overs = 0; int remainingBalls = 0; int wicketCount = 0; double strikeRate = 0; double stockPrice; int totalMatchRun = 0; double lotOptionPrice = 0;

            totalRuns = cricketMatchData.TotalRuns;
            wicketCount = cricketMatchData.Wicket;

            for (int strkPrice = 10; strkPrice < 251;)
            {
                if (cricketMatchData.Inning == "1st Inning")
                    stockPrice = totalRuns + (totalBalls - cricketMatchData.BallNo);
                else
                {
                    totalMatchRun = cricketMatchData.TargetRun;
                    int projectedScore = totalRuns + (totalBalls - cricketMatchData.BallNo);
                    int adjustedProjectedScore = Math.Min(projectedScore, totalMatchRun);
                    stockPrice = adjustedProjectedScore;
                }

                double strikePrice = strkPrice;
                double riskFreeRate = 0.5;
                double volatility = 0.3;
                double timeToExpiration = TimeToExpiration.CalculateTimeToExpirationWithWicket(totalBalls, cricketMatchData.BallNo, wicketCount);
                //bool isCallOption = true;

                double optionPrice = OptionPrice.CalculateOptionPrice(
                    stockPrice,
                    strikePrice,
                    timeToExpiration,
                    riskFreeRate,
                    volatility
                );

                overs = cricketMatchData.BallNo / 6;
                remainingBalls = cricketMatchData.BallNo % 6;

                MatchOptionChainingResponseModel responseModel = new MatchOptionChainingResponseModel()
                {
                    CallOptionPrice = Math.Round(optionPrice, 2),
                    Strike = strkPrice,
                    BallNo = cricketMatchData.BallNo,
                    OverNo = double.Parse(overs.ToString() + "." + remainingBalls.ToString()),
                    Score = totalRuns + "/" + wicketCount
                };
                cricketDataResponse.Add(responseModel);

                strkPrice = strkPrice + 10;
            }
            return cricketDataResponse;
        }
    }
    public class TimeToExpiration
    {
        public static double CalculateTimeToExpirationWithWicket(int totalballs, int currentBall, int totalWicket)
        {
            double timeToExpiration;             //No.of ball left/120 * (1 - no of wicket * wicketdepreciation%)
            double cc = 1;

            double bb = (double)(totalballs - currentBall) / totalballs;
            if (totalWicket != 0)
                cc = 1 - (totalWicket * .10);// WicketCumulative.CalculateWicketCumulative(totalWicket));
            timeToExpiration = bb * cc;
            return timeToExpiration;
        }
    }
    public class OptionPrice
    {
        public static double CalculateOptionPrice(
            double stockPrice,
            double strikePrice,
            double timeToExpiration,
            double riskFreeRate,
            double volatility)
        {
            double d1 = (Math.Log(stockPrice / strikePrice) + (Math.Pow(volatility, 2) / 2) * timeToExpiration) / (volatility * Math.Sqrt(timeToExpiration));
            double d2 = d1 - volatility * Math.Sqrt(timeToExpiration);

            return stockPrice * CDF(d1) - strikePrice * Math.Exp(-riskFreeRate * timeToExpiration) * CDF(d2);
        }

        private static double CDF(double x)
        {
            return 0.5 * (1.0 + Erf(x / Math.Sqrt(2.0)));
        }

        private static double Erf(double x)
        {
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            int sign = x >= 0 ? 1 : -1;
            x = Math.Abs(x);

            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return sign * y;
        }
    }
}