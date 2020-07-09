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
        public StartSetup()
        {

        }

        private void printCases()
        {
            for (int i = 1; i < Enum.GetValues(typeof(UserCases)).Length; i++)
            {
                Console.WriteLine( i + " = " + 
                                   ((UserCases) i).ToString().Replace('_', ' '));
            }
        }

        public void Begin()
        {
            Console.WriteLine("Enter a number on your keyboard of the following : \n");
            printCases();
            Main();

        }

        private void Main()
        {
            short result;
            var readKey = Int16.TryParse(Console.ReadLine(), out result);

            while (result != (short)UserCases.Shut_down)
            {
                readKey = Int16.TryParse(Console.ReadLine(), out result);
            }
        }

        public async Task<List<string>> loadTournaments()
        {
            Console.WriteLine("Start");
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Exported", "Tournaments.txt");
            List<string> strings = new List<string>();
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    await Task.Run(() =>
                    {
                        Thread.Sleep(1000);
                        while (!reader.EndOfStream)
                        {
                            strings.Add(reader.ReadLine());
                        }
                        
                        Task.Delay(100);
                        Console.WriteLine("Finished");

                    });

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("The export file doesn't exists or there is error reading it");
            }

            return strings;


        }
    }
}
