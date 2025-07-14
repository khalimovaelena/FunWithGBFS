using FunWithGBFS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
