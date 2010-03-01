using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;

namespace CSMongo.Results {

    /// <summary>
    /// Information about the result of a FSync command
    /// </summary>
    public class FSyncResult : MethodResult {

        #region Constructors

        /// <summary>
        /// The information returned when calling the FSync command
        /// </summary>
        public FSyncResult(BsonObject document)
            : base(document) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the number of files after the FSync command
        /// </summary>
        public int NumberOfFiles {
            get { return this.Response.Get<int>("numFiles"); }
        }

        #endregion

    }

}
