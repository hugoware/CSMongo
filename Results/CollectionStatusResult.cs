using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;

namespace CSMongo.Results {

    /// <summary>
    /// Details about a collection in the database
    /// </summary>
    public class CollectionStatsResult : MethodResult {
        
        #region Constructors

        /// <summary>
        /// Creates details about the stats for a collection
        /// </summary>
        public CollectionStatsResult(BsonObject document)
            : base(document) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the full name of the collection
        /// </summary>
        public string FullName {
            get { return this.Response.Get<string>("ns"); }
        }

        /// <summary>
        /// The number of documents in the collection
        /// </summary>
        public int DocumentCount {
            get { return this.Response.Get<int>("count"); }
        }

        /// <summary>
        /// The current size of the collection
        /// </summary>
        public long Size {
            get { return this.Response.Get<long>("size"); }
        }

        /// <summary>
        /// The size of the collection per storage
        /// </summary>
        public long StorageSize {
            get { return this.Response.Get<long>("storageSize"); }
        }

        /// <summary>
        /// Returns the number of indexes in the collection
        /// </summary>
        public int IndexCount {
            get { return this.Response.Get<int>("nindexes"); }
        }

        #endregion

    }

}
