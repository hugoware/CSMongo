using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CSMongo.DataTypes;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using CSMongo.Exceptions;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace CSMongo.Bson {

    /// <summary>
    /// Class for working with an unknown object and values
    /// </summary>
    public class BsonObject {// : IEnumerable<KeyValuePair<string, object>> {

        #region Constructors

        /// <summary>
        /// Creates an empty DynamicObject
        /// </summary>
        public BsonObject() {
            this._Fields = new BsonMongoDictionary();
            this._Removed = new List<string>();
        }

        /// <summary>
        /// Creates a new DynamicObject using the provided type as a template
        /// </summary>
        public BsonObject(object source)
            : this() {
            BsonObject._Populate(this, source);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets if this dynamic object cares about properties
        /// being in the right case or not
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// Returns the total number of fields inside of this object
        /// </summary>
        public int FieldCount {
            get { return this._Fields.Count; }
        }

        //the fields that are included in this object
        private BsonMongoDictionary _Fields;
        private List<string> _Removed;

        #endregion

        #region Working With Fields

        /// <summary>
        /// Assigns or gets the value of a field on this object
        /// </summary>
        public object this[string field] {
            get { return this.Get(field); }
            set { this.Set(field, value); }
        }

        /// <summary>
        /// Uses an anonymous type as a template for the values to return
        /// </summary>
        public T Get<T>(T template) {
            if (BsonAnonymousTypeParser.IsAnonymousType(template)) {
                return BsonAnonymousTypeParser.PopulateAnonymousType<T>(this, template);
            }
            else {
                return template;
            }
        }

        /// <summary>
        /// Handles getting the value from this DynamicCObject
        /// </summary>
        public object Get(string field) {
            return this.Get<object>(field, null);
        }

        /// <summary>
        /// Handles getting the value from this DynamicCObject
        /// </summary>
        public T Get<T>(string field) {
            return this.Get(field, default(T));
        }

        /// <summary>
        /// Handles getting the value from this DynamicCObject
        /// </summary>
        public T Get<T>(string field, T @default) {

            //maps lower document levels
            if (BsonAnonymousTypeParser.IsAnonymousType(@default)) {
                return BsonAnonymousTypeParser.PopulateAnonymousType<T>(this, field, @default);
            }

            //locate the field first
            field = this._FormatField(field);
            MongoFieldReference detail = this._FindField(field, false);

            //if nothing is found then just return the value
            try {
                return detail.Field is MongoDataType
                    ? (T)detail.Field.Get<T>()
                    : @default;
            }
            //make sure they get some sort of value
            catch {
                return @default;
            }

        }

        /// <summary>
        /// Handles assigning an object to the DynamicObject
        /// </summary>
        public void Set(string field, object value) {
            this.Set<object>(field, value);
        }

        /// <summary>
        /// Handles assigning an object to the DynamicObject
        /// </summary>
        public void Set<T>(string field, T value) {

            //locate the field first
            field = this._FormatField(field);
            MongoFieldReference type = this._FindField(field, true);

            //check the field value was found
            if (type == null || type.Field == null) {
                type.Parent._Fields.Add(type.Name, null);
            }

            //if the type is found make sure it is compatible
            //and if not then simply create a new container
            if (type.Field == null || (type.Field is MongoDataType && !type.Field.IsAllowedValue(value))) {
                type.Parent._Fields[type.Name] = MongoDataType.FindTypeFor(value);
            }

            //then assign the value
            if (value is MongoDataType) {
                type.Parent._Fields[type.Name] = value as MongoDataType;
            }
            else {
                type.Parent._Fields[type.Name].Set(value);
            }

        }

        /// <summary>
        /// Returns if a field has a value of the specified type
        /// </summary>
        public bool Has(string field) {
            return this.Has<object>(field);
        }

        /// <summary>
        /// Returns if a field has a value of the specified type
        /// </summary>
        public bool Has<T>(string field) {
            return this.Get<T>(field) is T;
        }

        /// <summary>
        /// Serializes an object into a byte array
        /// </summary>
        public void Serialize<T>(string field, T value) where T : class {
            this.Serialize<T>(field, value, BsonObject._BinarySerialize);
        }

        /// <summary>
        /// Serializes an object into a byte array
        /// </summary>
        public void Serialize<T>(string field, T value, Func<T, byte[]> serialize) where T : class {
            byte[] bytes = serialize(value);
            this.Set<byte[]>(field, bytes);
        }

        /// <summary>
        /// Moves a field to the front of the document
        /// </summary>
        public void MoveToStart(string field) {
            this._MoveElement(field, true);
        }

        /// <summary>
        /// Moves a field to the end of the document
        /// </summary>
        public void MoveToEnd(string field) {
            this._MoveElement(field, false);
        }

        //reorders the list using the names provided
        private void _MoveElement(string field, bool top) {

            //try and find the field first
            MongoFieldReference detail = this._FindField(field, false);
            if (detail == null || detail.Field == null) { return; }

            //reorder the items on the list
            var items = this._Fields.Select(item => new {
                field = item,
                move = top
                    ? item.Key.Equals(detail.Name, this._GetCaseComparison())
                    : !item.Key.Equals(detail.Name, this._GetCaseComparison())
            })
            .OrderByDescending(item => item.move);

            //recreate the list in the new order
            this._Fields = new BsonMongoDictionary();
            foreach (var item in items) {
                this._Fields.Add(item.field.Key, item.field.Value);
            }

        }

        #endregion 

        #region Operators

        /// <summary>
        /// Merges another object with this object - Same as calling Merge
        /// </summary>
        public static BsonObject operator +(BsonObject target, object value) {
            target.Merge(value);
            return target;
        }

        /// <summary>
        /// Merges another DynamicObject with this object - Same as calling Merge
        /// </summary>
        public static BsonObject operator +(BsonObject target, BsonObject value) {
            target.Merge(value);
            return target;
        }

        /// <summary>
        /// Removes the field provided - Same as calling Remove
        /// </summary>
        public static BsonObject operator -(BsonObject target, string field) {
            target.Remove(field);
            return target;
        }

        /// <summary>
        /// Removes the field provided - Same as calling Remove
        /// </summary>
        public static BsonObject operator -(BsonObject target, string[] fields) {
            target.Remove(fields);
            return target;
        }

        #endregion

        #region Modifications

        /// <summary>
        /// Merges this object with the provided object
        /// </summary>
        public virtual void Merge<T>(T source) where T : class {
            BsonObject._Populate(this, source);
        }

        /// <summary>
        /// Merges this object with the provided DynamicObject
        /// </summary>
        public virtual void Merge(MongoDocument source) {
            this.Merge(source as BsonObject);
        }

        /// <summary>
        /// Merges this object with the provided DynamicObject
        /// </summary>
        public virtual void Merge(BsonDocument source) {
            this.Merge(source as BsonObject);
        }

        /// <summary>
        /// Merges this object with the provided DynamicObject
        /// </summary>
        public virtual void Merge(BsonObject source) {
            foreach (var item in source._Fields) {
                this.Set(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Merges this object with the provided DynamicObject
        /// </summary>
        public virtual void Merge(Dictionary<string, object> source) {
            foreach (var item in source) {
                this.Set(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Removes the field provided
        /// </summary>
        public virtual void Remove(string field) {
            this.Remove(new string[] { field });
        }

        /// <summary>
        /// Removes each of the fields provided
        /// </summary>
        public virtual void Remove(params string[] fields) {

            //remove each field name from the correct area
            this._Removed.AddRange(fields);
            foreach(string field in fields) {
                MongoFieldReference detail = this._FindField(field, false);
                if (detail.Field is MongoDataType && detail.Parent is BsonObject) {
                    detail.Parent._Fields.Remove(detail.Name);
                }
            }

        }

        #endregion

        #region Private Methods

        //get how case should be handled
        private StringComparison _GetCaseComparison() {
            return this.IgnoreCase
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
        }

        //formats a field to work with the document
        private string _FormatField(string field) {
            return (field ?? string.Empty).Trim();
        }

        //finds a field value based on the name
        private MongoFieldReference _FindField(string field, bool createIfMissing) {

            //checks a series of values in a connection 
            Func<string, BsonObject, MongoDataType> checkSet = (identity, target) => {

                //try and find the value
                KeyValuePair<string, MongoDataType>? pair = target._Fields
                    .FirstOrDefault(item => item.Key.Equals(identity, this._GetCaseComparison()));

                //return the value if it is found
                return pair is KeyValuePair<string, MongoDataType>
                    ? pair.Value.Value
                    : null;

            };

            //create a default container for the result
            MongoFieldReference container = new MongoFieldReference() {
                Parent = this,
                Field = null
            };

            //split the values (if any)
            string[] parts = Regex.Split(field, @"\.");
            foreach (string part in parts) {
                bool last = parts.Last().Equals(part, this._GetCaseComparison());

                //start by trying to find the result
                container.Name = part;
                container.Field = checkSet(part, container.Parent);

                //if this is missing and requesting to create, do it now
                if ((container == null || container.Field == null) && !last && createIfMissing) {
                    container.Field = new MongoDocumentType();
                    container.Field.Set(new BsonDocument());
                    container.Parent._Fields.Add(part, container.Field);
                }

                //assign the parent as required
                if (!last && container.Field is MongoDocumentType) {
                    container.Parent = container.Field.Get<BsonDocument>() as BsonDocument;
                }
            }

            //return the value found
            return container;

        }

        #endregion

        #region Static Creation

        //handles populating an object with information
        private static void _Populate(BsonObject target, object source) {
            if (source == null) { return; }
            if (source is BsonObject) { return; }
            
            //get the type information
            Type type = source.GetType();
            if (!type.IsClass) { return; }

            //find all of the properties and fields that can be read
            foreach (MemberInfo member in type.GetMembers()) {

                //check for properties 
                if (member is PropertyInfo) {
                    PropertyInfo property = member as PropertyInfo;
                    if (property.CanRead) {
                        target.Set(property.Name, property.GetValue(source, null));
                    }
                }
                //and fields
                else if (member is FieldInfo) {
                    FieldInfo field = member as FieldInfo;
                    if (field.IsPublic) {
                        target.Set(field.Name, field.GetValue(source));
                    }
                }

            }

        }

        #endregion

        #region Static Serialization

        /// <summary>
        /// Serializes an object into a byte array
        /// </summary>
        public static byte[] Serialize(object value) {
            return BsonObject.Serialize(value, BsonObject._BinarySerialize);
        }

        /// <summary>
        /// Handles serializing an object quickly
        /// </summary>
        public static byte[] Serialize(object value, Func<object, byte[]> serialize) {
            return serialize(value);
        }

        /// <summary>
        /// Serializes an object into a byte array
        /// </summary>
        public static object Deserialize(byte[] bytes) {
            return BsonObject.Deserialize(bytes, BsonObject._BinaryDeserialize);
        }

        /// <summary>
        /// Handles serializing an object quickly
        /// </summary>
        public static object Deserialize(byte[] bytes, Func<byte[], object> deserialze) {
            return deserialze(bytes);
        }

        #endregion

        #region Private Static Methods

        //handles standard serialization of objects
        private static byte[] _BinarySerialize<T>(T value) where T : class {

            //perform standard binary serialization
            try {
                using (MemoryStream output = new MemoryStream()) {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(output, value);
                    return output.ToArray();
                }
            }
            //notify if they cannot serialize this object
            catch (SerializationException ex) {
                throw new LameException("Not allowed to seralize");
            }
            //notify for other unexpected errors
            catch (Exception ex) {
                throw new LameException("Failed to serialize object");
            }

        }

        //handles standard deserialization of objects
        private static object _BinaryDeserialize(byte[] bytes) {

            //perform standard binary serialization
            try {
                using (MemoryStream input = new MemoryStream(bytes)) {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return formatter.Deserialize(input);
                }
            }
            //notify for other unexpected errors
            catch (Exception ex) {
                throw new LameException("Failed to serialize object");
            }

        }

        #endregion

        #region Interface Functionality

        /// <summary>
        /// Returns all of the values in this document 
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> GetValues() {
            return this._Fields
                .ToDictionary(item => item.Key, item => item.Value.Get<object>());
        }

        /// <summary>
        /// Returns a list of identities of removed fields
        /// </summary>
        public IEnumerable<string> GetRemovedFields() {
            return this._Removed.AsEnumerable();
        }

        #endregion

        #region Generation

        /// <summary>
        /// Method used to allow reordering of fields before rendering 
        /// the final byte array when calling ToBsonByteArray
        /// </summary>
        protected virtual IEnumerable<BsonFieldDetail> OnBeforeFinishBsonRender(IEnumerable<BsonFieldDetail> fields) {
            return fields;
        }

        /// <summary>
        /// Generates the object within this collection as 
        /// an array of BSON bytes
        /// </summary>
        public virtual byte[] ToBsonByteArray() {

            //get a container to hold the bytes while we export
            List<BsonFieldDetail> values = new List<BsonFieldDetail>();

            //generate all of the bytes required
            foreach (KeyValuePair<string, MongoDataType> item in this._Fields) {

                //render the bytes for this request
                using (MemoryStream output = new MemoryStream()) {
                    using (BinaryWriter writer = new BinaryWriter(output)) {

                        //write the type, key and BSON value
                        writer.Write(BsonTranslator.AsByte((byte)item.Value.GetDataType()));
                        writer.Write(BsonTranslator.AsString(item.Key));
                        writer.Write(item.Value.ToBson());

                        //save our temporary information
                        values.Add(
                            new BsonFieldDetail(
                                item.Key,
                                output.ToArray(),
                                item.Value
                                ));

                    }
                }

            }

            //allow any reordering of bytes
            values = this.OnBeforeFinishBsonRender(values).ToList();

            //merge the final list
            List<byte> bytes = new List<byte>();
            foreach (byte[] set in values.Select(item => item.Bytes)) {
                bytes.AddRange(set);
            }

            //return the final bytes to use
            return bytes.ToArray();
            
        }

        #endregion

        #region Overriding Methods

        /// <summary>
        /// Returns a hash value of the contents of this object
        /// to determine when the object has been changed
        /// </summary>
        public string GetObjectHash() {
            return Convert.ToBase64String(SHA256.Create().ComputeHash(this.ToBsonByteArray()));
        }

        #endregion

    }

}
