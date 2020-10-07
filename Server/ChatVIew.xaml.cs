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
    /// Interaction logic for ChatVIew.xaml
    /// </summary>
    public partial class ChatView : Border
    {
        private ChatViewData data { get; set; }
        public ChatView(ChatViewData _data)
        {
            InitializeComponent();
            data = _data;
            DataContext = data;
        }
    }
}
