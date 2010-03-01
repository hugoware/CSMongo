using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMongo.Types {

    /// <summary>
    /// Possible data types to expect for a Mongo Document
    /// </summary>
    public enum MongoDataTypes : byte {

        Number = 1,

        String = 2,

        Object = 3,

        Array = 4,

        Binary = 5,

        Oid = 7,

        Boolean = 8,

        Date = 9,

        Null = 10,

        Regex = 11,

        Ref = 12,

        Code = 13,

        Symbol = 14,

        CodeWithScope = 15,

        Int32 = 16,

        Timestamp = 17,

        Int64 = 18,

        /* Required ?
         * MinKey = -1,
         * MaxKey = 127
         */

        /* Depeciated
         * Undefined = 6
         */

    }

}
