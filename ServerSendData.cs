using System;

namespace ServerApplicaton
{
    class ServerSendData
    {
        public static ServerSendData instance = new ServerSendData();

        public void SendDataToRoom(int room, byte[] data)
        {
            for (int i = 0; i < 2; i++)
            {
                if (RoomInstance._room[room].player[i] > 0)
                {
                    SendDataTo(RoomInstance._room[room].player[i], data);
                }
            }
        }

        public void SendDataTo(int index, byte[] data)
        {
            KaymakGames.KaymakGames buffer = new KaymakGames.KaymakGames();
            buffer.WriteBytes(data);
            Network.Clients[index].myStream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            buffer = null;
        }

        public void SendIngame(int index)
        {
            KaymakGames.KaymakGames buffer = new KaymakGames.KaymakGames();
            buffer.WriteInteger(1);

            if (Network.Player[index] != null)
            {
                // Player Data
                buffer.WriteString(Network.Player[index].Username);
                buffer.WriteString(Network.Player[index].Password);
                buffer.WriteInteger(Network.Player[index].Level);
                buffer.WriteByte(Network.Player[index].Access);
                buffer.WriteByte(Network.Player[index].FirstTime);
            }
            else
            {
                // Handle the case where Network.Player[index] is null
                // You can log an error or take appropriate action here.
            }

            SendDataTo(index, buffer.ToArray());
            buffer = null;
        }

        public void SendIsGameplayStart(int room)
        {
            KaymakGames.KaymakGames buffer = new KaymakGames.KaymakGames();
            buffer.WriteInteger(3);

            for (int i = 0; i < 2; i++)
            {
                int playerIndex = RoomInstance._room[room].player[i];
                if (playerIndex > 0)
                {
                    buffer.WriteString(Network.Player[playerIndex].Username);
                }
                else
                {
                    // If there is no player at this index, send an empty string
                    buffer.WriteString("");
                }
            }

            SendDataToRoom(room, buffer.ToArray());
            buffer = null;

            for (int i = 0; i < 2; i++)
            {
                int playerIndex = RoomInstance._room[room].player[i];
                if (playerIndex > 0)
                {
                    Network.TempPlayer[playerIndex].inMatch = true;
                }
            }
        }

        public void SendRefreshBar(int index)
        {
            KaymakGames.KaymakGames buffer = new KaymakGames.KaymakGames();
            buffer.WriteInteger(4);
            buffer.WriteInteger(Network.TempPlayer[index].Castbar);
            SendDataTo(index, buffer.ToArray());
            buffer = null;
        }

        // New method to send a cancellation confirmation to the player
        public void SendCancelMatchmaking(int index)
        {
            KaymakGames.KaymakGames buffer = new KaymakGames.KaymakGames();
            buffer.WriteInteger(5); // Use a unique packet number for cancel match success
            SendDataTo(index, buffer.ToArray());
            buffer = null;
        }

        public void SendLeaveMatch(int index)
        {
            KaymakGames.KaymakGames buffer = new KaymakGames.KaymakGames();
            buffer.WriteInteger(5); // Use a unique packet number for cancel match success
            SendDataTo(index, buffer.ToArray());
            buffer = null;
        }
    }
}
