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

        static List<Client> allClietsOnServer;

        static void Main(string[] args)
        {
            int puerto = 31416;
            Thread thread;
            allClietsOnServer = new List<Client>();
            IPHostEntry info = Dns.GetHostEntry("localhost");

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
                    Client clien = new Client(client);//creo al cliente
                    lock (key)
                    {
                        allClietsOnServer.Add(clien);//lo meto en la colección
                    }
                    thread = new Thread(ClientThread);
                    thread.Start(clien);//lanzamos el hilo del cliente y le paso el cliente
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

            bool exit = false;
            string message;
            Client clientObject = (Client)socket;

            Socket client = clientObject.SocketClient;
            IPEndPoint ieClient = (IPEndPoint)client.RemoteEndPoint;
            Console.WriteLine("conectado al puerto {0}", ieClient.Port);

            using (NetworkStream ns = new NetworkStream(client))
            using (StreamReader sr = new StreamReader(ns))
            using (StreamWriter sw = new StreamWriter(ns))
            {

                sw.WriteLine("Wellcome, who are you");
                sw.Flush();
                string name = sr.ReadLine();
                clientObject.Name = name; //meto el nombre del cliente

                lock (key) //avisamos que alguien se ha conectado al resto de clientes
                {
                    for (int i = 0; i < allClietsOnServer.Count; i++)
                    {
                        if (client != allClietsOnServer[i].SocketClient)
                        {
                            //Solo mandará el mensaje a los otros usuarios
                            using (NetworkStream nsInside = new NetworkStream(allClietsOnServer[i].SocketClient))
                            using (StreamWriter swInside = new StreamWriter(nsInside))
                            {
                                swInside.WriteLine("{0} se ha conectado", name);
                                swInside.Flush();
                            }
                        }

                    }
                }

                while (!exit)
                {
                    try
                    {

                        message = sr.ReadLine();
                        //mejor hacer un toUpper al mensaje de esa forma no hacemos tantos case
                        if (message != null)
                        {
                            switch (message)
                            {
                                case "#EXIT":
                                case "#exit":
                                case "#Exit":
                                    lock (key)
                                    {
                                        allClietsOnServer.Remove(clientObject);
                                        for (int i = 0; i < allClietsOnServer.Count; i++)
                                        {
                                            using (NetworkStream nsInside = new NetworkStream(allClietsOnServer[i].SocketClient))
                                            using (StreamWriter swInside = new StreamWriter(nsInside))
                                            {
                                                swInside.WriteLine("{0} se ha desconectado ", clientObject.Name);
                                                swInside.Flush();
                                            }
                                        }
                                    }
                                    exit = true;
                                    break;
                                case "#LIST":
                                case "#list":
                                case "#List":
                                    sw.WriteLine("Lista de personas conectadas al servidor");
                                    sw.Flush();
                                    lock (key)
                                    {
                                        for (int i = 0; i < allClietsOnServer.Count; i++)
                                        {
                                            sw.WriteLine(allClietsOnServer[i].Name);
                                            sw.Flush();
                                        }
                                    }
                                    break;
                                default:
                                    lock (key)
                                    {
                                        for (int i = 0; i < allClietsOnServer.Count; i++)
                                        {
                                            if (client != allClietsOnServer[i].SocketClient)
                                            {
                                                //Solo mandará el mensaje a los otros usuarios
                                                using (NetworkStream nsInside = new NetworkStream(allClietsOnServer[i].SocketClient))
                                                using (StreamWriter swInside = new StreamWriter(nsInside))
                                                {
                                                    swInside.WriteLine("{0}@{1}:{2} ", clientObject.Name, clientObject.ForIp.Address, message);
                                                    swInside.Flush();
                                                }
                                            }

                                        }
                                    }
                                    break;
                            }
                        }
                        else
                        {

                            for (int i = 0; i < allClietsOnServer.Count; i++)
                            {
                                using (NetworkStream nsInside = new NetworkStream(allClietsOnServer[i].SocketClient))
                                using (StreamWriter swInside = new StreamWriter(nsInside))
                                {
                                    swInside.WriteLine("{0} se ha desconectado ", clientObject.Name);
                                    swInside.Flush();
                                }
                            }
                            
                            exit = true;
                            client.Close();

                        }

                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }

                lock (key) // eliminar de la colección de clientes
                {
                    exit = true;
                    client.Close();
                    allClietsOnServer.Remove(clientObject);
                }
            }
        }



    }
}

