using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CSMongo.Bson;
using CSMongo.Types;

namespace CSMongo.DataTypes {

    /// <summary>
    /// Handles working with unknown types, mostly via serialization
    /// </summary>
    public class MongoObjectType : MongoDataType {

        /// <summary>
        /// Returns the Mongo type for this data
        /// </summary>
        public override MongoDataTypes GetDataType() {
            return MongoDataTypes.Object;
        }
    
        /// <summary>
        /// Catches generic objects and coverts them into DynamicObjects
        /// </summary>
        public override bool IsAllowedValue<T>(T value) {
            return value is object;
        }

        /// <summary>
        /// 
        /// </summary>
        public override byte[] ToBson() {
            return null;
        }

        public override object FromBsonStream(Stream stream) {
            throw new NotImplementedException();
        }

    }

}
