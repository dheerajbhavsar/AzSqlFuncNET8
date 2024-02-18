using Dapper;
using System.Data;
using AzSqlFuncNET8.Dapper.Data;
using AzSqlFuncNET8.Dapper.Models;
using Microsoft.Extensions.Logging;

namespace AzSqlFuncNET8.Dapper.Repositories;

public class CarsRepository(DapperContext context, ILoggerFactory loggerFactory) : ICarsRepository
{
    private readonly DapperContext _context = context ??
        throw new ArgumentNullException(nameof(context));

    private readonly ILogger<CarsRepository> _logger = loggerFactory.CreateLogger<CarsRepository>() ??
        throw new ArgumentNullException(nameof(loggerFactory));

    public async Task<Car> CreateAsync(Car car, CancellationToken token)
    {
        var query = """
            INSERT INTO [dbo].[Cars] (Name)
            OUTPUT INSERTED.Id
            VALUES(@Name);
            """;

        try
        {
            using var connection = _context.CreateConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            var insertedId = await connection.QuerySingleAsync<int>(new CommandDefinition(query, car, cancellationToken: token));

            return new Car { Id = insertedId, Name = car.Name };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while inserting a row into database");
            throw;
        }
        
    }

    public async Task<IEnumerable<Car>> GetAllAsync(CancellationToken token)
    {
        var selectQuery = "SELECT * FROM [dbo].[Cars]";
        try
        {
            using var connection = _context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            var result = await connection.QueryAsync<Car>(new CommandDefinition(selectQuery, cancellationToken: token));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting Car from the database");
            throw;
        }

    }

    public async Task<Car> GetCarAsync(int id, CancellationToken token)
    {
        var selectQuery = "SELECT TOP 1 * FROM [dbo].[Cars] WHERE Id = @id";

        try
        {
            using var connection = _context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            var result = await connection.QueryFirstOrDefaultAsync<Car>(new CommandDefinition(selectQuery, new { Id = id }, cancellationToken: token));
            return result;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting Cars from the database");
            throw;
        }
    }

    public async Task<Car> UpdateAsync(Car car, CancellationToken token)
    {
        var updateQuery = "UPDATE [dbo].[Cars] SET Name = @Name Where Id = @id";

        try
        {
            using var connection = _context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            var result = await connection.ExecuteAsync(new CommandDefinition(updateQuery, new { car.Id, car.Name }, cancellationToken: token));

            // Todo: 
            // In actual case, we should return the updated object from the database as a new udpated resource.
            // 
            return car;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating a row in the database");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken token)
    {
        var deleteQuery = "DELETE FROM [dbo].[Cars] WHERE Id = @id";

        try
        {
            using var connection = _context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            var result = await connection.ExecuteAsync(new CommandDefinition(deleteQuery, new { Id = id }, cancellationToken: token));
            return result > 0;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting a row the database");
            throw;
        }
    }
}
