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
        static void Main(string[] args)
        {
            var a = new StartSetup();
            a.Begin();
            Console.ReadLine();
        }
    }
}
