using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPCustomClaimsProvider;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var adTools = new ActiveDirectory();
            var users = adTools.FindUsers("cal");
            foreach (var user in users)
            {
                Console.WriteLine(string.Format("user - {0}",user));
            }
        }
    }
}
