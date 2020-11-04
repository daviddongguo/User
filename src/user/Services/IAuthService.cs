using System.Linq;
using System.Threading.Tasks;
using user.Models;

namespace user.Services
{
    public interface IAuthService
    {
        string CreateToken(User user);
        Task<IQueryable<User>> GetAllUsers(int pageSize = 5, int page = 1);
        Task<User> GetUserByEmail(string email);
        Task<bool> IsEmailExisted(string email);
        Task Delete(string id);
        Task<ServiceResponse<string>> Login(string email, string password);
        Task<ServiceResponse<string>> Register(User toSaveUser, string password);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}
