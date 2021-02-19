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
        public string Name { set; get; }
        public Socket SocketClient { set; get; }
        public IPEndPoint ForIp { set; get; }
        

        public Client(Socket socket, string name)
        {

            if(name.Length == 0)
            {
                this.Name = "Desconocido";
            }
            else
            {
                this.Name = name;
            }

            this.SocketClient = socket;
            this.ForIp = (IPEndPoint)this.SocketClient.RemoteEndPoint;
        }
    }
}
