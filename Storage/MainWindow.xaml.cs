using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Storage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Product> products = new List<Product>();
        public MainWindow()
        {
            InitializeComponent();
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
                    IEnumerable<Product> resultProduct = connection.Query<Product, Measure, Supplier, Product >(sql, (product, mesure,suplier) => 
                    {
                        product.Measure = mesure;
                        product.Suppliers.Add(suplier);
                        return product;
                    });

                    foreach (Product value in resultProduct)
                    {
                        products.Add(value);
                        ProductNames.Items.Add(value.Name);
                    }
                    sql = @"SELECT o.Id, o.Count, p.Name
                            FROM [Order] o
                            INNER JOIN Product p ON p.Id = o.ProductId";
                    IEnumerable<Order> resultOrder = connection.Query<Order, Product, Order>(sql, (order, product) =>
                    {
                        order.Product = product;
                        return order;
                    },splitOn:"Name");
                    foreach (Order value in resultOrder) 
                    {
                        ActiveListBox.Items.Add($"{value.Id},{value.Product.Name},{value.Count}");
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
                        PickedListBox.Items.Add($"{value.Id},{value.Product.Name},{value.Count},{value.Price},{value.ShippingDate.ToShortDateString()}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetConnectionString("SqlClient")))
                {
                    connection.Open();
                    connection.Query<Order>("INSERT [Order] VALUES (@ProductId,@StorageId, @Count,1)",
                new { ProductId = products.Find(p => p.Name == ProductNames.SelectedItem).Id, StorageId = 1, Count = IntegerCount.Value });

                }
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ProductNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SendButton.IsEnabled = true;
            var obj = products.Find(p => p.Name == ProductNames.SelectedItem);
            MeasureLabel.Content = obj.Measure.Name;
        }
    }
}