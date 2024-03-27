using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.ViewModel
{
    internal class LoginViewModel
    {
        public LoginViewModel() 
        {
        Storage.MainWindow window = new Storage.MainWindow();
            window.Show();
        }
    }
}
