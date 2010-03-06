using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using CSMongo.Requests;
using CSMongo.Exceptions;
using CSMongo.Responses;

namespace CSMongo {

    /// <summary>
    /// Creates a new connection to a Mongo database
    /// </summary>
    public class MongoConnection : IDisposable {

        #region Constructors

        /// <summary>
        /// Creates a new Mongo connection
        /// </summary>
        public MongoConnection(string host)
            : this (host, Mongo.DefaultPort, true) {
        }

        /// <summary>
        /// Creates a new Mongo connection
        /// </summary>
        public MongoConnection(string host, bool autoConnect)
            : this(host, Mongo.DefaultPort, autoConnect) {
        }
        
        /// <summary>
        /// Creates a new Mongo connection
        /// </summary>
        public MongoConnection(string host, int port)
            : this(host, port, true) {
        }

        /// <summary>
        /// Creates a new Mongo connection
        /// </summary>
        public MongoConnection(string host, int port, bool autoConnect) {
            this.Host = (host ?? string.Empty).Trim();
            this.Port = port;
            this.AutoConnect = autoConnect;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the port to connect on
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the host to connect to
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets if this connection should automatically open
        /// </summary>
        public bool AutoConnect { get; set; }

        /// <summary>
        /// Returns of this connection is currently open or not
        /// </summary>
        public bool Connected {
            get { return this._Client is TcpClient && this._Client.Connected; }
        }

        //the current connection to the host
        private TcpClient _Client;
        private BufferedStream _Buffer;
        private BinaryWriter _Writer;

        #endregion

        #region Events

        /// <summary>
        /// Event raised just before the database is closed
        /// </summary>
        public event Action<MongoConnection> BeforeConnectionOpened = (connection) => { };

        /// <summary>
        /// Event raised when right after the database is closed
        /// </summary>
        public event Action<MongoConnection> AfterConnectionOpen = (connection) => { };

        /// <summary>
        /// Event raised just before the database is closed
        /// </summary>
        public event Action<MongoConnection> BeforeConnectionClosed = (connection) => { };

        /// <summary>
        /// Event raised when right after the database is closed
        /// </summary>
        public event Action<MongoConnection> AfterConnectionClosed = (connection) => { };

        #endregion

        #region Methods

        /// <summary>
        /// Opens the connection to the database
        /// </summary>
        public void Open() {
            if (this.Connected) { return; }
            Console.WriteLine("opening...");

            //notify any event handlers this is opening
            if (this.BeforeConnectionOpened != null) { this.BeforeConnectionOpened(this); }

            //and then try and open the connection
            this._Client = new TcpClient();
            this._Client.Connect(this.Host, this.Port);
            this._Buffer = new BufferedStream(this._Client.GetStream());
            this._Writer = new BinaryWriter(this._Buffer);

            //notify this has been connected
            if (this.AfterConnectionOpen != null) { this.AfterConnectionOpen(this); }

        }

        /// <summary>
        /// Handles disconnecting from the client
        /// </summary>
        public void Close() {
            Console.WriteLine("closing...");

            //notify any event handlers
            if (this.BeforeConnectionClosed != null) { this.BeforeConnectionClosed(this); }

            //close up all of the streams
            if (this._Buffer is BufferedStream) { this._Buffer.Dispose(); }
            if (this._Writer is BinaryWriter) { this._Writer.Close(); }
            if (this._Client is TcpClient) { this._Client.Close(); }

            //and then finally any event handling
            if (this.AfterConnectionClosed != null) { this.AfterConnectionClosed(this); }
        }

        /// <summary>
        /// Sends a request to the server
        /// </summary>
        public ResponseBase SendRequest(RequestBase request) {

            //manage the connection state automatically if needed
            if (this.AutoConnect) { this.Open(); }

            //attempt to perform the request
            try {

                //perform normal checking
                if (!this.Connected) {
                    throw new LameException("Connection isn't open yet!");
                }

                //send the header first
                this._Writer.Write(request.GetHeader());
                this._Writer.Flush();

                //then the rest of the content
                this._Writer.Write(request.GetBody());
                this._Writer.Flush();

                //next, read for the response
                return request.OnResponse(this._Buffer);

            }
            //forward the exception onto the caller
            catch (Exception up) {

                //attempt to kill the connection
                //ignore any problems since we are
                //already forwarding an exception
                try { this.Dispose(); }
                catch { }

                //and then forward the error for handling
                throw up;
            }
            

        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Handles disconnecting and disposing a connection
        /// </summary>
        public virtual void Dispose() {
            this.Close();
        }

        #endregion

    }

}