using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;
using System.IO;
using CSMongo.Types;

namespace CSMongo.DataTypes {

    /// <summary>
    /// Handles working with boolean values
    /// </summary>
    public class MongoBooleanType : MongoDataType {

        /// <summary>
        /// Returns the Mongo type for this data
        /// </summary>
        public override MongoDataTypes GetDataType() {
            return MongoDataTypes.Boolean;
        }

        /// <summary>
        /// Checks for boolean values
        /// </summary>
        public override bool IsAllowedValue<T>(T value) {
            return value is bool || value is bool?;
        }

        /// <summary>
        /// Returns the bytes to generate this object
        /// </summary>
        public override byte[] ToBson() {
            return BsonTranslator.AsBoolean(this.Value as bool? ?? false);
        }

        /// <summary>
        /// Reads a boolean value from a stream
        /// </summary>
        public override object FromBsonStream(Stream stream) {
            return BsonTranslator.ReadBoolean(stream);
        }

    }

}
