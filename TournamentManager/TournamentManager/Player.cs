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
        public int Score { get; set; }
        public int Rank { get; set; }
        public Divisions Division { get; set; }
        public string Team { get; set; }


    }
}
