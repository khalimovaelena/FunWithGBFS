using FunWithGBFS.Application.Users.Interfaces;
using FunWithGBFS.Core.Models;
using FunWithGBFS.Presentation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Application.Users
{
    public class UserSessionService
    {
        private readonly IUserService _userService;
        private readonly IUserInteraction _interaction;

        public UserSessionService(IUserService userService, IUserInteraction interaction)
        {
            _userService = userService;
            _interaction = interaction;
        }

        public User GetOrCreateUser()
        {
            User? user = null;
            while (user == null)
            {
                _interaction.ShowMessage("Choose an option for user:\n1. Register\n2. Login");
                var choice = _interaction.Ask("");

                var username = _interaction.Ask("Username: ");
                var password = _interaction.Ask("Password: ");

                try
                {
                    user = choice == "1" //TODO: enum for options
                        ? _userService.Register(username, password)
                        : _userService.Login(username, password);

                    if (user == null)
                    {
                        _interaction.ShowWarning("Login failed.");
                    }
                }
                catch (Exception ex)
                {
                    _interaction.ShowError($"Error: {ex.Message}");
                }
            }

            return user;
        }
    }

}
