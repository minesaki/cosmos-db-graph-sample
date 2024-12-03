using System.Text.Json;
using cosmos_db_graph_sample.src.Components;
using ExRam.Gremlinq.Core.AspNet;
using ExRam.Gremlinq.Providers.CosmosDb.AspNet;
using ExRam.Gremlinq.Support.NewtonsoftJson.AspNet;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;


SetEnvironmentVariables();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Gremlinq
builder.Services.AddGremlinq(setup =>
    setup.UseCosmosDb<Vertex, Edge>()
        .UseNewtonsoftJson());

// Add Gremlin.Net
builder.Services.AddTransient(_ => GetGremlinClient());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // app.UseHsts();
}

// app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


static void SetEnvironmentVariables()
{
    dotenv.net.DotEnv.Load();

    var scheme = GetEnv("COSMOS_DB_SCHEME");
    var domain = GetEnv("COSMOS_DB_DOMAIN");
    var port = GetEnv("COSMOS_DB_PORT");

    // Set env vars for Gremlinq
    SetEnv("Gremlinq__CosmosDb__Uri", $"{scheme}://{domain}:{port}/");
    SetEnv("Gremlinq__CosmosDb__Database", GetEnv("COSMOS_DB_DB"));
    SetEnv("Gremlinq__CosmosDb__Graph", GetEnv("COSMOS_DB_GRAPH"));
    SetEnv("Gremlinq__CosmosDb__PartitionKey", GetEnv("COSMOS_DB_PARTITION_KEY"));
    SetEnv("Gremlinq__CosmosDb__AuthKey", GetEnv("COSMOS_DB_AUTH_KEY"));
}

static GremlinClient GetGremlinClient()
{
    var gremlinServer = new GremlinServer(
        GetEnv("COSMOS_DB_DOMAIN"),
        int.Parse(GetEnv("COSMOS_DB_PORT")),
        enableSsl: true,
        username: "/dbs/" + GetEnv("COSMOS_DB_DB") + "/colls/" + GetEnv("COSMOS_DB_GRAPH"),
        password: GetEnv("COSMOS_DB_AUTH_KEY"));
    var serializer = new GraphSON2MessageSerializer(new CustomGraphSON2Reader());
    return new GremlinClient(gremlinServer, serializer);
}

static string GetEnv(string key) => Environment.GetEnvironmentVariable(key) ?? string.Empty;

static void SetEnv(string key, string value) => Environment.SetEnvironmentVariable(key, value);

// Countermeasure for Gremlin.Net and Cosmos DB issue
// https://stackoverflow.com/questions/68092798/gremlin-net-deserialize-number-property/72316108#72316108
public class CustomGraphSON2Reader : GraphSON2Reader
{
    public override dynamic? ToObject(JsonElement graphSon)
    {
        return graphSon.ValueKind switch
        {
            // numbers
            JsonValueKind.Number when graphSon.TryGetInt32(out var intValue) => intValue,
            JsonValueKind.Number when graphSon.TryGetInt64(out var longValue) => longValue,
            JsonValueKind.Number when graphSon.TryGetDecimal(out var decimalValue) => decimalValue,
            _ => base.ToObject(graphSon)
        };
    }
}