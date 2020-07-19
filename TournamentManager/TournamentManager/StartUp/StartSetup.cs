using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TournamentManager
{
    public class StartSetup
    {
        private List<Tournament> _tournaments;
        private List<Player> _players;
        private Task loadingTournaments;
        private Task loadingPlayers;
        private Task export;
        private int result;
        private int errorMax = 0;
        private int lastRank;
        public StartSetup()
        {
            _tournaments = new List<Tournament>();
            _players = new List<Player>();
        }

        private void printUserCases()
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
            LoadPlayers();
            LoadTournaments();
            Main();

        }

        private void Main()
        {
            if (!loadingTournaments.IsCompleted)
            {
                Console.WriteLine("Waiting for data import completion..");
                loadingTournaments.Wait();
            }

            while (result != (int)UserCases.Shut_down)
            {
                switch ((UserCases)result)
                {
                    case UserCases.Create_new_tournament:
                        CreateNewTournament();
                        break;

                    case UserCases.Show_tournament:
                        SelectionOfTournament("Show");
                        break;

                    case UserCases.Show_actual_ranks:
                        break;

                    case UserCases.Edit_existing_tournament:
                        SelectionOfTournament("Edit");
                        break;

                    case UserCases.Add_player:
                        AddPlayer();
                        break;

                    default:
                        break;
                }
                printUserCases();
                bool readKey = int.TryParse(Console.ReadLine(), out result);
            }

        }

        private void LoadTournaments()
        {

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Exported", "Tournaments.txt");
            string line = null;
            if (!loadingPlayers.IsCompleted)
            {
                loadingPlayers.Wait();
            }

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

                            var playersLine = reader.ReadLine();

                            foreach (var player in playersLine.Split(';'))
                            {
                                var getPlayer = new Player();
                                var playerInfo = player.Split('|');

                                if (playerInfo.Length <= 1) 
                                    continue;

                                getPlayer.InGameName = playerInfo[0];
                                getPlayer.Name = playerInfo[1];
                                getPlayer.Surname = playerInfo[2];
                                getPlayer.KDA = double.Parse(playerInfo[3]);
                                getPlayer.Rank = int.Parse(playerInfo[4]);
                                getPlayer.Division = (Divisions)Enum.Parse(typeof(Divisions), playerInfo[5]);
                                getPlayer.Team = playerInfo[6];
                                existingTournament.Players.Add(getPlayer);
                            }
                            
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
            string line = null;
            var players = new List<Player>();
            try
            {
                loadingPlayers = Task.Run(() =>
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        while (!reader.EndOfStream)
                        {
                            line = reader.ReadLine();
                            if (line == "")
                                continue;
                            var existingPlayer = new Player();
                            existingPlayer.Name = line;
                            existingPlayer.Surname = reader.ReadLine();
                            existingPlayer.InGameName = reader.ReadLine();
                            existingPlayer.KDA = double.Parse(reader.ReadLine());
                            existingPlayer.Rank = int.Parse(reader.ReadLine());
                            existingPlayer.Division = (Divisions)Enum.Parse(typeof(Divisions), reader.ReadLine());
                            existingPlayer.Team = reader.ReadLine();

                            players.Add(existingPlayer);
                        }
                        
                    }
                    _players = players;
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
            AddPlayersToTournament(newTournament);
            newTournament.InitializeMatchesOfTournament();
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
        //TODO : WAIT FOR EXPORTS TO MAKE GOOD LOGIC
        private void ExportTournament(Tournament tournament)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Exported", "Tournaments.txt");
            try
            {
                export = Task.Run(() =>
                {
                    using (StreamWriter writer = File.AppendText(path))
                    {
                        writer.WriteLine(tournament.Name);
                        writer.WriteLine(tournament.IsCompetitive);
                        writer.WriteLine(tournament.HasPrizeMoney);
                        writer.WriteLine(tournament.HasPlayerLimit);
                        writer.WriteLine(tournament.PlayerLimit);
                        foreach (var player in tournament.Players)
                        {
                            writer.Write(player.InGameName + "|");
                            writer.Write(player.Name + "|" + player.Surname + "|");
                            writer.Write(player.KDA + "|");
                            writer.Write(player.Rank + "|");
                            writer.Write(player.Division + "|");
                            writer.Write(player.Team + ";");
                        }
                        writer.WriteLine();
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occuret during IO event, trying again..");
                ExportTournament(tournament);
            }
        }

        private void AddPlayersToTournament(Tournament tournament)
        {
            string input = " ";
            Task findTask = null;
            while(true)
            {
                if (findTask != null && !findTask.IsCompleted)
                {
                    Console.WriteLine("Waiting for validation..");
                    findTask.Wait();
                }

                if (tournament.PlayerLimit <= tournament.Players.Count && tournament.PlayerLimit != 0)
                {
                    break;
                }

                Console.WriteLine("Enter players in game name or enter empty line for end : ");
                input = Console.ReadLine();

                if (input == "")
                {
                    break;
                }

                findTask = Task.Run(() =>
                {
                    bool found = false;
                    foreach(var player in _players)
                    {
                        if (player.InGameName.Equals(input))
                        {
                            tournament.Players.Add(player);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        var newPlayer = CreatePlayer(input);
                        tournament.Players.Add(newPlayer);
                        ExportPlayer(newPlayer);
                    }
                });

            }

        }

        public void AddPlayer()
        {
            Console.WriteLine("Enter players in game name : ");
            string input = Console.ReadLine();

            bool exists = false;
            Console.WriteLine("Checking for validation...");
            var find = Task.Run(() =>
            {
                foreach(var player in _players)
                {
                    if (player.InGameName == input)
                    {
                        Console.WriteLine("Player with this in game name exists! Choose another name!");
                        exists = true;
                    }
                }
            });

            find.Wait();
            if (exists)
            {
                AddPlayer();
                return;
            }

            var newPlayer = CreatePlayer(input);
            ExportPlayer(newPlayer);
            _players.Add(newPlayer);
        }

        private Player CreatePlayer(string inGameName)
        {
            var newPlayer = new Player();
            Console.WriteLine("Write down players real name : ");
            newPlayer.Name = Console.ReadLine();
            Console.WriteLine("Write down players real surname : ");
            newPlayer.Surname = Console.ReadLine();
            newPlayer.InGameName = inGameName;
            newPlayer.Rank = lastRank;
            Console.WriteLine("Write down players team name or \"-\" for no team : ");
            newPlayer.Team = Console.ReadLine();

            return newPlayer;
        }

        private void ExportPlayer(Player player)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Exported", "Players.txt");
            try
            {
                export = Task.Run(() =>
                {
                    using (StreamWriter writer = File.AppendText(path))
                    {
                        writer.WriteLine(player.Name);
                        writer.WriteLine(player.Surname);
                        writer.WriteLine(player.InGameName);
                        writer.WriteLine(player.KDA);
                        writer.WriteLine(player.Rank);
                        writer.WriteLine(player.Division);
                        writer.WriteLine(player.Team);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occuret during IO event, trying again..");
                ExportPlayer(player);
            }
        }

        private void EditTournament(Tournament tournament)
        {
            Console.Clear();
            tournament.EditMatch();
        }

        private void SelectionOfTournament(string key)
        {
            Console.Clear();
            Console.WriteLine("Select your tournament: ");
            foreach (var tournament in _tournaments)
            {
                Console.WriteLine(tournament.Name);
            }

            string selection = Console.ReadLine();
            bool successfulySelected = false;
            foreach (var tournament in _tournaments)
            {
                if (selection.Equals(tournament.Name))
                {
                    if (key == "Show")
                        ShowTournament(tournament);
                    else if (key == "Edit")
                        EditTournament(tournament);
                    successfulySelected = true;
                }
            }
            if (!successfulySelected)
            {
                Console.WriteLine("Tournament with this name doesn't exist");
            }
            Console.WriteLine();
        }

        private void ShowTournament(Tournament tournament)
        {
            Console.Clear();
            Console.WriteLine("Tournament name : " + tournament.Name);
            Console.WriteLine("IsCompetitive : " + tournament.IsCompetitive);
            Console.WriteLine("Has prize money : " + tournament.HasPrizeMoney);
            Console.WriteLine("Has player limit : " + tournament.HasPlayerLimit);
            Console.WriteLine("Player limit : " + tournament.PlayerLimit);
            Console.WriteLine("Player names : ");
            foreach(var player in tournament.Players)//TODO: WRITE THE LOGIC IN TOURNAMENT CLASS
            {
                Console.WriteLine(player.InGameName);
            }
            Console.WriteLine("Matches : ");
            tournament.PrintMatches();
            if (tournament.isFinished)
            {
                Console.WriteLine("Tournament won by " + tournament.TournamentWinner);
            }

        }
    }
}
