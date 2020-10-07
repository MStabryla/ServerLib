using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerLib;

namespace ClientCommandServer
{
    class ServerClient : Client<ServerProtocol, ServerData>
    {
        public string Nick { get; set; }
        public event OnReceiveSocketEventHandler<ServerData> OnReseiveMessage;
        public ServerClient(ServerProtocol protocolHandler) : base(protocolHandler)
        {
            Nick = "";
            Socket.OnReceiveSocketEvent += OnReceiveMessageVoid;
            
        }
        private void OnReceiveMessageVoid(object sender, SocketReceiveEventArgs<ServerData> args)
        {
            OnReseiveMessage?.Invoke(this, args);
        }
        public void StartListening()
        {
            Socket.StartListening();
        }
        public void SendMessage(ServerData mess)
        {
            Socket.SendMessage(mess);
        }
    }

}
