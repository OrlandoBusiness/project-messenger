using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Server
{
    struct MyConection
    {
        public MyConection(int id, string name, string password, CancellationTokenSource tokenSource)
        {
            this.id = id;
            this.name = name;
            this.password = password;
            this.tokenSource = tokenSource;
        }
        public int id;
        public string name;
        public string password;
        public CancellationTokenSource tokenSource;
    }
    internal class Program
    {
        static void Main(string[] args)
        {

        }
        private List<MyConection> conections = new();
        TcpListener? _tcpListener;
        int nexIndex = 0;
        int maxUsers = 2;

        
        public void Start(string ip, string port)
        {

            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));
            try
            {
                _tcpListener = new TcpListener(IPAddress.Parse(ip), int.Parse(port));
                _tcpListener.Start(maxUsers + 1);
                _ = WaitForConnectionAsync(_tcpListener);
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Error. {ex.Message}");
            }
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
                    var input = Encoding.UTF8.GetString(buff).Split(" ", 2);
                    if (conections.Find(m => m.name == input[0]).name == input[0])
                    {
                        var msg = Encoding.UTF8.GetBytes("User with this name already conected. Use diferent name. Disconnected");
                        tcpClient.GetStream().Write(msg, 0, msg.Length);
                        tcpClient.Close();
                        
                        continue;
                    }
                    else if (maxUsers == 0)
                    {
                        var msg = Encoding.UTF8.GetBytes("Server is very busy and can't handle you. Try again later. Disconnected");
                        tcpClient.GetStream().Write(msg, 0, msg.Length);
                        tcpClient.Close();
                        
                        continue;
                    }
                    else
                    {
                        var msg = Encoding.UTF8.GetBytes("Conection successful. You're Connected");
                        tcpClient.GetStream().Write(msg, 0, msg.Length);



                        var tokenSourse = new CancellationTokenSource();
                        var token = tokenSourse.Token;
                        var task = Task.Factory.StartNew(() => { CheckUserAsync(tcpClient, input[0], 10); }, token);



                        conections.Add(new MyConection(nexIndex, input[0], input[1], tokenSourse));
                        maxUsers -= 1;
                        nexIndex++;
                        
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }

        }



        private async Task CheckUserAsync(TcpClient tcpClient, string name, int quotes)
        {
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
                    if (message.Contains("Disconect"))
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
                    //Console.WriteLine($"{ex.Message}");
                }
            }
        }
    }
    }

