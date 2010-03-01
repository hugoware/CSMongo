using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;

namespace CSMongo.Results {

    /// <summary>
    /// The details after deleting a collection
    /// </summary>
    public class DropCollectionResult : MethodResult {

        #region Constructors

        /// <summary>
        /// The result when trying to remove a collection
        /// </summary>
        public DropCollectionResult(BsonObject document)
            : base(document) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The number of indexes found when removing the collection
        /// </summary>
        public int IndexCount {
            get { return this.Response.Get<int>("nIndexesWas"); }
        }

        /// <summary>
        /// The namespace of the removed collection
        /// </summary>
        public string Namespace {
            get { return this.Response.Get<string>("ns"); }
        }

        /// <summary>
        /// The message returned from the server
        /// </summary>
        public string Message {
            get { return this.Response.Get<string>("msg"); }
        } 

        #endregion

    }

}
