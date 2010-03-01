using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;

namespace CSMongo.Results {

    /// <summary>
    /// A result when attempting to force an error on the server
    /// </summary>
    public class ForceErrorResult : MethodResult {
        
        #region Constructors

        /// <summary>
        /// Creates a new container for the error result
        /// </summary>
        public ForceErrorResult(BsonObject document)
            : base(document) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the assertion that was used for this error
        /// </summary>
        public string Assertion {
            get { return this.Response.Get<string>("assertion"); }
        }

        /// <summary>
        /// Returns the errormessage thrown by the server
        /// </summary>
        public string ErrorMessage {
            get { return this.Response.Get<string>("errmsg"); }
        }

        #endregion


    }

}
