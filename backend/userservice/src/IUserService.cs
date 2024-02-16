using UserService.Services; 

public interface IUserService {
    Task<UserDTO?> Get(int id);

    Task<IEnumerable<UserDTO>> GetAll();
    
    Task<UserDTO> Create(UserDTO userDTO);
    
    Task<UserDTO> Update(int id, UserDTO userDTO);
    
    Task<bool> Delete(int id);
}