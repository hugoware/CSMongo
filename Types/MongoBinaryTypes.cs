using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMongo.Types {
    
    /// <summary>
    /// Available binary formattings for an object
    /// </summary>
    public enum MongoBinaryTypes : byte {

        //not sure what most of these types are for
        //so binary is done using UserDefined

        Unknown = 1,

        Defined = 2,

        UUID = 3,

        MD5 = 5,

        UserDefined = 8

    }

}
