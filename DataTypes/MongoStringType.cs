using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;
using System.IO;
using CSMongo.Types;

namespace CSMongo.DataTypes {
    
    /// <summary>
    /// Handles assigning a value for 
    /// </summary>
    public class MongoStringType : MongoDataType {

        /// <summary>
        /// Returns the Mongo type for this data
        /// </summary>
        public override MongoDataTypes GetDataType() {
            return MongoDataTypes.String;
        }

        /// <summary>
        /// Returns if the type requested can be assigned to this type
        /// </summary>
        public override bool IsAllowedValue<T>(T value) {
            return value is string ? true : false;
        }

        /// <summary>
        /// Handles converting the value to a BSON format
        /// </summary>
        public override byte[] ToBson() {
            return BsonTranslator.AsCString(this.Value as string ?? string.Empty);
        }

        /// <summary>
        /// Handles reading a string value from an incoming stream
        /// </summary>
        public override object FromBsonStream(Stream stream) {
            return BsonTranslator.ReadCString(stream);
        }

    }

}
