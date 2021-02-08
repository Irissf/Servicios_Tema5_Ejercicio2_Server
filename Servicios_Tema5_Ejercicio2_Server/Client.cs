using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Servicios_Tema5_Ejercicio2_Server
{
    class Client
    {
        public string name;
        public Socket socket;

        public Client(Socket socket)
        {
            this.socket = socket;
        }
    }
}
