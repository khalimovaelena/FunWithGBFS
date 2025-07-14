using FunWithGBFS.Models;
using FunWithGBFS.Repository;
using FunWithGBFS.Repository.Interfaces;
using FunWithGBFS.Services.Users.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Services.Users
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
