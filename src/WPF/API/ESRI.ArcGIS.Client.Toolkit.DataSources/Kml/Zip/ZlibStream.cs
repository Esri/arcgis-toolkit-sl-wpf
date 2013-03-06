// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

// Copyright (c) 2009, Dino Chiesa.  
// This code is licensed under the Microsoft public license.  See the license.txt file in the source
// distribution for details. 
//
// The zlib code is derived from the jzlib implementation, but significantly modified.
// The object model is not the same, and many of the behaviors are different.
// Nonetheless, in keeping with the license for jzlib, I am reproducing the copyright to that code here.
// 
// -----------------------------------------------------------------------
// Copyright (c) 2000,2001,2002,2003 ymnk, JCraft,Inc. All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright 
// notice, this list of conditions and the following disclaimer in 
// the documentation and/or other materials provided with the distribution.
// 
// 3. The names of the authors may not be used to endorse or promote products
// derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL JCRAFT,
// INC. OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
// OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 

/*
* This code is based on zlib-1.1.3;  credit to authors
* Jean-loup Gailly(jloup@gzip.org) and Mark Adler(madler@alumni.caltech.edu)
* and contributors of zlib.
*/


using System;
namespace ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip
{
    /// <summary>
    /// A class for compressing and decompressing streams using the Deflate algorithm.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Data can be compressed or decompressed, and either of those can be through reading or writing. 
    /// For more information on the Deflate algorithm, see IETF RFC 1951, "DEFLATE Compressed Data 
    /// Format Specification version 1.3." 
    /// </para>
    /// <para>
    /// This class is similar to ZlibStream, except that <c>ZlibStream</c> adds the RFC1950 
    /// header bytes to a compressed stream when compressing, or expects the RFC1950 header bytes when 
    /// decompressing. The <c>DeflateStream</c> does not.
    /// </para>
    /// </remarks>
    internal class DeflateStream : System.IO.Stream
    {
        internal ZlibBaseStream _baseStream;


        /// <summary>
        /// Create a DeflateStream using the specified CompressionMode, and explicitly specify whether
        /// the stream should be left open after Deflation or Inflation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This constructor allows the application to request that the captive stream remain open after
        /// the deflation or inflation occurs.  By default, after Close() is called on the stream, the 
        /// captive stream is also closed. In some cases this is not desired, for example if the stream 
        /// is a memory stream that will be re-read after compression.  Specify true for the 
        /// leaveOpen parameter to leave the stream open. 
        /// </para>
        /// <para>
        /// The DeflateStream will use the default compression level.
        /// </para>
        /// <para>
        /// See the other overloads of this constructor for example code.
        /// </para>
        /// </remarks>
        /// <param name="stream">The stream which will be read or written. This is called the 
        /// "captive" stream in other places in this documentation.</param>
        /// <param name="mode">Indicates whether the DeflateStream will compress or decompress.</param>
        /// <param name="leaveOpen">true if the application would like the stream to remain open after inflation/deflation.</param>
        public DeflateStream(System.IO.Stream stream, CompressionMode mode, bool leaveOpen)
            : this(stream, mode, CompressionLevel.LEVEL6_DEFAULT, leaveOpen)
        {
        }

        /// <summary>
        /// Create a DeflateStream using the specified CompressionMode and the specified CompressionLevel, 
        /// and explicitly specify whether
        /// the stream should be left open after Deflation or Inflation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This constructor allows the application to request that the captive stream remain open after
        /// the deflation or inflation occurs.  By default, after Close() is called on the stream, the 
        /// captive stream is also closed. In some cases this is not desired, for example if the stream 
        /// is a memory stream that will be re-read after compression.  Specify true for the 
        /// leaveOpen parameter to leave the stream open. 
        /// </para>
        /// </remarks>
        /// <example>
        /// This example shows how to use a DeflateStream to compress data.
        /// <code>
        /// using (System.IO.Stream input = System.IO.File.OpenRead(fileToCompress))
        /// {
        ///     using (var raw = System.IO.File.Create(outputFile))
        ///     {
        ///         using (Stream compressor = new DeflateStream(raw, CompressionMode.Compress, true))
        ///         {
        ///             byte[] buffer = new byte[WORKING_BUFFER_SIZE];
        ///             int n= -1;
        ///             while (n != 0)
        ///             {
        ///                 if (n &gt; 0) 
        ///                     compressor.Write(buffer, 0, n);
        ///                 n= input.Read(buffer, 0, buffer.Length);
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// <code lang="VB">
        /// Dim outputFile As String = (fileToCompress &amp; ".compressed")
        /// Using input As Stream = File.OpenRead(fileToCompress)
        ///     Using raw As FileStream = File.Create(outputFile)
        /// 	Using compressor As Stream = New DeflateStream(raw, CompressionMode.Compress, True)
        /// 	    Dim buffer As Byte() = New Byte(4096) {}
        /// 	    Dim n As Integer = -1
        /// 	    Do While (n &lt;&gt; 0)
        /// 		If (n &gt; 0) Then
        /// 		    compressor.Write(buffer, 0, n)
        /// 		End If
        /// 		n = input.Read(buffer, 0, buffer.Length)
        /// 	    Loop
        /// 	End Using
        ///     End Using
        /// End Using
        /// </code>
        /// </example>
        /// <param name="stream">The stream which will be read or written.</param>
        /// <param name="mode">Indicates whether the DeflateStream will compress or decompress.</param>
        /// <param name="leaveOpen">true if the application would like the stream to remain open after inflation/deflation.</param>
        /// <param name="level">A tuning knob to trade speed for effectiveness.</param>
        public DeflateStream(System.IO.Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen)
        {
            _baseStream = new ZlibBaseStream(stream, mode, level, false, leaveOpen);
        }

