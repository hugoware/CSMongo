using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;

namespace CSMongo.Results {

    /// <summary>
    /// The resulting information from a count request for a database
    /// </summary>
    public class CollectionCountResult : MethodResult {

        #region Constructors

        /// <summary>
        /// Count information for the specified collection
        /// </summary>
        public CollectionCountResult(BsonObject document)
            : base(document) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The total documents found in the collection
        /// </summary>
        public long TotalDocuments {
            get { return this.Response.Get<long>("n"); }
        }

        #endregion

    }

}
