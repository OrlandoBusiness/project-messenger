using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Client
    {
        private DatabaseManager dbManager;

        public Client()
        {
            dbManager = new DatabaseManager();
        }

        public void ViewData()
        {
            // Логика просмотра данных из базы данных
        }

        public void AddRecord(string tableName, Record record)
        {
            // Логика добавления записи в базу данных
        }

        public void UpdateRecord(string tableName, Record record)
        {
            // Логика обновления записи в базе данных
        }
    }
}
