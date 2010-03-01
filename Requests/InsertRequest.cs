using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using CSMongo.Types;
using CSMongo.IO;
using CSMongo.Bson;

namespace CSMongo.Requests {

    /// <summary>
    /// Handles inserting content into a database
    /// </summary>
    public class InsertRequest : CollectionRequestBase {

        #region Constructors

        /// <summary>
        /// Creates an empty insert request
        /// </summary>
        public InsertRequest(MongoCollection collection)
            : base(OpCodeTypes.Insert, collection) {
            this.Documents = new List<MongoDocument>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// A collection of documents that should be inserted
        /// </summary>
        public List<MongoDocument> Documents { get; private set; }

        #endregion

        #region Required Methods

        /// <summary>
        /// Prepares the body of the request to send to the server
        /// </summary>
        protected override void GenerateBody(DynamicStream stream) {

            //required ZERO int after header
            stream.Append(BsonTranslator.AsInt32(0));

            //apply the collection and database
            stream.Append(BsonTranslator.AsString(this.GetDatabaseTarget()));

            //and generate each of the documents
            foreach (MongoDocument document in this.Documents) {
                stream.Append(document.ToBsonByteArray());
            }

        }

        #endregion

    }

}
