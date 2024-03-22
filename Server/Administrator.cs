using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Administrator
    {
        private DatabaseManager dbManager;
        private UserManager userManager;

        public Administrator()
        {
            dbManager = new DatabaseManager();
            userManager = new UserManager();
        }

        public void ManageUsers()
        {
            User newUser = new User("username", "password", UserRole.Admin);
            userManager.AddUser(newUser);

           
            userManager.DeleteUser("username");

       
            userManager.ChangeUserRole("username", UserRole.Regular);
        }

        public void ManageData()
        {
            
            Record newRecord = new Record();
            dbManager.AddRecord("TableName", newRecord);

            
            dbManager.DeleteRecord("TableName", recordID);

            
            dbManager.UpdateRecord("TableName", updatedRecord);
        }

        public void ManageSettings()
        {
            Settings currentSettings = LoadSettings();

            SettingsEditor settingsEditor = new SettingsEditor(currentSettings);
            settingsEditor.ShowDialog();

            if (settingsEditor.DialogResult == DialogResult.OK)
            {

                
                Settings updatedSettings = settingsEditor.GetUpdatedSettings();

               
                SaveSettings(updatedSettings);

               
                ApplySettings(updatedSettings);

            }
        }

        private Settings LoadSettings()
        {
            Settings settings = new Settings();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT SettingName, SettingValue FROM SettingsTable";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string settingName = reader["SettingName"].ToString();
                    string settingValue = reader["SettingValue"].ToString();
                }

                return settings;
            }

        }

        private Settings SaveSettings()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE SettingsTable SET SettingValue = @SettingValue WHERE SettingName = @SettingName";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                foreach (var setting in settings)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@SettingName", setting.Key);
                    command.Parameters.AddWithValue("@SettingValue", setting.Value);
                    command.ExecuteNonQuery();
                }
            }

        }

        private Settings ApplySettings()
        {
            if (settings.ContainsKey("AutoUpdateEnabled") && settings["AutoUpdateEnabled"] == "true")
            {
                ModuleManager.ReloadModules();
            }

        }
    }
}
