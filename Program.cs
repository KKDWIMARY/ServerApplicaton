using System;
using System.Threading;

namespace ServerApplicaton
{
    class Program
    {
        private static Thread threadConsole;
        private static bool consoleRunning;

        static void Main(string[] args)
        {
            threadConsole = new Thread(new ThreadStart(ConsoleThread));
            threadConsole.Start();

            Database.instance.CheckPath(Database.instance.PATH_ACCOUNT);
            ServerHandleData.instance.InitMessages();
            Network.instance.ServerStart();
            GameLogic.instance.ServerLoop();
        }

        private static void ConsoleThread()
        {
            string line;
            consoleRunning = true;

            while(consoleRunning)
            {
                line = Console.ReadLine();

                if(String.IsNullOrWhiteSpace(line))
                {
                    consoleRunning = false;
                    return;
                }

                else
                {

                }
            }
        }
    }
}
