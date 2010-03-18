using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMongo.Types {

    /// <summary>
    /// Update options available for a Mongo Update request
    /// </summary>
    public enum UpdateOptionTypes : int {

        /// <summary>
        /// Nothing special is required for this update
        /// </summary>
        None = 0,

        /// <summary>
        /// If no matching records are found the record will be inserted
        /// </summary>
        Upsert = 1,

        /// <summary>
        /// Updates all matching documents otherwise it will only match
        /// the first document found
        /// </summary>
        MultiUpdate = 2

    }

}
