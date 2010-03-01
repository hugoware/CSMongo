using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;
using System.IO;
using CSMongo.Types;

namespace CSMongo.DataTypes {

    /// <summary>
    /// In the event that an object cannot be determined
    /// the fall back should be the MongoNull class
    /// </summary>
    public class MongoNullType : MongoDataType {

        /// <summary>
        /// Returns the Mongo type for this data
        /// </summary>
        public override MongoDataTypes GetDataType() {
            return MongoDataTypes.Null;
        }

        /// <summary>
        /// Returns true if the value is empty
        /// </summary>
        public override bool IsAllowedValue<T>(T value) {
            return value == null;
        }

        /// <summary>
        /// Returns bytes to mark something as null
        /// </summary>
        public override byte[] ToBson() {
            return BsonTranslator.AsNull();
        }

        /// <summary>
        /// Reads the null value from the stream
        /// </summary>
        public override object FromBsonStream(Stream stream) {
            return BsonTranslator.ReadNull(stream);
        }

    }

}
