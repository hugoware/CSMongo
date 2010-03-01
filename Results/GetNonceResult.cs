using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;

namespace CSMongo.Results {

    /// <summary>
    /// The result when getting a nonce value for a database login
    /// </summary>
    public class GetNonceResult : MethodResult {
    
        #region Constructors

        /// <summary>
        /// The information returned when calling the FSync command
        /// </summary>
        public GetNonceResult(BsonObject document)
            : base(document) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the nonce found in the response
        /// </summary>
        public string Nonce {
            get { return this.Response.Get<string>("nonce", string.Empty); }
        }

        /// <summary>
        /// Returns if a nonce was actually found in the response
        /// </summary>
        public bool HasNonce {
            get { return !string.IsNullOrEmpty((this.Nonce ?? string.Empty).Trim()); }
        }

        #endregion

    
    }

}
