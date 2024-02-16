namespace BugService.Services;

public interface IBugService
{
    Task<BugDTO?> Get(int id);
    
    Task<IEnumerable<BugDTO>> GetAll();
    
    Task<BugDTO> Create(BugDTO bugDTO);
    
    Task<BugDTO> Update(int id, BugDTO bugDTO);
    
    Task<bool> Delete(int id);

    Task<bool> AssignBug(int bugId, int userId);

    Task<bool> UnassignBug(int bugId);

    Task UnassignFromUser(int userId);
}
