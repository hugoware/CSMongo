using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;

namespace CSMongo.Results {

    /// <summary>
    /// A general response after calling a Mongo database command
    /// </summary>
    public class MethodResult {

        #region Constructors

        /// <summary>
        /// Creates a new MethodResult from the provided document
        /// </summary>
        public MethodResult(BsonObject document) {
            this.HasResponse = document is BsonDocument;
            this.Response = document ?? new BsonDocument();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns if this method actually returned a result or not
        /// </summary>
        public bool HasResponse { get; private set; }

        /// <summary>
        /// The documents this response is based on
        /// </summary>
        public BsonObject Response { get; private set; }

        /// <summary>
        /// Returns if the response has an OK value
        /// </summary>
        public bool Ok {
            get { return this.Response.Get<int>("ok", 0).Equals(1); }
        }

        #endregion

    }

}
