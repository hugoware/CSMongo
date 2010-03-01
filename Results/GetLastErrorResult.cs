using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;

namespace CSMongo.Results {

    /// <summary>
    /// Returns details about the last error in the database
    /// </summary>
    public class GetLastErrorResult : MethodResult {
        
        #region Constructors

        /// <summary>
        /// Creates a container for the last error message found
        /// </summary>
        public GetLastErrorResult(BsonObject document)
            : base(document) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns information about the system that is hosting the database
        /// </summary>
        public string Error {
            get { return this.Response.Get<string>("err"); }
        }

        /// <summary>
        /// Returns the total count of errors found on the server
        /// </summary>
        public int NumberOfErrors {
            get { return this.Response.Get<int>("n", -1); }
        }

        #endregion

    }
}
