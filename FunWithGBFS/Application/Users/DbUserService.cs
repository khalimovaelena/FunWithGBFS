using FunWithGBFS.Application.Users.Interfaces;
using FunWithGBFS.Domain.Models;
using FunWithGBFS.Persistence.Repository.Interfaces;

namespace FunWithGBFS.Application.Users
{
    public class DbUserService: IUserService
    {
        private readonly IUserRepository _userRepository;

        public DbUserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User Register(string username, string password)
        {
            return _userRepository.Register(username, password);
        }

        public User? Login(string username, string password)
        {
            return _userRepository.Login(username, password);
        }

        public void SaveUser(User user)
        {
            _userRepository.SaveUser(user);
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }
    }
}
