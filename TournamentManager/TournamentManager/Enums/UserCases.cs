using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager
{
    public enum UserCases
    {
        Invalid_case,

        Create_new_tournament,
        Edit_existing_tournament,
        Show_tournament,
        Show_actual_ranks,
        Add_player,
        Show_player_info,

        Save_and_shut_down
    }
}
