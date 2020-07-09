using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TournamentManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var a = new StartSetup();
            await a.loadTournaments();
            //a.Begin();
            var result = Task.Run(() =>
            {
                Task.Delay(10000).Wait();
                return 5 * 10;
            });
            var taskDelay = Task.Delay(5000); // No blocking here, so
            Console.WriteLine("One");
            Console.WriteLine(result.Result);
            var b = await result;
            Console.WriteLine("two");
            Console.WriteLine(b);

            Console.ReadLine();
        }
    }
}
