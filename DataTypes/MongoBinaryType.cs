using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Types;
using System.IO;
using CSMongo.Bson;

namespace CSMongo.DataTypes {

    /// <summary>
    /// Handles working with arrays of bytes
    /// </summary>
    public class MongoBinaryType : MongoDataType {
        
        /// <summary>
        /// Returns the type of object this is
        /// </summary>
        public override MongoDataTypes GetDataType() {
            return MongoDataTypes.Binary;
        }

        /// <summary>
        /// Returns if a value is allowed for this type
        /// </summary>
        public override bool IsAllowedValue<T>(T value) {
            return value is byte[];
        }

        /// <summary>
        /// Handles reading the bytes from the request
        /// </summary>
        public override byte[] ToBson() {
            return BsonTranslator.AsBinary(this.Value as byte[] ?? new byte[] { });
        }

        /// <summary>
        /// Reads a binary array from the stream
        /// </summary>
        public override object FromBsonStream(Stream stream) {
            return BsonTranslator.ReadBinary(stream);
        }

        /// <summary>
        /// If this field is requested and not for the byte
        /// array then try and deserialze the value
        /// </summary>
        public override object Get<T>() {
            T fallback = default(T);

            //if just wanting the byte array, return it as is
            if (fallback is byte[] && this.Value is byte[]) {
                return this.Value as byte[];
            }

            //if not, try and deserialize it
            try {
                return BsonObject.Deserialize(this.Value as byte[]);
            }
            catch {
                return fallback;
            }
        }

        /// <summary>
        /// Try to serialize any values that are set and are not
        /// actually in binary format. This really shouldn't ever happen
        /// </summary>
        public override void Set<T>(T value) {
            if (value is byte[]) {
                this.Value = value;
            }
            else {
                this.Value = BsonObject.Serialize(value);
            }
        }

        /// <summary>
        /// No conversion required. Simply pass the value
        /// </summary>
        protected override object ConvertValue<T>(object value) {
            return value;
        }

    }

}
