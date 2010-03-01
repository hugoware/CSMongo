using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;

namespace CSMongo.Results {

    /// <summary>
    /// Returns about a previous error on the server
    /// </summary>
    public class GetPreviousErrorResult : GetLastErrorResult {

        #region Constructors

        /// <summary>
        /// Creates a container for the last error message found
        /// </summary>
        public GetPreviousErrorResult(BsonObject document)
            : base(document) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the operation count that has passed since this error
        /// </summary>
        public int NumberOfOperationsAgo {
            get { return this.Response.Get<int>("nPrev", -1); }
        }

        #endregion


    }

}
