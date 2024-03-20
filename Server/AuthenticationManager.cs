using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class AuthenticationManager
    {
        private List<User> users;

        public AuthenticationManager()
        {
       
            users = new List<User>();
        }

        
        public void AddUser(User user)
        {
            users.Add(user);
        }

        
        public User Authenticate(string username, string password)
        {
            foreach (User user in users)
            {
                if (user.Username == username && user.Password == password)
                {
                    return user; 
                }
            }
            return null; 
        }
    }
}
