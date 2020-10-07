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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServerPropertyViewModel model { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            model = new ServerPropertyViewModel(this);
            DataContext = model;
        }
        string temTitle;
        string temMessage;
        public void AddChatLog(string Title,string Message)
        {
            temTitle = Title;
            temMessage = Message;
            Dispatcher.Invoke(_addChatLog);
        }
        private void _addChatLog()
        {
            ChatViewData dat = new ChatViewData();
            dat.Title = temTitle;
            dat.Message = temMessage;
            ChatView xd = new ChatView(dat);
            ServerLogPanel.Children.Add(xd);
        }
    }
}
