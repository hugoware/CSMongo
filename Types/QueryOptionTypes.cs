using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMongo.Types {

    /// <summary>
    /// Options available when performing a query
    /// </summary>
    public enum QueryOptionTypes : int {

        None = 0,
        
        TailableCursor = 2,

        SlaveOK = 4,

        NoCursorTimeout = 16

    }

}
