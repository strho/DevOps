using Microsoft.EntityFrameworkCore;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly UserContext _context;

    public UserService(UserContext context)
    {
        _context = context;
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

    public async Task<int> Create(UserDTO userDTO)
    {
        var user = new User
        {
            Name = userDTO.Name,
            Email = userDTO.Email,
            Department = userDTO.Department
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user.Id;
    }

    public async Task<bool> Update(int id, UserDTO userDTO)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        user.Name = userDTO.Name ?? user.Name;
        user.Email = userDTO.Email ?? user.Email;
        user.Department = userDTO.Department ?? user.Department;

        await _context.SaveChangesAsync();
        return true;
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
        return true;
    }
}