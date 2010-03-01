using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;
using System.Collections;

namespace CSMongo.Results {

    /// <summary>
    /// Information related to the databases found in this Mongo instance
    /// </summary>
    public class ListDatabasesResult : MethodResult, IEnumerable<DatabaseDetailResult> {
        
        #region Constructors

        /// <summary>
        /// Creates a list of databases in this instance of Mongo
        /// </summary>
        public ListDatabasesResult(BsonObject detail)
            : base(detail) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the total size of the databases found
        /// </summary>
        public long TotalSize {
            get { return this.Response.Get<long>("totalSize", -1); }
        }

        /// <summary>
        /// The list of databases that were found
        /// </summary>
        public IEnumerable<DatabaseDetailResult> Databases {
            get {
                IEnumerable<BsonDocument> databases = this.Response.Get<IEnumerable<BsonDocument>>("databases");
                foreach (BsonDocument document in databases) {
                    yield return new DatabaseDetailResult(document);
                }
            }
        }

        /// <summary>
        /// The total count of databases available
        /// </summary>
        public int Count {
            get { return this.Databases.Count(); }
        }

        #endregion

        #region Enumeration

        /// <summary>
        /// Returns an enumerator to loop through the collection
        /// </summary>
        public IEnumerator<DatabaseDetailResult> GetEnumerator() {
            return this.Databases.GetEnumerator();
        }
    
        /// <summary>
        /// Returns an enumerator to loop through the collection
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() {
            return this.Databases.GetEnumerator();
        }

        #endregion
    
    }

}
