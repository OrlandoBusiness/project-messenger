using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;
using Storage.ViewModel;
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
            DataContext = new MainWindowViewModel(this);
            
        }
        /*
        private void Button_Click(object sender, RoutedEventArgs e) Click="Button_Click"
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetConnectionString("SqlClient")))
                {
                    connection.Open();
                    connection.Query<Order>("INSERT [Order] VALUES (@ProductId,@StorageId, @Count,1)",
                new { ProductId = products.Find(p => p.Name == ProductNames.SelectedItem).Id, StorageId = 1, Count = IntegerCount.Value });


                    var sql = @"SELECT o.Id, o.Count, p.Name
                            FROM [Order] o
                            INNER JOIN Product p ON p.Id = o.ProductId";
                    IEnumerable<Order> resultOrder = connection.Query<Order, Product, Order>(sql, (order, product) =>
                    {
                        order.Product = product;
                        return order;
                    }, splitOn: "Name");
                    ActiveListBox.Items.Clear();
                foreach (Order value in resultOrder)
                {
                    ActiveListBox.Items.Add($"{value.Id},{value.Product.Name},{value.Count}");
                }
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
            var obj = products.Find(p => p.Name == ProductNames.SelectedItem); SelectionChanged = "ProductNames_SelectionChanged"
            MeasureLabel.Content = obj.Measure.Name;
        }*/
    }
}