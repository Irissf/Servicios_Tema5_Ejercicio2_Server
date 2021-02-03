using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicios_Tema5_Ejercicio2_Server
{
    class Program
    {
        static string message;
        static List<IPEndPoint> clients;

        static void Main(string[] args)
        {
            int puerto = 31416;
            clients = new List<IPEndPoint>();
            Thread thread;

            //IPEndPoint => Representa un punto de conexión de red como una dirección IP y un número de puerto.
            IPEndPoint ie = new IPEndPoint(IPAddress.Any, puerto);

            //recordar que meter entre parentesis
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //si puerto libre
                socket.Bind(ie);
                Console.WriteLine("puerto correcto");

                socket.Listen(10);
                while (true)
                {
                    Socket client = socket.Accept();
                    thread = new Thread(ClientThread);
                    thread.Start(client);
                }

            }
            catch (SocketException e) when (e.ErrorCode == (int)SocketError.AddressAlreadyInUse)
            {

                Console.WriteLine("puerto en uso");
            }
            socket.Close();
            Console.ReadLine();
        }

        static void ClientThread(object socket)
        {
            Socket client = (Socket)socket;
            IPEndPoint ieClient = (IPEndPoint)client.RemoteEndPoint;
            Console.WriteLine("conectado al puerto {0}", ieClient.Port);
            clients.Add(ieClient);

            using (NetworkStream ns = new NetworkStream(client))
            using (StreamReader sr = new StreamReader(ns))
            using (StreamWriter sw = new StreamWriter(ns))
            {
                //mandamos al cliente un mensaje de bienvenida :)
                sw.WriteLine("Wellcome, who are you");//你好， 你是谁?
                sw.Flush();

                try
                {
                    message = sr.ReadLine();
                    if (message != null)
                    {
                        sw.WriteLine(message);
                        sw.Flush();
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }
    }
}