        #region Zlib properties

        /// <summary>
        /// This property sets the flush behavior on the stream.  
        /// Sorry, though, not sure exactly how to describe all the various settings.
        /// </summary>
        virtual public int FlushMode
        {
            get { return (this._baseStream._flushMode); }
            set { this._baseStream._flushMode = value; }
        }

        /// <summary>
        /// Callers can set the buffer size of the working buffer with this property.  
        /// </summary>
        /// <remarks>
        /// The working buffer is used for all stream operations.
        /// The default size is 1024 bytes.  The minimum size is 128 bytes. You may get better 
        /// performance with a larger buffer.  Then again, you might not.  I don't know, I haven't tested it.  
        /// </remarks>
        public int BufferSize
        {
            get
            {
                return this._baseStream._workingBuffer.Length;
            }
            set
            {
                if (value < this._baseStream.WORKING_BUFFER_SIZE_MIN) throw new ZlibException("Don't be silly. Use a bigger buffer.");
                this._baseStream._workingBuffer = new byte[value];
            }
        }

        /// <summary> Returns the total number of bytes input so far.</summary>
        virtual public long TotalIn
        {
            get
            {
                return this._baseStream._z.TotalBytesIn;
            }
        }

        /// <summary> Returns the total number of bytes output so far.</summary>
        virtual public long TotalOut
        {
            get
            {
                return this._baseStream._z.TotalBytesOut;
            }
        }

        #endregion

        #region System.IO.Stream methods
        /// <summary>
        /// Close the stream.  
        /// </summary>
        /// <remarks>
        /// This may or may not close the captive stream. 
        /// See the ctor's with leaveOpen parameters for more information.
        /// </remarks>
        public override void Close()
        {
            _baseStream.Close();
        }

        /// <summary>
        /// Indicates whether the stream can be read.
        /// </summary>
        /// <remarks>
        /// The return value depends on whether the captive stream supports reading.
        /// </remarks>
        public override bool CanRead
        {
            get { return _baseStream._stream.CanRead; }
        }

        /// <summary>
        /// Indicates whether the stream supports Seek operations.
        /// </summary>
        /// <remarks>
        /// Always returns false.
        /// </remarks>
        public override bool CanSeek
        {
            get { return false; }
        }


        /// <summary>
        /// Indicates whether the stream can be written.
        /// </summary>
        /// <remarks>
        /// The return value depends on whether the captive stream supports writing.
        /// </remarks>
        public override bool CanWrite
        {
            get { return _baseStream._stream.CanWrite; }
        }

        /// <summary>
        /// Flush the stream.
        /// </summary>
        public override void Flush()
        {
            _baseStream.Flush();
        }

