using System;
using System.Net.Sockets;
using System.Net;

namespace ServerLib
{
    /// <summary>
    /// Special socket with implemented methods to receiving client's connection ( used in Server Mode ).
    /// </summary>
    public class ListenSocket : Socket
    {
        public event OnListenSocketEventHandler OnListenEvent;
        public event OnStartListeningSocketEventHandler OnStartListeningEvent;
        public event OnShutDownEventHandler OnShutDownEvent;
        private IPEndPoint Client;
        /// <summary>
        /// Create new instance of ListenSocket class. This object will listening on provided address.
        /// </summary>
        /// <param name="client">Address on which this socket will be listening.</param>
        public ListenSocket(IPEndPoint client) : base(client.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
        {
            Client = client;
            
        }
        /// <summary>
        /// Create new instance of ListenSocket class. This object will listening on provided address.
        /// </summary>
        /// <param name="client">Address on which this socket will be listening.</param>
        /// <param name="callback">Function which will be added to OnListenEvent event.</param>
        public ListenSocket(IPEndPoint client,OnListenSocketEventHandler callback) : base(AddressFamily.Unspecified, SocketType.Stream, ProtocolType.Tcp)
        {
            Client = client;
            OnListenEvent += callback;
        }
        /// <summary>
        /// This function binding included IpAdress to this socket and start listening for clients.
        /// </summary>
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
        /// <summary>
        /// Recursive function which accepts connection from external hosts.
        /// </summary>
        private void Listening()
        {
            BeginAccept(OnListenig, null);
        }
        /// <summary>
        /// Method added to BeginAccept function as a callback. This method triggers an OnListenEvent and start listening again.
        /// </summary>
        /// <param name="result"></param>
        private void OnListenig(IAsyncResult result)
        {
            Listening();
            if (OnListenEvent != null)
            {
                OnListenEvent(this, new AsyncEventArgs(result));
            }
        }
        /// <summary>
        /// Method which triggers OnShutDownEvent and close actual connection.
        /// </summary>
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
