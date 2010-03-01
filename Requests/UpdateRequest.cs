using System.Collections.Specialized;
using CSMongo.Bson;
using CSMongo.Extensions;
using CSMongo.IO;
using CSMongo.Types;

namespace CSMongo.Requests {

    /// <summary>
    /// Handles inserting content into a database
    /// </summary>
    public class UpdateRequest : CollectionRequestBase {

        #region Constructors

        /// <summary>
        /// Creates an empty update request
        /// </summary>
        public UpdateRequest(MongoCollection collection)
            : base(OpCodeTypes.Update, collection) {
            this.Parameters = new BsonDocument();
            this.Modifications = new BsonDocument();
            this.Options = UpdateOptionTypes.Upsert;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The options available to update the document
        /// </summary>
        public UpdateOptionTypes Options { get; set; }

        /// <summary>
        /// The query filter information to use
        /// </summary>
        public BsonDocument Parameters { get; set; }

        /// <summary>
        /// The modifications to make to the object that are found
        /// </summary>
        public BsonDocument Modifications { get; set; }

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

            //generate the options and write them
            //BitVector32 options = new BitVector32(0);
            //options[0] = this.Options.Has(UpdateOptionTypes.Upsert);
            //options[1] = this.Options.Has(UpdateOptionTypes.MultiUpdate);
            stream.Append(BsonTranslator.AsInt32((int)this.Options));

            //then the actual selection
            stream.Append(this.Parameters.ToBsonByteArray());

            //and lastly the changes
            stream.Append(this.Modifications.ToBsonByteArray());

        }

        #endregion

    }

}