        /// <summary>
        /// Reading this property always throws a NotImplementedException.
        /// </summary>
        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Reading or Writing this property always throws a NotImplementedException.
        /// </summary>
        public override long Position
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Read data from the stream. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// If you wish to use the DeflateStream to compress data while reading, you can create a DeflateStream with 
        /// CompressionMode.Compress, providing an uncompressed data stream.  Then call Read() on that DeflateStream, 
        /// and the data read will be compressed. 
        /// If you wish to use the DeflateStream to decompress data while reading, you can create a DeflateStream with 
        /// CompressionMode.Decompress, providing a readable compressed data stream.  Then call Read() on that DeflateStream, 
        /// and the data read will be decompressed. 
        /// </para>
        /// <para>
        /// A DeflateStream can be used for Read() or Write(), but not both. 
        /// </para>
        /// </remarks>
        /// <param name="buffer">The buffer into which the read data should be placed.</param>
        /// <param name="offset">the offset within that data array to put the first byte read.</param>
        /// <param name="count">the number of bytes to read.</param>
        /// <returns>the number of bytes actually read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _baseStream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Calling this method always throws a <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="offset">this is irrelevant, since it will always throw!</param>
        /// <param name="origin">this is irrelevant, since it will always throw!</param>
        /// <returns>irrelevant!</returns>
        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calling this method always throws a NotImplementedException.
        /// </summary>
        /// <param name="value">this is irrelevant, since it will always throw!</param>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write data to the stream. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// If you wish to use the DeflateStream to compress data while writing, you can create a DeflateStream with 
        /// CompressionMode.Compress, and a writable output stream.  Then call Write() on that DeflateStream, 
        /// providing uncompressed data as input.  The data sent to the output stream will be the compressed form of the data written.
        /// If you wish to use the DeflateStream to decompress data while writing, you can create a DeflateStream with 
        /// CompressionMode.Decompress, and a writable output stream.  Then call Write() on that stream, providing previously 
        /// compressed data. The data sent to the output stream will be the decompressed form of the data written.   
        /// </para>
        /// <para>
        /// A DeflateStream can be used for Read() or Write(), but not both. 
        /// </para>
        /// </remarks>
        /// <param name="buffer">The buffer holding data to write to the stream.</param>
        /// <param name="offset">the offset within that data array to find the first byte to write.</param>
        /// <param name="count">the number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _baseStream.Write(buffer, offset, count);
        }
        #endregion

    }


 
    internal class ZlibBaseStream : System.IO.Stream
    {
        protected internal ZlibCodec _z = new ZlibCodec();
        protected internal readonly int WORKING_BUFFER_SIZE_DEFAULT = 16384; // 1024;
        protected internal readonly int WORKING_BUFFER_SIZE_MIN = 128;

        protected internal StreamMode _streamMode = StreamMode.Undefined;
        protected internal int _flushMode;
        protected internal bool _leaveOpen;
        protected internal byte[] _workingBuffer;
        protected internal byte[] _buf1 = new byte[1];
        protected internal bool _wantCompress;

        protected internal System.IO.Stream _stream;


        public ZlibBaseStream(System.IO.Stream stream, CompressionMode compressionMode, CompressionLevel level, bool wantRfc1950Header, bool leaveOpen)
            : base()
        {
            this._flushMode = ZlibConstants.Z_NO_FLUSH;
            this._workingBuffer = new byte[WORKING_BUFFER_SIZE_DEFAULT];
            this._stream = stream;
            this._leaveOpen = leaveOpen;
            if (compressionMode == CompressionMode.Decompress)
            {
                _z.InitializeInflate(wantRfc1950Header);
                this._wantCompress = false;
            }
        }


        public override void WriteByte(byte b)
        {
            _buf1[0] = (byte)b;
            Write(_buf1, 0, 1);
        }

        public override void Write(System.Byte[] buffer, int offset, int length)
        {
            if (_streamMode == StreamMode.Undefined) _streamMode = StreamMode.Writer;
            if (_streamMode != StreamMode.Writer)
                throw new ZlibException("Cannot Write after Reading.");

            if (length == 0)
                return;

            _z.InputBuffer = buffer;
            _z.NextIn = offset;
            _z.AvailableBytesIn = length;
            do
            {
                _z.OutputBuffer = _workingBuffer;
                _z.NextOut = 0;
                _z.AvailableBytesOut = _workingBuffer.Length;
                int rc = (_wantCompress)
                    ? 1
                    : _z.Inflate(_flushMode);
                if (rc != ZlibConstants.Z_OK && rc != ZlibConstants.Z_STREAM_END)
                    throw new ZlibException((_wantCompress ? "de" : "in") + "flating: " + _z.Message);
                _stream.Write(_workingBuffer, 0, _workingBuffer.Length - _z.AvailableBytesOut);
            }
            while (_z.AvailableBytesIn > 0 || _z.AvailableBytesOut == 0);
        }


        private void finish()
        {
            if (_streamMode == StreamMode.Writer)
            {
                do
                {
                    _z.OutputBuffer = _workingBuffer;
                    _z.NextOut = 0;
                    _z.AvailableBytesOut = _workingBuffer.Length;
                    int rc = (_wantCompress)
                        ? -1
                        : _z.Inflate(ZlibConstants.Z_FINISH);

                    if (rc != ZlibConstants.Z_STREAM_END && rc != ZlibConstants.Z_OK)
                        throw new ZlibException((_wantCompress ? "de" : "in") + "flating: " + _z.Message);

                    if (_workingBuffer.Length - _z.AvailableBytesOut > 0)
                    {
                        _stream.Write(_workingBuffer, 0, _workingBuffer.Length - _z.AvailableBytesOut);
                    }
                }
                while (_z.AvailableBytesIn > 0 || _z.AvailableBytesOut == 0);
                Flush();
            }
        }


        private void end()
        {
            if (_z == null)
                return;
            if (_wantCompress)
            {
                //_z.EndDeflate();
            }
            else
            {
                _z.EndInflate();
            }
            _z = null;
        }

        public override void Close()
        {
            try
            {
                try
                {
                    finish();
                }
                catch (System.IO.IOException)
                {
                    // swallow exceptions?
                }
            }
            finally
            {
                end();
                if (!_leaveOpen) _stream.Close();
                _stream = null;
            }
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override System.Int64 Seek(System.Int64 offset, System.IO.SeekOrigin origin)
        {
            throw new NotImplementedException();
            //_outStream.Seek(offset, origin);
        }
        public override void SetLength(System.Int64 value)
        {
            _stream.SetLength(value);
        }


        public int Read()
        {
            if (Read(_buf1, 0, 1) == -1)
                return -1;
            return (_buf1[0] & 0xFF);
        }

        private bool nomoreinput = false;



        public override System.Int32 Read(System.Byte[] buffer, System.Int32 offset, System.Int32 count)
        {
            if (_streamMode == StreamMode.Undefined)
            {
                // for the first read, set up some controls.
                _streamMode = StreamMode.Reader;
                _z.AvailableBytesIn = 0;
            }
            if (_streamMode != StreamMode.Reader)
                throw new ZlibException("Cannot Read after Writing.");

            if (!this._stream.CanRead) throw new ZlibException("The stream is not readable.");
            if (count == 0)
                return 0;

            int rc;

            // set up the output of the deflate/inflate codec:
            _z.OutputBuffer = buffer;
            _z.NextOut = offset;
            _z.AvailableBytesOut = count;

            // this is not always necessary, but is helpful in case _workingBuffer has been resized. (new byte[])
            _z.InputBuffer = _workingBuffer;

            do
            {
                // need data in _workingBuffer in order to deflate/inflate.  Here, we check if we have any.
                if ((_z.AvailableBytesIn == 0) && (!nomoreinput))
                {
                    // No data available, so try to Read data from the captive stream.
                    _z.NextIn = 0;
                    _z.AvailableBytesIn = SharedUtils.ReadInput(_stream, _workingBuffer, 0, _workingBuffer.Length);
                    //(bufsize<z.avail_out ? bufsize : z.avail_out));
                    if (_z.AvailableBytesIn == -1)
                    {
                        _z.AvailableBytesIn = 0;
                        nomoreinput = true;
                    }
                }
                // we have data in InputBuffer; now compress or decompress as appropriate
                rc = (_wantCompress)
                    ? -1
                    : _z.Inflate(_flushMode);

                if (nomoreinput && (rc == ZlibConstants.Z_BUF_ERROR))
                    return (-1);
                if (rc != ZlibConstants.Z_OK && rc != ZlibConstants.Z_STREAM_END)
                    throw new ZlibException((_wantCompress ? "de" : "in") + "flating: " + _z.Message);
                if ((nomoreinput || rc == ZlibConstants.Z_STREAM_END) && (_z.AvailableBytesOut == count))
                    return (-1);
            }
            while (_z.AvailableBytesOut == count && rc == ZlibConstants.Z_OK);

            return (count - _z.AvailableBytesOut);
        }



        public override System.Boolean CanRead
        {
            get { return this._stream.CanRead; }
        }

        public override System.Boolean CanSeek
        {
            get { return this._stream.CanSeek; }
        }

        public override System.Boolean CanWrite
        {
            get { return this._stream.CanWrite; }
        }

        public override System.Int64 Length
        {
            get { return _stream.Length; }
        }

        public override long Position
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        internal enum StreamMode
        {
            Writer,
            Reader,
            Undefined,
        }
    }
}