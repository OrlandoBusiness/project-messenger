namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Administrator admin = new Administrator();
            admin.ManageUsers();
            admin.ManageData();
            admin.ManageSettings();

            Client client = new Client();
            client.ViewData();
            client.AddRecord("Suppliers", new Record());
            client.UpdateRecord("Products", new Record());
        }
    }
}
