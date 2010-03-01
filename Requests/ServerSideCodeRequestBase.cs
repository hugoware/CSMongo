using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.IO;
using CSMongo.Bson;
using CSMongo.Types;

namespace CSMongo.Requests {
    
    /// <summary>
    /// Base class for sending non-query commands to the server
    /// </summary>
    public abstract class ServerSideCodeRequestBase : CollectionRequestBase {

        #region Constructors

        /// <summary>
        /// Creates a new CommandRequest for the target database
        /// </summary>
        public ServerSideCodeRequestBase(string database, string target)
            : base(OpCodeTypes.Query, database, target) {
            this.Arguments = new BsonDocument();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Arguments to include as part of the command
        /// </summary>
        public BsonDocument Arguments { get; private set; }

        #endregion

        #region Generating

        /// <summary>
        /// Generates the command to send to the server
        /// </summary>
        protected override void GenerateBody(DynamicStream stream) {

            //default 'none' for the query options
            stream.Append(BsonTranslator.AsInt32(0));

            //apply the collection and database which should
            //be the database.$target format
            stream.Append(BsonTranslator.AsString(this.GetDatabaseTarget()));

            //skip 0 and take 1 means to do a 'FindOne' command
            //which actually executes our command as code
            stream.Append(BsonTranslator.AsInt32(0));
            stream.Append(BsonTranslator.AsInt32(1));

            //insert the command value at the front of the request
            stream.Append(this.Arguments.ToBsonByteArray());

        }

        #endregion

    }

}
