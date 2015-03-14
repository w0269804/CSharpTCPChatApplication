using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib
{
    /// <summary>
    /// Used to carry information from the event within
    /// the chat service when it picks up a message from
    /// the server.
    /// </summary>
    public class MessageRecievedEventArgs : EventArgs
    {
        string message;

        /// <summary>
        /// Constructs event args which contain the 
        /// message to display.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public MessageRecievedEventArgs(string message)
        {
            this.message = message;
        }

        /// <summary>
        /// The message to display.
        /// </summary>
        public String Message
        {
            get
            {
                return message;
            }
        }

    }
}

