using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;
using Storage.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Storage.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        List<Product> products = new List<Product>();
        public ObservableCollection<string> ProductNames { get; set; }
        public ObservableCollection<string> ActiveListBox { get; set; }
        public ObservableCollection<string> PickedListBox
        {
            get; set;
        }
        public int CountValue { get; set; } = 0;
        private ICommand sendCommand;
        private ICommand selectionChangedCommand;
        private MainWindow mainWodow;
        private string? selectedProduct;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string? SelectedProduct
        {
            get => selectedProduct;
            set
            {
                if (selectedProduct != value)
                {
                    selectedProduct = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainWindowViewModel(MainWindow window) 
        {
            mainWodow = window;
            ProductNames = new ObservableCollection<string>();
            ActiveListBox = new ObservableCollection<string>();
            PickedListBox = new ObservableCollection<string>(); 
            try
            {
                string connectionString = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetConnectionString("SqlClient");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var sql = @"SELECT p.Id, p.Name,p.Price,p.LifeInDays, m.Id, m.Name, s.Password , s.Id, s.Name
                                FROM [Product] p 
                                INNER JOIN Measure m ON p.MeasureId = m.Id
                                INNER JOIN ProductSupplier ps ON p.Id = ps.ProductId
                                INNER JOIN Supplier s ON ps.SupplierId = s.Id";
                    IEnumerable<Product> resultProduct = connection.Query<Product, Measure, Supplier, Product>(sql, (product, mesure, suplier) =>
                    {
                        product.Measure = mesure;
                        product.Suppliers.Add(suplier);
                        return product;
                    });

                    foreach (Product value in resultProduct)
                    {
                        products.Add(value);
                        ProductNames.Add(value.Name);
                        
                    }
                    MessageBox.Show(ProductNames.Count.ToString());
                    sql = @"SELECT o.Id, o.Count, p.Name
                            FROM [Order] o
                            INNER JOIN Product p ON p.Id = o.ProductId";
                    IEnumerable<Order> resultOrder = connection.Query<Order, Product, Order>(sql, (order, product) =>
                    {
                        order.Product = product;
                        return order;
                    }, splitOn: "Name");
                    foreach (Order value in resultOrder)
                    {
                        ActiveListBox.Add($"{value.Id},{value.Product.Name},{value.Count}");
                    }
                    sql = @"SELECT s.Id, s.Count,s.Price,s.ShippingDate, p.Name
                            FROM [Supplie] s
                            INNER JOIN Product p ON p.Id = s.ProductId";
                    IEnumerable<Supplies> resultSupplies = connection.Query<Supplies, Product, Supplies>(sql, (supplies, product) =>
                    {
                        supplies.Product = product;
                        return supplies;
                    }, splitOn: "Name");
                    foreach (Supplies value in resultSupplies)
                    {
                        PickedListBox.Add($"{value.Id},{value.Product.Name},{value.Count},{value.Price},{value.ShippingDate.ToShortDateString()}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public ICommand? SelectionChangedCommand => selectionChangedCommand ??= new RelayCommand(param =>
        {
            var obj = products.Find(p => p.Name == selectedProduct);
            mainWodow.MeasureLabel.Content = obj.Measure.Name;
        });
        public ICommand? SendCommand => sendCommand ??= new RelayCommand(
            param =>
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetConnectionString("SqlClient")))
                    {
                        connection.Open();
                        connection.Query<Order>("INSERT [Order] VALUES (@ProductId,@StorageId, @Count,1)",
                    new { ProductId = products.Find(p => p.Name == selectedProduct).Id, StorageId = 1, Count = mainWodow.IntegerCount.Value });


                        var sql = @"SELECT o.Id, o.Count, p.Name
                            FROM [Order] o
                            INNER JOIN Product p ON p.Id = o.ProductId";
                        IEnumerable<Order> resultOrder = connection.Query<Order, Product, Order>(sql, (order, product) =>
                        {
                            order.Product = product;
                            return order;
                        }, splitOn: "Name");
                        ActiveListBox.Clear();
                        foreach (Order value in resultOrder)
                        {
                            ActiveListBox.Add($"{value.Id},{value.Product.Name},{value.Count}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            });
    }
}
