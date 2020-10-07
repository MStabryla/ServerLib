using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net;
using System.Windows;

namespace ClientCommandServer
{
    public partial class ClientDataViewModel : INotifyPropertyChanged
    {
        private MainWindow mWindow { get; set; }
        private ServerClient client { get; set; }

        public ClientDataViewModel(MainWindow refer)
        {
            mWindow = refer;
            _address = "";
            _messcontent = "";
            _connect = new RelayCommand(Init);
            _send = new RelayCommand(SendMess);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChaged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        private string _address;
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
                OnPropertyChaged("Address");
            }
        }

        private ICommand _connect;
        public ICommand Connecting
        {
            get
            {
                return _connect;
            }
            set
            {
                _connect = value;
                OnPropertyChaged("Connecting");
            }
        }
       
        private bool _connected;
        public bool Connected
        {
            get
            {
                return _connected;
            }
            set
            {
                _connected = value;
                OnPropertyChaged("Connected");
            }
        }
        public string NickName
        {
            get
            {
                return client.Nick;
            }
            set
            {
                client.Nick = value;
                OnPropertyChaged("NickName");
            }
        }
        private string _messcontent;
        public string Message
        {
            get
            {
                return _messcontent;
            }
            set
            {
                _messcontent = value;
                OnPropertyChaged("MessContent");
            }
        }
        private ICommand _send;
        public ICommand Sending
        {
            get
            {
                return _send;
            }
            set
            {
                _send = value;
                OnPropertyChaged("Connecting");
            }
        }
        private void Init()
        {
            try
            {
                GetNick nick = new GetNick(this);
                client = new ServerClient(new ServerProtocol());
                bool? x = nick.ShowDialog();
                Connect();
            }
            catch(Exception ex)
            {
                mWindow.AddLog("System","Błąd : " + ex.Message);
            }
           
        }
        private void SendMess()
        {
            if(Connected && Message != "")
            {
                Send(new ServerData(3, NickName + "|" + Message));
                mWindow.Message.Text = "";
            }
                
        }
    }
}
