using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using ServerLib;

namespace ClientCommandServer
{
    partial class ClientDataViewModel : INotifyPropertyChanged
    {
        private async void Connect()
        {
            try
            {
                IPAddress ip = IPAddress.Parse(Address);
                this.Connected = await client.SocketConnect(ip, 6515);
                if (!this.Connected)
                {
                    throw new Exception("Nie nawiązano połączenia");
                }
                mWindow.AddLog("System","Połączono");
                FirstSend();
            }
            catch (Exception ex)
            {
                mWindow.AddLog("System","Błąd : " + ex.Message);
            }

        }
        private void FirstSend()
        {
            client.OnReseiveMessage += Interpretade;
            //client.StartListening();
            client.SendMessage(new ServerData(1, NickName));
        }
        private void Interpretade(object sender, SocketReceiveEventArgs<ServerData> args)
        {
            ServerClient cl = (ServerClient)sender;
            if (args.Response.ActionCode > 100 && args.Response.ActionCode < 200)
            {
                NormalInterpretade(cl, args);
            }
            else if (args.Response.ActionCode > 200)
            {
                ErrorInterpretade(cl, args);
            }
        }
        private void NormalInterpretade(ServerClient sender, SocketReceiveEventArgs<ServerData> args)
        {
            int code = args.Response.ActionCode - 100;
            mWindow.Dispatcher.Invoke(delegate
            {
                switch (code)
                {
                    case 1:
                        //Wybranie pokoju

                        GetRoomList ls = new GetRoomList(this, args.Response.Text);
                        ls.ShowDialog();
                        break;
                    case 2:
                        //Potwierdzenie dołączenia do pokoju
                        if (args.Response.Text == "true")
                        {
                            mWindow.AddLog("Serwer","Połączono z pokojem");
                        }
                        else
                        {
                            mWindow.AddLog("Serwer",args.Response.Text);
                        }
                        break;
                    case 3:
                        //Odebranie wiadomości
                        string[] tab = args.Response.Text.Split('|');
                        if(tab.Length > 1)
                        {
                            mWindow.AddLog(tab[0], tab[1]);
                        }
                        break;
                    case 4:
                        //Odebranie wiadomości
                        break;
                    case 5:
                        //Potwierdzenie wyjścia z pokoju
                        break;
                    case 6:
                        //Zmiana pokoju
                        break;
                }
            });
        }
        private void ErrorInterpretade(ServerClient sender, SocketReceiveEventArgs<ServerData> args)
        {
            mWindow.Dispatcher.Invoke(delegate
            {
                MessageBox.Show("Error " + args.Response.ActionCode + " : " + args.Response.Text);
            });
        }
        public void Send(ServerData dat)
        {
            client.SendMessage(dat);
        }
        
    }
}
