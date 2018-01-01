using System;
using System.Net.Sockets;
using System.Net;

namespace ServerLib
{
    class ListenSocket : Socket
    {
        public event OnListenSocketEventHandler OnListenEvent;
        public event OnStartListeningSocketEventHandler OnStartListeningEvent;
        public event OnShutDownEventHandler OnShutDownEvent;
        private IPEndPoint Client;
        public ListenSocket(IPEndPoint client) : base(AddressFamily.Unspecified, SocketType.Stream, ProtocolType.Tcp)
        {
            Client = client;
            
        }
        public ListenSocket(IPEndPoint client,OnListenSocketEventHandler callback) : base(AddressFamily.Unspecified, SocketType.Stream, ProtocolType.Tcp)
        {
            Client = client;
            OnListenEvent += callback;
        }
        public void StartListening()
        {
            Bind(Client);
            Listen(100);
            if(OnStartListeningEvent != null)
            {
                OnStartListeningEvent(this, new StartListeningArgs(Client));
            }
            Listening();
        }
        private void Listening()
        {
            BeginAccept(OnListenig, null);
        }
        private void OnListenig(IAsyncResult result)
        {
            Listening();
            if (OnListenEvent != null)
            {
                OnListenEvent(this, new AsyncEventArgs(result));
            }
        }
        public void ShutDown()
        {
            if(OnShutDownEvent != null)
            {
                OnShutDownEvent(this, null);
            }
            if(Connected)
            {
                Disconnect(false);
                Close();
            }
        }
    }
}
