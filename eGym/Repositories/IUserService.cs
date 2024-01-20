using eGym.Models;
using Microsoft.AspNetCore.Http;

namespace eGym.Repositories
{
    public interface IUserService
    {
        Task<StatusModel> LoginAsync(LoginModel model);
        Task LogoutAsync();
        Task<StatusModel> RegisterAsync(RegisterModel model);
        bool Update(UserModel model);
        Task<UserModel> GetById(string id);
        Task<bool> Delete(string id);
        Task<StatusModel> Add(RegisterModel model);
    }
}
