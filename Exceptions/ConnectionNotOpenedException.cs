using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMongo.Exceptions {
    
    /// <summary>
    /// Thrown when a request is sent to the server but 
    /// the connection has not been opened yet
    /// </summary>
    public class ConnectionNotOpenedException : InvalidOperationException {

        /// <summary>
        /// Throws an exception that the connection isn't ready yet
        /// </summary>
        public ConnectionNotOpenedException(string host)
            : base(string.Format("Connection to {0} has not been opened yet. Either use 'autoconnect' or Open() before sending requests.", host)) {
        }
    
    }

}
