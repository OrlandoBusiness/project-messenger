using System.IO;
using System.Windows;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;
using Supplier.ViewModels;
using Dapper;

namespace Supplier.View;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(this);
    }
}