using FunWithGBFS.Models;
using FunWithGBFS.Services.Users.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FunWithGBFS.Services.Users
{
    public class FileUserService: IUserService
    {
        private readonly string _filePath = "users.json"; //TODO: appsettings for path

        public List<User> GetAllUsers()
        {
            if (!File.Exists(_filePath))
            {
                return new List<User>();
            }

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new();
        }

        public User Register(string username, string password)
        {
            var users = GetAllUsers(); //TODO: when use DB or in-memory storage, should create GetUser method to extract user by username

            if (users.Any(u => u.Username == username))
            {
                throw new Exception("User already exists.");
            }

            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password)
            };

            users.Add(user);
            File.WriteAllText(_filePath, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));
            return user;
        }

        public User? Login(string username, string password)
        {
            var users = GetAllUsers();
            var user = users.FirstOrDefault(u => u.Username == username && u.PasswordHash == HashPassword(password));
            return user;
        }

        public void SaveUser(User user)
        {
            var users = GetAllUsers();
            var index = users.FindIndex(u => u.Username == user.Username);
            if (index != -1)
            {
                users[index] = user;
                File.WriteAllText(_filePath, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        private string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
