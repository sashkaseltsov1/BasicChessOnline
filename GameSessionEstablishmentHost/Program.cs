using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GameSessionEstablishmentHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new ServiceHost(typeof(GameSessionEstablishmentService.GameSessionEstablishment)))
            {
                host.Open();
                Console.WriteLine("GameSessionEstablishment service statred...");
                Console.ReadLine();
            }
        }
    }
}
