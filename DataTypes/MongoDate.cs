using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Types;
using CSMongo.Bson;
using System.IO;

namespace CSMongo.DataTypes {
    
    public class MongoDate : MongoDataType {

        /// <summary>
        /// Returns the data type for this object
        /// </summary>
        /// <returns></returns>
        public override MongoDataTypes GetDataType() {
            return MongoDataTypes.Date;
        }

        /// <summary>
        /// Determines allowed values for this object
        /// </summary>
        public override bool IsAllowedValue<T>(T value) {
            return value is DateTime || value is DateTime?;
        }

        /// <summary>
        /// Returns the value for this field in binary format
        /// </summary>
        public override byte[] ToBson() {
            return BsonTranslator.AsDate(this.Value as DateTime? ?? DateTime.MinValue);
        }

        /// <summary>
        /// Reads this value from an incoming stream
        /// </summary>
        public override object FromBsonStream(Stream stream) {
            return BsonTranslator.ReadDate(stream);
        }

    }

}
