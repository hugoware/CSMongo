using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;

namespace CSMongo.Results {

    /// <summary>
    /// Unique information about a database on the server
    /// </summary>
    public class DatabaseDetailResult : MethodResult {

        #region Constructors

        /// <summary>
        /// Creates the detail for a Database listed in Mongo
        /// </summary>
        public DatabaseDetailResult(BsonObject detail)
            : base(detail) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the database
        /// </summary>
        public string Name {
            get { return this.Response.Get("name", "Undefined"); }
        }

        /// <summary>
        /// The actual size of the database
        /// </summary>
        public long SizeOnDisk {
            get { return this.Response.Get<long>("sizeOnDisk", -1); }
        }

        /// <summary>
        /// Returns if this database is empty or not
        /// </summary>
        public bool Empty {
            get { return this.Response.Get("empty", false); }
        }

        #endregion

    }

}
