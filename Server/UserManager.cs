using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class UserManager
    {
        private List<User> users;

        public UserManager()
        {
            
            users = new List<User>();
        }

        
        public void AddUser(User user)
        {
            users.Add(user);
        }

        
        public void DeleteUser(string username)
        {
            
            User userToRemove = users.Find(u => u.Username == username);
            if (userToRemove != null)
            {
                users.Remove(userToRemove);
            }
        }

        
        public void UpdateUserRole(string username, string newRole)
        {
            
            User userToUpdate = users.Find(u => u.Username == username);
            if (userToUpdate != null)
            {
                userToUpdate.Role = newRole;
            }
        }
    }
}
