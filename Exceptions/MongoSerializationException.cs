using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CSMongo.Exceptions {
    
    /// <summary>
    /// Thrown when trying to serialize an object to a byte
    /// array but the binary formatter fails
    /// </summary>
    public class MongoSerializationException : ApplicationException {

        /// <summary>
        /// Throws that an object could not be serialized
        /// </summary>
        public MongoSerializationException(string type, Exception ex)
            : base(string.Format("Couldn't serialize type {0} to a byte array.", type), ex) {
        }
    
    }

}
