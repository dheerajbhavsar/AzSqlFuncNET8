using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace AzSqlFuncNET8.Dapper.Data;

public class DapperContext
{
    private readonly IConfiguration? _configuration;
    private readonly string? _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _connectionString = _configuration?.GetConnectionString("AzureSql") ?? throw new ArgumentNullException("ConnectionString", "ConnectionString:AzureSql must be specified on settings.json");
    }

    public IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);
}
