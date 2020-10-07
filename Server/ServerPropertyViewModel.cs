using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net;
using System.Net.Sockets;
using ServerLib;

namespace ServerCommandSystem
{
    class ServerPropertyViewModel : INotifyPropertyChanged
    {
        private MainWindow mWindow { get; set; }
        public LocalServer Server { get; set; }
       
        public ServerPropertyViewModel(MainWindow _ref)
        {
            Text = "___text___";
            mWindow = _ref;
            _list = new RelayCommand(InitServer);
        }
        private ICommand _list;
        public ICommand Listening
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
                OnPropertyChanged("Listening");
            }
        }
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void InitServer()
        {
            IPAddress[] adresses = Dns.GetHostAddresses(Environment.MachineName).Where(x => x.AddressFamily == AddressFamily.InterNetwork).ToArray();
            Server = new LocalServer(new IPEndPoint(adresses[0].MapToIPv4(),LocalServer.port), new ServerProtocol(), new ClientManager());
            Server.StartServer();
            Server.OnReceiveClientEvent += NewClient;
            mWindow.AddChatLog("Server", "Serwer nasłuchuje na adresie " + adresses[0].MapToIPv4().ToString() + " i  porcie " + LocalServer.port);
            Listening = new RelayCommand(StopServer);
            RoomPanel p = new RoomPanel(Server.Rooms[0]);
            mWindow.ClientListPanel.Children.Add(p);
        }

        private void NewClient(object sender, ReceiveClientEventArgs<ServerClient, ServerProtocol, ServerData> args)
        {
            IPEndPoint rAddr = (IPEndPoint)args.Socket.RemoteEndPoint;
            mWindow.AddChatLog("Klient", "Nowy klient - adres " + rAddr.ToString()  + " - " + rAddr.Port);
            
        }

        private void StopServer()
        {
            Server.ShutDownServer();
            mWindow.AddChatLog("Server", "Zatrzymanie serwera");
            Listening = new RelayCommand(InitServer);
        }
    }
}
