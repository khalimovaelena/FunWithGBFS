using FunWithGBFS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Repository.Interfaces
{
    public interface IUserRepository
    {
        User Register(string username, string password);
        User? Login(string username, string password);
        void SaveUser(User user);
        List<User> GetAllUsers();
    }
}
