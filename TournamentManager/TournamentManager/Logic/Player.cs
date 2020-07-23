using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager
{
    public class Player
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string InGameName { get; set; }
        public double KDA { get; set; }
        public int Rank { get; set; }
        public Divisions Division { get; set; }
        public string Team { get; set; }
        public int WonTournaments { get; set; }

        public int Kills
        {
            get
            {
                return _kills;
            }

            set
            {
                _kills += value;
                ReworkKDA();
            }
        }

        public int Deaths
        {
            get
            {
                return _deaths;
            }

            set
            {
                _deaths += value;
                ReworkKDA();
            }
        }

        private int _kills;
        private int _deaths;

        public Player()
        {
            Division = Divisions.Unranked;
            WonTournaments = 0;
        }

        public override string ToString()
        {
            return InGameName;
        }

        private void ReworkKDA()
        {
            if (_deaths == 0)
            {
                KDA = _kills;
            }
            else
            {
                KDA = Math.Round((double)_kills / (double)_deaths, 2);
            }
        }

        public void ShowInfo()
        {
            Console.WriteLine("In game name = " + InGameName);
            Console.WriteLine("Name = " + Name);
            Console.WriteLine("Surname = " + Surname);
            Console.WriteLine("KDA = " + KDA);
            Console.WriteLine("Kills = " + Kills);
            Console.WriteLine("Deaths = " + Deaths);
            Console.WriteLine("Rank = " + Rank);
            Console.WriteLine("Won Tournaments = " + WonTournaments);
            Console.WriteLine("Division = " + Division);
            Console.WriteLine("Team = " + Team);
        }
    }
}
