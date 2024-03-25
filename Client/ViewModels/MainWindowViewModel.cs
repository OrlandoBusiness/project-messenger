using Supplier.Infrastructure;
using System.ComponentModel;
using System.Net.Sockets;
using System.Windows.Input;

namespace Supplier.ViewModels;

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
