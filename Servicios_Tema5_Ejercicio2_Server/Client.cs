using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Servicios_Tema5_Ejercicio2_Server
{
    class Client
    {
        private string name;
        public string Name
        {
            set
            {
                
                name = value;
                
            }
            get
            {
                return name;
            }
        }
        public Socket SocketClient { set; get; }
        public IPEndPoint ForIp { set; get; }
        

        public Client(Socket socket)
        {

            

            this.SocketClient = socket;
            this.ForIp = (IPEndPoint)this.SocketClient.RemoteEndPoint;
        }
    }
}
