using FunWithGBFS.Presentation.Interfaces;

namespace FunWithGBFS.Presentation.Console
{
    public class ConsoleUserInteraction : IUserInteraction
    {
        public void ShowMessage(string message)
        {
            System.Console.WriteLine(message);
        }

        public void ShowError(string message)
        {
            System.Console.ForegroundColor = System.ConsoleColor.Red;
            System.Console.WriteLine($"[ERROR]: {message}");
            System.Console.ResetColor();
        }

        public void ShowWarning(string message)
        {
            System.Console.ForegroundColor = System.ConsoleColor.Yellow;
            System.Console.WriteLine($"[WARNING]: {message}");
            System.Console.ResetColor();
        }

        public void ClearScreen()
        {
            System.Console.Clear();
        }

        public string Ask(string prompt)
        {
            System.Console.Write(prompt);
            return System.Console.ReadLine() ?? string.Empty;
        }

        public void Pause(string message = "Press any key to continue...")
        {
            System.Console.WriteLine(message);
            System.Console.ReadKey();
        }
    }
}