using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;

namespace CSMongo.Results {

    /// <summary>
    /// Details about the indexes deleted from a collection
    /// </summary>
    public class DeleteCollectionIndexResult : MethodResult {

        #region Constructors

        /// <summary>
        /// The information returned when calling the FSync command
        /// </summary>
        public DeleteCollectionIndexResult(BsonObject document)
            : base(document) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The number of indexes found
        /// </summary>
        public int IndexCount {
            get { return this.Response.Get<int>("nIndexesWas"); }
        }

        #endregion

    }

}
