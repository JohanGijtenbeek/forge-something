using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Erp.Infrastructure.Persistence;

public class DbConnectionFactory
{
    private readonly string _connectionString;

    static DbConnectionFactory()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        // SQL Server DATE columns are returned as DateTime by SqlClient;
        // these handlers convert between DateTime and DateOnly.
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new NullableDateOnlyTypeHandler());
    }

    public DbConnectionFactory(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException(
                                "ConnectionString 'DefaultConnection' niet gevonden.");
    }

    public SqlConnection Create() => new(_connectionString);
}

file sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value.ToDateTime(TimeOnly.MinValue);
    }

    public override DateOnly Parse(object value) =>
        DateOnly.FromDateTime((DateTime)value);
}

file sealed class NullableDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly?>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly? value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value.HasValue ? (object)value.Value.ToDateTime(TimeOnly.MinValue) : DBNull.Value;
    }

    public override DateOnly? Parse(object value) =>
        value is null or DBNull ? null : DateOnly.FromDateTime((DateTime)value);
}