using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CSMongo.Exceptions;
using CSMongo.Types;

namespace CSMongo.DataTypes {
    
    /// <summary>
    /// Base type for other defined Mongo types
    /// </summary>
    public abstract class MongoDataType {

        #region Static Properties

        //buillt in types that cannot be removed
        private static readonly MongoDataType[] _BuiltIn = new MongoDataType[] {
            new MongoNullType(),
            new MongoOidType(),
            new MongoStringType(),
            new MongoBooleanType(),
            new MongoDate(),
            new SmallEnumeratedType(),
            new LargeEnumeratedType(),
            new MongoInt32Type(),
            new MongoInt64Type(),
            new MongoNumberType(),
            new MongoBinaryType(),
            new MongoArrayType(),
            new MongoDocumentType()
        };

        //a list of custom mongo types that are evaluated before built in ones
        private static List<MongoDataType> _Custom = new List<MongoDataType>();

        #endregion

        #region Properties

        /// <summary>
        /// The current value of this field
        /// </summary>
        protected object Value { get; set; }

        #endregion

        #region Optional Methods

        /// <summary>
        /// Handles assigning a value to a field
        /// </summary>
        public virtual void Set<T>(T value) {
            this.Value = this.ConvertValue<T>(value);
        }

        /// <summary>
        /// Handles returning a value in the requested type
        /// </summary>
        public virtual object Get<T>() {
            return this.ConvertValue<T>(this.Value);
        }

        /// <summary>
        /// Handles converting the passed in value to the base 
        /// type for this Mongo object
        /// </summary>
        protected virtual object ConvertValue<T>(object value) {
            try {
                return Convert.ChangeType(value, typeof(T));
            }
            catch {
                return default(T);
            }
        }

        #endregion

        #region Required Functionality

        /// <summary>
        /// Used to determine the Mongo Op code for this type
        /// </summary>
        public abstract MongoDataTypes GetDataType();

        /// <summary>
        /// Determines if this type can be cast into the requested type
        /// </summary>
        public abstract bool IsAllowedValue<T>(T value);

        /// <summary>
        /// Writes a value to BSON format
        /// </summary>
        public abstract byte[] ToBson();

        /// <summary>
        /// Handles reading the content to find the value of an object
        /// </summary>
        public abstract object FromBsonStream(Stream stream);

        #endregion

        #region Static Methods

        /// <summary>
        /// Handles finding the correct data type for a value
        /// </summary>
        public static MongoDataType FindTypeFor(object value) {

            //check each type for a valid handler
            foreach (MongoDataType type in MongoDataType._Custom.Union(MongoDataType._BuiltIn)) {
                if (type.IsAllowedValue(value)) {
                    return Activator.CreateInstance(type.GetType()) as MongoDataType;
                }
            }

            //if no conversion was found, just render as a null value
            return new MongoNullType();

        }

        /// <summary>
        /// Registers a new MongoData type to use with the 
        /// </summary>
        public static void RegisterMongoDataType<T>() where T : MongoDataType {

            //make sure this isn't already added
            if (MongoDataType.IsTypeRegistered<T>()) {
                throw new MongoTypeAlreadyRegisteredException(typeof(T));
            }

            //add the instance
            MongoDataType._Custom.Add(Activator.CreateInstance<T>());
        }

        /// <summary>
        /// Unregisters a type from the MongoData
        /// </summary>
        public static void UnregisterMongoDataType<T>() where T : MongoDataType {
            MongoDataType._Custom.RemoveAll(item => item is T);
        }

        /// <summary>
        /// Returns if any of the current registered types matches the incoming type
        /// </summary>
        private static bool IsTypeRegistered<T>() {
            return MongoDataType._Custom.Union(MongoDataType._BuiltIn).Any(item => item is T);
        }

        #endregion

    }

}
