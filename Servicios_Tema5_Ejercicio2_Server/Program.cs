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

        static object key = new Object();

        static List<Socket> allClietsOnServer;

        static void Main(string[] args)
        {
            int puerto = 31416;
            Thread thread;
            allClietsOnServer = new List<Socket>();

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
            string message;

            Socket client = (Socket)socket;
            IPEndPoint ieClient = (IPEndPoint)client.RemoteEndPoint;
            Console.WriteLine("conectado al puerto {0}", ieClient.Port);

            //meter esta zona en el cliente, darle una visual
            using (NetworkStream ns = new NetworkStream(client))
            using (StreamReader sr = new StreamReader(ns))
            using (StreamWriter sw = new StreamWriter(ns))
            {

                //mandamos al cliente un mensaje de bienvenida :) ¿Es un elemento común?
                sw.WriteLine("Wellcome, who are you");//你好， 你是谁?
                sw.Flush();

                Client clien = new Client(client, sr.ReadLine());
                allClietsOnServer.Add(client);

                Console.WriteLine("the user conect to port{0}, is {1}", ieClient.Port, clien.Name);
                sw.WriteLine("Welcome {0}.", clien.Name);
                sw.Flush();

                while (true)
                {
                    try
                    {
                        
                         message = sr.ReadLine(); //protegemos la variable común
                        
                        if (message != null)
                        {
                            lock (key)
                            {
                                for (int i = 0; i < allClietsOnServer.Count; i++)
                                {
                                    using (NetworkStream nsInside = new NetworkStream(allClietsOnServer[i]))
                                    using (StreamWriter swInside = new StreamWriter(nsInside))
                                    {
                                        swInside.WriteLine("{0}:{1} ",clien.Name,message);
                                        swInside.Flush();
                                    }

                                }
                            } 
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
}
