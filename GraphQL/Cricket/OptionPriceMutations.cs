using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Cricket
{
    [MutationType]
    public static class OptionPriceMutations
    {
        public static async Task<Attendee> OptionPriceMutAsync(
            OptionPriceInput input,
            CancellationToken cancellationToken)
        {
            var attendee = new Attendee
            {
                FirstName = "A",
                LastName = "B",
                Username = input.BallNo.ToString(),
                EmailAddress = input.StikePrice.ToString()
            };

            return attendee;
        }

        public static async Task<string> MyOptionPriceMutAsync(
           OptionPriceInput input,
           CancellationToken cancellationToken)
        {

            return input.BallNo.ToString();
        }
    }
}