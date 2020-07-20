using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private void PrintUserCases()
        {
            for (int i = 1; i < Enum.GetValues(typeof(UserCases)).Length; i++)
            {
                Console.WriteLine(i + " = " +
                                   ((UserCases)i).ToString().Replace('_', ' '));
            }
        }

        public void Begin()
        {
            LoadPlayers();
            LoadTournaments();

            Console.WriteLine("Enter a number on your keyboard of the following : \n");
            Main();
        }

        private void Main()
        {
            if (!loadingTournaments.IsCompleted)
            {
                Console.WriteLine("Waiting for data import completion..");
                loadingTournaments.Wait();
                Console.Clear();
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
                PrintUserCases();
                bool readKey = int.TryParse(Console.ReadLine(), out result);
            }

        }

        private void LoadTournaments()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Exported", "Tournaments.txt");
            string line = null;
            if (!loadingPlayers.IsCompleted)
            {
                Console.WriteLine("Waiting for player to import..");
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
                            existingTournament.PrizeMoney = int.Parse(reader.ReadLine());
                            existingTournament.HasPlayerLimit = bool.Parse(reader.ReadLine());
                            existingTournament.PlayerLimit = int.Parse(reader.ReadLine());
                            existingTournament.isFinished = bool.Parse(reader.ReadLine());
                            existingTournament.IsGroupMatches = bool.Parse(reader.ReadLine());
                            existingTournament.NumberOfSides = int.Parse(reader.ReadLine());
                            var numberOfMatchesPlayed = int.Parse(reader.ReadLine());
                            existingTournament.SetLastMatchAddedPos(int.Parse(reader.ReadLine()));

                            var getLine = reader.ReadLine();

                            foreach (var player in getLine.Split(';'))
                            {
                                Player getPlayer = new Player();
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

                            getLine = reader.ReadLine();

                            foreach (var match in getLine.Split(';'))
                            {
                                var matchInfo = match.Split('|');
                                MatchInfo getMatch;

                                if (matchInfo.Length <= 1)
                                    continue;

                                string redPlayer = matchInfo[0];
                                string bluePlayer = matchInfo[1];

                                if (bluePlayer == "No player")
                                {
                                    getMatch = new MatchInfo(existingTournament.FindPlayer(redPlayer));
                                }
                                else
                                {
                                    getMatch = new MatchInfo(existingTournament.FindPlayer(redPlayer), existingTournament.FindPlayer(bluePlayer));
                                }

                                getMatch.RedKills = int.Parse(matchInfo[2]);
                                getMatch.RedDeaths = int.Parse(matchInfo[3]);
                                getMatch.BlueKills = int.Parse(matchInfo[4]);
                                getMatch.BlueDeaths = int.Parse(matchInfo[5]);
                                getMatch.Won = bool.Parse(matchInfo[6]);
                                if (getMatch.Won)
                                    getMatch.Winner = existingTournament.FindPlayer(matchInfo[7]);

                                existingTournament.Matches.Add(getMatch);
                            }

                            int groupPos = -1;
                            while (!(getLine = reader.ReadLine()).Equals(";"))
                            {
                                groupPos++;
                                existingTournament.ListsOfDividedPlayers.Add(new List<Player>());
                                var playersInGroup = getLine.Split('|');
                                foreach(var playerInGameName in playersInGroup)
                                {
                                    if (playerInGameName.Equals(""))
                                        continue;

                                    existingTournament.ListsOfDividedPlayers[groupPos].Add(existingTournament.FindPlayer(playerInGameName));
                                }
                            }

                            existingTournament.SetNumberOfMatchesPlayed(numberOfMatchesPlayed);

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
                        Console.WriteLine("The import file doesn't exists or there is an error reading it," +
                            " there won't be access to history of matches ");
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
                    Console.WriteLine("The import file doesn't exists or there is an error reading it," +
                            " there won't be access to history of players ");
                }
            }
        }

        private Player FindPlayer(string inGameName)
        {
            foreach(var player in _players)
            {
                if (player.InGameName == inGameName)
                {
                    return player;
                }
            }
            Console.WriteLine("No player with " + inGameName + " in database. Enter players info below : ");
            return CreatePlayer(inGameName);
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
                        writer.WriteLine(tournament.PrizeMoney);
                        writer.WriteLine(tournament.HasPlayerLimit);
                        writer.WriteLine(tournament.PlayerLimit);
                        writer.WriteLine(tournament.isFinished);
                        writer.WriteLine(tournament.IsGroupMatches);
                        writer.WriteLine(tournament.NumberOfSides);
                        writer.WriteLine(tournament.GetNumberOfMatchesPlayed());
                        writer.WriteLine(tournament.GetLastMatchAddedPos());
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
                        foreach (var match in tournament.Matches)
                        {
                            writer.Write(match.RedPlayer.InGameName + "|");
                            writer.Write(match.BluePlayer.InGameName + "|");
                            writer.Write(match.RedKills + "|");
                            writer.Write(match.RedDeaths + "|");                           
                            writer.Write(match.BlueKills + "|");
                            writer.Write(match.BlueDeaths + "|");
                            writer.Write(match.Won + "|");
                            writer.Write(match.Winner + ";");
                        }
                        writer.WriteLine();
                        foreach(var group in tournament.ListsOfDividedPlayers)
                        {
                            foreach(var player in group)
                            {
                                writer.Write(player.InGameName + "|");
                            }
                            writer.WriteLine();
                        }
                        writer.Write(";");
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
            if (!loadingTournaments.IsCompleted)
            {
                Console.WriteLine("Waiting for importing all tournaments..");
                loadingTournaments.Wait();
            }

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
                    {
                        ShowTournament(tournament);
                    }
                    else if (key == "Edit")
                    {
                        EditTournament(tournament);
                        ExportTournament(tournament); //TODO : BIG - FIND AND REPLACE!! REDO REXPORT TO EDIT/ADD (rewrite, append)
                    }
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
