using Microsoft.EntityFrameworkCore;

namespace BugService.Services;

public class BugService : IBugService
{
    private readonly BugContext _context;

    public BugService(BugContext context)
    {
        _context = context;
    }

    public async Task<BugDTO?> Get(int id)
    {
        return await _context.Bugs.FindAsync(id)
            is Bug bug ? new BugDTO(bug) : null;
    }

    public async Task<IEnumerable<BugDTO>> GetAll()
    {
        return await _context.Bugs.Select(bug => new BugDTO(bug)).ToListAsync();
    }

    public async Task<int> Create(BugDTO bugDTO)
    {
        var bug = new Bug
        {
            Title = bugDTO.Title,
            Description = bugDTO.Description,
            Status = bugDTO.Status
        };

        _context.Bugs.Add(bug);
        await _context.SaveChangesAsync();
        
        return bug.Id;
    }

    public async Task<bool> Update(int id, BugDTO bugDTO)
    {
        var bug = await _context.Bugs.FindAsync(id);
        if (bug == null)
        {
            return false;
        }

        bug.Title = bugDTO.Title ?? bug.Title;
        bug.Description = bugDTO.Description ?? bug.Description;
        bug.Status = bugDTO.Status ?? bug.Status;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(int id)
    {
        var bug = await _context.Bugs.FindAsync(id);
        if (bug == null)
        {
            return false;
        }

        _context.Bugs.Remove(bug);
        await _context.SaveChangesAsync();
        return true;
    }
}
