using System;
using System.Net.Sockets;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

namespace ServerLib
{
    /// <summary>
    /// Special class which contains Socket class and method to maintain a connection ( used in both modes ).
    /// </summary>
    /// <typeparam name="IProtoco">The class type inherited from IProtocol class with specified data class type.</typeparam>
    /// <typeparam name="T">The data class type which your server ( and other classes too ) will use to store data.</typeparam>
    public class ClientSocket<IProtoco, T> where IProtoco : IProtocol<T>
    {

        public event OnStartListeningSocketEventHandler OnStartListeningEvent;
        public event OnReceiveSocketEventHandler<T> OnReceiveSocketEvent;
        public event OnReceiveSocketErrorEventHandler OnReceiveSocketErrorEvent;
        public event OnSendSocketEventHandler<T> OnSendSocketEvent;
        public event OnShutDownEventHandler OnShutDownEvent;

        private Socket Client { get; set; }
        private byte[] Buffer { get; set; }
        private IProtocol<T> ProtocolHandler { get; set; }
        /// <summary>
        /// Create an instance of ClientSocket with IpAddress and port. Contructor starts an connection with external host ( used in Client Mode ).
        /// </summary>
        /// <param name="addr">Ip Address of server.</param>
        /// <param name="dPort">Port Connection of server.</param>
        /// <param name="protocolHandler">Interface responsible for encoding nad decoding messages.</param>
        public ClientSocket(IPAddress addr,int dPort,IProtocol<T> protocolHandler)
        {
            ProtocolHandler = protocolHandler;
            Buffer = new byte[1024];
            RealConnect(addr, dPort);
        }
        /// <summary>
        /// Create an instance of ClientSocket without receiving an active connection ( used in Client Mode ) ( using RealConnect method required ).
        /// </summary>
        /// <param name="protocolHandler">Interface responsible for encoding nad decoding messages.</param>
        public ClientSocket(IProtocol<T> protocolHandler)
        {
            ProtocolHandler = protocolHandler;
            Buffer = new byte[1024];
        }
        /// <summary>
        /// Create an instance of ClientSocket with IpAddress and port. Contructor starts an connection with external host ( used in Client Mode ). Additionany this contructor contains information about buffer size.
        /// </summary>
        /// <param name="addr">Ip Address of server.</param>
        /// <param name="dPort">Port Connection of server.</param>
        /// <param name="bufferSize">Size of byte array whick contains bytes from external host.</param>
        /// <param name="protocolHandler">Interface responsible for encoding nad decoding messages.</param>
        public ClientSocket(IPAddress addr, int dPort,int bufferSize, IProtocol<T> protocolHandler)
        {
            ProtocolHandler = protocolHandler;
            Buffer = new byte[bufferSize];
            RealConnect(addr, dPort);
        }
        /// <summary>
        /// Create an instance of ClientSocket with active Socket connection ( used in Server Mode ).
        /// </summary>
        /// <param name="client">Active connection with the client.</param>
        /// <param name="protocolHandler">Interface responsible for encoding nad decoding messages.</param>
        public ClientSocket(Socket client, IProtocol<T> protocolHandler) 
        {
            Client = client;
            //TryBindingScoket(Client);
            ProtocolHandler = protocolHandler;
            Buffer = new byte[1024];
        }
        /// <summary>
        /// Create an instance of ClientSocket with active Socket connection ( used in Server Mode ). Additionany this contructor contains special callback function.
        /// </summary>
        /// <param name="client">Active connection with the client.</param>
        /// <param name="protocolHandler">Interface responsible for encoding nad decoding messages.</param>
        /// <param name="callback">Function which will be added to OnReceiveSocketEvent event.</param>
        public ClientSocket(Socket client, IProtocol<T> protocolHandler, OnReceiveSocketEventHandler<T> callback)
        {
            Client = client;
            //TryBindingScoket(Client);
            Buffer = new byte[1024];
            ProtocolHandler = protocolHandler;
            OnReceiveSocketEvent += callback;
        }
        /// <summary>
        /// Create an instance of ClientSocket with active Socket connection ( used in Server Mode ). Additionany this contructor contains information about buffer size.
        /// </summary>
        /// <param name="client">Active connection with the client.</param>
        /// <param name="bufferSize">Size of byte array whick contains bytes from external host.</param>
        /// <param name="protocolHandler">Interface responsible for encoding nad decoding messages.</param>
        public ClientSocket(Socket client,int bufferSize, IProtocol<T> protocolHandler)
        {
            Client = client;
            //TryBindingScoket(Client);
            ProtocolHandler = protocolHandler;
            Buffer = new byte[bufferSize];
        }
        /// <summary>
        /// Create an instance of ClientSocket with active Socket connection ( used in Server Mode ). Additionany this contructor contains information about buffer size and special callback function.
        /// </summary>
        /// <param name="client">Active connection with the client.</param>
        /// <param name="bufferSize">Size of byte array whick contains bytes from external host.</param>
        /// <param name="protocolHandler">Interface responsible for encoding nad decoding messages.</param>
        /// <param name="callback">Function which will be added to OnReceiveSocketEvent event.</param>
        public ClientSocket(Socket client, int bufferSize, IProtocol<T> protocolHandler, OnReceiveSocketEventHandler<T> callback)
        {
            Client = client;
            //TryBindingScoket(Client);
            Buffer = new byte[bufferSize];
            ProtocolHandler = protocolHandler;
            OnReceiveSocketEvent += callback;
        }
        /// <summary>
        /// This starts triggers an OnStartListening event and start Receiving function
        /// </summary>
        public void StartListening()
        {
            if (OnStartListeningEvent != null)
            {
                OnStartListeningEvent(this, new StartListeningArgs((IPEndPoint)Client.RemoteEndPoint));
            }
            Receiving();
        }
        /// <summary>
        /// Recursive function which is trying to receive data from external host.
        /// </summary>
        private void Receiving()
        {
            try
            {
                Client.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, OnReceiving, null);
            }
            catch(SocketException)
            {
                ShutDown();
            }
        }
        private void TryBindingScoket(IPEndPoint endP)
        {
            while (endP.Port < 65535)
            {
                try
                {
                    Client.Bind(endP);
                    return;
                }
                catch
                {
                    endP = new IPEndPoint(endP.Address, ++endP.Port);
                }
            }
        }
        /// <summary>
        /// Method added to BeginReceive function as a callback. This method move data bytes to uffer array and encode it to T type object. Next it's trigger OnReceiveSocket event.
        /// </summary>
        /// <param name="result"></param>
        private void OnReceiving(IAsyncResult result)
        {
            try
            {
                int countByteTable = Client.EndReceive(result);
                if (countByteTable > 0)
                {
                    int realCountByteBuffer = GetRealCountByteBuffer(Buffer);
                    byte[] realBuffer = CutArray(Buffer, realCountByteBuffer);
                    T response = ProtocolHandler.DecodeProtocol(realBuffer);
                    OnReceiveSocketEvent?.Invoke(this, new SocketReceiveEventArgs<T>(response));
                    ClearBuffer(Buffer);
                }
            }
            catch (Exception ex)
            {
                OnReceiveSocketErrorEvent?.Invoke(this, new SocketReceiveErrorEventArgs(ex));
            }
            finally
            {
                Receiving();
            }
        }
        /// <summary>
        /// Method used to connect with external host.
        /// </summary>
        /// <param name="REP">Full Address of external host.</param>
        public void Connect(IPEndPoint REP)
        {
            Client.Connect(REP);
        }
        /// <summary>
        /// Method used to connect with external host.
        /// </summary>
        /// <param name="REP">IP Address of external host.</param>
        /// <param name="port">Port Connection of external host.</param>
        public void Connect(IPAddress REP,int port)
        {
            Client.Connect(REP,port);
        }
        /// <summary>
        /// Method used to sendingan message in T type to external host. Thism ethod triggers an SendMessage event.
        /// </summary>
        /// <param name="message">T type object contains data to send</param>
        public void SendMessage(T message)
        {
            if (OnSendSocketEvent != null)
            {
                OnSendSocketEvent(this, new SocketSendEventArgs<T>(message));
            }
            byte[] byteMessage = ProtocolHandler.EncodeProtocol(message);
            Client.Send(byteMessage, 0, byteMessage.Length, SocketFlags.None);
        }
        /// <summary>
        /// Method to disconnecting client
        /// </summary>
        public void ShutDown()
        {
            if (OnShutDownEvent != null)
            {
                OnShutDownEvent(this, null);
            }
            if (Client.Connected)
            {

                Client.Disconnect(false);
                Client.Close();
            }
            
        }
        private static int GetRealCountByteBuffer(byte[] buffer)
        {
            int index = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != 0)
                    index = i;
            }
            return index+1;
        }
        private static byte[] CutArray(byte[] buffer,int length)
        {
            byte[] newBuffer = new byte[length];
            for(int i=0;i<newBuffer.Length;i++)
            {
                newBuffer[i] = buffer[i];
            }
            return newBuffer;
        }
        private static void ClearBuffer(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = 0;
        }
        public static ClientSocket<IProtoco, T> GetClient(Socket socket, IProtoco protocol)
        {
            return new ClientSocket<IProtoco, T>(socket, protocol);
        }
        public bool RealConnect(IPAddress addr,int dPort,int startPort = 6560)
        {
            if (Client != null && Client.Connected)
            {
                return true;
            }
            IPAddress[] temAddresses = Dns.GetHostEntry(Environment.MachineName).AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork).ToArray();
            bool Completed = false;
            while (!Completed && startPort < 6600)
            {
                try
                {
                    foreach (IPAddress temIp in temAddresses)
                    {
                        try
                        {
                            var endP = new IPEndPoint(temIp.MapToIPv4(), startPort);
                            var s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            s.Bind(endP);
                            s.Connect(addr, dPort);
                            Client = s;
                            Receiving();
                            return true;
                        }
                        catch (Exception)
                        {

                        }
                    }
                    if (!Completed)
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    startPort++;
                    Client = null;
                }
            }
            return false;
        }
        public async Task<bool> RealConnectAsync(IPAddress addr,int dPort,int startPort = 6560)
        {
            if (Client != null && Client.Connected)
            {
                return true;
            }
            IPAddress[] temAddresses = Dns.GetHostEntry(Environment.MachineName).AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork).ToArray();
            bool Completed = false;
            while (!Completed && startPort < 6600)
            {
                try
                {
                    foreach (IPAddress temIp in temAddresses)
                    {
                        try
                        {
                            var localEndPoint = new IPEndPoint(temIp.MapToIPv4(), startPort);
                            var remoteEndPoint = new IPEndPoint(addr.MapToIPv4(),dPort);
                            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            socket.Bind(localEndPoint);
                            await socket.ConnectAsync(addr, dPort);
                            Client = socket;
                            Receiving();
                            return true;
                        }
                        catch (Exception)
                        {

                        }
                    }
                    if (!Completed)
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    startPort++;
                    Client = null;
                }
            }
            return false;
        }
    }
}
