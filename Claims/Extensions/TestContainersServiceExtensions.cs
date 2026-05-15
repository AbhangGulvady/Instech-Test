using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;

namespace Claims.Extensions;

public static class TestContainersServiceExtensions
{
    public static async Task<(MsSqlContainer SqlContainer, MongoDbContainer MongoContainer)> StartTestContainersAsync(IConfiguration configuration)
    {
        var sqlImage = configuration["TestContainers:MsSql:Image"]
            ?? "mcr.microsoft.com/mssql/server:2022-latest";
        var mongoImage = configuration["TestContainers:MongoImage"]
            ?? "mongo:latest";

        var sqlContainer = (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? new MsSqlBuilder().WithImage(sqlImage)
                : new MsSqlBuilder()
            ).Build();

        var mongoContainer = new MongoDbBuilder()
            .WithImage(mongoImage)
            .Build();

        await sqlContainer.StartAsync();
        await mongoContainer.StartAsync();

        return (sqlContainer, mongoContainer);
    }
}