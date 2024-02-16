using Microsoft.EntityFrameworkCore;
using MassTransit;
using BugTracker.Contracts;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly UserContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public UserService(UserContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<UserDTO?> Get(int id)
    {
        return await _context.Users.FindAsync(id)
            is User user ? new UserDTO(user) : null;
    }

    public async Task<IEnumerable<UserDTO>> GetAll()
    {
        return await _context.Users.Select(user => new UserDTO(user)).ToListAsync();
    }

    public async Task<UserDTO> Create(UserDTO userDTO)
    {
        var user = new User
        {
            Name = userDTO.Name,
            Email = userDTO.Email,
            Department = userDTO.Department
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDTO(user);
    }

    public async Task<UserDTO> Update(int id, UserDTO userDTO)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return userDTO;
        }

        user.Name = userDTO.Name ?? user.Name;
        user.Email = userDTO.Email ?? user.Email;
        user.Department = userDTO.Department ?? user.Department;

        await _context.SaveChangesAsync();
        
        return new UserDTO(user);
    }

    public async Task<bool> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        await _publishEndpoint.Publish<UserDeletedMessage>(new { Id = id });

        return true;
    }
}