
using BugService.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BugContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBugService, BugService.Services.BugService>();
builder.Services.AddHttpClient<UserClient>();

var app = builder.Build();
app.UseHttpsRedirection();

using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<BugContext>();
    context.Database.Migrate();
}

var bugs = app.MapGroup("/bugs");

bugs.MapGet("/", async (IBugService service) =>
    TypedResults.Ok(await service.GetAll())
);

bugs.MapGet("/{id}", async Task<Results<Ok<BugDTO>, NotFound>> (IBugService service, int id) =>
    await service.Get(id) is BugDTO bugDTO
        ? TypedResults.Ok(bugDTO)
        : TypedResults.NotFound()
);

bugs.MapPost("/", async (IBugService service, [FromBody] BugDTO bugDTO) =>
    TypedResults.Created($"/bugs/{await service.Create(bugDTO)}", bugDTO)
);

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

bugs.MapPut("/{id}", async Task<Results<Ok, NotFound>> (IBugService service, int id, [FromBody] BugDTO bugDTO) =>
    await service.Update(id, bugDTO)
        ? TypedResults.Ok()
        : TypedResults.NotFound()
);

bugs.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (IBugService service, int id) =>
    await service.Delete(id)
        ? TypedResults.Ok()
        : TypedResults.NotFound()
);

app.Run();
