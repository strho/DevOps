using UserService.Services; 

public interface IUserService {
    Task<UserDTO?> Get(int id);

    Task<IEnumerable<UserDTO>> GetAll();
    
    Task<int> Create(UserDTO userDTO);
    
    Task<bool> Update(int id, UserDTO userDTO);
    
    Task<bool> Delete(int id);
}