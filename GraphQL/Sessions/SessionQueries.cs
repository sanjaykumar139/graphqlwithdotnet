using ConferencePlanner.GraphQL.Data;

namespace ConferencePlanner.GraphQL.Sessions;

[QueryType]
public static class SessionQueries
{
    //public static async Task<IEnumerable<Session>> GetSessionsAsync(
    //    ApplicationDbContext dbContext,
    //    CancellationToken cancellationToken)
    //{
    //    return await dbContext.Sessions.ToListAsync(cancellationToken);
    //}

    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Session> GetSessions(ApplicationDbContext dbContext)
    {
        return dbContext.Sessions.OrderBy(s => s.Title);
    }

    [NodeResolver]
    public static async Task<Session?> GetSessionByIdAsync(
        int id,
        SessionByIdDataLoader sessionById,
        CancellationToken cancellationToken)
    {
        return await sessionById.LoadAsync(id, cancellationToken);
    }

    public static async Task<IEnumerable<Session>> GetSessionsByIdAsync(
        [ID<Session>] int[] ids,
        SessionByIdDataLoader sessionById,
        CancellationToken cancellationToken)
    {
        return await sessionById.LoadRequiredAsync(ids, cancellationToken);
    }
}
