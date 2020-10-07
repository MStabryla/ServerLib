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

namespace ClientCommandServer
{
    /// <summary>
    /// Interaction logic for Mess.xaml
    /// </summary>
    public partial class Mess : Border
    {
        public Mess(string source,string mess)
        {
            InitializeComponent();
            Source.Text = source;
            Content.Text = mess;
        }
    }
}
