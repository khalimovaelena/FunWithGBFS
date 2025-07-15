using FunWithGBFS.Core.Models;

namespace FunWithGBFS.Application.Users.Interfaces
{
    public interface IUserService
    {
        User Register(string username, string password);
        User? Login(string username, string password);
        void SaveUser(User user);
        List<User> GetAllUsers();
    }
}
