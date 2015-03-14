using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib
{
 
        /// <summary>
        /// This delegate is defined as the type of our ProgressChanged event.
        /// Note the signature of the delegate...return void and takes a parameter of ProgressChangedEventArgs
        /// Any method that this delegate is pointed to must have the same signature.
        /// </summary>
        /// <param name="pce"></param>
        public delegate void MessageRecievedHandler(MessageRecievedEventArgs mre);
        public delegate void SendMessage(String message);
        public delegate void DisonnectedEventHandler();
    
}
