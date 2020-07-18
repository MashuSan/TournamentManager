using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager
{
    public class Tournament
    {
        public string Name { get; set; }
        public bool IsCompetitive { get; set; }
        public bool HasPrizeMoney { get; set; }
        public bool HasPlayerLimit { get; set; }
        public int PlayerLimit { get; set; }
        public List<Player> Players { get; set; }
        public List<MatchInfo> Matches { get; set; }

        private bool IsSetupCompleted = false;

        public Tournament(string name)
        {
            Name = name;
            IsCompetitive = false;
            HasPrizeMoney = false;
            HasPlayerLimit = false;
            PlayerLimit = 0;
            Players = new List<Player>();

        }

        public void SetSettings()
        {
            while (!IsSetupCompleted)
            {
                SetBoolean();
                CheckPlayerLimit();
                SetGroupStyle();
                IsSetupCompleted = true;
            }
        }

        private void PrintTournamentCases()
        {
            for (int i = 1; i < Enum.GetValues(typeof(TournamentCases)).Length; i++)
            {
                Console.WriteLine(i + " = " +
                                   ((TournamentCases)i).ToString().Replace('_', ' '));
            }
        }

        private void SetGroupStyle()
        {
            Console.WriteLine("Choose your prefered tournament style :");
            PrintTournamentCases();
            int result = 0;

            while (result <= 0 || result >= Enum.GetValues(typeof(TournamentCases)).Length)
            {
                bool readKey = int.TryParse(Console.ReadLine(), out result);
                switch ((TournamentCases)result)
                {
                    case TournamentCases.One_group:
                        break;
                    case TournamentCases.Two_groups:
                        break;
                    case TournamentCases.Four_groups:
                        break;
                    case TournamentCases.Sixteen_groups:
                        break;
                    case TournamentCases.Eight_groups:
                        break;
                    case TournamentCases.One_side:
                        break;
                    case TournamentCases.Two_sides:
                        break;

                    default:
                        continue;
                }
            }
        }

        private bool ReadBoolKey()
        {
            bool boolResult;
            var isReadKey = bool.TryParse(Console.ReadLine(), out boolResult);
            while (!isReadKey && (boolResult != true || boolResult != false))
            {
                isReadKey = bool.TryParse(Console.ReadLine(), out boolResult);
            }
            return boolResult;
        }

        private void SetBoolean()
        {
            Console.WriteLine("Is this tournament competitive? false - NO; true - YES");

            IsCompetitive = ReadBoolKey();

            Console.WriteLine("Does this tournament have prize money? false - NO; true - YES");

            HasPrizeMoney = ReadBoolKey();

            Console.WriteLine("Does this tournament have player limit? false - NO; true - YES");

            HasPlayerLimit = ReadBoolKey();
        }

        private void CheckPlayerLimit()
        {
            if (HasPlayerLimit)
            {
                Console.WriteLine("Enter the number of players to compete in this tournament : ");
                int intResult = 0;
                var isReadKey = int.TryParse(Console.ReadLine(), out intResult);
                while (!isReadKey)
                {
                    isReadKey = int.TryParse(Console.ReadLine(), out intResult);
                }

                PlayerLimit = intResult;
            }
        }

    }
}
