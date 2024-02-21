
using BugService.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using MassTransit;
using MassTransit.Metadata;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BugContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBugService, BugService.Services.BugService>();
builder.Services.AddHttpClient<UserClient>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddMassTransit(options =>
{
    options.SetKebabCaseEndpointNameFormatter();

    var assembly = Assembly.GetEntryAssembly();
    options.AddConsumers(assembly);

    options.UsingRabbitMq((context, config) =>
    {
        config.Host(HostMetadataCache.IsRunningInContainer ? "rabbitmq" : "localhost", "/");
        config.ConfigureEndpoints(context);
    });
});

var app = builder.Build();
app.UseCors();
app.UseHttpsRedirection();

using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<BugContext>();
    context.Database.Migrate();
}

var bugs = app.MapGroup("/bugs");

app.MapGet("/health", () => TypedResults.Ok("Healthy"));

bugs.MapGet("/", async (IBugService service) =>
    TypedResults.Ok(await service.GetAll())
);

bugs.MapGet("/{id}", async Task<Results<Ok<BugDTO>, NotFound>> (IBugService service, int id) =>
    await service.Get(id) is BugDTO bugDTO
        ? TypedResults.Ok(bugDTO)
        : TypedResults.NotFound()
);

bugs.MapPost("/", async (IBugService service, [FromBody] BugDTO bugDTO) => {
    var result = await service.Create(bugDTO);
    return TypedResults.Created($"/bugs/{result.Id}", result);
});

bugs.MapPost("/{id}/assign/{userId}", async Task<Results<Ok, NotFound>> (IBugService service, int id, int userId) =>
    await service.AssignBug(id, userId)
        ? TypedResults.Ok()
        : TypedResults.NotFound()
);

bugs.MapPost("/{id}/unassign", async Task<Results<Ok, NotFound>> (IBugService service, int id) =>
    await service.UnassignBug(id)
        ? TypedResults.Ok()
        : TypedResults.NotFound()
);

bugs.MapPut("/{id}", async Task<Results<Ok<BugDTO>, NotFound>> (IBugService service, int id, [FromBody] BugDTO bugDTO) => {
    if(!(await service.Get(id) is BugDTO))
        return TypedResults.NotFound();
    
    var result = await service.Update(id, bugDTO);
    return TypedResults.Ok(result);
});

bugs.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (IBugService service, int id) =>
    await service.Delete(id)
        ? TypedResults.Ok()
        : TypedResults.NotFound()
);

app.Run();
