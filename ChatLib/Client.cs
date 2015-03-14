using System;
using System.Net.Sockets;
using System.Net;

namespace ChatLib
{
    /// <summary>
    /// A chat client for a TCPClient/TCPListener chat
    /// application. Inherits from the chat service which
    /// handles the sending and recieving of messages 
    /// on the stream.
    /// </summary>
    public class Client : ChatService
    {

        private static int defaultPort = 12201; // the default port

        /// <summary>
        /// Creates a new client with the provided
        /// IPAddress and port.
        /// </summary>
        /// <param name="IP">The IPAddress used for a server connection</param>
        /// <param name="port">The port used for a server connection.</param>
        public Client(String ipAddress, Int32 port)
        {
            log = new LoggerLib.Logger();
            this.IP = IPAddress.Parse(ipAddress);
            this.port = port;
        }


        public Client(String ipAddress, Int32 port, bool logging)
        {
            this.IP = IPAddress.Parse(ipAddress);
            log = new LoggerLib.Logger();
            this.port = port;

            this.logging = logging;
        }

        /// <summary>
        /// Connects the TCP client and establishes
        /// the stream for reading and writing.
        /// </summary>
        /// <returns>The status of connection.</returns>
        public override bool Connect()
        {
            try
            {
                client = new TcpClient();
                client.Connect(IP, port);
                stream = client.GetStream();
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// The default IP of the server.
        /// </summary>
        public static string DefaultIP
        {
            get
            {
                return IPAddress.Loopback.ToString();
            }
        }

        /// <summary>
        /// The default port of the server.
        /// </summary>
        public static int DefaultPort
        {
            get
            {
                return defaultPort;
            }
        }

    }

}


