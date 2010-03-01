using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.IO;
using CSMongo.Bson;
using CSMongo.Types;
using CSMongo.Responses;
using System.IO;

namespace CSMongo.Requests {

    /// <summary>
    /// Sends the request to remove the provided cursors
    /// </summary>
    public class KillCursorsRequest : RequestBase {

        #region Constructors

        /// <summary>
        /// Creates a new KillCursor command for the cursors provided
        /// </summary>
        public KillCursorsRequest(IEnumerable<long> cursors)
            : base(OpCodeTypes.KillCursors) {
            this.Cursors = cursors;
        }

        #endregion

        #region Properties

        /// <summary>
        /// A list of the Cursors to kill
        /// </summary>
        public IEnumerable<long> Cursors { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Generates the body of this request
        /// </summary>
        protected override void GenerateBody(DynamicStream stream) {
            
            //Start with a required ZERO byte
            stream.Append(BsonTranslator.AsInt32(0));

            //then the number of cursors being written
            int count = this.Cursors.Count();
            stream.Append(BsonTranslator.AsInt32(this.Cursors.Count()));

            //write each of the items to the stream
            foreach (long cursor in this.Cursors.ToArray()) {
                stream.Append(BsonTranslator.AsInt64(cursor));
            }

        }

        #endregion

    }

}
