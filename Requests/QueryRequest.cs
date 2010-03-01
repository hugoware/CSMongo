using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.IO;
using CSMongo.Types;
using CSMongo.Bson;
using System.IO;
using CSMongo.Responses;
using System.Text.RegularExpressions;

namespace CSMongo.Requests {

    /// <summary>
    /// Creates a new QueryRequest for records
    /// </summary>
    public class QueryRequest : CollectionRequestBase {

        #region Constructors

        /// <summary>
        /// Creates a new insert request for the provided database
        /// </summary>
        public QueryRequest(MongoCollection collection)
            : this(collection.Database.Name, collection.Name) {
        }

        /// <summary>
        /// Creates a new insert request for the provided database
        /// </summary>
        public QueryRequest(string database, string collection)
            : base(OpCodeTypes.Query, database, collection) {
            this.Skip = 0;
            this.Take = 0;
            this.Fields = new List<string>();
            this.Options = QueryOptionTypes.None;
            this.Parameters = new BsonDocument();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The total records to skip past
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// The total records to return
        /// </summary>
        public int Take { get; set; }

        /// <summary>
        /// The options to use with this query
        /// </summary>
        public QueryOptionTypes Options { get; set; }

        /// <summary>
        /// A list of fields to select - If empty then 
        /// everything is returned
        /// </summary>
        public List<string> Fields { get; private set; }

        /// <summary>
        /// The query filter information to use
        /// </summary>
        public BsonDocument Parameters { get; set; }

        #endregion

        #region Working With Request
        
        /// <summary>
        /// Creates the body of the request to send
        /// </summary>
        protected override void GenerateBody(DynamicStream stream) {

            //determine the correct options to use
            stream.Append(BsonTranslator.AsInt32((int)this.Options));

            //apply the collection and database
            stream.Append(BsonTranslator.AsString(this.GetDatabaseTarget()));

            //update the range information for this request
            stream.Append(BsonTranslator.AsInt32(this.Skip));
            stream.Append(BsonTranslator.AsInt32(this.Take));

            //generate the query
            stream.Append(this.Parameters.ToBsonByteArray());

            //generate the field selectors if there are any
            if (this.Fields.Count > 0) {

                //create the selector document
                BsonDocument select = new BsonDocument();
                for (int i = 0; i < this.Fields.Count; i++) {
                    select.Set(this.Fields[i], i + 1);
                }

                //append the bytes
                stream.Append(select.ToBsonByteArray());

            }

        }

        /// <summary>
        /// Reads the response stream for results
        /// </summary>
        public override ResponseBase OnResponse(Stream stream) {
            return new QueryResponse(stream);
        }

        #endregion

    }
}
