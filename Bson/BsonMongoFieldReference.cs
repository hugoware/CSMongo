using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.Bson;
using CSMongo.DataTypes;

namespace CSMongo.Bson {

    /// <summary>
    /// Details about a MongoDataType and the location
    /// </summary>
    public class MongoFieldReference {

        /// <summary>
        /// The parent object for this field
        /// </summary>
        public BsonObject Parent { get; set; }

        /// <summary>
        /// The name of the field that holds this value
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The actual field controller for this type
        /// </summary>
        public MongoDataType Field { get; set; }
    
    }

}
