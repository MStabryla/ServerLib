using System.Net;

namespace ServerLib
{
    /// <summary>
    /// Provides a base class for server clients.All others library class required client class, that inherits from this class.
    /// </summary>
    /// <typeparam name="IProtoco">The class type inherited from IProtocol class with specified data class type.</typeparam>
    /// <typeparam name="T">The data class type which your server ( and other classes too ) will use to store data.</typeparam>
    public abstract class Client<IProtoco,T> where IProtoco : IProtocol<T> 
    {
        
        protected ClientSocket<IProtoco, T> Socket { get; set; }

        /// <summary>
        /// Initializes a new instance of the ServerLib class with prepared ClientSocket(used in Server Mode).
        /// </summary>
        /// <param name="socket">Prepared ClientSocket class which include connected Socket class (used in Server Mode ).</param>
        public Client(ClientSocket<IProtoco, T> socket)
        {
            Socket = socket;
        }

        /// <summary>
        /// Initializes a new instance of the ServerLib class with specified IPAddress and port(used in Client Mode).
        /// </summary>
        /// <param name="addr">The IP Address with which this client will be connected.</param>
        /// <param name="dPort">The Port number on which this client will be connected.</param>
        /// <param name="protocolHandler">Interface responsible for encoding nad decoding messages (required to Create ClientSocket object ).</param>
        public Client(IPAddress addr,int dPort,IProtoco protocolHandler)
        {
            Socket = new ClientSocket<IProtoco, T>(addr,dPort,protocolHandler);
        }

        /// <summary>
        /// Initializes a new instance of the ServerLib class with no specified address.The connection must be established using the SocketConnect function(used in Client Mode) .
        /// </summary>
        /// <param name="protocolHandler">Interface responsible for encoding nad decoding messages(required to Create ClientSocket object ).</param>
        public Client(IProtoco protocolHandler)
        {
            Socket = new ClientSocket<IProtoco, T>(protocolHandler);
        }

        /// <summary>
        /// This function is trying to establish the connection with external host using IPAddress and port number ( used in Client Mode ).
        /// </summary>
        /// <param name="addr">The IP Address with which this client will be connected.</param>
        /// <param name="dPort">The Port number on which this client will be connected.</param>
        /// <returns>Information about reaching connection success</returns>
        public bool SocketConnect(IPAddress addr,int dPort)
        {
            return Socket.RealConnect(addr, dPort);
        }
    }
}
