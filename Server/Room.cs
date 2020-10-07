using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommandSystem
{
    public class Room
    {
        public List<ServerClient> LocalClients
        {
            get;
            private set;
        }
        public string Name { get; set; }
        public RoomPanel Panel
        {
            get; set;
        }
        public Room(string name)
        {
            Name = name;
            LocalClients = new List<ServerClient>();
        }
        public void AddToRoom(ServerClient cl)
        {
            cl.SetRoom(this);
            LocalClients.Add(cl);
            if(Panel != null)
            {
                Panel.Dispatcher.Invoke(delegate
                {
                    Panel.AddMember(cl);
                });
                
            }
        }
        public void RemoveFromRoom(ServerClient cl)
        {
            cl.SetRoom(null);
            LocalClients.Remove(cl);
            if (Panel != null)
            {
                Panel.Dispatcher.Invoke(delegate
                {
                    Panel.Refresh();
                });
            }
        }
        public void SendToAll(ServerData serverData)
        {
            foreach(ServerClient cl in LocalClients)
            {
                cl.SendMessage(serverData);
            }
        }
    }
}
