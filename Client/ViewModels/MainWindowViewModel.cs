using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfMvvmPhonebook_P1415.Infrastructure;
using System.ComponentModel;

namespace Client.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        TcpClient _tcpClient;
        NetworkStream _stream;

        Timer _timer;
        private ICommand? sendButton;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand? SendButton => sendButton ??= new RelayCommand(
            param =>
            {

               

            });
        public MainWindowViewModel() 
        {
            
            //_tcpClient = tcpClient;
            _stream = _tcpClient.GetStream();
        }
    }
}
