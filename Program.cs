using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCardBot
{
    class Program
    {
        public static Bot Crow;
        public static string path = AppDomain.CurrentDomain.BaseDirectory;

        static void Main(string[] args)
        {
            Crow = new Bot();
            Crow.Connect();
        }
    }
}
