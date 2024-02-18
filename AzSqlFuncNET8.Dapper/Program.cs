using AzSqlFuncNET8.Dapper.Data;
using AzSqlFuncNET8.Dapper.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<DapperContext>();
        services.AddScoped<ICarsRepository, CarsRepository>();
    })
    .Build();

host.Run();
