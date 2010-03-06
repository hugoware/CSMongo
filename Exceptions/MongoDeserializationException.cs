using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMongo.Exceptions {

    /// <summary>
    /// Thrown when a type cannot be deserialized into an expected type
    /// </summary>
    public class MongoDeserializationException : ApplicationException {

        /// <summary>
        /// Throws that deserialization failed for the type requested
        /// </summary>
        public MongoDeserializationException(Exception ex)
            : base("Couldn't deserialize into the requested type.", ex) {
        }

    }

}
