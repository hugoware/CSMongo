using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMongo.Exceptions {

    /// <summary>
    /// Lame exception message that should be replaced later with better ones
    /// </summary>
    public class LameException : Exception {
        public LameException(string error)
            : base(string.Format("Lame Exception: {0}", error)) {
        }

    }
}
