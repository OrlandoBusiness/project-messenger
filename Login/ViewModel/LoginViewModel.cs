using Login.View;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Login.ViewModel;

public class LoginViewModel
{
    private TcpClient _tcpClient;
    private NetworkStream _stream;
    private MainWindow _mainWindow;
    private ICommand _loginCommand;
    private ICommand _registerCommand;
    private IPEndPoint _endPoint = new IPEndPoint(IPAddress.Parse("26.178.12.137"), 7969);

    public LoginViewModel(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public void Connect(string user, string password, string wh, string enterState)
    {
        try
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect(_endPoint);
            _stream = _tcpClient.GetStream();
            var message = user + "@" + password + "@" + "storage" + "@" + "login";
            byte[] buff = Encoding.UTF8.GetBytes(message);
            _stream.Write(buff, 0, message.Length);
            buff = new byte[1024];
            _stream.Read(buff);
            string msg = Encoding.UTF8.GetString(buff);
            MessageBox.Show(msg);

            if (msg.Contains("disconnected"))
            {

            }
            else if (msg.Contains("connected"))
            {
                Storage.View.MainWindow window = new Storage.View.MainWindow(_tcpClient);
                window.Show();

                _mainWindow.Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error. {ex.Message}");
        }
    }

    public ICommand? LoginCommand => _loginCommand ??= new Infrastructure.RelayCommand(
            param =>
            {
                Connect(_mainWindow.LoginTextBox.Text, _mainWindow.PasswordTextBox.Text, (_mainWindow.StorageButton.IsChecked == true) ? "storage" : "supplier", "login");
            });

    public ICommand? RegisterCommand => _registerCommand ??= new Infrastructure.RelayCommand(
            param =>
            {
                Connect(_mainWindow.LoginTextBox.Text, _mainWindow.PasswordTextBox.Text, (_mainWindow.StorageButton.IsChecked == true) ? "storage" : "supplier", "login");
            });
}
