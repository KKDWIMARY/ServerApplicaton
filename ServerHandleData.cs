using System;
using System.Collections.Generic;

namespace ServerApplicaton
{
    class ServerHandleData
    {
        public static ServerHandleData instance = new ServerHandleData();
        private delegate void Packet_(int Index, byte[] Data);
        private Dictionary<int, Packet_> Packets;

        public void InitMessages()
        {
            Packets = new Dictionary<int, Packet_>();
            Packets.Add(1, HandleNewAccount);
            Packets.Add(2, HandleLogin);
            Packets.Add(3, HandleLookingForMatch);
            Packets.Add(5, HandleCancelMatchmaking);
            Packets.Add(6, HandleLeaveMatch);
        }

        public void HandleData(int index, byte[] data)
        {
            int packetnum;
            Packet_ Packet;
            KaymakGames.KaymakGames buffer = new KaymakGames.KaymakGames();
            buffer.WriteBytes(data);
            packetnum = buffer.ReadInteger();
            buffer = null;
            if (packetnum == 0)
                return;

            if (Packets.TryGetValue(packetnum, out Packet))
            {
                Packet.Invoke(index, data);
            }
        }

        void HandleNewAccount(int index, byte[] Data)
        {
            KaymakGames.KaymakGames buffer = new KaymakGames.KaymakGames();
            buffer.WriteBytes(Data);
            int packet = buffer.ReadInteger();
            string username = buffer.ReadString();
            string password = buffer.ReadString();

            if (Database.instance.AccountExist(username) == true)
            {
                //SendAlertMsg
                return;
            }

            Database.instance.AddAccount(index, username, password);

            ServerSendData.instance.SendIngame(index);
        }

        void HandleLogin(int index, byte[] Data)
        {
            KaymakGames.KaymakGames buffer = new KaymakGames.KaymakGames();
            buffer.WriteBytes(Data);
            int packet = buffer.ReadInteger();
            string username = buffer.ReadString();
            string password = buffer.ReadString();

            if (Database.instance.AccountExist(username) == false)
            {
                //SendAlertMsg user does not exist
                return;
            }

            if (Database.instance.PasswordOK(username, password) == false)
            {
                //SendAlertMsg password does not match
                return;
            }
            Database.instance.LoadPlayer(index, username);
            ServerSendData.instance.SendIngame(index);
        }

        void HandleLookingForMatch(int index, byte[] Data)
        {
            KaymakGames.KaymakGames buffer = new KaymakGames.KaymakGames();
            buffer.WriteBytes(Data);
            int packet = buffer.ReadInteger();

            RoomInstance.instance.JoinOrCreateRoom(index);
        }

        // New method to handle the CancelMatch packet (packet number 5)
        void HandleCancelMatchmaking(int index, byte[] Data)
        {
            // Implement the logic to cancel matchmaking for the player with 'index'.
            // This could involve leaving the matchmaking queue or taking appropriate action.
            // You can also send a response back to the client to confirm the cancellation.

            // Check if the player is currently in a matchmaking queue
            if (Network.TempPlayer[index].Room == 0 && Network.TempPlayer[index].inMatch == false)
            {
                Console.WriteLine($"Player {index} is not in a matchmaking queue. Nothing to cancel.");
                return;
            }

            // Remove the player from the matchmaking queue
            if (Network.TempPlayer[index].Room > 0)
            {
                int roomIndex = Network.TempPlayer[index].Room;
                RoomInstance._room[roomIndex]._state = Room.RoomState.Empty;
                Network.TempPlayer[index].Room = 0;

                // Notify the player that they have successfully canceled matchmaking
                ServerSendData.instance.SendCancelMatchmaking(index);
                Console.WriteLine($"Player {index} has canceled matchmaking.");
            }
            else if (Network.TempPlayer[index].inMatch)
            {
                // Implement the logic to gracefully exit an ongoing match if needed.
                // This could involve notifying other players, ending the match, etc.

                // For example, you might have a function like LeaveMatch that handles leaving matches.
                // LeaveMatch(index);

                // Reset the player's matchmaking status
                Network.TempPlayer[index].inMatch = false;

                // Notify the player that they have successfully canceled matchmaking
                ServerSendData.instance.SendCancelMatchmaking(index);
                Console.WriteLine($"Player {index} has canceled matchmaking and left the match.");
            }
        }

        void HandleLeaveMatch(int index, byte[] Data)
        {

        }
    }
}
/*
 using System;
using System.Collections.Generic;

namespace ServerApplicaton
{
    class ServerHandleData
    {
        public static ServerHandleData instance = new ServerHandleData();
        private delegate void Packet_(int Index, byte[] Data);
        private Dictionary<int, Packet_> Packets;

        public void InitMessages()
        {
            Packets = new Dictionary<int, Packet_>();
            Packets.Add(1, HandleNewAccount);
            Packets.Add(2, HandleLogin);
            Packets.Add(3, HandleLookingForMatch);
            Packets.Add(5, HandleCancelMatch); // Add handling for CancelMatch packet
        }

        public void HandleData(int index, byte[] data)
        {
            int packetnum;
            Packet_ Packet;
            KaymakGames.KaymakGames buffer = new KaymakGames.KaymakGames();
            buffer.WriteBytes(data);
            packetnum = buffer.ReadInteger();
            buffer = null;
            if (packetnum == 0)
                return;

            if (Packets.TryGetValue(packetnum, out Packet))
            {
                Packet.Invoke(index, data);
            }
        }

        void HandleNewAccount(int index, byte[] Data)
        {
            KaymakGames.KaymakGames buffer = new KaymakGames.KaymakGames();
            buffer.WriteBytes(Data);
            int packet = buffer.ReadInteger();
            string username = buffer.ReadString();
            string password = buffer.ReadString();

            if (Database.instance.AccountExist(username) == true)
            {
                //SendAlertMsg
                return;
            }

            Database.instance.AddAccount(index, username, password);

            ServerSendData.instance.SendIngame(index);
        }

        void HandleLogin(int index, byte[] Data)
        {
            KaymakGames.KaymakGames buffer = new KaymakGames.KaymakGames();
            buffer.WriteBytes(Data);
            int packet = buffer.ReadInteger();
            string username = buffer.ReadString();
            string password = buffer.ReadString();

            if (Database.instance.AccountExist(username) == false)
            {
                //SendAlertMsg user does not exist
                return;
            }

            if (Database.instance.PasswordOK(username, password) == false)
            {
                //SendAlertMsg password does not match
                return;
            }
            Database.instance.LoadPlayer(index, username);
            ServerSendData.instance.SendIngame(index);
        }

        void HandleLookingForMatch(int index, byte[] Data)
        {
            KaymakGames.KaymakGames buffer = new KaymakGames.KaymakGames();
            buffer.WriteBytes(Data);
            int packet = buffer.ReadInteger();

            RoomInstance.instance.JoinOrCreateRoom(index);
        }

        // New method to handle the CancelMatch packet (packet number 5)
        void HandleCancelMatch(int index, byte[] Data)
        {
            RoomInstance.instance.LeaveRoom(index);
        }
    }
}
 */
