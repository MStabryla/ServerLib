using System;

namespace ServerLib
{
    public delegate void OnListenSocketEventHandler(object sender, AsyncEventArgs args);
    public delegate void OnShutDownEventHandler(object sender, EventArgs args);
    public delegate void OnReceiveClientEventHandler<T, IProtoco, Data>(object sender, ReceiveClientEventArgs<T,IProtoco, Data> args) where T : Client<IProtoco,Data> where IProtoco : IProtocol<Data>;
    public delegate void OnReceiveSocketEventHandler<T>(object sender, SocketReceiveEventArgs<T> args);
    public delegate void OnReceiveSocketErrorEventHandler(object sender, SocketReceiveErrorEventArgs args);
    public delegate void OnSendSocketEventHandler<T>(object sender, SocketSendEventArgs<T> args);
    public delegate void OnStartListeningSocketEventHandler(object sender, StartListeningArgs args);
}
