#define OPTIMIZE_WI6612

// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

//
// Copyright (c) 2006, 2007, 2008, 2009 Microsoft Corporation.  All rights reserved.
//
// This class library reads and writes zip files, according to the format
// described by pkware, at:
// http://www.pkware.com/business_and_developers/developer/popups/appnote.txt
//
// This implementation was originally based on the
// System.IO.Compression.DeflateStream base class in the .NET Framework
// v2.0 base class library, but now includes a managed-code port of Zlib.
//
// 
// This code is released under the Microsoft Public License . 
// See the License.txt for details.  
//
// 
// Fri, 31 Mar 2006  14:43
//


using System;
using System.IO;


namespace ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip
{
    /// <summary>
    /// The ZipFile type represents a zip archive file.  This is the main type in the 
    /// DotNetZip class library.  This class reads and writes zip files, as defined in the format
    /// for zip described by PKWare.  The compression for this implementation was, at one time, based on the
    /// System.IO.Compression.DeflateStream base class in the .NET Framework
    /// base class library, available in v2.0 and later of the .NET Framework. As of v1.7 of DotNetZip,
    /// the compression is provided by a managed-code version of Zlib, included with DotNetZip. 
    /// </summary>
    internal partial class ZipFile : System.Collections.Generic.IEnumerable<ZipEntry>,
    IDisposable
    {

        #region public properties

       
        /// <summary>
        /// A comment attached to the zip archive.
        /// </summary>
        ///
        /// <remarks>
        ///
        /// <para>
        /// This property is read/write for the zipfile. It allows the application to
        /// specify a comment for the zipfile, or read the comment for the zipfile. 
        /// After setting this property, changes are only made permanent when you call a
        /// <c>Save()</c> method.
        /// </para>
        ///
        /// <para>
        /// According to the zip specification, the comment is not encrypted, even if there is a password
        /// set on the zip archive. 
        /// </para>
        ///
        /// <para>
        /// The zip spec does not describe how to encode the comment string in a code page other than IBM437. 
        /// Therefore, for "compliant" zip tools and libraries, comments will use IBM437.  However, there are
        /// situations where you want an encoded Comment, for example using code page 950 "Big-5 Chinese".
        /// DotNetZip will encode the comment in the code page specified by <see cref="ProvisionalAlternateEncoding"/>,
        /// at the time of the call to ZipFile.Save().
        /// </para>
        ///
        /// <para>
        /// When creating a zip archive using this library, it is possible to change the value of 
        /// <see cref="ProvisionalAlternateEncoding" /> between each entry you add, and between adding entries and the 
        /// call to Save(). Don't do this.  It will likely result in a zipfile that is not readable by 
        /// any tool or application. 
        /// For best interoperability, leave <see cref="ProvisionalAlternateEncoding" /> alone, or 
        /// specify it only once, before adding any entries to the ZipFile instance.
        /// </para>
        ///
        /// </remarks>
        public string Comment
        {
            get { return _Comment; }
            set
            {
                _Comment = value;
            }
        }

        /// <summary>
        /// Indicates whether to perform case-sensitive matching on the filename when retrieving
        /// entries in the zipfile via the string-based indexer.  
        /// </summary>
        /// <remarks>
        /// The default value is <c>false</c>,
        /// which means DON'T do case-sensitive matching. In other words, retrieving
        /// zip["ReadMe.Txt"] is the same as zip["readme.txt"].
        /// It really makes sense to set this to <c>true</c> only if you are not running on
        /// Windows, which has case-insensitive filenames. But since this library is not built for
        /// non-Windows platforms, in most cases you should just leave this property alone. 
        /// </remarks>
        public bool CaseSensitiveRetrieval
        {
            get { return _CaseSensitiveRetrieval; }
            set { _CaseSensitiveRetrieval = value; }
        }


        /// <summary>
        /// Indicates whether to encode entry filenames and entry comments using Unicode 
        /// (UTF-8) according to the PKWare specification, for those filenames and comments
        /// that cannot be encoded in the IBM437 character set.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The PKWare specification provides for encoding in either the IBM437 code page, or in UTF-8. 
        /// This flag selects the encoding according to that specification. 
        /// By default, this flag is false, and filenames and comments are encoded 
        /// into the zip file in the IBM437 codepage. 
        /// Setting this flag to true will specify that
        /// filenames and comments are encoded with UTF-8. 
        /// </para>
        /// <para>
        /// Zip files created with strict adherence to the PKWare specification
        /// with respect to UTF-8 encoding can contain entries with filenames containing
        /// any combination of Unicode characters, including the full 
        /// range of characters from Chinese, Latin, Hebrew, Greek, Cyrillic, and many 
        /// other alphabets. 
        /// However, because the UTF-8 portion of the PKWare specification is not broadly
        /// supported by other zip libraries and utilities, such zip files may not
        /// be readable by your favorite zip tool or archiver. In other words, interoperability
        /// will decrease if you set this flag to true. 
        /// </para>
        /// <para>
        /// In particular, Zip files created with strict adherence to the PKWare 
        /// specification with respect to UTF-8 encoding will not work well with 
        /// Explorer in Windows XP or Windows Vista, because Vista compressed folders 
        /// do not support UTF-8 in zip files.  Vista can read the zip files, but shows
        /// the filenames incorrectly.  Unpacking from Windows Vista Explorer will result in filenames
        /// that have rubbish characters in place of the high-order UTF-8 bytes.
        /// </para>
        /// <para>
        /// Also, zip files that use UTF-8 encoding will not work well 
        /// with Java applications that use the java.util.zip classes, as of 
        /// v5.0 of the Java runtime. The Java runtime does not correctly 
        /// implement the PKWare specification in this regard.
        /// </para>
        /// <para>
        /// As a result, we have the unfortunate situation that "correct" 
        /// behavior by the DotNetZip library with regard to Unicode during zip creation will result 
        /// in zip files that are readable by strictly compliant and current tools (for example the most 
        /// recent release of the commercial WinZip tool); but these zip files will
        /// not  be readable by various other tools or libraries, including Windows Explorer.
        /// </para>
        /// <para>
        /// The DotNetZip library can read and write zip files 
        /// with UTF8-encoded entries, according to the PKware spec.  If you use DotNetZip for both 
        /// creating and reading the zip file, and you use UTF-8, there will be no loss of information 
        /// in the filenames. For example, using a self-extractor created by this
        /// library will allow you to unpack files correctly with no loss of 
        /// information in the filenames. 
        /// </para>
        /// <para>
        /// Encoding filenames and comments using the IBM437 codepage, the default
        /// behavior, will cause loss of information on some filenames,
        /// but the resulting zipfile will
        /// be more interoperable with other utilities. As an example of the 
        /// loss of information, the o-tilde character will be down-coded to plain o. 
        /// Likewise, the O with a stroke through it, used in Danish and Norwegian,
        /// will be down-coded to plain o. Chinese characters cannot be represented
        /// in codepage IBM437; when using the default encoding, Chinese characters in 
        /// filenames will be represented as ?.  
        /// </para>
        /// <para>
        /// The loss of information associated to the use of the IBM437 encoding can lead to
        /// runtime errors. For example, using IBM437, any sequence of 4 Chinese characters will
        /// be encoded as ????.  If your application creates a ZipFile, then adds two files, each
        /// with names of four Chinese characters each, this will result in a duplicate filename
        /// exception.  In the case where you add a single file with a name containing four
        /// Chinese characters, calling Extract() on the entry that has question marks in the
        /// filename will result in an exception, because the question mark is not legal for use
        /// within filenames on Windows.  These are just a few examples of the problems associated
        /// to loss of information.
        /// </para>
        /// <para>
        /// This flag is independent of the encoding of the content within the 
        /// entries in the zip file.  
        /// </para>
        /// <para>
        /// Rather than specify the encoding in a binary fashion using this flag, an application
        /// can specify an arbitrary encoding via the <see
        /// cref="ProvisionalAlternateEncoding"/> property.  Setting 
        /// the encoding explicitly when creating zip archives will result in non-compliant 
        /// zip files that, curiously, are fairly interoperable.  The challenge is, the PKWare specification
        /// does not provide for a way to specify that an entry in a zip archive uses a code page that is
        /// neither IBM437 nor UTF-8.   Therefore 
        /// if you set the encoding explicitly when creating a zip archive, you must take care upon 
        /// reading the zip archive to use the same code page.  If you get it wrong, the behavior is 
        /// undefined and may result in incorrect filenames, exceptions, stomach upset, hair loss, and acne.  
        /// </para>
        /// </remarks>
        /// <seealso cref="ProvisionalAlternateEncoding">ProvisionalAlternateEncoding</seealso>
        public bool UseUnicodeAsNecessary
        {
            get
            {
                return _provisionalAlternateEncoding == System.Text.Encoding.UTF8;
            }
            set
            {
                _provisionalAlternateEncoding = (value) ? System.Text.Encoding.UTF8 : DefaultEncoding;
            }
        }

        /// <summary>
        /// The text encoding to use when writing new entries to the ZipFile, for those
        /// entries that cannot be encoded with the default (IBM437) encoding; or, the
        /// text encoding that was used when reading the entries from the ZipFile.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// In its AppNote.txt document, PKWare describes how to specify in the zip entry
        /// header that a filename or comment containing non-ANSI characters is encoded with
        /// UTF-8.  But, some archivers do not follow the specification, and instead encode
        /// super-ANSI characters using the system default code page.  For example, WinRAR
        /// when run on a machine in Shanghai may encode filenames with the Big-5 Chinese
        /// (950) code page.  This behavior is contrary to the Zip specification, but it
        /// occurs anyway.
        /// </para>
        ///
        /// <para>
        /// When using DotNetZip to write zip archives that will be read by one of these other
        /// archivers, set this property to specify the code page to use when encoding the <see
        /// cref="ZipEntry.FileName"/> and <see cref="ZipEntry.Comment"/> for each ZipEntry in the zip file,
        /// for values that cannot be encoded with the default codepage for zip files, IBM437.
        /// This is why this property is "provisional".  In all cases, IBM437 is used where
        /// possible, in other words, where no loss of data would result. It is possible, therefore, to have a given 
        /// entry with a Comment encoded in IBM437 and a FileName encoded with the specified "provisional" codepage. 
        /// </para>
        ///
        /// <para>
        /// Be aware that a zip file created after you've explicitly set the <see
        /// cref="ProvisionalAlternateEncoding" /> property to a value other than IBM437 may not be
        /// compliant to the PKWare specification, and may not be readable by compliant archivers.
        /// On the other hand, many (most?) archivers are non-compliant and can read zip files
        /// created in arbitrary code pages.  The trick is to use or specify the proper codepage
        /// when reading the zip.
        /// </para>
        ///
        /// <para>
        /// When creating a zip archive using this library, it is possible to change the value of
        /// <see cref="ProvisionalAlternateEncoding" /> between each entry you add, and between
        /// adding entries and the call to Save(). Don't do this. It will likely result in a
        /// zipfile that is not readable.  For best interoperability, either leave <see
        /// cref="ProvisionalAlternateEncoding" /> alone, or specify it only once, before adding
        /// any entries to the ZipFile instance.  There is one exception to this recommendation,
        /// described later.
        /// </para>
        ///
        /// <para>
        /// When using an arbitrary, non-UTF8 code page for encoding, there is no standard way for
        /// the creator application - whether DotNetZip, WinZip, WinRar, or something else - to
        /// formally specify in the zip file which codepage has been used for the entries. As a
        /// result, readers of zip files are not able to inspect the zip file and determine the
        /// codepage that was used for the entries contained within it.  It is left to the
        /// application or user to determine the necessary codepage when reading zipfiles encoded
        /// this way.  If you use an incorrect codepage when reading a zipfile, you will get
        /// entries with filenames that are incorrect, and the incorrect filenames may even contain
        /// characters that are not legal for use within filenames in Windows. Extracting entries
        /// with illegal characters in the filenames will lead to exceptions. It's too bad, but
        /// this is just the way things are with code pages in zip files. Caveat Emptor.
        /// </para>
	///
        /// <para>
        /// When using DotNetZip to read a zip archive, and the zip archive uses an arbitrary code
        /// page, you must specify the encoding to use before or when the zipfile is READ.  This
        /// means you must use a ZipFile.Read() method that allows you to specify a
        /// System.Text.Encoding parameter.  Setting the ProvisionalAlternateEncoding property
        /// after your application has read in the zip archive will not affect the entry names of
        /// entries that have already been read in, and is probably not what you want.
        /// </para>
        ///	
	/// <para>
        /// And now, the exception to the rule described above.  One strategy for specifying the
        /// code page for a given zip file is to describe the code page in a human-readable form in
        /// the Zip comment. For example, the comment may read "Entries in this archive are encoded
        /// in the Big5 code page".  For maximum interoperability, the Zip comment in this case
        /// should be encoded in the default, IBM437 code page.  In this case, the zip comment is
        /// encoded using a different page than the filenames.  To do this, specify
        /// ProvisionalAlternateEncoding to your desired region-specific code page, once before
        /// adding any entries, and then reset ProvisionalAlternateEncoding to IBM437 before
        /// setting the <see cref="Comment"/> property and calling Save().
	/// </para>
        /// </remarks>
        /// 
		/// <seealso cref="ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.ZipFile.DefaultEncoding">DefaultEncoding</seealso>
        public System.Text.Encoding ProvisionalAlternateEncoding
        {
            get
            {
                return _provisionalAlternateEncoding;
            }
            set
            {
                _provisionalAlternateEncoding = value;
            }
        }

        /// <summary>
        /// The default text encoding used in zip archives.  It is numeric 437, also known as IBM437. 
        /// </summary>
		/// <seealso cref="ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.ZipFile.ProvisionalAlternateEncoding">ProvisionalAlternateEncoding</seealso>

		public readonly static System.Text.Encoding DefaultEncoding = System.Text.Encoding.GetEncoding(437);


        internal System.IO.Stream ReadStream
        {
            get
            {
                return _readstream;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a zip file, without specifying a target filename or stream to save to. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// See the documentation on the ZipFile(String) ZipFile constructor
        /// that accepts a single string argument for basic information on all the ZipFile constructors.
        /// </para>
        ///
        /// <para>
        /// After instantiating with this constructor and adding entries to the archive,
        /// your application should call ZipFile.Save(String) or ZipFile.Save(System.IO.Stream) to save to a file or a stream, respectively. 
        /// If you call the no-argument Save() method, the Save() will throw, as there is no 
        /// known place to save the file. 
        /// </para>
        ///
        /// <para>
        /// Instances of the ZipFile class are not multi-thread safe.  You may not party on a single
        /// instance with multiple threads.  You may have multiple threads that each use a distinct ZipFile 
        /// instance, or you can synchronize multi-thread access to a single instance.
        /// </para>
        /// 
        /// </remarks>
        /// 
        /// <example>
        /// This example creates a Zip archive called Backup.zip, containing all the files
        /// in the directory DirectoryToZip. Files within subdirectories are not zipped up.
        /// <code>
        /// using (ZipFile zip = new ZipFile())
        /// { 
        ///   // Store all files found in the top level directory, into the zip archive.
        ///   // note: this code does not recurse subdirectories!
        ///   String[] filenames = System.IO.Directory.GetFiles(DirectoryToZip);
        ///   foreach (String filename in filenames)
        ///   {
        ///     Console.WriteLine("Adding {0}...", filename);
        ///     zip.AddFile(filename);
        ///   }  
        ///   zip.Save("Backup.zip");
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        /// Using zip As New ZipFile
        ///     ' Store all files found in the top level directory, into the zip archive.
        ///     ' note: this code does not recurse subdirectories!
        ///     Dim filenames As String() = System.IO.Directory.GetFiles(DirectoryToZip)
        ///     Dim filename As String
        ///     For Each filename In filenames
        ///         Console.WriteLine("Adding {0}...", filename)
        ///         zip.AddFile(filename)
        ///     Next
        ///     zip.Save("Backup.zip")
        /// End Using
        /// </code>
        /// </example>
        private ZipFile()
        {
            InitFile();
        }

        private void InitFile()
        {
			_entries = new System.Collections.Generic.List<ZipEntry>();
            return;
        }
        #endregion



        #region Events

        private string ArchiveNameForEvent
        {
            get
            {
                return "(stream)";
            }
        }

        private Int64 _lengthOfReadStream = -99;
        private Int64 LengthOfReadStream
        {
            get
            {
                if (_lengthOfReadStream == -99)
                {
					_lengthOfReadStream = -1;
                }
                return _lengthOfReadStream;
            }
        }

        #endregion

        #region Reading Zip Files

        /// <summary>
        /// Reads a zip archive from a stream.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// This is useful when when the zip archive content is available from 
        /// an already-open stream. The stream must be open and readable when calling this
        /// method.  The stream is left open when the reading is completed. 
        /// </para>
        /// <para>
        /// The stream is read using the default <c>System.Text.Encoding</c>, which is the <c>IBM437</c> codepage.  
        /// </para>
        /// </remarks>
        ///
        /// <example>
        /// This example shows how to Read zip content from a stream, and extract
        /// one entry into a different stream. In this example, the filename
        /// "NameOfEntryInArchive.doc", refers only to the name of the entry
        /// within the zip archive.  A file by that name is not created in the
        /// filesystem.  The I/O is done strictly with the given streams.
        /// <code>
        /// using (ZipFile zip = ZipFile.Read(InputStream))
        /// {
        ///   zip.Extract("NameOfEntryInArchive.doc", OutputStream);
        /// }
        /// </code>
        /// <code lang="VB">
        /// Using zip as ZipFile = ZipFile.Read(InputStream)
        ///   zip.Extract("NameOfEntryInArchive.doc", OutputStream)
        /// End Using
        /// </code>
        /// </example>
        ///
        /// <param name="zipStream">the stream containing the zip data.</param>
        ///
        /// <returns>an instance of ZipFile</returns>
        public static ZipFile Read(System.IO.Stream zipStream)
        {
            return Read(zipStream, DefaultEncoding);
        }


        /// <summary>
        /// Reads a zip archive from a stream, using the specified text Encoding, the 
        /// specified TextWriter for status messages, 
        /// and the specified ReadProgress event handler.
        /// </summary>
        ///
        /// <param name="zipStream">the stream containing the zip data.</param>
        ///
        /// <param name="encoding">
        /// The text encoding to use when reading entries that do not have the UTF-8 encoding
        /// bit set.  Be careful specifying the encoding.  If the value you use here is not the
        /// same as the Encoding used when the zip archive was created (possibly by a different
        /// archiver) you will get unexpected results and possibly exceptions.  See the
        /// ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.ZipFile.ProvisionalAlternateEncoding ProvisionalAlternateEncoding
        /// property for more information.
        /// </param>
        /// 
        /// <returns>an instance of ZipFile</returns>
        public static ZipFile Read(System.IO.Stream zipStream,
                  System.Text.Encoding encoding)
        {
            if (zipStream == null)
                throw new ZipException("Cannot read.", new ArgumentException("The stream must be non-null", "zipStream"));

            ZipFile zf = new ZipFile();
            zf._provisionalAlternateEncoding = encoding;
            zf._readstream = zipStream;
            zf._ReadStreamIsOurs = false;
            ReadIntoInstance(zf);
            return zf;
        }


        /// <summary>
        /// Reads a zip archive from a byte array.
        /// </summary>
        /// 
        /// <remarks>
        /// This is useful when the data for the zipfile is contained in a byte array, 
        /// for example, downloaded from an FTP server without being saved to a
        /// filesystem. 
        /// </remarks>
        /// 
        /// <param name="buffer">
        /// The byte array containing the zip data.  
        /// (I don't know why, but sometimes the compiled helpfuile (.chm) indicates a 2d 
        /// array when it is just one-dimensional.  This is a one-dimensional array.)
        /// </param>
        /// 
        /// <returns>an instance of ZipFile. The name on the ZipFile will be null (nothing in VB)). </returns>
        public static ZipFile Read(byte[] buffer)
        {
            return Read(buffer, DefaultEncoding);
        }

        /// <summary>
        /// Reads a zip archive from a byte array, using the given StatusMessageWriter and text Encoding.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This method is useful when the data for the zipfile is contained in a byte array, for
        /// example when retrieving the data from a database or other non-filesystem store.  
        /// </para>
        /// 
        /// </remarks>
        /// 
        /// <param name="buffer">the byte array containing the zip data.</param>
        ///
        /// <param name="encoding">
        /// The text encoding to use when reading entries that do not have the UTF-8 encoding
        /// bit set.  Be careful specifying the encoding.  If the value you use here is not the
        /// same as the Encoding used when the zip archive was created (possibly by a different
        /// archiver) you will get unexpected results and possibly exceptions.  See the <see
        /// cref="ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.ZipFile.ProvisionalAlternateEncoding"/>
        /// property for more information.
        /// </param>
        /// 
        /// <returns>an instance of ZipFile. The name is set to null.</returns>
        /// 
        public static ZipFile Read(byte[] buffer, System.Text.Encoding encoding)
        {
            ZipFile zf = new ZipFile();
            zf._provisionalAlternateEncoding = encoding;
            zf._readstream = new System.IO.MemoryStream(buffer);
            zf._ReadStreamIsOurs = true;
            ReadIntoInstance(zf);
            return zf;
        }


        private static void ReadIntoInstance(ZipFile zf)
        {
            System.IO.Stream s = zf.ReadStream;
            try
            {
#if OPTIMIZE_WI6612
                if (!s.CanSeek)
                {
                    ReadIntoInstance_Orig(zf);
                    return;
                }

                long origPosn = s.Position;

                // Try reading the central directory, rather than scanning the file. 


                uint datum = VerifyBeginningOfZipFile(s);

                if (datum == ZipConstants.EndOfCentralDirectorySignature)
                    return;


                // start at the end of the file...
                // seek backwards a bit, then look for the EoCD signature. 
                int nTries = 0;
                bool success = false;

                // The size of the end-of-central-directory-footer plus 2 bytes is 18.
                // This implies an archive comment length of 0.
                // We'll add a margin of safety and start "in front" of that, when 
                // looking for the EndOfCentralDirectorySignature
                long posn = s.Length - 64;
                long maxSeekback = Math.Max(s.Length - 0x4000, 10);
                do
                {
                    s.Seek(posn, System.IO.SeekOrigin.Begin);
                    long bytesRead = SharedUtilities.FindSignature(s, (int)ZipConstants.EndOfCentralDirectorySignature);
                    if (bytesRead != -1)
                        success = true;
                    else
                    {
                        nTries++;
                        //weird - with NETCF, negative offsets from SeekOrigin.End DO NOT WORK
                        posn -= (32 * (nTries + 1) * nTries); // increasingly larger
                        if (posn < 0) posn = 0;
                    }
                }
                //while (!success && nTries < 3);
                while (!success && posn > maxSeekback);

                if (success)
                {
                    byte[] block = new byte[16];
                    zf.ReadStream.Read(block, 0, block.Length);
                    int i = 12;

                    uint Offset32 = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
                    if (Offset32 == 0xFFFFFFFF)
                    {
                        Zip64SeekToCentralDirectory(s);
                    }
                    else
                    {
                        s.Seek(Offset32, System.IO.SeekOrigin.Begin);
                    }

                    ReadCentralDirectory(zf);
                }
                else
                {
                    // Could not find the central directory.
                    // Fallback to the old method.
                    s.Seek(origPosn, System.IO.SeekOrigin.Begin);
                    ReadIntoInstance_Orig(zf);
                }

#else
                ReadIntoInstance_Orig(zf);
#endif

            }
            catch //(Exception e1)
            {
                if (zf._ReadStreamIsOurs && zf._readstream != null)
                {
                    try
                    {
                        zf._readstream.Close();
                        zf._readstream.Dispose();
                        zf._readstream = null;
                    }
                    finally { }
                }

                throw; // new Ionic.Utils.Zip.ZipException("Exception while reading", e1);
            }
        }



#if OPTIMIZE_WI6612

        private static void Zip64SeekToCentralDirectory(System.IO.Stream s)
        {
            byte[] block = new byte[16];

            // seek back to find the ZIP64 EoCD
            s.Seek(-40, System.IO.SeekOrigin.Current);
            s.Read(block, 0, 16);

            Int64 Offset64 = BitConverter.ToInt64(block, 8);
            s.Seek(Offset64, System.IO.SeekOrigin.Begin);

            uint datum = (uint)ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.SharedUtilities.ReadInt(s);
            if (datum != ZipConstants.Zip64EndOfCentralDirectoryRecordSignature)
                throw new BadReadException(String.Format("  ZipFile::Read(): Bad signature (0x{0:X8}) looking for ZIP64 EoCD Record at position 0x{1:X8}", datum, s.Position));

            s.Read(block, 0, 8);
            Int64 Size = BitConverter.ToInt64(block, 0);

            block = new byte[Size];
            s.Read(block, 0, block.Length);

            Offset64 = BitConverter.ToInt64(block, 36);
            s.Seek(Offset64, System.IO.SeekOrigin.Begin);
        }


        private static uint VerifyBeginningOfZipFile(System.IO.Stream s)
        {
            uint datum = (uint)ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.SharedUtilities.ReadInt(s);
            if (datum != ZipConstants.PackedToRemovableMedia  // weird edge case
                && datum != ZipConstants.ZipEntrySignature   // normal BOF marker
                && datum != ZipConstants.EndOfCentralDirectorySignature  // for zip file with no entries
                && (datum & 0x0000FFFF) != 0x00005A4D // PE/COFF BOF marker (for SFX)
                )
            {
                //Console.WriteLine("WTF, datum = 0x{0:X8}", datum);
                throw new BadReadException(String.Format("  ZipFile::Read(): Bad signature (0x{0:X8}) at start of file at position 0x{1:X8}", datum, s.Position));
            }
            return datum;
        }



        private static void ReadCentralDirectory(ZipFile zf)
        {
            //zf._direntries = new System.Collections.Generic.List<ZipDirEntry>();
            zf._entries = new System.Collections.Generic.List<ZipEntry>();

            ZipEntry de;
            while ((de = ZipEntry.ReadDirEntry(zf.ReadStream, zf.ProvisionalAlternateEncoding)) != null)
            {
                //zf._direntries.Add(de);

                de.ResetDirEntry();
                de._zipfile = zf;
                de._Source = EntrySource.Zipfile;
                de._archiveStream = zf.ReadStream;
                zf._entries.Add(de);
            }

            ReadCentralDirectoryFooter(zf);

        }
#endif


        // build the TOC by reading each entry in the file.
        private static void ReadIntoInstance_Orig(ZipFile zf)
        {
            zf._entries = new System.Collections.Generic.List<ZipEntry>();
            ZipEntry e;

            // work item 6647:  PK00 (packed to removable disk)
            bool firstEntry = true;
            while ((e = ZipEntry.Read(zf, firstEntry)) != null)
            {
                zf._entries.Add(e);
                firstEntry = false;
            }

            // read the zipfile's central directory structure here.
            //zf._direntries = new System.Collections.Generic.List<ZipDirEntry>();

            ZipEntry de;
            while ((de = ZipEntry.ReadDirEntry(zf.ReadStream, zf.ProvisionalAlternateEncoding)) != null)
            {
                //zf._direntries.Add(de);
                // Housekeeping: Since ZipFile exposes ZipEntry elements in the enumerator, 
                // we need to copy the comment that we grab from the ZipDirEntry
                // into the ZipEntry, so the application can access the comment. 
                // Also since ZipEntry is used to Write zip files, we need to copy the 
                // file attributes to the ZipEntry as appropriate. 
                foreach (ZipEntry e1 in zf._entries)
                {
                    if (e1.FileName == de.FileName)
                    {
                        e1._Comment = de.Comment;
                        if (de.AttributesIndicateDirectory) e1.MarkAsDirectory();
                        break;
                    }
                }
            }

            ReadCentralDirectoryFooter(zf);
        }




        private static void ReadCentralDirectoryFooter(ZipFile zf)
        {
            System.IO.Stream s = zf.ReadStream;
            int signature = ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.SharedUtilities.ReadSignature(s);

            byte[] block = null;
            int i = 0;
            if (signature == ZipConstants.Zip64EndOfCentralDirectoryRecordSignature)
            {
                // We have a ZIP64 EOCD
                // This data block is 4 bytes sig, 8 bytes size, 44 bytes fixed data, 
                // followed by a variable-sized extension block.  We have read the sig already. 
                block = new byte[8 + 44];
                s.Read(block, 0, block.Length);

                Int64 DataSize = BitConverter.ToInt64(block, 0);  // == 44 + the variable length

                if (DataSize < 44)
                    throw new ZipException("Bad DataSize in the ZIP64 Central Directory.");

                i = 8;
                i += 2; // version made by
                i += 2; // version needed to extract

                i += 4; // number of this disk
                i += 4; // number of the disk with the start of the CD

                i += 8; // total number of entries in the CD on this disk
                i += 8; // total number of entries in the CD 

                i += 8; // size of the CD

                i += 8; // offset of the CD

                block = new byte[DataSize - 44];
                s.Read(block, 0, block.Length);
                // discard the result

                signature = ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.SharedUtilities.ReadSignature(s);
                if (signature != ZipConstants.Zip64EndOfCentralDirectoryLocatorSignature)
                    throw new ZipException("Inconsistent metadata in the ZIP64 Central Directory.");

                block = new byte[16];
                s.Read(block, 0, block.Length);
                // discard the result

                signature = ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.SharedUtilities.ReadSignature(s);
            }

            // Throw if this is not a signature for "end of central directory record"
            // This is a sanity check.
            if (signature != ZipConstants.EndOfCentralDirectorySignature)
            {
                s.Seek(-4, System.IO.SeekOrigin.Current);
                throw new BadReadException(String.Format("  ZipFile::Read(): Bad signature ({0:X8}) at position 0x{1:X8}", signature, s.Position));
            }

            // read a bunch of metadata for supporting multi-disk archives, which this library does not do.
            block = new byte[16];
            zf.ReadStream.Read(block, 0, block.Length); // discard result

            // read the comment here
            ReadZipFileComment(zf);
        }



        private static void ReadZipFileComment(ZipFile zf)
        {
            // read the comment here
            byte[] block = new byte[2];
            zf.ReadStream.Read(block, 0, block.Length);

            Int16 commentLength = (short)(block[0] + block[1] * 256);
            if (commentLength > 0)
            {
                block = new byte[commentLength];
                zf.ReadStream.Read(block, 0, block.Length);

                // workitem 6513 - only use UTF8 as necessary
                // test reflexivity
                string s1 = DefaultEncoding.GetString(block, 0, block.Length);
                byte[] b2 = DefaultEncoding.GetBytes(s1);
                if (BlocksAreEqual(block, b2))
                {
                    zf.Comment = s1;
                }
                else
                {
                    // need alternate (non IBM437) encoding
                    // workitem 6415
                    // use UTF8 if the caller hasn't already set a non-default encoding
#if !SILVERLIGHT
                    System.Text.Encoding e = (zf._provisionalAlternateEncoding.CodePage == 437)
                        ? System.Text.Encoding.UTF8
                        : zf._provisionalAlternateEncoding;
#else
					System.Text.Encoding e = System.Text.Encoding.UTF8;
#endif
					zf.Comment = e.GetString(block, 0, block.Length);
                }
            }
        }


        private static bool BlocksAreEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }


        /// <summary>
        /// Generic IEnumerator support, for use of a ZipFile in a foreach construct.  
        /// </summary>
        ///
        /// <remarks>
        /// You probably do not want to call <c>GetEnumerator</c> explicitly. Instead 
        /// it is implicitly called when you use a <c>foreach</c> loop in C#, or a 
        /// <c>For Each</c> loop in VB.
        /// </remarks>
        ///
        /// <example>
        /// This example reads a zipfile of a given name, then enumerates the 
        /// entries in that zip file, and displays the information about each 
        /// entry on the Console.
        /// <code>
        /// using (ZipFile zip = ZipFile.Read(zipfile))
        /// {
        ///   bool header = true;
        ///   foreach (ZipEntry e in zip)
        ///   {
        ///     if (header)
        ///     {
        ///        System.Console.WriteLine("Zipfile: {0}", zip.Name);
        ///        System.Console.WriteLine("Version Needed: 0x{0:X2}", e.VersionNeeded);
        ///        System.Console.WriteLine("BitField: 0x{0:X2}", e.BitField);
        ///        System.Console.WriteLine("Compression Method: 0x{0:X2}", e.CompressionMethod);
        ///        System.Console.WriteLine("\n{1,-22} {2,-6} {3,4}   {4,-8}  {0}",
        ///                     "Filename", "Modified", "Size", "Ratio", "Packed");
        ///        System.Console.WriteLine(new System.String('-', 72));
        ///        header = false;
        ///     }
        ///
        ///     System.Console.WriteLine("{1,-22} {2,-6} {3,4:F0}%   {4,-8}  {0}",
        ///                 e.FileName,
        ///                 e.LastModified.ToString("yyyy-MM-dd HH:mm:ss"),
        ///                 e.UncompressedSize,
        ///                 e.CompressionRatio,
        ///                 e.CompressedSize);
        ///
        ///     e.Extract();
        ///   }
        /// }
        /// </code>
        ///
        /// <code lang="VB">
        ///   Dim ZipFileToExtract As String = "c:\foo.zip"
        ///   Using zip As ZipFile = ZipFile.Read(ZipFileToExtract)
        ///       Dim header As Boolean = True
        ///       Dim e As ZipEntry
        ///       For Each e In zip
        ///           If header Then
        ///               Console.WriteLine("Zipfile: {0}", zip.Name)
        ///               Console.WriteLine("Version Needed: 0x{0:X2}", e.VersionNeeded)
        ///               Console.WriteLine("BitField: 0x{0:X2}", e.BitField)
        ///               Console.WriteLine("Compression Method: 0x{0:X2}", e.CompressionMethod)
        ///               Console.WriteLine(ChrW(10) &amp; "{1,-22} {2,-6} {3,4}   {4,-8}  {0}", _
        ///                 "Filename", "Modified", "Size", "Ratio", "Packed" )
        ///               Console.WriteLine(New String("-"c, 72))
        ///               header = False
        ///           End If
        ///           Console.WriteLine("{1,-22} {2,-6} {3,4:F0}%   {4,-8}  {0}", _
        ///             e.FileName, _
        ///             e.LastModified.ToString("yyyy-MM-dd HH:mm:ss"), _
        ///             e.UncompressedSize, _
        ///             e.CompressionRatio, _
        ///             e.CompressedSize )
        ///           e.Extract
        ///       Next
        ///   End Using
        /// </code>
        /// </example>
        /// 
        /// <returns>A generic enumerator suitable for use  within a foreach loop.</returns>
        public System.Collections.Generic.IEnumerator<ZipEntry> GetEnumerator()
        {
            foreach (ZipEntry e in _entries)
                yield return e;
        }

        /// <summary>
        /// IEnumerator support, for use of a ZipFile in a foreach construct.  
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Extract a single specified file from the archive, to the given stream.   
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// Calling this method, the entry is extracted using the Password that is 
        /// specified on the ZipFile instance. If you have not set the Password property, then
        /// the password is null, and the entry is extracted with no password.
        /// </para>
        ///
        /// <para>
        /// The ExtractProgress event is invoked before and after extraction, if it has been set. 
        /// </para>
        /// </remarks>
        /// 
		/// <exception cref="ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.ZipException">
        /// Thrown if the outputStream is not writable, or if the filename is 
        /// null or empty. The inner exception is an ArgumentException in each case.
        /// </exception>
        ///
        /// <param name="fileName">
        /// the file to extract. It should include pathnames used in the archive, if any.
        /// The filename match is not case-sensitive by default; you can use the
        /// <c>CaseSensitiveRetrieval</c> property to change this behavior.The
        /// application can specify pathnames using forward-slashes or backward slashes.
        /// </param>
        ///
        /// <param name="outputStream">
        /// the stream to which the extacted, decompressed file data is written. 
        /// The stream must be writable.
        /// </param>
        public void Extract(string fileName, System.IO.Stream outputStream)
        {
            if (outputStream == null || !outputStream.CanWrite)
                throw new ZipException("Cannot extract.", new ArgumentException("The OutputStream must be a writable stream.", "outputStream"));

            if (String.IsNullOrEmpty(fileName))
                throw new ZipException("Cannot extract.", new ArgumentException("The file name must be neither null nor empty.", "fileName"));

            ZipEntry e = this[fileName];
			if (e != null)
				e.Extract(outputStream);
			else
				throw new FileNotFoundException();
        }


        /// <summary>
        /// This is an integer indexer into the Zip archive.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This property is read-write. But don't get too excited: When setting the value, the
        /// only legal value is null. If you assign a non-null value
        /// (non Nothing in VB), the setter will throw an exception.
        /// </para>
        ///
        /// <para>
        /// Setting the value to null is equivalent to calling ZipFile.RemoveEntry(String)
        /// with the filename for the given entry.
        /// </para>
        /// </remarks>
        /// 
        /// <exception cref="System.ArgumentException">
        /// Thrown if the caller attempts to assign a non-null value to the indexer, 
        /// or if the caller uses an out-of-range index value.
        /// </exception>
        ///
        /// <param name="ix">
        /// The index value.
        /// </param>
        /// 
        /// <returns>
        /// The ZipEntry within the Zip archive at the specified index. If the 
        /// entry does not exist in the archive, this indexer throws.
        /// </returns>
        /// 
        public ZipEntry this[int ix]
        {
            // workitem 6402
            get
            {
                return _entries[ix];
            }
        }


        /// <summary>
        /// This is a name-based indexer into the Zip archive.  
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// Retrieval by the string-based indexer is done on a case-insensitive basis, 
        /// by default.  Set the <see cref="CaseSensitiveRetrieval"/> property to use case-sensitive 
        /// comparisons. 
        /// </para>
        /// <para>
        /// This property is read-write. When setting the value, the
        /// only legal value is null. Setting the value to null is
        /// equivalent to calling ZipFile.RemoveEntry(String) with the filename.
        /// </para>
        /// <para>
        /// If you assign a non-null value
        /// (non Nothing in VB), the setter will throw an exception.
        /// </para>
        /// <para>
        /// It is not always the case that <c>this[value].FileName == value</c>.  In
        /// the case of directory entries in the archive, you may retrieve them with
        /// the name of the directory with no trailing slash, even though in the
        /// entry itself, the actual <see cref="ZipEntry.FileName"/> property may
        /// include a trailing slash.  In other words, for a directory entry named
        /// "dir1", you may find <c>this["dir1"].FileName == "dir1/"</c>.
        /// </para>
        /// </remarks>
        /// 
        /// <example>
        /// This example extracts only the entries in a zip file that are .txt files.
        /// <code>
        /// using (ZipFile zip = ZipFile.Read("PackedDocuments.zip"))
        /// {
        ///   foreach (string s1 in zip.EntryFilenames)
        ///   {
        ///     if (s1.EndsWith(".txt"))
        ///       zip[s1].Extract("textfiles");
        ///   }
        /// }
        /// </code>
        /// <code lang="VB">
        ///   Using zip As ZipFile = ZipFile.Read("PackedDocuments.zip")
        ///       Dim s1 As String
        ///       For Each s1 In zip.EntryFilenames
        ///           If s1.EndsWith(".txt") Then
        ///               zip(s1).Extract("textfiles")
        ///           End If
        ///       Next
        ///   End Using
        /// </code>
        /// </example>
        ///
        /// <exception cref="System.ArgumentException">
        /// Thrown if the caller attempts to assign a non-null value to the indexer.
        /// </exception>
        ///
        /// <param name="fileName">
        /// The name of the file, including any directory path, to retrieve from the zip. 
        /// The filename match is not case-sensitive by default; you can use the
        /// <see cref="CaseSensitiveRetrieval"/> property to change this behavior. The
        /// pathname can use forward-slashes or backward slashes.
        /// </param>
        /// 
        /// <returns>
        /// The ZipEntry within the Zip archive, given by the specified filename. If the named
        /// entry does not exist in the archive, this indexer returns null.
        /// </returns>
        /// 
        public ZipEntry this[String fileName]
        {
            get
            {
                foreach (ZipEntry e in _entries)
                {
                    if (this.CaseSensitiveRetrieval)
                    {
                        // check for the file match with a case-sensitive comparison.
                        if (e.FileName == fileName) return e;
                        // also check for equivalence
                        if (fileName.Replace("\\", "/") == e.FileName) return e;
                        if (e.FileName.Replace("\\", "/") == fileName) return e;

                        // check for a difference only in trailing slash
                        if (e.FileName.EndsWith("/"))
                        {
                            var FileNameNoSlash = e.FileName.Trim("/".ToCharArray());
                            if (FileNameNoSlash == fileName) return e;
                            // also check for equivalence
                            if (fileName.Replace("\\", "/") == FileNameNoSlash) return e;
                            if (FileNameNoSlash.Replace("\\", "/") == fileName) return e;
                        }

                    }
                    else
                    {
                        // check for the file match in a case-insensitive manner.
                        if (String.Compare(e.FileName, fileName, StringComparison.CurrentCultureIgnoreCase) == 0) return e;
                        // also check for equivalence
                        if (String.Compare(fileName.Replace("\\", "/"), e.FileName, StringComparison.CurrentCultureIgnoreCase) == 0) return e;
                        if (String.Compare(e.FileName.Replace("\\", "/"), fileName, StringComparison.CurrentCultureIgnoreCase) == 0) return e;

                        // check for a difference only in trailing slash
                        if (e.FileName.EndsWith("/"))
                        {
                            var FileNameNoSlash = e.FileName.Trim("/".ToCharArray());

                            if (String.Compare(FileNameNoSlash, fileName, StringComparison.CurrentCultureIgnoreCase) == 0) return e;
                            // also check for equivalence
                            if (String.Compare(fileName.Replace("\\", "/"), FileNameNoSlash, StringComparison.CurrentCultureIgnoreCase) == 0) return e;
                            if (String.Compare(FileNameNoSlash.Replace("\\", "/"), fileName, StringComparison.CurrentCultureIgnoreCase) == 0) return e;

                        }

                    }

                }
                return null;
            }
        }

        /// <summary>
        /// The list of filenames for the entries contained within the zip archive.  The 
        /// filenames use forward slashes in pathnames. 
        /// </summary>
        ///
		/// <seealso cref="ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.ZipFile.this[string]"/>
        ///
        /// <example>
        /// This example shows one way to test if a filename is already contained within 
        /// a zip archive.
        /// <code>
        /// String ZipFileToRead= "PackedDocuments.zip";
        /// string Candidate = "DatedMaterial.xps";
        /// using (ZipFile zip = new ZipFile(ZipFileToRead))
        /// {
        ///   if (zip.EntryFilenames.Contains(Candidate))
        ///     Console.WriteLine("The file '{0}' exists in the zip archive '{1}'",
        ///                       Candidate,
        ///                       ZipFileName);
        ///   else
        ///     Console.WriteLine("The file, '{0}', does not exist in the zip archive '{1}'",
        ///                       Candidate,
        ///                       ZipFileName);
        ///   Console.WriteLine();
        /// }
        /// </code>
        /// <code lang="VB">
        ///   Dim ZipFileToRead As String = "PackedDocuments.zip"
        ///   Dim Candidate As String = "DatedMaterial.xps"
        ///   Using zip As New ZipFile(ZipFileToRead)
        ///       If zip.EntryFilenames.Contains(Candidate) Then
        ///           Console.WriteLine("The file '{0}' exists in the zip archive '{1}'", _
        ///                       Candidate, _
        ///                       ZipFileName)
        ///       Else
        ///         Console.WriteLine("The file, '{0}', does not exist in the zip archive '{1}'", _
        ///                       Candidate, _
        ///                       ZipFileName)
        ///       End If
        ///       Console.WriteLine
        ///   End Using
        /// </code>
        /// </example>
        ///
        /// <returns>
        /// The list of strings for the filenames contained within the Zip archive.
        /// </returns>
        /// 
        public System.Collections.ObjectModel.ReadOnlyCollection<string> EntryFileNames
        {
            get
            {
				System.Collections.Generic.List<string> filenames = new System.Collections.Generic.List<string>(_entries.Count);
				foreach(ZipEntry entry in _entries)
					filenames.Add(entry.FileName);
				System.Collections.ObjectModel.ReadOnlyCollection<string> coll = new System.Collections.ObjectModel.ReadOnlyCollection<string>(filenames);
				return coll;
			}
        }


        /// <summary>
        /// Returns the readonly collection of entries in the Zip archive.
        /// </summary>
        /// <remarks>
        /// If there are no entries in the current ZipFile, the value returned is a non-null zero-element collection.
        /// </remarks>
        public System.Collections.ObjectModel.ReadOnlyCollection<ZipEntry> Entries
        {
            get
            {
                return _entries.AsReadOnly();
            }
        }


        /// <summary>
        /// Returns the number of entries in the Zip archive.
        /// </summary>
        public int Count
        {
            get
            {
                return _entries.Count;
            }
        }

        #endregion

        #region Destructors and Disposers

        /// <summary>
        /// This is the class Destructor, which gets called implicitly when the instance is destroyed.  
        /// Because the ZipFile type implements IDisposable, this method calls Dispose(false).  
        /// </summary>
        ~ZipFile()
        {
            // call Dispose with false.  Since we're in the
            // destructor call, the managed resources will be
            // disposed of anyways.
            Dispose(false);
        }

        /// <summary>
        /// Handles closing of the read and write streams associated
        /// to the ZipFile, if necessary.  The Dispose() method is generally 
        /// employed implicitly, via a using() {} statement. 
        /// </summary>
        /// <example>
        /// <code>
        /// using (ZipFile zip = ZipFile.Read(zipfile))
        /// {
        ///   foreach (ZipEntry e in zip)
        ///   {
        ///     if (WantThisEntry(e.FileName)) 
        ///       zip.Extract(e.FileName, Console.OpenStandardOutput());
        ///   }
        /// } // Dispose() is called implicitly here.
        /// </code>
        /// </example>
        public void Dispose()
        {
            // dispose of the managed and unmanaged resources
            Dispose(true);

            // tell the GC that the Finalize process no longer needs
            // to be run for this object.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The Dispose() method.  It disposes any managed resources, 
        /// if the flag is set, then marks the instance disposed.
        /// This method is typically not called from application code.
        /// </summary>
        /// <param name="disposeManagedResources">indicates whether the method should dispose streams or not.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (!this._disposed)
            {
                if (disposeManagedResources)
                {
                    // dispose managed resources
                    if (_ReadStreamIsOurs)
                    {
                        if (_readstream != null)
                        {
                            // workitem 7704
							_readstream.Close();
                            _readstream.Dispose();
                            _readstream = null;
                        }
                    }
                }
                this._disposed = true;
            }
        }
        #endregion

        #region private properties

        #endregion

        #region private fields
       // private System.IO.TextWriter _StatusMessageTextWriter;
        private bool _CaseSensitiveRetrieval;
        private System.IO.Stream _readstream;
        private bool _disposed;
        private System.Collections.Generic.List<ZipEntry> _entries;
       private string _Comment;
        private bool _ReadStreamIsOurs = true;
        private object LOCK = new object();
        internal bool _inExtractAll = false;
        //private System.Text.Encoding _encoding = System.Text.Encoding.GetEncoding("IBM437"); // default = IBM437
		private System.Text.Encoding _provisionalAlternateEncoding = System.Text.Encoding.Unicode; // default = IBM437

        internal Zip64Option _zip64 = Zip64Option.Default;
        #endregion
    }

    /// <summary>
    /// Options for using ZIP64 extensions when saving zip archives. 
    /// </summary>
    internal enum Zip64Option
    {
        /// <summary>
        /// The default behavior, which is "Never".
        /// </summary>
        Default = 0,
        /// <summary>
        /// Do not use ZIP64 extensions when writing zip archives.
        /// </summary>
        Never = 0,
        /// <summary>
        /// Use ZIP64 extensions when writing zip archives, as necessary. 
        /// For example, when a single entry exceeds 0xFFFFFFFF in size, or when the archive as a whole 
        /// exceeds 0xFFFFFFFF in size, or when there are more than 65535 entries in an archive.
        /// </summary>
        AsNecessary = 1,
        /// <summary>
        /// Always use ZIP64 extensions when writing zip archives, even when unnecessary.
        /// </summary>
        Always
    }


    enum AddOrUpdateAction
    {
        AddOnly = 0,
        AddOrUpdate
    }
}



// Example usage: 
// 1. Extracting all files from a Zip file: 
//
//     try 
//     {
//       using(ZipFile zip= ZipFile.Read(ZipFile))
//       {
//         zip.ExtractAll(TargetDirectory, true);
//       }
//     }
//     catch (System.Exception ex1)
//     {
//       System.Console.Error.WriteLine("exception: " + ex1);
//     }
//
// 2. Extracting files from a zip individually:
//
//     try 
//     {
//       using(ZipFile zip= ZipFile.Read(ZipFile)) 
//       {
//         foreach (ZipEntry e in zip) 
//         {
//           e.Extract(TargetDirectory);
//         }
//       }
//     }
//     catch (System.Exception ex1)
//     {
//       System.Console.Error.WriteLine("exception: " + ex1);
//     }
//
// 3. Creating a zip archive: 
//
//     try 
//     {
//       using(ZipFile zip= new ZipFile(NewZipFile)) 
//       {
//
//         String[] filenames= System.IO.Directory.GetFiles(Directory); 
//         foreach (String filename in filenames) 
//         {
//           zip.Add(filename);
//         }
//
//         zip.Save(); 
//       }
//
//     }
//     catch (System.Exception ex1)
//     {
//       System.Console.Error.WriteLine("exception: " + ex1);
//     }
//
//
// ==================================================================
//
//
//
// Information on the ZIP format:
//
// From
// http://www.pkware.com/documents/casestudies/APPNOTE.TXT
//
//  Overall .ZIP file format:
//
//     [local file header 1]
//     [file data 1]
//     [data descriptor 1]  ** sometimes
//     . 
//     .
//     .
//     [local file header n]
//     [file data n]
//     [data descriptor n]   ** sometimes
//     [archive decryption header] 
//     [archive extra data record] 
//     [central directory]
//     [zip64 end of central directory record]
//     [zip64 end of central directory locator] 
//     [end of central directory record]
//
// Local File Header format:
//         local file header signature ... 4 bytes  (0x04034b50)
//         version needed to extract ..... 2 bytes
//         general purpose bit field ..... 2 bytes
//         compression method ............ 2 bytes
//         last mod file time ............ 2 bytes
//         last mod file date............. 2 bytes
//         crc-32 ........................ 4 bytes
//         compressed size................ 4 bytes
//         uncompressed size.............. 4 bytes
//         file name length............... 2 bytes
//         extra field length ............ 2 bytes
//         file name                       varies
//         extra field                     varies
//
//
// Data descriptor:  (used only when bit 3 of the general purpose bitfield is set)
//         (although, I have found zip files where bit 3 is not set, yet this descriptor is present!)
//         local file header signature     4 bytes  (0x08074b50)  ** sometimes!!! Not always
//         crc-32                          4 bytes
//         compressed size                 4 bytes
//         uncompressed size               4 bytes
//
//
//   Central directory structure:
//
//       [file header 1]
//       .
//       .
//       . 
//       [file header n]
//       [digital signature] 
//
//
//       File header:  (This is a ZipDirEntry)
//         central file header signature   4 bytes  (0x02014b50)
//         version made by                 2 bytes
//         version needed to extract       2 bytes
//         general purpose bit flag        2 bytes
//         compression method              2 bytes
//         last mod file time              2 bytes
//         last mod file date              2 bytes
//         crc-32                          4 bytes
//         compressed size                 4 bytes
//         uncompressed size               4 bytes
//         file name length                2 bytes
//         extra field length              2 bytes
//         file comment length             2 bytes
//         disk number start               2 bytes
//         internal file attributes **     2 bytes
//         external file attributes ***    4 bytes
//         relative offset of local header 4 bytes
//         file name (variable size)
//         extra field (variable size)
//         file comment (variable size)
//
// ** The internal file attributes, near as I can tell, 
// uses 0x01 for a file and a 0x00 for a directory. 
//
// ***The external file attributes follows the MS-DOS file attribute byte, described here:
// at http://support.microsoft.com/kb/q125019/
// 0x0010 => directory
// 0x0020 => file 
//
//
// End of central directory record:
//
//         end of central dir signature    4 bytes  (0x06054b50)
//         number of this disk             2 bytes
//         number of the disk with the
//         start of the central directory  2 bytes
//         total number of entries in the
//         central directory on this disk  2 bytes
//         total number of entries in
//         the central directory           2 bytes
//         size of the central directory   4 bytes
//         offset of start of central
//         directory with respect to
//         the starting disk number        4 bytes
//         .ZIP file comment length        2 bytes
//         .ZIP file comment       (variable size)
//
// date and time are packed values, as MSDOS did them
// time: bits 0-4 : seconds (divided by 2)
//            5-10: minute
//            11-15: hour
// date  bits 0-4 : day
//            5-8: month
//            9-15 year (since 1980)
//
// see http://msdn.microsoft.com/en-us/library/ms724274(VS.85).aspx

