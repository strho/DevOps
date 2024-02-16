using MassTransit;
using BugTracker.Contracts;
using BugService.Services;

public class UserDeletedConsumer : IConsumer<UserDeletedMessage>
{
    private readonly ILogger<UserDeletedConsumer> _logger;
    private readonly IBugService _service;

    public UserDeletedConsumer(IBugService service, ILogger<UserDeletedConsumer> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserDeletedMessage> context)
    {
        _logger.LogInformation("User deleted: {UserId}", context.Message.Id);
        
        await _service.UnassignFromUser(context.Message.Id);
    }
}