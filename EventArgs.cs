using System;
using System.Net;
using System.Net.Sockets;

namespace ServerLib
{
    /// <summary>
    ///  Event Argument which contain IAsyncResult inplementation
    /// </summary>
    public class AsyncEventArgs : EventArgs
    {
        public IAsyncResult res { get; set; }
        public AsyncEventArgs(IAsyncResult _res)
        {
            res = _res;
        }
    }
    /// <summary>
    ///     Event Argument which contain response data from other host in T class object.
    /// </summary>
    /// <typeparam name="T">Type of reseived data</typeparam>
    public class SocketReceiveEventArgs<T> : EventArgs
    {
        public T Response { get; set; }
        public SocketReceiveEventArgs(T responce)
        {
            Response = responce;
        }
    }
    /// <summary>
    /// Event Argument wich contain Exception data
    /// </summary>
    public class SocketReceiveErrorEventArgs : EventArgs
    {
        public Exception Ex { get; set; }
        public SocketReceiveErrorEventArgs(Exception ex)
        {
            Ex = ex;
        }
    }
    /// <summary>
    ///     Event Argument which contain request data to other host in T class object.
    /// </summary>
    /// <typeparam name="T">Type of sended data</typeparam>
    public class SocketSendEventArgs<T> : EventArgs
    {
        public T Response { get; set; }
        public SocketSendEventArgs(T responce)
        {
            Response = responce;
        }
    }
    /// <summary>
    /// Event Argument which contain information about listening address.
    /// </summary>
    public class StartListeningArgs : EventArgs
    {
        public IPEndPoint client { get; set; }
        public StartListeningArgs(IPAddress addr,int port)
        {
            client = new IPEndPoint(addr,port);
        }
        public StartListeningArgs(IPEndPoint end)
        {
            client = end;
        }
    }
    /// <summary>
    /// Event Argument which contain client object and Socket object.
    /// </summary>
    /// <typeparam name="T">Type of client class which inherits from Client.</typeparam>
    /// <typeparam name="IProtoco">The class type inherited from IProtocol class with specified data class type.</typeparam>
    /// <typeparam name="Data">The data class type which your server ( and other classes too ) will use to store data.</typeparam>
    public class ReceiveClientEventArgs<T,IProtoco,Data> : EventArgs where T : Client<IProtoco,Data> where IProtoco : IProtocol<Data>
    {
        public Socket Socket { get; set; }
        public T Client { get; set; }
        public ReceiveClientEventArgs(Socket socket, T client)
        {
            Socket = socket;
            Client = client;
        }
    }
}
