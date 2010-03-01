using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMongo.Exceptions {

    /// <summary>
    /// Exception thrown when the server returns an error
    /// </summary>
    public class MongoServerException : InvalidOperationException {

        /// <summary>
        /// Throws a new exception for when the Mongo server has an error
        /// </summary>
        public MongoServerException(string error)
            : base(string.Format("Database returned an error: {0}", error)) {
        }
    
    }

}
