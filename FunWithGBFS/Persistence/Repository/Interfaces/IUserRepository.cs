using FunWithGBFS.Core.Models;

namespace FunWithGBFS.Persistence.Repository.Interfaces
{
    public interface IUserRepository
    {
        User Register(string username, string password);
        User? Login(string username, string password);
        void SaveUser(User user);
        List<User> GetAllUsers();
    }
}
