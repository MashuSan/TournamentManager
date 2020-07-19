using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager
{
    public class MatchInfo
    {
        public Player RedPlayer { get; set; }
        public Player BluePlayer { get; set; }

        public Player Winner { get; set; }

        public int RedKills  { get; private set; }
        public int BlueKills { get; private set; }

        public int RedDeaths { get; private set; }
        public int BlueDeaths { get; private set; }
        public bool Won { get; private set; } = false;

        public MatchInfo(Player redPlayer, Player bluePlayer)
        {
            RedPlayer = redPlayer;
            BluePlayer = bluePlayer;

            RedKills = 0;
            RedDeaths = 0;
            BlueKills = 0;
            BlueDeaths = 0;

            Winner = null;
        }

        public MatchInfo(Player redPlayer)
        {
            Winner = redPlayer;
            Won = true;
            RedPlayer = redPlayer;
            BluePlayer = new Player();
            BluePlayer.InGameName = "No player";
        }

        public override string ToString()
        {
            if (BluePlayer.InGameName == "No player")
            {
                return RedPlayer + " won match by default";
            }

            if (Won)
            {
                return (RedPlayer + " vs " + BluePlayer + " = " + Winner + " won. ");
            }

            return (RedPlayer + " vs " + BluePlayer);
        }

        private void EnterKillsAndDeaths(string playerSide)
        {
            Console.WriteLine("Enter kills and Deaths for " + playerSide + "Player : ");

            Console.WriteLine("Kills : ");
            int resultKills = 0;
            while (!int.TryParse(Console.ReadLine(), out resultKills)) { }

            Console.WriteLine("Deaths : ");
            int resultDeaths = 0;
            while (!int.TryParse(Console.ReadLine(), out resultDeaths)) { }

            if (playerSide == "Red")
            {
                RedKills = resultKills;
                RedDeaths = resultDeaths;
            }
            else if (playerSide == "Blue")
            {
                BlueKills = resultKills;
                BlueDeaths = resultDeaths;
            }

        }

        public void WhoWon()
        {
            Console.WriteLine("Who won? Red/Blue");

            string input = "";
            while (input != "Red" && input != "Blue")
            {
                input = Console.ReadLine();
            }

            if (input == "Red")
            {
                Winner = RedPlayer;
            }
            else
            {
                Winner = BluePlayer;
            }

            Won = true;

            EnterKillsAndDeaths("Red");
            EnterKillsAndDeaths("Blue");
        }
    }
}
