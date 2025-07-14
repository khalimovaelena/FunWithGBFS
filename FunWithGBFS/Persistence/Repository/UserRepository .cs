using FunWithGBFS.Core.Models;
using FunWithGBFS.Persistence.Context;
using FunWithGBFS.Persistence.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Persistence.Repository
{
    public class UserRepository: IUserRepository
    {
        private readonly GameDbContext _dbContext;

        public UserRepository(GameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User Register(string username, string password)
        {
            if (_dbContext.Users.Any(u => u.Username == username))
            {
                throw new Exception("Username already exists");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return user;
        }

        public User? Login(string username, string password)
        {
            var user = _dbContext.Users
                .Include(u => u.Attempts)
                .FirstOrDefault(u => u.Username == username);

            if (user == null)
                return null;

            // Verify hashed password using BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            return isPasswordValid ? user : null;
        }

        public List<User> GetAllUsers()
        {
            return _dbContext.Users.Include(u => u.Attempts).ToList();
        }

        public void SaveUser(User user)
        {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
        }
    }
}
