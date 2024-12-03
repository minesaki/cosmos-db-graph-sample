# Cosmos DB Graph Sample

This sample app shows how to use Cosmos DB (as a graph database) with Gremlinq or Gremlin.Net.

## Prerequisites

- .NET is installed. (This project uses .NET 9.0)
- Azure Cosmos DB account is created.

### WARNING
No warranty for any result. This app deletes all data in your graph based on your operation. To prevent deleting your data, create a new graph and use it for this app.

## How to run

### Create .env file 

Create .env file as follows, then place it to the project root.
```
# Gremlin endpoint info of your Cosmos DB
COSMOS_DB_DOMAIN=<your-cosmos-db-account-name>.gremlin.cosmos.azure.com
COSMOS_DB_PORT=443
COSMOS_DB_SCHEME=wss

COSMOS_DB_DB=<your-db-name>
COSMOS_DB_GRAPH=<your-graph-name>
# If you haven't changed, the partition key should be 'partitionKey' by default. Be careful that it's case-sensitive.
COSMOS_DB_PARTITION_KEY=<your-partition-key>
COSMOS_DB_AUTH_KEY=<your-key>
```

### Run

```
dotnet run
```
