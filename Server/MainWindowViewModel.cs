using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ServerCommandSystem
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            _name = "Test name";
            _test = "Test";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string obj)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(obj));
        }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        private string _test;
        public string Test
        {
            get
            {
                return _test;
            }
            set
            {
                OnPropertyChanged("Test");
            }
        }
        public ICommand Use
        {
            get
            {
                return new RelayCommand(UseExec);
            }
        }
        private void UseExec()
        {
            MessageBox.Show("test");
        }
    }
}
