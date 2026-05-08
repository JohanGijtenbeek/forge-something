using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Erp.Infrastructure.Persistence;

public class DbConnectionFactory
{
    private readonly string _connectionString;

    static DbConnectionFactory()
    {
        // Dapper snake_case → PascalCase mapping
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public DbConnectionFactory(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException(
                                "ConnectionString 'DefaultConnection' niet gevonden.");
    }

    public SqlConnection Create() => new(_connectionString);
}