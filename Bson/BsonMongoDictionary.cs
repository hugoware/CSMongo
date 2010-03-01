using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.DataTypes;

namespace CSMongo.Bson {

    /// <summary>
    /// General container for holding Mongo fields in a document
    /// </summary>
    public class BsonMongoDictionary : Dictionary<string, MongoDataType> {
    }

}
