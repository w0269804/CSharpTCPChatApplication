using System;
using System.Linq; /* for Lambda method */
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation; /* for TCPConnectionInformation */

namespace ChatLib
{

    /// <summary>
    /// The base for either a TCP chat client or a TCP chat client. 
    /// </summary>
    public abstract class ChatService
    {
        protected TcpClient client; // to retrieve messages from stream
        protected TcpListener server; // to host the chat sessions
        protected NetworkStream stream; // holds messages between client and server

        protected IPAddress IP; // the IP the service will connect to
        protected Int32 port; // the port the service will connect to

        protected bool logging = true; // whether logging is active
        protected LoggerLib.Logger log; // the logging object
        public event MessageRecievedHandler MessageReceived; // for handling message recieves
        private volatile bool stopListening = false; // to control listening flow


        /// <summary>
        /// Connects the service.
        /// </summary>
        public abstract bool Connect();

        /// <summary>
        /// Closes the network stream and the TCP objects
        /// active connection.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                stream.Close();
            }
            catch (SocketException)
            {

            }
            catch (Exception) 
            {

            }

            finally
            {
                if (server != null)
                {
                    server.Stop();
                }

                if (client != null)
                {
                    client.Close();
                }

            }
        }

        /// <summary>
        /// Writes messages to the network stream.
        /// </summary>
        /// <param name="message">The message to transmit on network stream.</param>
        public void SendMessageToStream(String message)
        {
            try
            {
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                if (stream != null && stream.CanWrite)
                {
                    stream.Write(data, 0, data.Length);

                    if (logging)
                        log.WriteToFile(DateTime.Now.ToString() + " CLIENT: " + message);

                }
            }
            catch (SocketException e)
            {

            }
            catch (Exception e)
            {

            }

        }

        /// <summary>
        /// Retrieves messages from the network stream.
        /// </summary>
        /// <returns>The data which is on the stream.</returns>
        public String GetMessageFromStream()
        {
            Byte[] data = new Byte[256];
            String responseData = String.Empty;

            try
            {
                if (stream != null && stream.DataAvailable)
                {
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                    if (responseData.Contains("\0"))
                    {
                        responseData = String.Empty;
                    }
                }
            }
            catch (SocketException e)
            {
                ///TODO
            }
            catch(Exception e)
            {
                ///TODO
            }
 
            return responseData;
        }

        /// <summary>
        /// Checks the stream for a message, if a message 
        /// exists then an event is raised and the message 
        /// is sent as event args. If logging is enabled
        /// then the message is logged.
        /// </summary>
        public void RecieveMessage()
        {
            String message = GetMessageFromStream();

            if (MessageReceived != null && message != String.Empty)
            {
                MessageReceived(new MessageRecievedEventArgs(message));

                if (logging)
                    log.WriteToFile(DateTime.Now.ToString() + " SERVER: " + message);
            }

        }

        /// <summary>
        /// Gets all active TCP connections on the local host where the connection
        /// matches the client local end point and remote end point. In this case 
        /// the client's end point will be the server in both instances.
        /// </summary>
        /// <returns>The service connection status.</returns>
        public bool IsConnected()
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();

            try
            {
                /// Gets all active TCP connections from the local machine and creates an array of
                /// TCPConnectionInformation objects which where the IP of the TCP connection matches
                /// that of the clients'. (IE localhost --> server). Uses a lamba expression to 
                /// iterate through the list and retrieve those IP addresses. 

                TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections().
                    Where(x => x.LocalEndPoint.Equals(client.Client.LocalEndPoint) &&
                     x.RemoteEndPoint.Equals(client.Client.RemoteEndPoint)).ToArray();

                if (tcpConnections != null && tcpConnections.Length > 0)
                {
                    return (tcpConnections.First().State == TcpState.Established);

                }

                return false;

            }
            catch (SocketException)
            {

                return false;
            }
            catch (Exception)
            {
                return false;
            }

        } // end function


        /// <summary>
        /// Listens for messages on the stream and
        /// fires and event if a message is recieved.
        /// </summary>
        public void ListenForMessage()
        {
            while (!StopListening)
            {
                if (!IsConnected())
                    break;

                RecieveMessage();
            }
        }
            
        /// <summary>
        /// Handles control of listening for messages.
        /// </summary>
        public bool StopListening
        {
            get { return stopListening; }
            set { stopListening = value; }
        }

    } // end class
} // end namespace

