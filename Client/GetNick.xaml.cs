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
    /// Interaction logic for GetNick.xaml
    /// </summary>
    partial class GetNick : Window
    {
        private ClientDataViewModel reference { get; set; }
        public GetNick(ClientDataViewModel refer)
        {
            InitializeComponent();
            reference = refer;
            NickConfirmed.Click += Confirm;
        }
        private void Confirm(object sender,RoutedEventArgs args)
        {
            reference.NickName = Nick.Text;
            Close();
        }
    }
}
