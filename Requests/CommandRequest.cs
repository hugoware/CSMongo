using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CSMongo.Responses;
using CSMongo.Bson;

namespace CSMongo.Requests {

    /// <summary>
    /// Starts a new request to execute a command
    /// </summary>
    public class CommandRequest : ServerSideCodeRequestBase {

        #region Constructors

        /// <summary>
        /// Creates a new CommandRequest for the specified database
        /// </summary>
        public CommandRequest(MongoDatabase database)
            : this(database.Name) {
            this.ExpectResponse = true;
        }

        /// <summary>
        /// Creates a new CommandRequest for the specified database
        /// </summary>
        public CommandRequest(string database)
            : base(database, "$cmd") {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Does this request expect a response from the server
        /// </summary>
        public bool ExpectResponse { get; set; }

        #endregion

        #region Handling Responses

        /// <summary>
        /// Reads the response from the server
        /// </summary>
        public override ResponseBase OnResponse(Stream stream) {
            if (!this.ExpectResponse) { return null; }
            return new CommandResponse(stream);
        }

        #endregion

    }

}
