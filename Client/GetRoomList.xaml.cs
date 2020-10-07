using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientCommandServer
{
    /// <summary>
    /// Interaction logic for GetRoomList.xaml
    /// </summary>
    public partial class GetRoomList : Window
    {
        private ClientDataViewModel refer { get; set; }
        public GetRoomList(ClientDataViewModel refer,string list)
        {
            this.refer = refer;
            InitializeComponent();

            string[] listOfRooms = list.Split(',');
            foreach(string rm in listOfRooms)
            {
                string[] con = rm.Split('|');
                AddRoom(con[0],Convert.ToInt16(con[1]));
            }
        }
        private void AddRoom(string RoomName,int RoomId)
        {
            Room rm = new Room(RoomId);
            rm.RoomName.Text = RoomName;
            rm.MouseUp += RoomClick;
            RoomList.Children.Add(rm);
        }
        private void RoomClick(object sender,RoutedEventArgs args)
        {
            Room RRef = (Room)sender;
            ServerData dat = new ServerData(2,RRef.Id.ToString());
            refer.Send(dat);
            Close();
        }
    }
}
