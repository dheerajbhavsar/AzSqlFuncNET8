using AzSqlFuncNET8.Dapper.Dtos;
using AzSqlFuncNET8.Dapper.Models;
using AzSqlFuncNET8.Dapper.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace AzSqlFuncNET8.Dapper;

public class SQLFunction(ICarsRepository repository, ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<SQLFunction>() ??
        throw new ArgumentNullException(nameof(loggerFactory));

    private readonly ICarsRepository _repository = repository ??
        throw new ArgumentNullException(nameof(repository));

    [Function(nameof(GetAllCars))]
    public async Task<IActionResult> GetAllCars(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "cars")] HttpRequestData req,
        CancellationToken token
    )
    {
        _logger.LogInformation("Executing GetAllCars function.");

        try
        {
            var cars = await _repository.GetAllAsync(token);
            return new OkObjectResult(cars);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing GetAllCars function");
            throw;
        }
        
    }

    [Function(nameof(GetCarById))]
    public async Task<IActionResult> GetCarById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "cars/{id:int}")] HttpRequestData req,
        int id,
        CancellationToken token
    )
    {
        _logger.LogInformation("Executing GetCarById function with {id}.", id);

        try
        {
            var car = await _repository.GetCarAsync(id, token);

            if (car is null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(car);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing GetCarById function");
            throw;
        }
        
    }

    [Function(nameof(DeleteCar))]
    public async Task<IActionResult> DeleteCar(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "cars/{id:int}")] HttpRequestData req,
        int id,
        CancellationToken token
    )
    {
        _logger.LogInformation("Executing DeleteCar function with {id}.", id);

        try
        {
            var car = await _repository.GetCarAsync(id, token);

            if (car is null)
            {
                return new NotFoundResult();
            }

            await _repository.DeleteAsync(id, token);
            return new NoContentResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing DeleteCar function");
            throw;
        }

    }

    [Function(nameof(CreateCar))]
    public async Task<IActionResult> CreateCar(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "cars")] HttpRequestData req,
        [FromBody] AddCarDto car,
        CancellationToken token
    )
    {
        _logger.LogInformation("Executing CreateCar function.");

        try
        {
            var createdCar = await _repository.CreateAsync(new Car { Name = car.Name}, token);
            return new CreatedResult($"{req.Url}/{createdCar.Id}", createdCar);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing CreateCar function");
            throw;
        }

    }

    [Function(nameof(UpdateCar))]
    public async Task<IActionResult> UpdateCar(
        [HttpTrigger(AuthorizationLevel.Function, "PUT", Route = "cars/{id:int}")] HttpRequestData req,
        int id,
        [FromBody] UpdateCarDto car,
        CancellationToken token
    )
    {
        _logger.LogInformation("Executing UpdateCar function.");

        try
        {
            if (car.Id != id)
            {
                _logger.LogInformation("Query parameters {id} and car {carId} do not match. Could not update a resource.", id, car.Id);
                return new BadRequestResult();
            }

            var carFromDb = await _repository.GetCarAsync(id, token);

            if (carFromDb is null)
            {
                _logger.LogInformation("Car {id} does not exist. Could not update a resource.", id);
                return new NotFoundResult();
            }

            var createdCar = await _repository.UpdateAsync(new Car { Id = car.Id, Name = car.Name }, token);
            _logger.LogInformation("Updated a car {id} sucessfully.", id);

            return new CreatedResult(req.Url, createdCar);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing UpdateCar function");
            throw;
        }
    }
    
}
