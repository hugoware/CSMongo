using System.Collections.Generic;
using System.Linq;

namespace CSMongo.IO {

    /// <summary>
    /// Works with bytes that could be modified in different locations
    /// at any given time
    /// </summary>
    public class DynamicStream {

        #region Constructors

        /// <summary>
        /// Creates an empty DynamicStream
        /// </summary>
        public DynamicStream()
           : this(0) {
        }

        /// <summary>
        /// Creates a stream with the provided length with all 0 bytes
        /// </summary>
        public DynamicStream(int length)
            : this(length, (byte)0) {
        }

        /// <summary>
        /// Creates a stream with the provided length defaulting to the byte specified
        /// </summary>
        public DynamicStream(int length, byte @default) {
            this._Output = new List<byte>();
            for (int i = 0; i < length; i++) {
                this._Output.Add(@default);
            }
        }

        #endregion

        #region Fields

        //contains the bytes that are being modified
        List<byte> _Output;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the current length of the stream
        /// </summary>
        public int Length {
            get { return this._Output.Count; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a new byte at the specified index
        /// </summary>
        public void InsertAt(int index, byte[] bytes) {
            this._Reset();
            this._Output.InsertRange(index, bytes);
        }

        /// <summary>
        /// Inserts new bytes at the specified index
        /// </summary>
        public void InsertAt(int index, byte @byte) {
            this._Reset();
            this._Output.Insert(index, @byte);
        }

        /// <summary>
        /// Overwrites the byte at the specified index
        /// </summary>
        public void WriteAt(int index, byte @byte) {
            this._Reset();
            this._Output.RemoveAt(index);
            this.InsertAt(index, @byte);
        }

        /// <summary>
        /// Overwrites the bytes the the specified index
        /// </summary>
        public void WriteAt(int index, byte[] bytes) {
            this._Reset();
            this._Output.RemoveRange(index, bytes.Length);
            this.InsertAt(index, bytes);
        }

        /// <summary>
        /// Appends the byte to the end of the stream
        /// </summary>
        public void Append(byte @byte) {
            this._Reset();
            this._Output.Add(@byte);
        }

        /// <summary>
        /// Appends the bytes to the end of the stream
        /// </summary>
        public void Append(byte[] bytes) {
            this._Reset();
            this._Output.InsertRange(this._Output.Count, bytes);
        }

        /// <summary>
        /// Reads the bytes within the specified area
        /// </summary>
        public byte[] Read(int start, int length) {
            return this._Output.Skip(start).Take(length).ToArray();
        }

        /// <summary>
        /// Returns all of the bytes for the stream as an array
        /// </summary>
        public byte[] ToArray() {
            if (this._Generated == null) {
                this._Generated = this._Output.ToArray();
            }
            return this._Generated;
        }
        private byte[] _Generated;

        //clears the saved array of bytes, if any
        private void _Reset() {
            this._Generated = null;
        }

        #endregion

    }

}
