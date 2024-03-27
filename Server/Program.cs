using System.Net.Sockets;
using System.Net;
using System.Text;
using Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Net.Http;
using System.Collections.ObjectModel;

namespace Server
{
    struct MyConection
    {
        public MyConection(int id, string name, string password,string user, CancellationTokenSource tokenSource)
        {
            this.id = id;
            this.name = name;
            this.password = password;
            this.tokenSource = tokenSource;
            this.user = user;
        }
        public int id;
        public string name;
        public string password;
        public string user;
        public CancellationTokenSource tokenSource;
    }
    internal class Program
    {
        private List<MyConection> conections = new();
        private List<Storage> Storages = new();
        private List<Supplier> Suppliers = new();
        List<Product> ProductNames = new List<Product>();//
        List<Order> ActiveListBox = new List<Order>();
        List<Supplie> PickedListBox = new List<Supplie>();
        TcpListener? _tcpListener;
        int nexIndex = 0;
        int maxUsers = 10;

        
        public async Task Start(string ip, string port)
        {
            try
            {
                string connectionString = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetConnectionString("SqlClient");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    IEnumerable<Storage> StorageResult = connection.Query<Storage>("SELECT * FROM [Storage]");

                    foreach (Storage value in StorageResult)
                    {
                        Storages.Add(value);
                    }

                    IEnumerable<Supplier> SupplierResult = connection.Query<Supplier>("SELECT * FROM [Supplier]");

                    foreach (Supplier value in SupplierResult)
                    {
                        Suppliers.Add(value);
                    }
                    Console.WriteLine("We Did it");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            try
            {
                _tcpListener = new TcpListener(IPAddress.Parse(ip), int.Parse(port));
                _tcpListener.Start(maxUsers + 1);
                await WaitForConnectionAsync(_tcpListener);
            }
            catch (Exception ex)
            {
                //Messagebo.Show($"Error. {ex.Message}");
            }
        }
        async Task Login(string[] input,TcpClient tcpClient) 
        {
            bool IsExist = false;
            if (input[2].Contains("storage"))
            {
                IsExist = Storages.Exists(s => s.Name == input[0] && s.Password == input[1]);
            }
            else
            {
                IsExist = Suppliers.Exists(s => s.Name == input[0] && s.Password == input[1]);
            }

            if (conections.Find(m => m.name == input[0]).name == input[0])
            {
                var msg = Encoding.UTF8.GetBytes("User with this name already conected. Use diferent name. disconnected");
                tcpClient.GetStream().Write(msg, 0, msg.Length);
                tcpClient.Close();

                
            }
            else if (maxUsers == 0)
            {
                var msg = Encoding.UTF8.GetBytes("Server is very busy and can't handle you. Try again later. disconnected");
                tcpClient.GetStream().Write(msg, 0, msg.Length);
                tcpClient.Close();

                
            }
            else if(IsExist)
            {
                var msg = Encoding.UTF8.GetBytes("Conection successful. You're connected");
                tcpClient.GetStream().Write(msg, 0, msg.Length);



                var tokenSourse = new CancellationTokenSource();
                var token = tokenSourse.Token;
                if (input[2] == "storage")
                {
                    var task = Task.Factory.StartNew(() => { StorageCheckUserAsync(tcpClient, input[0]); }, token);
                }
                else if (input[2] == "suplier")
                {
                    var task = Task.Factory.StartNew(() => { SuplierCheckUserAsync(tcpClient, input[0]); }, token);
                }


                conections.Add(new MyConection(nexIndex, input[0], input[1], input[2], tokenSourse));
                maxUsers -= 1;
                nexIndex++;

            }
        }
        async Task Register(string[] input, TcpClient tcpClient) 
        {
        
        }
        async Task WaitForConnectionAsync(TcpListener tcpListener)
        {
            while (true)
            {
                try
                {
                    string log;
                    var tcpClient = await tcpListener.AcceptTcpClientAsync();
                    var buff = new byte[1024];
                    tcpClient.GetStream().Read(buff);
                    var input = Encoding.UTF8.GetString(buff).Split("@", 4);
                    if (input[3].Contains("login")) 
                    {
                    await Login(input, tcpClient);
                    }
                    else if (input[3].Contains("register")) 
                    {
                        await Register(input, tcpClient);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }

        }



        private async Task StorageCheckUserAsync(TcpClient tcpClient, string name)
        {
            var quotes = 1;
            var client = conections.Find(m => m.name == name);
            var clientId = conections.FindIndex(m => m.name == name);
            var rnd = new Random();
            while (true)
            {
                try
                {
                    var ns = tcpClient.GetStream();
                    var buff = new byte[1024];
                    ns.Read(buff);
                    var message = Encoding.UTF8.GetString(buff);
                    if (message.Contains("Disconect") || !tcpClient.Connected)
                    {
                        buff = Encoding.UTF8.GetBytes(message);
                        ns.Write(buff, 0, message.Length);
                        tcpClient.Close();
                        conections.Remove(client);
                        maxUsers += 1;
                        client.tokenSource.Cancel();
                        break;
                    }

                    if (message.Contains("startup"))
                    {
                        ProductNames.Clear();
                        ActiveListBox.Clear();
                        PickedListBox.Clear();

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
                                    ProductNames.Add(value);

                                }
                                sql = @"SELECT o.Id, o.Count,p.Id, p.Name,p.Price,p.LifeInDays, m.Id, m.Name
                                FROM [Order] o
                                INNER JOIN Product p ON p.Id = o.ProductId
                                INNER JOIN Measure m ON p.MeasureId = m.Id";
                                IEnumerable<Order> resultOrder = connection.Query<Order, Product, Measure,  Order >(sql, (order, product, mesure) =>
                                {
                                    product.Measure = mesure;
                                    //product.Suppliers.Add(suplier);
                                    order.Product = product;

                                    return order;
                                });
                                foreach (Order value in resultOrder)
                                {
                                    ActiveListBox.Add(value);
                                }
                                sql = @"SELECT s.Id, s.Count,s.Price,s.ShippingDate,s.ArrivalDate,p.Id, p.Name,p.Price,p.LifeInDays, m.Id, m.Name,st.Id,st.Name,st.Password,st.MaxCapacity, sup.Id,sup.Name,sup.Password,sup.Rating
                                FROM [Supplie] s
                                INNER JOIN Product p ON p.Id = s.ProductId
                                INNER JOIN Measure m ON p.MeasureId = m.Id
                                INNER JOIN Storage st ON s.StorageId = st.Id
                                INNER JOIN Supplier sup ON s.SupplierId = sup.Id";
                                IEnumerable<Supplie> resultSupplies = connection.Query<Supplie, Product, Measure, Storage, Supplier, Supplie>(sql, (supplies, product, measure, storage, supplier) =>
                                {
                                    product.Measure = measure;
                                    supplies.Supplier = supplier;
                                    supplies.Storage = storage;
                                    supplies.Product = product;
                                    return supplies;
                                }, splitOn: "Name");
                                foreach (Supplie value in resultSupplies)
                                {
                                    PickedListBox.Add(value);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        message = ProductNames.Count.ToString();
                        buff = Encoding.UTF8.GetBytes(message);
                        ns.Write(buff, 0, buff.Length);
                        buff = new byte[1024];
                        ns.Read(buff);
                        foreach (Product product in ProductNames)
                        {
                            buff = Product.GetBytes(product);
                            ns.Write(buff, 0, buff.Length);
                            Console.WriteLine("Product sent");
                            buff = new byte[1024];
                            ns.Read(buff);
                            message = Encoding.UTF8.GetString(buff);
                        }

                        message = ActiveListBox.Count.ToString();
                        buff = Encoding.UTF8.GetBytes(message);
                        ns.Write(buff, 0, buff.Length);
                        buff = new byte[1024];
                        ns.Read(buff);
                        foreach (Order order in ActiveListBox)
                        {
                            buff = Order.GetBytes(order);
                            ns.Write(buff, 0, buff.Length);
                            Console.WriteLine("Order sent");
                            buff = new byte[1024];
                            ns.Read(buff);
                            message = Encoding.UTF8.GetString(buff);
                        }

                        message = PickedListBox.Count.ToString();
                        buff = new byte[1024];
                        buff = Encoding.UTF8.GetBytes(message);
                        ns.Write(buff, 0, buff.Length);
                        buff = new byte[1024];
                        ns.Read(buff);
                        message = Encoding.UTF8.GetString(buff);
                        foreach (Supplie supplie in PickedListBox)
                        {
                            buff = Supplie.GetBytes(supplie);
                            ns.Write(buff, 0, buff.Length);
                            Console.WriteLine("Supplie sent");
                            buff = new byte[1024];
                            ns.Read(buff);
                            message = Encoding.UTF8.GetString(buff);
                        }

                    }

                    if (message.Contains("send"))
                    {
                        try
                        {
                            using (SqlConnection connection = new SqlConnection(new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetConnectionString("SqlClient")))
                            {
                                message = "qwerty";
                                buff = new byte[1024];
                                buff = Encoding.UTF8.GetBytes(message);
                                ns.Write(buff, 0, buff.Length);
                                buff = new byte[1024];
                                ns.Read(buff);
                                Order order = Order.GetOrder(buff);
                                connection.Open();
                                connection.Query<Order>("INSERT [Order] VALUES (@ProductId,@StorageId, @Count,1)",
                            new { ProductId = order.Product.Id, StorageId = 1, Count = order.Count });
                                Console.WriteLine("Order Inserted");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }
        }
        private async Task SuplierCheckUserAsync(TcpClient tcpClient, string name)
        {
            var quotes = 1;
            var client = conections.Find(m => m.name == name);
            var clientId = conections.FindIndex(m => m.name == name);
            var rnd = new Random();
            while (true)
            {
                try
                {
                    var ns = tcpClient.GetStream();
                    var buff = new byte[1024];
                    ns.Read(buff);
                    var message = Encoding.UTF8.GetString(buff);
                    if (message.Contains("Disconect") || !tcpClient.Connected)
                    {
                        buff = Encoding.UTF8.GetBytes(message);
                        ns.Write(buff, 0, message.Length);
                        tcpClient.Close();
                        conections.Remove(client);
                        maxUsers += 1;
                        client.tokenSource.Cancel();
                        break;
                    }


                    if (quotes > 0 && message.Contains("quote"))
                    {
                        message = "qweqweqwe";
                        var msg = Encoding.UTF8.GetBytes(message);
                        ns.Write(msg, 0, message.Length);
                        quotes -= 1;
                    }
                    else if (quotes == 0)
                    {

                        message = "Your limit of quotes have reached maximum";
                        var msg = Encoding.UTF8.GetBytes(message);
                        ns.Write(msg, 0, message.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }
        }
    }
    }

