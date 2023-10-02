using System;

namespace ServerApplicaton
{
    [Serializable]
    class Player
    {
        //General
        public string Username;
        public string Password;
        public int Level;
        public byte Access;
        public byte FirstTime;
    }
}
