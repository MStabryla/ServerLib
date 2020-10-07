using System;
using System.Collections.Generic;
using System.Net;
using ServerLib;

namespace ServerCommandSystem
{
    class LocalServer : Server<ServerClient, ServerProtocol, ServerData>
    {
        #region conf
        public static readonly int port = 6515;
        #endregion

        public List<Room> Rooms { get; set; }
        public LocalServer(IPEndPoint listeningAddr, ServerProtocol protocol, IClientManager<ServerClient, ServerProtocol, ServerData> converter) : base(listeningAddr, protocol, converter)
        {
            base.OnReceiveClientEvent += ReceivingClient;
            Rooms = new List<Room>();
            Rooms.Add(new Room("Pokój testowy"));
        }

        private void ReceivingClient(object sender, ReceiveClientEventArgs<ServerClient, ServerProtocol, ServerData> args)
        {
            args.Client.OnReseiveMessage += Analyser;
            var test = Clients;
        }
        private void Analyser(object sender, SocketReceiveEventArgs<ServerData> args)
        {
            ServerClient refer = (ServerClient)sender;
            ServerData dat = null;
            switch (args.Response.ActionCode)
            {
                case 1:
                    // Nawiązanie połączenia
                    try
                    {
                        refer.Nick = args.Response.Text;
                        string ids = "";
                        for (int i = 0; i < Rooms.Count; i++)
                        {
                            var elem = Rooms[i];
                            ids += elem.Name + "|" + i;
                            if (i != Rooms.Count - 1)
                            {
                                ids += ",";
                            }
                        }
                        dat = new ServerData(101, ids);
                    }
                    catch(Exception ex)
                    {
                        dat = new ServerData(201, ex.Message);
                    }
                    finally
                    {
                        refer.SendMessage(dat);
                    }
                    break;
                case 2:
                    // Wybieranie pokoju
                    
                    try
                    {
                        int roomId = Convert.ToInt16(args.Response.Text);
                        if (Rooms.Count > roomId)
                        {
                            Rooms[roomId].AddToRoom(refer);
                        }
                        dat = new ServerData(102, "true");
                    }
                    catch(Exception ex)
                    {
                        dat = new ServerData(202, ex.Message);
                    }
                    finally
                    {
                        refer.SendMessage(dat);
                    }
                    break;
                case 3:
                    // Wysłanie wiadomości
                    try
                    {
                        if (refer.RoomReference != null)
                        {
                            ServerData nd = new ServerData(103, args.Response.Text);
                            refer.RoomReference.SendToAll(nd);
                        }
                    }
                    catch (Exception ex)
                    {
                        dat = new ServerData(203, ex.Message);
                    }
                    break;
                case 4:
                    // Wysłanie do wszyskich
                    try
                    {
                        foreach (ServerClient cl in Clients)
                        {
                            ServerData nd = new ServerData(104, args.Response.Text);
                            refer.RoomReference.SendToAll(nd);
                        }
                    }
                    catch (Exception ex)
                    {
                        dat = new ServerData(204, ex.Message);
                    }
                    break;
                case 5:
                    // Wyjście z pokoju
                    try
                    {
                        if (refer.RoomReference != null)
                        {
                            refer.RoomReference.RemoveFromRoom(refer);
                        }
                        dat = new ServerData(105, "true");
                    }
                    catch (Exception ex)
                    {
                        dat = new ServerData(205, ex.Message);
                    }
                    finally
                    {
                        refer.SendMessage(dat);
                    }
                    break;
                case 6:
                    //Zmiana pokoju
                    try
                    {
                        int newRoomId = Convert.ToInt16(args.Response.Text);
                        if (refer.RoomReference != null)
                            refer.RoomReference.RemoveFromRoom(refer);
                        if (Rooms.Count > newRoomId)
                            refer.RoomReference.AddToRoom(refer);
                        dat = new ServerData(106, "true|" + newRoomId);
                    }
                    catch (Exception ex)
                    {
                        dat = new ServerData(205, ex.Message);
                    }
                    finally
                    {
                        refer.SendMessage(dat);
                    }
                    break;
            }
        }
    }
}
