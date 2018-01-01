using System;
using System.Collections.Generic;
using System.Net;

namespace ServerLib
{
    public abstract class Server<T, IProtoco, Data> where T : Client<IProtoco, Data> where IProtoco : IProtocol<Data>
    {
        public event OnStartListeningSocketEventHandler OnStartServer;
        public event OnReceiveClientEventHandler<T, IProtoco, Data> OnReceiveClientEvent;
        protected List<T> Clients { get; set; }
        private ListenSocket Listening { get; set; }
        private IProtoco protoco { get; set; }
        private IClientManager<T, IProtoco, Data> ClientConverter { get; set; }
        public Server(IPEndPoint listeningAddr,IProtoco protocol,IClientManager<T, IProtoco, Data> converter)
        {
            Clients = new List<T>();
            protoco = protocol;
            ClientConverter = converter;
            Listening = new ListenSocket(listeningAddr);
        }
        public void StartServer()
        {
            Listening.OnStartListeningEvent += OnStartServer;
            Listening.OnListenEvent += ServingClient;
            Listening.StartListening();
        }
        public void ShutDownServer()
        {
            Listening.ShutDown();
        }
        private void ServingClient(object sender,AsyncEventArgs args)
        {
            var socket = Listening.EndAccept(args.res);
            ClientSocket<IProtoco, Data> clientSocket = ClientSocket<IProtoco,Data>.GetClient(socket,protoco);
            var client = ClientConverter.Convert(clientSocket);
            OnReceiveClientEvent?.Invoke(this, new ReceiveClientEventArgs<T,IProtoco, Data>(socket,client));
            Clients.Add(client);
        }
    }
}
