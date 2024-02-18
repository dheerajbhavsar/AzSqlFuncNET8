using AzSqlFuncNET8.Dapper.Models;

namespace AzSqlFuncNET8.Dapper.Repositories;

public interface ICarsRepository
{
    Task<IEnumerable<Car>> GetAllAsync(CancellationToken token);
    Task<Car> GetCarAsync(int id, CancellationToken token);

    Task<Car> CreateAsync(Car car, CancellationToken token);
    Task<Car> UpdateAsync(Car car, CancellationToken token);
    Task<bool> DeleteAsync(int id, CancellationToken token);
}
