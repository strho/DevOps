using UserService.Services;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using MassTransit;
using MassTransit.Metadata;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

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
    options.UsingRabbitMq((context, config) =>
    {
        config.Host(HostMetadataCache.IsRunningInContainer ? "rabbitmq" : "localhost", "/");
        config.ConfigureEndpoints(context);
    });
});

var app = builder.Build();
app.UseCors();
app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<UserContext>();
    context.Database.Migrate();
}

var user = app.MapGroup("/users");

user.MapGet("/", async (IUserService service) =>
    TypedResults.Ok(await service.GetAll())
);

user.MapGet("/{id}", async Task<Results<Ok<UserDTO>, NotFound>> (IUserService service, int id) =>
    await service.Get(id) is UserDTO userDTO
        ? TypedResults.Ok(userDTO)
        : TypedResults.NotFound()
);

user.MapPost("/", async (IUserService service, [FromBody] UserDTO userDTO) =>
{
    var result = await service.Create(userDTO);
    return TypedResults.Created($"/users/{result.Id}", result);
});

user.MapPut("/{id}", async Task<Results<Ok<UserDTO>, NotFound>> (IUserService service, int id, [FromBody] UserDTO userDTO) =>
{
    if (!(await service.Get(id) is UserDTO))
        return TypedResults.NotFound();

    var result = await service.Update(id, userDTO);
    return TypedResults.Ok(result);
});

user.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (IUserService service, int id) =>
    await service.Delete(id)
        ? TypedResults.Ok()
        : TypedResults.NotFound()
);

app.Run();