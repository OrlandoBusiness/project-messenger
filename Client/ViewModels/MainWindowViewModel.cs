using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;
using Supplier.Infrastructure;
using Supplier.View;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Supplier.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private MainWindow _window;
    private ICommand? _sendButton;
    private string _connectionString = string.Empty;

    private Order? _selectedOrder;
    public Order SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            if (_selectedOrder != value)
            {
                _selectedOrder = value;
                NotifyPropertyChanged();
            }
        }
    }

    public ObservableCollection<Order>? Orders { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand? SendButton => _sendButton ??= new RelayCommand(
        param =>
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    if (SelectedOrder.Count > _window.SupplieCount.Value)
                    {
                        if (InsertSupllie(new Supplies { Product = SelectedOrder.Product, Supplier = new Model.Supplier { Id = 1 }, Storage = new Storage { Id = 1 }, Count = (int)_window.SupplieCount.Value, Price = 1, ArrivalDate = _window.ArrivedDate.SelectedDate.Value }))
                        {
                            connection.ExecuteScalar("EXEC DecreaseAt @Id, @Count", new { Id = SelectedOrder.Id, Count = _window.SupplieCount.Value });
                        }
                    }
                    else
                    {
                        if (InsertSupllie(new Supplies { Product = SelectedOrder.Product, Supplier = new Model.Supplier { Id = 1 }, Storage = new Storage { Id = 1 }, Count = (int)_window.SupplieCount.Value, Price = 1, ArrivalDate = _window.ArrivedDate.SelectedDate.Value }))
                        {
                            connection.ExecuteScalar("DELETE [Order] WHERE [Order].Id = @Id", new { Id = SelectedOrder.Id });
                        }
                    }
                    UpdateOrders();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

    private bool InsertSupllie(Model.Supplies supplies)
    {
        try
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            connection.Query<Model.Supplies>("INSERT Supplie VALUES (@ProductId, @SupplierId, @StorageId, @Count, @Price, @ShippingDate, @ArrivalDate)",
                new { ProductId = supplies.Product.Id, SupplierId = supplies.Supplier.Id, StorageId = supplies.Storage.Id, Count = supplies.Count, Price = supplies.Price, ShippingDate = DateTime.Now, ArrivalDate = supplies.ArrivalDate });

            return true;
        }
        catch
        {
            return false;
        }
    }

    public MainWindowViewModel(MainWindow window)
    {
        _window = window;
        _connectionString = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetConnectionString("SqlClient");

        Orders = new ObservableCollection<Order>();

        try
        {
            UpdateOrders();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    public void UpdateOrders()
    {
        Orders.Clear();

        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        IEnumerable<Order> result = connection.Query<Order, Product, Order>("SELECT * FROM [Order] JOIN Product ON [Order].ProductId = Product.Id", (o, p) => { o.Product = p; return o; });

        foreach (Order value in result)
        {
            Orders.Add(value);
        }
    }

    public void NotifyPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
