using UserService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

var app = builder.Build();
app.UseHttpsRedirection();

using(var scope = app.Services.CreateScope())
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
    TypedResults.Created($"/users/{await service.Create(userDTO)}", userDTO)
);

user.MapPut("/{id}", async Task<Results<Ok, NotFound>> (IUserService service, int id, [FromBody] UserDTO userDTO) =>
    await service.Update(id, userDTO)
        ? TypedResults.Ok()
        : TypedResults.NotFound()
);

user.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (IUserService service, int id) =>
    await service.Delete(id)
        ? TypedResults.Ok()
        : TypedResults.NotFound()
);

app.Run();