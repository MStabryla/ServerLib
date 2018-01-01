using System.Net;

namespace ServerLib
{
    public abstract class Client<IProtoco,T> where IProtoco : IProtocol<T> 
    {
        protected ClientSocket<IProtoco, T> Socket { get; set; }
        public Client(ClientSocket<IProtoco, T> socket)
        {
            Socket = socket;
        }
        public Client(IPAddress addr,int dPort,IProtoco protocolHandler)
        {
            Socket = new ClientSocket<IProtoco, T>(addr,dPort,protocolHandler);
        }
        public Client(IProtoco protocolHandler)
        {
            Socket = new ClientSocket<IProtoco, T>(false, protocolHandler);
        }
        public bool SocketConnect(IPAddress addr,int dPort)
        {
            return Socket.RealConnect(addr, dPort);
        }
    }
}
