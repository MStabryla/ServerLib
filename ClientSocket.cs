using System;
using System.Net.Sockets;
using System.Net;
using System.Linq;

namespace ServerLib
{
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
        public ClientSocket(IPAddress addr,int dPort,IProtocol<T> protocolHandler)
        {
            ProtocolHandler = protocolHandler;
            Buffer = new byte[1024];
            RealConnect(addr, dPort);
        }
        public ClientSocket(bool autoConnect, IProtocol<T> protocolHandler)
        {
            ProtocolHandler = protocolHandler;
            Buffer = new byte[1024];
        }
        public ClientSocket(IPAddress addr, int dPort,int bufferSize, IProtocol<T> protocolHandler)
        {
            ProtocolHandler = protocolHandler;
            Buffer = new byte[bufferSize];
            RealConnect(addr, dPort);
        }
        public ClientSocket(Socket client, IProtocol<T> protocolHandler) 
        {
            Client = client;
            //TryBindingScoket(Client);
            ProtocolHandler = protocolHandler;
            Buffer = new byte[1024];
        }
        public ClientSocket(Socket client, IProtocol<T> protocolHandler, OnReceiveSocketEventHandler<T> callback)
        {
            Client = client;
            //TryBindingScoket(Client);
            Buffer = new byte[1024];
            ProtocolHandler = protocolHandler;
            OnReceiveSocketEvent += callback;
        }
        public ClientSocket(Socket client, int bufferSize, IProtocol<T> protocolHandler, OnReceiveSocketEventHandler<T> callback)
        {
            Client = client;
            //TryBindingScoket(Client);
            Buffer = new byte[bufferSize];
            ProtocolHandler = protocolHandler;
            OnReceiveSocketEvent += callback;
        }
        public void StartListening()
        {
            if (OnStartListeningEvent != null)
            {
                OnStartListeningEvent(this, new StartListeningArgs((IPEndPoint)Client.RemoteEndPoint));
            }
            Receiving();
        }
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
        public void Connect(IPEndPoint REP)
        {
            Client.Connect(REP);
        }
        public void Connect(IPAddress REP,int port)
        {
            Client.Connect(REP,port);
        }
        public void SendMessage(T message)
        {
            if (OnSendSocketEvent != null)
            {
                OnSendSocketEvent(this, new SocketSendEventArgs<T>(message));
            }
            byte[] byteMessage = ProtocolHandler.CodeProtocol(message);
            Client.Send(byteMessage, 0, byteMessage.Length, SocketFlags.None);
        }
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
    }
}
