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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServerCommandSystem
{
    /// <summary>
    /// Interaction logic for RoomPanel.xaml
    /// </summary>
    public partial class RoomPanel : Grid
    {
        private Room RoomRefer { get; set; }
        public RoomPanel(Room r)
        {
            RoomRefer = r;
            RoomRefer.Panel = this;
            InitializeComponent();
            RoomName.Text = RoomRefer.Name;
        }
        private void GenerateListOfClients()
        {
            foreach(ServerClient cl in RoomRefer.LocalClients)
            {
                AddMember(cl);
            }
        }
        public void AddMember(ServerClient cl)
        {
            TextBlock txtn = new TextBlock();
            txtn.Text = cl.Nick;
            txtn.TextAlignment = TextAlignment.Right;
            txtn.FontSize = 13;
            Members.Children.Add(txtn);
        }
        public void Refresh()
        {
            Members.Children.Clear();
            GenerateListOfClients();
        }
    }
}
