using System;
using Microsoft.Owin.Hosting;

namespace Flagmax.WebApi    
{
    class Program
    {
        static void Main()
        {
            var baseAddress = "http://localhost:9000/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(baseAddress))
            {
                Console.WriteLine(baseAddress);
                Console.ReadLine();
            }

        }
    }
}
