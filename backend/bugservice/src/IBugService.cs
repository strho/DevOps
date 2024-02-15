namespace BugService.Services;

public interface IBugService
{
    Task<BugDTO?> Get(int id);
    
    Task<IEnumerable<BugDTO>> GetAll();
    
    Task<int> Create(BugDTO bugDTO);
    
    Task<bool> Update(int id, BugDTO bugDTO);
    
    Task<bool> Delete(int id);
}
