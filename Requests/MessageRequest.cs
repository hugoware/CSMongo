using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.IO;
using CSMongo.Bson;
using CSMongo.Types;

namespace CSMongo.Requests {

    /// <summary>
    /// Sends a message?
    /// </summary>
    public class MessageRequest : RequestBase {

        public MessageRequest()
            : base(OpCodeTypes.Query) {

        }

        /// <summary>
        /// The command to send
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Generates the message to send
        /// </summary>
        protected override void GenerateBody(DynamicStream stream) {

            BsonDocument document = new BsonDocument();
            document["db.users.remove()"] = 1.0;


            stream.Append(document.ToBsonByteArray());
        }

    }
}
