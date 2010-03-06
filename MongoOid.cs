using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Text;
using CSMongo.IO;
using CSMongo.Exceptions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Net;
using System.Diagnostics;

namespace CSMongo {

    /// <summary>
    /// Represents an ID number to use for MongoOids
    /// </summary>
    public class MongoOid {

        #region Constants

        /// <summary>
        /// The number of bytes to expect from a MongoOid
        /// </summary>
        public static readonly int MongoOidByteLength = 12;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Oid and generates an Oid automatically
        /// </summary>
        public MongoOid() {
            this.Value = MongoOid.Generate();
        }

        /// <summary>
        /// Creates an ID using a string version of the Oid
        /// </summary>
        public MongoOid(string id) {
            this.SetId(id);
        }

        /// <summary>
        /// Creates an new Oid with the provided bytes - Must be 12 bytes long
        /// </summary>
        public MongoOid(byte[] identifier) {
            this.Value = identifier;
        }

        #endregion

        #region Static Fields
        
        //holds the current incremented value
        private static int _Identifier = 0;

        #endregion

        #region Static Methods

        //handles getting access to a shared identifier
        private static int _GetIdentifier() {
            return Interlocked.Increment(ref MongoOid._Identifier);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The byte value of the identifier
        /// </summary>
        public byte[] Value { get; private set; }

        /// <summary>
        /// Sets the ID to a specific value using a set of bytes
        /// </summary>
        public void SetId(byte[] value) {
            this.Value = value;
        }

        /// <summary>
        /// Converts a string into an ID - IDs should be a hexadecimal string
        /// that is either 35 characters (with hyphens) or 24 characters without
        /// </summary>
        public void SetId(string value) {

            //clean up the string first
            value = Regex.Replace(value, "[^a-z0-9]", string.Empty, RegexOptions.IgnoreCase);

            //verify the length
            if (value.Length != 24) {
                throw new ArgumentException("The provided ID was not in an expected format.", "value");
            }

            //parse every pair of bytes
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < value.Length; i += 2) {
                string pair = value.Substring(i, 2);
                byte parsed = byte.Parse(pair, NumberStyles.HexNumber);
                bytes.Add(parsed);
            }

            //and then assign normally
            this.SetId(bytes.ToArray());

        }

        /// <summary>
        /// Returns a string version of this ID
        /// </summary>
        public string GetId() {
            return BitConverter.ToString(this.Value)
                .Replace("-", string.Empty)
                .ToLower();
        }

        #endregion

        #region Overriding Methods

        /// <summary>
        /// Returns the string format of the Oid
        /// </summary>
        public override string ToString() {
            return string.Format("Oid:{0}", this.GetId());
        }

        #endregion

        #region Static Creation

        /// <summary>
        /// Generates the bytes for a new MongoOid
        /// </summary>
        public static byte[] Generate() {
            List<byte> bytes = new List<byte>();

            //generate the correct prefix (Suggestion from Sam Corder)
            double span = (DateTime.UtcNow - Mongo.Epoch).TotalSeconds;
            int floor = Convert.ToInt32(Math.Floor(span));
            bytes.AddRange(BitConverter.GetBytes(floor).Reverse());

            //use a semi-unique value - Not sure if this
            //is supposed to be any paticular format
            bytes.AddRange(Guid.NewGuid().ToByteArray().Skip(8));
            return bytes.ToArray();

        }

        #endregion

    }

}
