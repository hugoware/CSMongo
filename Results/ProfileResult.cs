using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;

namespace CSMongo.Results {

    /// <summary>
    /// Contains information about the profile level for the server
    /// </summary>
    public class ProfileResult : MethodResult {
        
        #region Constructors

        /// <summary>
        /// Returns information about the profile level for this database
        /// </summary>
        public ProfileResult(BsonObject document)
            : base(document) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The previous level of the profile before the change
        /// </summary>
        public int PreviousLevel {
            get { return this.Response.Get<int>("was"); }
        }

        #endregion

    }
}
