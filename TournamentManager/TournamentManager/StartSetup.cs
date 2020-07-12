using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TournamentManager
{
    public class StartSetup
    {
        private List<Tournament> _tournaments;
        private Task loadingTournaments;
        private Task loadingPlayers;
        private int result;
        private int errorMax = 0;
        public StartSetup()
        {
            _tournaments = new List<Tournament>(); 
        }

        private void printCases()
        {
            for (int i = 1; i < Enum.GetValues(typeof(UserCases)).Length; i++)
            {
                Console.WriteLine(i + " = " +
                                   ((UserCases)i).ToString().Replace('_', ' '));
            }
        }

        public void Begin()
        {
            Console.WriteLine("Enter a number on your keyboard of the following : \n");
            LoadTournaments();
            //LoadPlayers();
            Main();

        }

        private void Main()
        {
            while (result != (int)UserCases.Shut_down)
            {
                switch ((UserCases)result)
                {
                    case UserCases.Create_new_tournament:
                        CreateNewTournament();
                        break;

                    case UserCases.Show_tournament:
                        ShowTournament();
                        break;
                }
                printCases();
                bool readKey = int.TryParse(Console.ReadLine(), out result);
            }

        }

        private void LoadTournaments()
        {

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Exported", "Tournaments.txt");
            string line = null;

            loadingTournaments = Task.Run(() =>
            {
                var tournaments = new List<Tournament>();
                try
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        while (!reader.EndOfStream)
                        {
                            line = reader.ReadLine();
                            if (line == "")
                                continue;
                            var existingTournament = new Tournament(line);
                            existingTournament.IsCompetitive = bool.Parse(reader.ReadLine());
                            existingTournament.HasPrizeMoney = bool.Parse(reader.ReadLine());
                            existingTournament.HasPlayerLimit = bool.Parse(reader.ReadLine());
                            existingTournament.PlayerLimit = int.Parse(reader.ReadLine());
                            tournaments.Add(existingTournament);
                        }
                    }
                }catch(Exception ex)
                {
                    Console.WriteLine("The import file doesn't exists or there is an error reading it");
                    if (errorMax < 3)
                    {
                        errorMax += 1;
                        LoadTournaments();
                    }
                    else
                    {
                        Console.WriteLine("The import file doesn't exists or there is an error reading it, shutting down the program");
                        result = (int)UserCases.Shut_down;
                    }
                }
                _tournaments = tournaments;
            });

        }

        private void LoadPlayers()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Exported", "Players.txt");
            List<string> strings = new List<string>();
            try
            {
                    loadingPlayers = Task.Run(() =>
                    {
                        using (StreamReader reader = new StreamReader(path))
                        {
                            while (!reader.EndOfStream)
                            {
                                strings.Add(reader.ReadLine());
                            }
                            //TODO : convert the players to objects
                        }
                    });

            }
            catch (Exception ex)
            {
                Console.WriteLine("The export file doesn't exists or there is an error reading it");
                if (errorMax < 3)
                {
                    errorMax += 1;
                    LoadTournaments();
                }
                else
                {
                    Console.WriteLine("The import file doesn't exist or there is an error reading it, shutting down the program");
                    result = (int)UserCases.Shut_down;
                }
            }
        }

        private void CreateNewTournament()
        {
            Tournament newTournament = new Tournament(SetTournamentName());
            newTournament.SetSettings();
            AddTournamentToCollection(newTournament);
        }

        private string SetTournamentName()
        {
            string name;
            Console.WriteLine("Set name of a tournament : ");
            try
            {
                name = Console.ReadLine();

                Console.WriteLine("Checking name availability");

                foreach (var tournamentName in _tournaments)
                {
                    if (tournamentName.Name.Equals(name))
                    {
                        throw new Exception();
                    }
                }

                return name;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Name taken or IO error!");
                return SetTournamentName();
            }
        }

        private void AddTournamentToCollection(Tournament tournament)
        {
            _tournaments.Add(tournament);
            ExportTournament(tournament);
        }

        private void ExportTournament(Tournament tournament)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Exported", "Tournaments.txt");
            try
            {
                using (StreamWriter writer = File.AppendText(path))
                {
                    writer.WriteLine(tournament.Name);
                    writer.WriteLine(tournament.IsCompetitive);
                    writer.WriteLine(tournament.HasPrizeMoney);
                    writer.WriteLine(tournament.HasPlayerLimit);
                    writer.WriteLine(tournament.PlayerLimit);
                    writer.WriteLine();
                    foreach (var player in tournament.Players)
                    {
                        writer.Write(player.ID + " ; ");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occuret during IO event, trying again..");
                ExportTournament(tournament);
            }
        }

        private void ShowTournament()
        {
            Console.WriteLine("Select your tournament to display info of : ");
            foreach(var tournament in _tournaments)
            {
                Console.WriteLine(tournament.Name);
            }

            string selection = Console.ReadLine();
            bool successfulySelected = false;
            foreach(var tournament in _tournaments)
            {
                if (selection.Equals(tournament.Name))
                {
                    successfulySelected = true;
                    Console.WriteLine("Tournament name : " + tournament.Name);
                    Console.WriteLine("IsCompetitive : " + tournament.IsCompetitive);
                    Console.WriteLine("Has prize money : " + tournament.HasPrizeMoney);
                    Console.WriteLine("Has player limit : " + tournament.HasPlayerLimit);
                    Console.WriteLine("Player limit : " + tournament.PlayerLimit);
                    Console.WriteLine("Player names : ");
                    foreach(var player in tournament.Players)
                    {
                        Console.WriteLine(player.Name);
                    }
                }
            }
            if (!successfulySelected)
            {
                Console.WriteLine("Tournament with this name doesn't exist");
            }

        }
    }
}
