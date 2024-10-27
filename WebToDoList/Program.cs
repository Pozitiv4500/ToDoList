using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов в контейнер.
builder.Services.AddControllers();

builder.Services.AddDbContext<TodoContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TodoContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    int maxRetries = 5;
    int delay = 2000;

    for (int retry = 0; retry < maxRetries; retry++)
    {
        try
        {
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Миграции успешно применены.");
            break; 
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Не удалось применить миграции. Попытка {Retry} из {MaxRetries}.", retry + 1, maxRetries);
            if (retry == maxRetries - 1)
            {
                throw; 
            }
            await Task.Delay(delay);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();