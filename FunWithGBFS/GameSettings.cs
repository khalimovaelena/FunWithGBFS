using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS
{
    public class GameSettings
    {
        public int NumberOfQuestions { get; set; }
        public int GameDurationSeconds { get; set; }

        public int InitialScore { get; set; }
        public int PointsPerCorrectAnswer { get; set; }
        public int PointsPerWrongAnswer { get; set; }

        public string ProvidersFile { get; set; } = "providers.json";

        public string UsersFile { get; set; } = "users.json";
    }
}
