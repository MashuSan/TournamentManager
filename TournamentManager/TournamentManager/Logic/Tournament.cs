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
        public double PrizeMoney { get; set; }
        public bool HasPlayerLimit { get; set; }
        public int PlayerLimit { get; set; }
        public List<Player> Players { get; set; }
        public List<MatchInfo> Matches { get; set; }
        public Player TournamentWinner { get; set; }
        public bool IsGroupMatches { get; set; } = false;
        public int NumberOfSides { get; set; }
        public List<List<Player>> ListsOfDividedPlayers { get; set; }
        public bool isFinished { get; set; } = false;

        private bool IsSetupCompleted = false;
        private int _numberOfMatchesPlayed;
        private int lastMatchAddedPos = 0;

        public Tournament(string name)
        {
            Name = name;
            IsCompetitive = false;
            HasPrizeMoney = false;
            HasPlayerLimit = false;
            PlayerLimit = 0;
            Players = new List<Player>();
            Matches = new List<MatchInfo>();
            ListsOfDividedPlayers = new List<List<Player>>();
            _numberOfMatchesPlayed = 0;
            TournamentWinner = null;
        }

        public int GetNumberOfMatchesPlayed()
        {
            return _numberOfMatchesPlayed;
        }

        public void SetNumberOfMatchesPlayed(int number)
        {
            if (number == Matches.Count)
            {
                _numberOfMatchesPlayed = number;
                CreateContinualMatches();
                if (_numberOfMatchesPlayed == Matches.Count)
                {
                    isFinished = true;
                    TournamentWinner = Matches.Last().Winner;
                }
            }
            else
            {
                _numberOfMatchesPlayed = number;
            }
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

        public void InitializeMatchesOfTournament()
        {
            var rand = new Random();
            var randomList = Players.OrderBy(x => rand.Next()).ToList();

            int actualNumberOfPlayers = Players.Count;
            int actualPositionOnPlayer = 0;


            for (int i = 0; i < NumberOfSides; i++)
            {
                ListsOfDividedPlayers[i] = new List<Player>();
                for (int j = actualPositionOnPlayer; j < (actualNumberOfPlayers / NumberOfSides) + actualPositionOnPlayer; j++)
                {
                    ListsOfDividedPlayers[i].Add(randomList[j]);
                }
                actualPositionOnPlayer = (actualNumberOfPlayers / NumberOfSides) + actualPositionOnPlayer;
            }

            CreateMatches();
        }

        private void CreateMatches()
        {
            if (IsGroupMatches)
            {
                foreach (var group in ListsOfDividedPlayers)
                {
                    for (int i = 0; i < group.Count; i++)
                    {
                        for (int j = i + 1; j < group.Count; j++)
                        {
                            var newMatch = new MatchInfo(group[i], group[j]);
                            Matches.Add(newMatch);
                        }
                    }
                }
            }
            else
            {
                foreach (var group in ListsOfDividedPlayers)
                {
                    for (int i = 0; i < group.Count; i += 2)
                    {
                        MatchInfo newMatch;
                        if (i + 1 >= group.Count)
                        {
                            newMatch = new MatchInfo(group[i]);
                            Matches.Add(newMatch);
                            SetNumberOfMatchesPlayed(_numberOfMatchesPlayed + 1);
                            break;
                        }                            

                        newMatch = new MatchInfo(group[i], group[i + 1]);
                        Matches.Add(newMatch);
                    }
                }
            }
        }

        private void CreateContinualMatches() //GET WINNER AND SHUFFLE THEM, add tournament of 5, One side, bug!
        {
            if (isFinished)
                return;

            var newMatches = new List<MatchInfo>();

            for (int i = lastMatchAddedPos; i < Matches.Count; i += 2)
            {
                MatchInfo newMatch;
                if (i + 1 >= Matches.Count)
                {
                    newMatch = new MatchInfo(Matches[i].Winner);
                    newMatches.Add(newMatch);
                    SetNumberOfMatchesPlayed(_numberOfMatchesPlayed + 1);
                    break;
                }
                newMatch = new MatchInfo(Matches[i].Winner, Matches[i + 1].Winner);
                newMatches.Add(newMatch);
            }

            Matches.AddRange(newMatches);
            lastMatchAddedPos = Matches.Count - 1;
        }
        
        public void PrintMatches()
        {
            int iterator = 0;

            foreach(var match in Matches)
            {
                Console.WriteLine((iterator++) + " - " + match);
            }
        }

        public void EditMatch()
        {
            Console.WriteLine("Which match do you want to edit? :");
            PrintMatches();

            int result = 0;
            while (true)
            {
                if (Matches.Count == 0)
                {
                    Console.WriteLine("No matches!");
                    break;
                }                    

                if (int.TryParse(Console.ReadLine(), out result))
                {
                    if (result >= 0 && result < Matches.Count)
                    {
                        var foundMatch = Matches[result];
                        
                        if (!foundMatch.Won)
                        {
                            Matches[result].WhoWon();
                            SetNumberOfMatchesPlayed(_numberOfMatchesPlayed + 1);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Match has been already played!");
                            break;
                        }
                        
                    }
                }
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
            int numberOfSides = 0;

            while (result <= 0 || result >= Enum.GetValues(typeof(TournamentCases)).Length)
            {
                bool readKey = int.TryParse(Console.ReadLine(), out result);
                switch ((TournamentCases)result)
                {
                    case TournamentCases.One_group:
                        numberOfSides = 1;
                        IsGroupMatches = true;
                        break;
                    case TournamentCases.Two_groups:
                        numberOfSides = 2;
                        IsGroupMatches = true;
                        break;
                    case TournamentCases.Four_groups:
                        numberOfSides = 4;
                        IsGroupMatches = true;
                        break;
                    case TournamentCases.Eight_groups:
                        numberOfSides = 8;
                        IsGroupMatches = true;
                        break;
                    case TournamentCases.One_side:
                        numberOfSides = 1;
                        break;
                    case TournamentCases.Two_sides:
                        numberOfSides = 2;
                        break;

                    default:
                        continue;
                }
            }
            SetNumbersOfSides(numberOfSides);
        }

        private void SetNumbersOfSides(int number)
        {
            NumberOfSides = number;

            for (int i = 0; i < NumberOfSides; i++)
            {
                ListsOfDividedPlayers.Add(new List<Player>());
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

            Console.WriteLine("Set the prize money : ");

            int result;
            while(!int.TryParse(Console.ReadLine(), out result))
            PrizeMoney = result;

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
