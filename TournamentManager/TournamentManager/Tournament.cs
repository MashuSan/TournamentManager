using System;
using System.Collections.Generic;
using System.Linq;
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
                IsSetupCompleted = true;
            }
        }

        public void SetPlayers()
        {

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
