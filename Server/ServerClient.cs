using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerLib;

namespace ServerCommandSystem
{
    public class ServerClient : Client<ServerProtocol, ServerData>
    {
        public string Nick { get; set; }
        public Room RoomReference
        {
            get;
            private set;
        }
        public event OnReceiveSocketEventHandler<ServerData> OnReseiveMessage;
        public ServerClient(ClientSocket<ServerProtocol, ServerData> socket) : base(socket)
        {
            Socket.OnReceiveSocketEvent += OnReceiveMessageVoid;
            Socket.StartListening();
        }
        public void SetRoom(Room _room)
        {
            RoomReference = _room;
        }
        private void OnReceiveMessageVoid(object sender, SocketReceiveEventArgs<ServerData> args)
        {
            OnReseiveMessage?.Invoke(this, args);
        }

        public void SendMessage(ServerData mess)
        {
            Socket.SendMessage(mess);
        }
    }
    class ClientManager : IClientManager<ServerClient, ServerProtocol, ServerData>
    {
        public ServerClient Convert(ClientSocket<ServerProtocol, ServerData> clientS)
        {
            return new ServerClient(clientS);
        }
    }
}
