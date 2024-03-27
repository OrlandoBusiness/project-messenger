using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class StartUpClass
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            _ = program.Start("26.178.12.137", "7969");
            Console.ReadKey();
        }
    }
}
