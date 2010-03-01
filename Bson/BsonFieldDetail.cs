using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.DataTypes;

namespace CSMongo.Bson {

    /// <summary>
    /// Information about a BSON field after it has
    /// been rendered into bytes
    /// </summary>
    public class BsonFieldDetail {

        #region Constructors

        /// <summary>
        /// Creates a new set of details for a BSON field
        /// </summary>
        public BsonFieldDetail(string field, byte[] bytes, MongoDataType type) {
            this.Field = field;
            this.Bytes = bytes;
            this.Type = type;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The string name for this field
        /// </summary>
        public string Field { get; private set; }

        /// <summary>
        /// The original MongoDataType for this field
        /// </summary>
        public MongoDataType Type { get; private set; }

        /// <summary>
        /// The bytes that create this object
        /// </summary>
        public byte[] Bytes { get; private set; }

        /// <summary>
        /// Returns the total length of the bytes in the set
        /// </summary>
        public int Length {
            get { return this.Bytes is byte[] ? this.Bytes.Length : 0; }
        }

        #endregion

    }
}
