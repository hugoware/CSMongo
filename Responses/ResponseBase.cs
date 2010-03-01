using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Requests;
using System.IO;
using CSMongo.Types;
using CSMongo.Exceptions;
using CSMongo.Bson;

namespace CSMongo.Responses {

    /// <summary>
    /// A response from a request to the database. No need to 
    /// make changes or act with the stream in the constructor
    /// since the abstract method ParseStream handles that
    /// </summary>
    public abstract class ResponseBase {

        #region Constants

        /// <summary>
        /// Response from the server meaning a request completed successfully
        /// </summary>
        public static readonly double ResultOk = 1.0;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new response by reading the standard Mongo header stream
        /// </summary>
        public ResponseBase(Stream response) {
            this._ReadResponse(response);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The length of the response message
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The id of this current request
        /// </summary>
        public int RequestId { get; private set; }

        /// <summary>
        /// The id of the request that sent the query
        /// </summary>
        public int ResponseTo { get; private set; }

        /// <summary>
        /// The code that was returned from the server
        /// </summary>
        public OpCodeTypes OpCode { get; private set; }

        #endregion

        #region Required Methods

        /// <summary>
        /// Required function for reading incoming content
        /// </summary>
        protected abstract void ParseStream(Stream stream);

        #endregion

        #region Handling Errors

        //handles reading the stream content
        private void _ReadResponse(Stream response) {

            //perform any reading as required
            BinaryReader reader = new BinaryReader(response);

            //read the header first
            this.Length = reader.ReadInt32();
            this.RequestId = reader.ReadInt32();
            this.ResponseTo = reader.ReadInt32();
            this.OpCode = (OpCodeTypes)reader.ReadInt32();

            //next, call the reading for the body of the content
            this.ParseStream(response);

        }

        #endregion

    }

}
