using ConferencePlanner.GraphQL.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<ApplicationDbContext>(
        options => options.UseNpgsql("Host=127.0.0.1;Username=graphql_workshop;Password=secret"))
    .AddGraphQLServer()
    .AddRedisSubscriptions(_ => ConnectionMultiplexer.Connect("127.0.0.1:6379"))
    .AddGlobalObjectIdentification()
    .AddGraphQLTypes()
    .AddMutationConventions()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();

app.UseWebSockets();
app.MapGraphQL();

app.MapGet("/", () => "Hello World!");

//app.Run();
await app.RunWithGraphQLCommandsAsync(args);