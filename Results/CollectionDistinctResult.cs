using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;

namespace CSMongo.Results {

    /// <summary>
    /// The response when finding the distinct values of a collection
    /// </summary>
    public class CollectionDistinctResult : MethodResult {

        #region Constructors

        /// <summary>
        /// Creates the details for the distinct values in a collection
        /// </summary>
        public CollectionDistinctResult(BsonObject document)
            : base(document) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the values found for the provided index
        /// </summary>
        public object[] Values {
            get { return this.Response.Get<object[]>("values"); }
        }

        #endregion

    }

}
