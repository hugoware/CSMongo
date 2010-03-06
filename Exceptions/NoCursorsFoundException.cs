using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMongo.Exceptions {

    /// <summary>
    /// Thrown when attempting to get access to a cursor but
    /// the collection has no cursors available
    /// </summary>
    public class NoCursorsFoundException : InvalidOperationException {

        /// <summary>
        /// Throws that the collection has no cursors waiting
        /// </summary>
        public NoCursorsFoundException()
            : base("No cursors are available to use. Have you performed any queries yet?") {
        }

    }

}
