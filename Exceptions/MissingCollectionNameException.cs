using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMongo.Exceptions {

    /// <summary>
    /// Thrown when an empty string is used as a MongoCollection name
    /// </summary>
    public class MissingCollectionNameException : ArgumentException {

        /// <summary>
        /// Throws that a collection name was not found
        /// </summary>
        public MissingCollectionNameException(string argument)
            : base("You must provide the name for the MongoCollection to access", argument) {
        }

    }

}
