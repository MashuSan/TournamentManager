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
        public bool Competitive { get; set; }
        public bool HasPrizeMoney { get; set; }
        public bool HasPlayerLimit { get; set; }
        public int PlayerLimit { get; set; }

    }
}
