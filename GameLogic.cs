using System;
using System.Threading;

namespace ServerApplicaton
{
    class GameLogic
    {
        public static GameLogic instance = new GameLogic();

        public Timer timer1seconds;
        public Timer timer5seconds;
        public Timer timer1Minute;

        public int oneSecondCounter;
        public int fiveSecondCounter;

        public void ServerLoop()
        {
            timer1seconds = new Timer(OnesecondsFunction, null, 0, 1000);
            timer5seconds = new Timer(FivesecondsFunction, null, 0, 5000);
        }

        private void OnesecondsFunction(Object o)
        {
            for (int i = 1; i < 100; i++)
            {
                if (Network.TempPlayer[i].inMatch == true)
                {
                    if (Network.TempPlayer[i].Castbar < 10)
                    {
                        Network.TempPlayer[i].Castbar += 1;
                        ServerSendData.instance.SendRefreshBar(i);
                    }
                }
            }
        }

        private void FivesecondsFunction(Object o)
        {
            fiveSecondCounter += 1;
        }
    }
}
