using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using ChatLib;

namespace ThreadedTCPChatApplication
{

    /// <summary>
    /// Notes: The form will allow the user to send a message to a connected network
    /// stream using a library built earlier for a console based syncronous chat program.
    /// The listening and displaying of the message.
    /// </summary>

    public partial class ChatForm : Form
    {
       
        private const string ChatIndicator = ">>";
        private const string ErrorMessage = "Can't Connect to Server.";

        SendMessage sendMessage;  // delegate
        Thread sendMessageThread; // thread for sending message
        Thread receiveMessageThread; // thread for recieving message
        Client client; // the chat client

        public ChatForm()
        {
            client = new ChatLib.Client(Client.DefaultIP, Client.DefaultPort); 
            sendMessage = new SendMessage(client.SendMessageToStream);
            client.MessageReceived += new MessageRecievedHandler(RecieveMessage);
            InitializeComponent();
            ShowUIConnected(false);
        }

        /// <summary>
        /// Send a message to the stream, append to the log 
        /// on the form and clear the input. TODO Move this 
        /// action to a separate thread to avoid issues with game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, EventArgs e)
        {      
            String message = txtBoxUserMessage.Text;
            txtBoxChatLog.AppendText(ChatIndicator + ' ' + message + Environment.NewLine);
            txtBoxUserMessage.Text = String.Empty;
            SendMessage(message);
          
         
        }

        /// <summary>
        /// Send a message to the server.
        /// </summary>
        private void SendMessage(String message)
        {
            if(client.IsConnected())
            {
                sendMessageThread = new Thread(() => sendMessage(message));       
                sendMessageThread.Name = "MessageThread";
                sendMessageThread.IsBackground = true;
                sendMessageThread.Start();
            }
            else
            {
                DisconnectClient();
            }
            
        }

        /// <summary>
        /// Connect to an available server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectClient();
        }

        /// <summary>
        /// Disconnect from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisconnectClient();
        }

        /// <summary>
        /// Connect the client to the server.
        /// </summary>
        private void ConnectClient()
        {
            if (client.Connect())
            {
                client.StopListening = false;
                receiveMessageThread = new Thread(client.ListenForMessage);
                receiveMessageThread.Name = "RecieveThread";
                receiveMessageThread.IsBackground = true;
                receiveMessageThread.Start();
                ShowUIConnected(true);
            }
            else
            {
                MessageBox.Show(ErrorMessage);
                ShowUIConnected(false);
            }
        }

        /// <summary>
        /// Adjust the UI to display either connected
        /// or not connected. 
        /// </summary>
        /// <param name="connected">The state of the connection</param>
        private void ShowUIConnected(bool connected)
        {
            connectToolStripMenuItem.Enabled = !connected;
            disconnectToolStripMenuItem.Enabled = connected;
            btnSend.Enabled = connected;
            txtBoxUserMessage.Enabled = connected;
            txtBoxChatLog.Enabled = connected;

            if(connected)
            {
                pnlConnectionStatus.BackColor = Color.Green;
            }
            else
            {
                pnlConnectionStatus.BackColor = Color.Red;
            }
                
        }

        /// <summary>
        /// Close the stream.
        /// </summary>
        private void DisconnectClient()
        {
            client.StopListening = true;
            client.Disconnect();
            ShowUIConnected(false);
        }

        /// <summary>
        /// Recieve message from the server.
        /// </summary>
        /// <param name="mce"></param>
        private void RecieveMessage(MessageRecievedEventArgs mce)
        {
            if (client.IsConnected())
            { 
            if (txtBoxChatLog.InvokeRequired)
            {
                MethodInvoker myMethod = new MethodInvoker(
                       delegate
                       {
                           txtBoxChatLog.AppendText( mce.Message + Environment.NewLine);
                       }
                   );

                txtBoxChatLog.Invoke(myMethod);
            }
            }
            else
            {
                DisconnectClient();
            }
        }

        /// <summary>
        /// Closes the client connection on
        /// form closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.StopListening = true;
            client.Disconnect();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            client.StopListening = true;
            client.Disconnect();
            Application.Exit();
        }

    }
}
