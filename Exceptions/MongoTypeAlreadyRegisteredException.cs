using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMongo.DataTypes;

namespace CSMongo.Exceptions {

    /// <summary>
    /// Thrown when attempting to register the same MongoDataType 
    /// more than once
    /// </summary>
    public class MongoTypeAlreadyRegisteredException : InvalidOperationException {

        /// <summary>
        /// Throws that the requested type has already been registered
        /// </summary>
        public MongoTypeAlreadyRegisteredException(Type type)
            : base(string.Format("The MongoDataType {0} has already been registered!", type.Name))  {
        }
    
    }

}
