using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;
using Storage.Infrastructure;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Storage.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        List<Product> _products = new List<Product>();
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Order> ActiveListBox { get; set; }
        public ObservableCollection<Supplie> PickedListBox { get; set; }
        public int CountValue { get; set; } = 0;

        private TcpClient _tcpClient;
        private NetworkStream _stream;

        private ICommand _sendCommand;
        private View.MainWindow _mainWodow;

        private Product? _selectedProduct;
        private string? _selectedMeasure;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (_selectedProduct != value)
                {
                    _selectedProduct = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string? SelectedMeasure
        {
            get => _selectedMeasure;
            set
            {
                if (_selectedMeasure != value)
                {
                    _selectedMeasure = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            if (name == "SelectedProduct")
            {
                Product product = _products.Find(p => p.Name == _selectedProduct.Name);
                SelectedMeasure = product.Measure.Name;
            }
        }

        public MainWindowViewModel(View.MainWindow window, TcpClient tcpClient)
        {
            _mainWodow = window;
            _tcpClient = tcpClient;
            _stream = _tcpClient.GetStream();

            Products = new ObservableCollection<Product>();
            ActiveListBox = new ObservableCollection<Order>();
            PickedListBox = new ObservableCollection<Supplie>();

            try
            {
                _stream.Write(Encoding.UTF8.GetBytes("startup"));

                byte[] buffer = new byte[1024];
                _stream.Read(buffer);

                int productLength = int.Parse(Encoding.UTF8.GetString(buffer));
                _stream.Write(Encoding.UTF8.GetBytes("start"));

                for (int i = 0; i < productLength; i++)
                {
                    _stream.Read(buffer);
                    Product product = Model.Product.GetProduct(buffer);

                    Products.Add(product);

                    _stream.Write(Encoding.UTF8.GetBytes("continue"));
                }

                buffer = new byte[1024];
                _stream.Read(buffer);
                int orderLength = int.Parse(Encoding.UTF8.GetString(buffer));
                _stream.Write(Encoding.UTF8.GetBytes("start"));

                for (int i = 0; i < orderLength; i++)
                {
                    _stream.Read(buffer);
                    Order order = Model.Order.GetOrder(buffer);

                    ActiveListBox.Add(order);

                    _stream.Write(Encoding.UTF8.GetBytes("continue"));
                }

                buffer = new byte[1024];
                _stream.Read(buffer);
                int supplieLegth = int.Parse(Encoding.UTF8.GetString(buffer));
                _stream.Write(Encoding.UTF8.GetBytes("start"));

                for (int i = 0; i < supplieLegth; i++)
                {
                    _stream.Read(buffer);
                    Supplie supplie = Model.Supplie.GetSupplie(buffer);

                    PickedListBox.Add(supplie);

                    _stream.Write(Encoding.UTF8.GetBytes("continue"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //{ ProductId = _products.Find(p => p.Name == _selectedProduct.Name).Id, StorageId = 1, Count = _mainWodow.IntegerCount.Value });
        public ICommand? SendCommand => _sendCommand ??= new RelayCommand(
            param =>
            {
                try
                {
                    _stream.Write(Encoding.UTF8.GetBytes("send"));
                    _stream.Read(null);

                    Order newOrder = new Order { Id = 1, Product = SelectedProduct, Count = _mainWodow.IntegerCount.Value.Value, IsActive = true };
                    _stream.Write(Order.GetBytes(newOrder));

                    ActiveListBox.Add(newOrder);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            });
    }
}
