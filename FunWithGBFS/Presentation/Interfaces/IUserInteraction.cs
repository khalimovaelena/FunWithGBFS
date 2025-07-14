using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Presentation.Interfaces
{
    public interface IUserInteraction
    {
        void ShowMessage(string message);
        void ShowError(string message);
        void ShowWarning(string message);
        void ClearScreen();
        string Ask(string prompt);
        void Pause(string message = "Press any key to continue...");
    }
}
