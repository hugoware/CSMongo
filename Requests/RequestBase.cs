using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CSMongo.Types;
using CSMongo.IO;
using CSMongo.Responses;

namespace CSMongo.Requests {
    
    /// <summary>
    /// Base class for making requests to the server
    /// </summary>
    public abstract class RequestBase {

        #region Constants

        private const int DEFAULT_HEADER_LENGTH = 16;
        private const int POSITION_REQUEST_LENGTH = 0;
        private const int POSITION_REQUEST_ID = 4;
        private const int POSITION_RESPONSE_ID = 8;
        private const int POSITION_OP_CODE = 12;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new request for the specified type
        /// </summary>
        public RequestBase(OpCodeTypes code) {
            this.OpCode = code;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current length of this request
        /// </summary>
        public int RequestLength {
            get { return this._Output.Length; }
        }

        /// <summary>
        /// Returns the OpCode used for this request
        /// </summary>
        public OpCodeTypes OpCode { get; private set; }

        /// <summary>
        /// The current RequestId for this 
        /// </summary>
        public int RequestId { get; set; }

        /// <summary>
        /// The curretn ResponseID for this request
        /// </summary>
        public int ResponseId { get; set; }

        //the container for the stream to write
        private DynamicStream _Output;

        #endregion

        #region Methods

        //generates the entire request
        private void _GenerateStream() {

            //if the stream has already been created then don't bother
            if (this._Output is DynamicStream) { return; }

            //called just before the generation starts
            this.OnBeforeGenerateStream();

            //start building the header
            DynamicStream stream = new DynamicStream(DEFAULT_HEADER_LENGTH);
            stream.WriteAt(POSITION_OP_CODE, BitConverter.GetBytes((int)this.OpCode));

            //generate the bytes to use for the body
            this.GenerateBody(stream);

            //update the request/response IDs incase they change when building
            stream.WriteAt(POSITION_REQUEST_ID, BitConverter.GetBytes(this.RequestId));
            stream.WriteAt(POSITION_RESPONSE_ID, BitConverter.GetBytes(this.ResponseId));

            //finally, remember to update the length
            stream.WriteAt(POSITION_REQUEST_LENGTH, BitConverter.GetBytes(stream.Length));

            //cache this value to use it later
            this._Output = stream;

        }

        /// <summary>
        /// Resets the bytes for this request
        /// </summary>
        public void Reset() {
            this._Output = null;
        }

        /// <summary>
        /// Returns the bytes to send as a header for this request
        /// </summary>
        public byte[] GetHeader() {
            this._GenerateStream();
            return this._Output.Read(0, DEFAULT_HEADER_LENGTH);
        }

        /// <summary>
        /// Returns the bytes that should be sent as a header
        /// </summary>
        public byte[] GetBody() {
            this._GenerateStream();
            return this._Output.Read(DEFAULT_HEADER_LENGTH, this._Output.Length - DEFAULT_HEADER_LENGTH);
        }

        #endregion

        #region Required Methods

        /// <summary>
        /// Required function to generate the content for sending
        /// </summary>
        protected abstract void GenerateBody(DynamicStream stream);

        #endregion

        #region Optional Methods

        //optional method to read a return stream from the 
        public virtual ResponseBase OnResponse(Stream stream) {
            return null;
        }

        /// <summary>
        /// Optional functionality to perform before generating 
        /// the stream content
        /// </summary>
        protected virtual void OnBeforeGenerateStream() { }

        #endregion

    }

}
