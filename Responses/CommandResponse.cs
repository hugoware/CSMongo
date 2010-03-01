using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CSMongo.Responses {

    /// <summary>
    /// Handles working with a response from a command
    /// </summary>
    public class CommandResponse : QueryResponse {

        #region Constructors

        /// <summary>
        /// Creates a response to a command request
        /// </summary>
        public CommandResponse(Stream stream)
            : base(stream) {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Verifies that a query response has an okay response within it
        /// </summary>
        public bool IsResponseOk() {
            return this.GetDefaultResponse().Get<double>("ok", 0) == ResponseBase.ResultOk;
        }

        /// <summary>
        /// Returns if an actual response was found from this request
        /// </summary>
        public bool HasResponse {
            get { return this.Documents is List<MongoDocument> && this.Documents.Count > 0; }
        }

        /// <summary>
        /// Returns the main document for the response or
        /// an empty document if nothing was found
        /// </summary>
        public MongoDocument Detail {
            get { return this.HasResponse ? this.Documents.First() : new MongoDocument(); }
        }

        #endregion

    }

}
