using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMongo.Exceptions {

    /// <summary>
    /// Thrown when a connection string cannot be parsed
    /// </summary>
    public class InvalidMongoConnectionStringException : ArgumentException {

        /// <summary>
        /// Throws that the provided connection stirng is not
        /// in an acceptable format
        /// </summary>
        public InvalidMongoConnectionStringException()
            : base("The connection string provided was not in the correct format.") {
        }

    }

}
