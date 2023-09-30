using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            int port;
            const string hostname = "localhost";
            ServerPort clientPort;
            GUIChatClient clientLogic;
            string message;
            string nick;
            string portString;

            clientLogic = new GUIChatClient(false, "localhost", 1001, "localhost");

            Console.WriteLine("This is DAD Chat...");
            Console.WriteLine("What is your nickname?");
            nick = Console.ReadLine();
            Console.WriteLine("Which port do you want to use? Pick something different from the server and other clients...");

            portString = Console.ReadLine();

            clientLogic.Register(nick, portString);

            Console.WriteLine("Client is listening for messages.");
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            Console.WriteLine("Press enter to send a message (send 'q!' to exit).");
            while (true)
            {
                Console.ReadKey();
                lock (clientLogic)
                {
                    Console.Write("me (" + nick + "): ");
                    message = Console.ReadLine();
                    if (message == "q!")
                    {
                        clientLogic.ServerShutdown();
                        return;
                    }
                    else
                    {
                        clientLogic.BcastMsg(message);
                    }
                }
            }
        }
    }
}

