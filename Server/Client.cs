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
            List<Record> records = dbManager.GetRecords(tableName);
        }

        public void AddRecord(string tableName, Record record)
        {
            dbManager.AddRecord(tableName, record);
        }

        public void UpdateRecord(string tableName, Record record)
        {
            dbManager.UpdateRecord(tableName, record);
        }
    }
}
