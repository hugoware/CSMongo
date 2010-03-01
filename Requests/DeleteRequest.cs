using CSMongo.Bson;
using CSMongo.IO;
using CSMongo.Types;

namespace CSMongo.Requests {

    /// <summary>
    /// Handles inserting content into a database
    /// </summary>
    public class DeleteRequest : CollectionRequestBase {

        #region Constructors

        /// <summary>
        /// Creates an empty delete request
        /// </summary>
        public DeleteRequest(MongoCollection collection)
            : base(OpCodeTypes.Delete, collection) {
            this.Parameters = new BsonDocument();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The query filter information to use
        /// </summary>
        public BsonDocument Parameters { get; set; }

        #endregion

        #region Required Methods

        /// <summary>
        /// Prepares the body of the request to send to the server
        /// </summary>
        protected override void GenerateBody(DynamicStream stream) {

            //required ZERO byte after header
            stream.Append(BsonTranslator.AsInt32(0));

            //apply the collection and database
            stream.Append(BsonTranslator.AsString(this.GetDatabaseTarget()));

            //required ZERO byte before selector
            stream.Append(BsonTranslator.AsInt32(0));

            //then the actual selection
            stream.Append(this.Parameters.ToBsonByteArray());

        }

        #endregion

    }

}
