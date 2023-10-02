using System;

namespace ServerApplicaton
{
    class RoomInstance
    {
        public static Room[] _room = new Room[100];
        public static RoomInstance instance = new RoomInstance();

        public void JoinOrCreateRoom(int index)
        {
            if (Network.TempPlayer[index].Room > 0)
            {
                Console.WriteLine("Player already in a room");
                return;
            }

            int i = 0;

            for (i = 1; i < 100; i++)
            {
                if (_room[i]._state == Room.RoomState.Searching)
                {
                    if (_room[i].playerCount < 2)
                    {
                        // Another player joins the existing room here
                        _room[i].player[_room[i].playerCount] = Network.Clients[index].Index;
                        _room[i].playerCount++; // Increment the player count
                        Network.TempPlayer[index].Room = i;
                        Console.WriteLine("Player joined room: " + i + " | Player added: " + index);

                        if (_room[i].playerCount == 2)
                        {
                            // Room is now full with 3 players, consider matchmaking complete
                            _room[i]._state = Room.RoomState.Closed;
                            ServerSendData.instance.SendIsGameplayStart(i);
                            Console.WriteLine("Gameplay started now: " + i);
                        }
                        return;
                    }
                }
            }

            // If no available rooms were found, create a new room
            for (i = 1; i < 100; i++)
            {
                if (_room[i]._state == Room.RoomState.Empty)
                {
                    // Player 1 creates a room here
                    _room[i].player[0] = Network.Clients[index].Index;
                    _room[i]._state = Room.RoomState.Searching;
                    Network.TempPlayer[index].Room = i;
                    _room[i].playerCount = 1; // Initialize the player count to 1
                    Console.WriteLine("Room created: " + i + " | Player added: " + " Index: " + index);
                    return;
                }
            }
        }

        public void LeaveRoom(int index)
        {
            int roomIndex = Network.TempPlayer[index].Room;

            if (roomIndex > 0 && roomIndex < _room.Length)
            {
                Room room = _room[roomIndex];
                if (room != null)
                {
                    room._state = Room.RoomState.Empty;

                    for (int i = 0; i < room.playerCount; i++)
                    {
                        if (room.player[i] == index)
                        {
                            // Player leaving the room
                            room.player[i] = 0;
                            break;
                        }
                    }

                    // Notify remaining players in the room about the player leaving
                    for (int i = 0; i < room.playerCount; i++)
                    {
                        int playerIndex = room.player[i];
                        if (playerIndex > 0)
                        {
                            ServerSendData.instance.SendIngame(playerIndex);
                        }
                    }

                    Network.TempPlayer[index].Room = 0;
                    Network.TempPlayer[index].Castbar = 0;
                }
            }
        }
    }

    class Room
    {
        public int roomIndex;
        public int[] player = new int[2]; // Increase the player array size to 3
        public int playerCount; // Track the number of players in the room

        public RoomState _state;
        public enum RoomState
        {
            Empty,
            Searching,
            Closed
        }
    }
} 
