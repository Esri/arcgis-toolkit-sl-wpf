#define OPTIMIZE_WI6612

// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

// ZipEntry.cs
//
// Copyright (c) 2006, 2007, 2008, 2009 Microsoft Corporation.  All rights reserved.
//
// Part of an implementation of a zipfile class library. 
// See the file ZipFile.cs for the license and for further information.
//
// Created: Tue, 27 Mar 2007  15:30
// 

using System;
using System.IO;
using RE = System.Text.RegularExpressions;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip
{
    /// <summary>
    /// An enum that provides the various encryption algorithms supported by this library.
    /// </summary>
    /// <remarks>
    /// <para>
    /// PkzipWeak implies the use of Zip 2.0 encryption, which is known to be weak and subvertible. 
    /// </para>
    /// <para>
    /// A note on interoperability: Values of PkzipWeak and None are specified in the PKWare AppNote.txt document, are 
    /// considered to be "standard".  Zip archives produced using these options will be interoperable with many other
    /// zip tools and libraries, including Windows Explorer.
    /// </para>
    /// <para>
    /// Values of WinZipAes128 and WinZipAes256 are not part of the Zip specification, but rather imply the use of a 
    /// vendor-specific extension from WinZip. If you want to produce interoperable Zip archives, do not use these values. 
    /// For example, if you
    /// produce a zip archive using WinZipAes256, you will be able to open it in Windows Explorer on Windows XP and Vista, 
    /// but you will not be able to extract entries; trying this will lead to an "unspecified error". For this reason, 
    /// some people have said that a zip archive that uses WinZip's AES encryption is not actually a zip archive at all.
    /// A zip archive produced this way will be readable with the WinZip tool
    /// (Version 11 and beyond).
    /// </para>
    /// <para>
    /// There are other third-party tools and libraries, both commercial and otherwise, that support WinZip's 
    /// AES encryption. These will be able to read AES-encrypted zip archives produced by DotNetZip, and conversely applications 
    /// that use DotNetZip to read zip archives will be able to read AES-encrypted archives produced by those tools
    /// or libraries.  Consult the documentation for those other tools and libraries to find out if WinZip's AES 
    /// encryption is supported. 
    /// </para>
    /// <para>
    /// In case you care: According to the WinZip specification, the actual key used is derived from the 
    /// ZipEntry.Password via an algorithm that complies with RFC 2898, using an iteration count of 1000.
    /// I am no security expert, but I think you should use a long-ish password if you employ 256-bit AES
    /// encryption.  Make it 16 characters or more.  
    /// </para>
    /// <para>
    /// The WinZip AES algorithms are not supported with the version of DotNetZip that runs on the .NET Compact Framework. 
    /// This is because .NET CF lacks the HMACSHA1 class that is required for producing the archive.
    /// </para>
    /// </remarks>
    internal enum EncryptionAlgorithm
    {
        /// <summary>
        /// No encryption at all.
        /// </summary>
        None = 0,

        /// <summary>
        /// Traditional or Classic pkzip encryption.
        /// </summary>
        PkzipWeak,

#if AESCRYPTO
        /// <summary>
        /// WinZip AES encryption (128 key bits).
        /// </summary>
        WinZipAes128,

        /// <summary>
        /// WinZip AES encryption (256 key bits).
        /// </summary>
        WinZipAes256,
#endif

        // others... not implemented (yet?)
    }

    /// <summary>
    /// An enum that specifies the source of the ZipEntry. 
    /// </summary>
    internal enum EntrySource
    {
        /// <summary>
        /// Default value.  Invalid on a bonafide ZipEntry.
        /// </summary>
        None = 0,

        /// <summary>
        /// Entry was instantiated by Adding an entry from the filesystem.
        /// </summary>
        Filesystem,

        /// <summary>
        /// Entry was instantiated by reading a zipfile.
        /// </summary>
        Zipfile,

        /// <summary>
        /// Entry was instantiated via a stream or string.
        /// </summary>
        Stream,
    }


    /// <summary>
    /// Represents a single entry in a ZipFile. Typically, applications
    /// get a ZipEntry by enumerating the entries within a ZipFile,
    /// or by adding an entry to a ZipFile.  
    /// </summary>
    internal partial class ZipEntry
    {
		internal const string _IBM437_ = "UTF8";
        internal ZipEntry() { }

        /// <summary>
        /// The time and date at which the file indicated by the ZipEntry was last modified. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The DotNetZip library sets the LastModified value for an entry, equal to the 
        /// Last Modified time of the file in the filesystem.  If an entry is added from a stream, 
        /// in which case no Last Modified attribute is available, the library uses 
        /// <c>System.DateTime.Now</c> for this value, for the given entry. 
        /// </para>
        ///
        /// <para>
        /// It is also possible to set the LastModified value on an entry, to an arbitrary
        /// value.  Be aware that because of the way the PKZip specification describes how
        /// times are stored in the zip file, the full precision of the
        /// <c>System.DateTime</c> datatype is not stored in LastModified when saving zip
        /// files.  For more information on how times are formatted, see the PKZip
        /// specification.
        /// </para>
        ///
        /// <para>
        /// The last modified time of the file created upon a call to <c>ZipEntry.Extract()</c> 
        /// may be adjusted during extraction to compensate
        /// for differences in how the .NET Base Class Library deals
        /// with daylight saving time (DST) versus how the Windows
        /// filesystem deals with daylight saving time. 
        /// See http://blogs.msdn.com/oldnewthing/archive/2003/10/24/55413.aspx for more context.
        /// </para>
        /// <para>
        /// In a nutshell: Daylight savings time rules change regularly.  In
        /// 2007, for example, the inception week of DST changed.  In 1977,
        /// DST was in place all year round. In 1945, likewise.  And so on.
        /// Win32 does not attempt to guess which time zone rules were in
        /// effect at the time in question.  It will render a time as
        /// "standard time" and allow the app to change to DST as necessary.
        ///  .NET makes a different choice.
        /// </para>
        /// <para>
        /// Compare the output of FileInfo.LastWriteTime.ToString("f") with
        /// what you see in the Windows Explorer property sheet for a file that was last
        /// written to on the other side of the DST transition. For example,
        /// suppose the file was last modified on October 17, 2003, during DST but
        /// DST is not currently in effect. Explorer's file properties
        /// reports Thursday, October 17, 2003, 8:45:38 AM, but .NETs
        /// FileInfo reports Thursday, October 17, 2003, 9:45 AM.
        /// </para>
        /// <para>
        /// Win32 says, "Thursday, October 17, 2002 8:45:38 AM PST". Note:
        /// Pacific STANDARD Time. Even though October 17 of that year
        /// occurred during Pacific Daylight Time, Win32 displays the time as
        /// standard time because that's what time it is NOW.
        /// </para>
        /// <para>
        /// .NET BCL assumes that the current DST rules were in place at the
        /// time in question.  So, .NET says, "Well, if the rules in effect
        /// now were also in effect on October 17, 2003, then that would be
        /// daylight time" so it displays "Thursday, October 17, 2003, 9:45
        /// AM PDT" - daylight time.
        /// </para>
        /// <para>
        /// So .NET gives a value which is more intuitively correct, but is
        /// also potentially incorrect, and which is not invertible. Win32
        /// gives a value which is intuitively incorrect, but is strictly
        /// correct.
        /// </para>
        /// <para>
        /// Because of this funkiness, this library adds one hour to the LastModified time
        /// on the extracted file, if necessary.  That is to say, if the time in question
        /// had occurred in what the .NET Base Class Library assumed to be DST (an
        /// assumption that may be wrong given the constantly changing DST rules).
        /// </para>
        /// </remarks>
        ///
        public DateTime LastModified
        {
            get { return _LastModified; }
        }

        /// <summary>
        /// The name of the filesystem file, referred to by the ZipEntry. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This may be different than the path used in the archive itself. What I mean is, 
        /// if you call <c>Zip.AddFile("fooo.txt", AlternativeDirectory)</c>, then the 
        /// path used for the ZipEntry within the zip archive will be different than this path.  
        /// This path is used to locate the thing-to-be-zipped on disk. 
        /// </para>
        /// <para>
        /// If the entry is being added from a stream, then this is null (Nothing in VB).
        /// </para>
        /// 
        /// </remarks>
        /// <seealso cref="FileName"/>
        public string LocalFileName
        {
            get { return _LocalFileName; }
        }

        /// <summary>
        /// The name of the file contained in the ZipEntry. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// When writing a zip, this path has backslashes replaced with 
        /// forward slashes, according to the zip spec, for compatibility
        /// with Unix(tm) and ... get this.... Amiga!
        /// </para>
        ///
        /// <para>
        /// This is the name of the entry in the ZipFile itself.  This name may be different
        /// than the name of the filesystem file used to create the entry (LocalFileName). In fact, there
        /// may be no filesystem file at all, if the entry is created from a stream or a string.
        /// </para>
        ///
        /// <para>
        /// When setting this property, the value is made permanent only after a call to one of the ZipFile.Save() methods 
        /// on the ZipFile that contains the ZipEntry. By reading in a ZipFile, then explicitly setting the FileName on an
        /// entry contained within the ZipFile, and then calling Save(), you will effectively rename the entry within 
        /// the zip archive.
        /// </para>
        /// </remarks>
        /// <seealso cref="LocalFileName"/>
        public string FileName
        {
            get { return _FileNameInArchive; }
        }

        /// <summary>
        /// The version of the zip engine needed to read the ZipEntry.  
        /// </summary>
        /// <remarks>
        /// This is usually 0x14. 
        /// (Decimal 20). If ZIP64 is in use, the version will be decimal 45.  
        /// </remarks>
        public Int16 VersionNeeded
        {
            get { return _VersionNeeded; }
        }

        /// <summary>
        /// The comment attached to the ZipEntry. 
        /// </summary>
        ///
        /// <remarks>
        /// By default, the Comment is encoded in IBM437 code page. You can specify 
        /// an alternative with <see cref="ProvisionalAlternateEncoding"/>
        /// </remarks>
        /// <seealso cref="ProvisionalAlternateEncoding">ProvisionalAlternateEncoding</seealso>
        public string Comment
        {
            get { return _Comment; }
        }


        /// <summary>
        /// The bitfield as defined in the zip spec. You probably never need to look at this.
        /// </summary>
        ///
        /// <remarks>
        /// <code>
        /// bit  0 - set if encryption is used.
        /// b. 1-2 - set to determine whether normal, max, fast deflation.  
        ///          This library always leaves these bits unset when writing (indicating 
        ///          "normal" deflation").
        ///
        /// bit  3 - indicates crc32, compressed and uncompressed sizes are zero in
        ///          local header.  We always leave this as zero on writing, but can read
        ///          a zip with it nonzero. 
        ///
        /// bit  4 - reserved for "enhanced deflating". This library doesn't do enhanced deflating.
        /// bit  5 - set to indicate the zip is compressed patched data.  This library doesn't do that.
        /// bit  6 - set if strong encryption is used (must also set bit 1 if bit 6 is set)
        /// bit  7 - unused
        /// bit  8 - unused
        /// bit  9 - unused
        /// bit 10 - unused
        /// Bit 11 - Language encoding flag (EFS).  If this bit is set,
        ///          the filename and comment fields for this file
        ///          must be encoded using UTF-8. This library currently does not support UTF-8.
        /// Bit 12 - Reserved by PKWARE for enhanced compression.
        /// Bit 13 - Used when encrypting the Central Directory to indicate 
        ///          selected data values in the Local Header are masked to
        ///          hide their actual values.  See the section describing 
        ///          the Strong Encryption Specification for details.
        /// Bit 14 - Reserved by PKWARE.
        /// Bit 15 - Reserved by PKWARE.
        /// </code>
        /// </remarks>

        public Int16 BitField
        {
            get { return _BitField; }
        }

        /// <summary>
        /// The compression method employed for this ZipEntry. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The ZIP specification allows a variety of compression methods.  This library 
        /// supports just two:  0x08 = Deflate.  0x00 = Store (no compression).  
        /// </para>
        /// 
        /// <para>
        /// When reading an entry from an existing zipfile, the value you retrieve here
        /// indicates the compression method used on the entry by the original creator of the zip.  
        /// When writing a zipfile, you can specify either 0x08 (Deflate) or 0x00 (None).  If you 
        /// try setting something else, you will get an exception.  
        /// </para>
        /// 
        /// <para>
        /// You may wish to set CompressionMethod to 0 (None) when zipping previously compressed
        /// data like a jpg, png, or mp3 file.  This can save time and cpu cycles.
        /// Setting CompressionMethod to 0 is equivalent to setting ForceNoCompression to true. 
        /// </para>
        /// 
        /// <para>
        /// When updating a ZipFile, you may not modify the CompressionMethod on an entry that has been encrypted. 
        /// In other words, if you read an existing ZipFile with one of the ZipFile.Read() methods, and then 
        /// change the CompressionMethod on an entry that has Encryption not equal to None, you will receive an exception. 
        /// There is no way to modify the compression on an encrypted entry, without extracting it and re-adding it 
        /// into the ZipFile.  
        /// </para>
        /// </remarks>
        /// 
        /// <example>
        /// In this example, the first entry added to the zip archive uses 
        /// the default behavior - compression is used where it makes sense.  
        /// The second entry, the MP3 file, is added to the archive without being compressed.
        /// <code>
        /// using (ZipFile zip = new ZipFile(ZipFileToCreate))
        /// {
        ///   ZipEntry e1= zip.AddFile(@"c:\temp\Readme.txt");
        ///   ZipEntry e2= zip.AddFile(@"c:\temp\StopThisTrain.mp3");
        ///   e2.CompressionMethod = 0;
        ///   zip.Save();
        /// }
        /// </code>
        /// 
        /// <code lang="VB">
        /// Using zip as new ZipFile(ZipFileToCreate)
        ///   zip.AddFile("c:\temp\Readme.txt")
        ///   Dim e2 as ZipEntry = zip.AddFile("c:\temp\StopThisTrain.mp3")
        ///   e2.CompressionMethod = 0
        ///   zip.Save
        /// End Using
        /// </code>
        /// </example>
        public Int16 CompressionMethod
        {
            get { return _CompressionMethod; }
        }


        /// <summary>
        /// The compressed size of the file, in bytes, within the zip archive. 
        /// </summary>
        /// <remarks>
        /// The compressed size is computed during compression. This means that it is only
        /// valid to read this AFTER reading in an existing zip file, or AFTER saving a
        /// zipfile you are creating.
        /// </remarks>
        public Int64 CompressedSize
        {
            get { return _CompressedSize; }
        }

        /// <summary>
        /// The size of the file, in bytes, before compression, or after extraction. 
        /// </summary>
        /// <remarks>
        /// This property is valid AFTER reading in an existing zip file, or AFTER saving the 
        /// ZipFile that contains the ZipEntry.
        /// </remarks>
        public Int64 UncompressedSize
        {
            get { return _UncompressedSize; }
        }

        /// <summary>
        /// The ratio of compressed size to uncompressed size of the ZipEntry.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// This is a ratio of the compressed size to the uncompressed size of the entry,
        /// expressed as a double in the range of 0 to 100+. A value of 100 indicates no
        /// compression at all.  It could be higher than 100 when the compression algorithm
        /// actually inflates the data.
        /// </para>
        ///
        /// <para>
        /// You could format it for presentation to a user via a format string of "{3,5:F0}%"
        /// to see it as a percentage. 
        /// </para>
        ///
        /// <para>
        /// If the size of the original uncompressed file is 0, (indicating a denominator of 0)
        /// the return value will be zero. 
        /// </para>
        ///
        /// <para>
        /// This property is valid AFTER reading in an existing zip file, or AFTER saving the 
        /// ZipFile that contains the ZipEntry.
        /// </para>
        ///
        /// </remarks>
        public Double CompressionRatio
        {
            get
            {
                if (UncompressedSize == 0) return 0;
                return 100 * (1.0 - (1.0 * CompressedSize) / (1.0 * UncompressedSize));
            }
        }

        /// <summary>
        /// The CRC (Cyclic Redundancy Check) on the contents of the ZipEntry. 
        /// </summary>
        /// 
        /// <remarks>
        /// You probably don't need to concern yourself with this. The CRC is generated according
        /// to the algorithm described in the Pkzip specification. It is a read-only property;
        /// when creating a Zip archive, the CRC for each entry is set only after a call to
        /// Save() on the containing ZipFile.
        /// </remarks>
        public Int32 Crc32
        {
            get { return _Crc32; }
        }

        /// <summary>
        /// True if the entry is a directory (not a file). 
        /// This is a readonly property on the entry.
        /// </summary>
        public bool IsDirectory
        {
            get { return _IsDirectory; }
        }

        /// <summary>
        /// A derived property that is <c>true</c> if the entry uses encryption.  
        /// </summary>
        /// <remarks>
        /// This is a readonly property on the entry.
        /// Upon reading an entry, this bool is determined by
        /// the data read.  When writing an entry, this bool is
        /// determined by whether the Encryption property is set to something other than
        /// EncryptionAlgorithm.None. 
        /// </remarks>
        public bool UsesEncryption
        {
            get { return (Encryption != EncryptionAlgorithm.None); }
        }

        /// <summary>
        /// Set this to specify which encryption algorithm to use for the entry.
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// When setting this property, you must also set a Password on the entry.  The set of
        /// algorithms supported is determined by the authors of this library.  The PKZIP
        /// specification from PKWare defines a set of encryption algorithms, and the data formats
        /// for the zip archive that support them. Other vendors of tools and libraries, such as
        /// WinZip or Xceed, also specify and support different encryption algorithms and data
        /// formats.
        /// </para>
        ///
        /// <para>
        /// There is no common, ubiquitous multi-vendor standard for strong encryption. There is
        /// broad support for "traditional" Zip encryption, sometimes called Zip 2.0 encryption,
        /// as specified by PKWare, but this encryption is considered weak. This library currently
        /// supports AES 128 and 256 in addition to the Zip 2.0 "weak" encryption.
        /// </para>
        ///
        /// <para>
        /// The WinZip AES encryption algorithms are not supported on the .NET Compact Framework. 
        /// </para>
        /// </remarks>
        internal EncryptionAlgorithm Encryption
        {
            get
            {
                return _Encryption;
            }
        }

        /// <summary>
        /// Set to indicate whether to use UTF-8 encoding on filenames and 
        /// comments, according to the PKWare specification.  
        /// </summary>
        /// <remarks>
        /// If this flag is set, the entry will be marked as encoded with UTF-8, 
        /// according to the PWare spec, if necessary.  Necessary means, if the filename or 
        /// entry comment (if any) cannot be reflexively encoded with the default (IBM437) code page. 
        /// </remarks>
        /// <remarks>
        /// Setting this flag to true is equivalent to setting <see cref="ProvisionalAlternateEncoding"/> to <c>System.Text.Encoding.UTF8</c>
        /// </remarks>
        public bool UseUnicodeAsNecessary
        {
            get
            {
                return _provisionalAlternateEncoding == System.Text.Encoding.UTF8;
            }
            set
            {
                _provisionalAlternateEncoding = (value) ? System.Text.Encoding.UTF8 : ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.ZipFile.DefaultEncoding;
            }
        }

        /// <summary>
        /// The text encoding to use for this ZipEntry, when the default
        /// encoding is insufficient.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        /// According to the zip specification from PKWare, filenames and comments for a
        /// ZipEntry are encoded either with IBM437 or with UTF8.  But, some archivers do not
        /// follow the specification, and instead encode characters using the system default
        /// code page, or an arbitrary code page.  For example, WinRAR when run on a machine in
        /// Shanghai may encode filenames with the Chinese (Big-5) code page.  This behavior is
        /// contrary to the Zip specification, but it occurs anyway.  This property exists to
        /// support that non-compliant behavior when reading or writing zip files.
        /// </para>
        /// <para>
        /// When writing zip archives that will be read by one of these other archivers, use this property to 
        /// specify the code page to use when encoding filenames and comments into the zip
        /// file, when the IBM437 code page will not suffice.
        /// </para>
        /// <para>
        /// Be aware that a zip file created after you've explicitly specified the code page will not 
        /// be compliant to the PKWare specification, and may not be readable by compliant archivers. 
        /// On the other hand, many archivers are non-compliant and can read zip files created in 
        /// arbitrary code pages. 
        /// </para>
        /// <para>
        /// When using an arbitrary, non-UTF8 code page for encoding, there is no standard way for the 
        /// creator (DotNetZip) to specify in the zip file which code page has been used. DotNetZip is not
        /// able to inspect the zip file and determine the codepage used for the entries within it. Therefore, 
        /// you, the application author, must determine that.  If you use a codepage which results in filenames
        /// that are not legal in Windows, you will get exceptions upon extract. Caveat Emptor.
        /// </para>
        /// </remarks>
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
        /// The text encoding actually used for this ZipEntry.
        /// </summary>
        public System.Text.Encoding ActualEncoding
        {
            get
            {
                return _actualEncoding;
            }
        }



        private static bool ReadHeader(ZipEntry ze, System.Text.Encoding defaultEncoding)
        {
            int bytesRead = 0;

            ze._RelativeOffsetOfLocalHeader = (int)ze.ArchiveStream.Position;

            int signature = ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.SharedUtilities.ReadSignature(ze.ArchiveStream);
            bytesRead += 4;

            // Return false if this is not a local file header signature.
            if (ZipEntry.IsNotValidSig(signature))
            {
                // Getting "not a ZipEntry signature" is not always wrong or an error. 
                // This will happen after the last entry in a zipfile.  In that case, we 
                // expect to read : 
                //    a ZipDirEntry signature (if a non-empty zip file) or 
                //    a ZipConstants.EndOfCentralDirectorySignature.  
                //
                // Anything else is a surprise.

                ze.ArchiveStream.Seek(-4, System.IO.SeekOrigin.Current); // unread the signature
                if (ZipEntry.IsNotValidZipDirEntrySig(signature) && (signature != ZipConstants.EndOfCentralDirectorySignature))
                {
                    throw new BadReadException(String.Format("  ZipEntry::ReadHeader(): Bad signature (0x{0:X8}) at position  0x{1:X8}", signature, ze.ArchiveStream.Position));
                }
                return false;
            }

            byte[] block = new byte[26];
            int n = ze.ArchiveStream.Read(block, 0, block.Length);
            if (n != block.Length) return false;
            bytesRead += n;

            int i = 0;
            ze._VersionNeeded = (short)(block[i++] + block[i++] * 256);
            ze._BitField = (short)(block[i++] + block[i++] * 256);
            ze._CompressionMethod = (short)(block[i++] + block[i++] * 256);
            ze._TimeBlob = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;
            // transform the time data into something usable (a DateTime)
            ze._LastModified = ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.SharedUtilities.PackedToDateTime(ze._TimeBlob);

            // NB: if ((ze._BitField & 0x0008) != 0x0008), then the Compressed, uncompressed and 
            // CRC values are not true values; the true values will follow the entry data.  
            // Nevertheless, regardless of the statis of bit 3 in the bitfield, the slots for 
            // the three amigos may contain marker values for ZIP64.  So we must read them.
            {
                ze._Crc32 = (Int32)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
                ze._CompressedSize = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
                ze._UncompressedSize = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);

                // validate ZIP64?  No.  We don't need to be pedantic about it. 
                //if (((uint)ze._CompressedSize == 0xFFFFFFFF &&
                //    (uint)ze._UncompressedSize != 0xFFFFFFFF) ||
                //    ((uint)ze._CompressedSize != 0xFFFFFFFF &&
                //    (uint)ze._UncompressedSize == 0xFFFFFFFF))
                //    throw new BadReadException(String.Format("  ZipEntry::Read(): Inconsistent uncompressed size (0x{0:X8}) for zip64, at position  0x{1:X16}", ze._UncompressedSize, ze.ArchiveStream.Position));

                if ((uint)ze._CompressedSize == 0xFFFFFFFF ||
                    (uint)ze._UncompressedSize == 0xFFFFFFFF)

                    ze._InputUsesZip64 = true;


                //throw new BadReadException("  DotNetZip does not currently support reading the ZIP64 format.");
            }
            //             else
            //             {
            //                 // The CRC, compressed size, and uncompressed size stored here are not valid.
            // 		// The actual values are stored later in the stream.
            //                 // Here, we advance the pointer to skip the dummy data.
            //                 i += 12;
            //             }

            Int16 filenameLength = (short)(block[i++] + block[i++] * 256);
            Int16 extraFieldLength = (short)(block[i++] + block[i++] * 256);

            block = new byte[filenameLength];
            n = ze.ArchiveStream.Read(block, 0, block.Length);
            bytesRead += n;

            // if the UTF8 bit is set for this entry, we override the encoding the application requested.
            ze._actualEncoding = ((ze._BitField & 0x0800) == 0x0800)
                ? System.Text.Encoding.UTF8
                : defaultEncoding;

            // need to use this form of GetString() for .NET CF
            ze._FileNameInArchive = ze._actualEncoding.GetString(block, 0, block.Length);

            // when creating an entry by reading, the LocalFileName is the same as the FileNameInArchive
            ze._LocalFileName = ze._FileNameInArchive;

            // workitem 6898
            if (ze._LocalFileName.EndsWith("/")) ze.MarkAsDirectory();

            bytesRead += ze.ProcessExtraField(extraFieldLength);

            ze._LengthOfTrailer = 0;

            // workitem 6607 - don't read for directories
            // actually get the compressed size and CRC if necessary
            if (!ze._LocalFileName.EndsWith("/") && (ze._BitField & 0x0008) == 0x0008)
            {
                // This descriptor exists only if bit 3 of the general
                // purpose bit flag is set (see below).  It is byte aligned
                // and immediately follows the last byte of compressed data.
                // This descriptor is used only when it was not possible to
                // seek in the output .ZIP file, e.g., when the output .ZIP file
                // was standard output or a non-seekable device.  For ZIP64(tm) format
                // archives, the compressed and uncompressed sizes are 8 bytes each.

                long posn = ze.ArchiveStream.Position;

                // Here, we're going to loop until we find a ZipEntryDataDescriptorSignature and 
                // a consistent data record after that.   To be consistent, the data record must 
                // indicate the length of the entry data. 
                bool wantMore = true;
                long SizeOfDataRead = 0;
                int tries = 0;
                while (wantMore)
                {
                    tries++;
                    // We call the FindSignature shared routine to find the specified signature
                    // in the already-opened zip archive, starting from the current cursor
                    // position in that filestream.  There are two possibilities: either we
                    // find the signature or we don't.  If we cannot find it, then the routine
                    // returns -1, and the ReadHeader() method returns false, indicating we
                    // cannot read a legal entry header.  If we have found it, then the
                    // FindSignature() method returns the number of bytes in the stream we had
                    // to seek forward, to find the sig.  We need this to determine if the zip
                    // entry is valid, later.

                    long d = ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.SharedUtilities.FindSignature(ze.ArchiveStream, ZipConstants.ZipEntryDataDescriptorSignature);
                    if (d == -1) return false;

                    // total size of data read (through all loops of this). 
                    SizeOfDataRead += d;

                    if (ze._InputUsesZip64 == true)
                    {
                        // read 1x 4-byte (CRC) and 2x 8-bytes (Compressed Size, Uncompressed Size)
                        block = new byte[20];
                        n = ze.ArchiveStream.Read(block, 0, block.Length);
                        if (n != 20) return false;

                        // do not increment bytesRead - it is for entry header only.
                        // the data we have just read is a footer (falls after the file data)
                        //bytesRead += n; 

                        i = 0;
                        ze._Crc32 = (Int32)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
                        ze._CompressedSize = BitConverter.ToInt64(block, i);
                        i += 8;
                        ze._UncompressedSize = BitConverter.ToInt64(block, i);
                        i += 8;

                        ze._LengthOfTrailer += 24;  // bytes including sig, CRC, Comp and Uncomp sizes
                    }
                    else
                    {
                        // read 3x 4-byte fields (CRC, Compressed Size, Uncompressed Size)
                        block = new byte[12];
                        n = ze.ArchiveStream.Read(block, 0, block.Length);
                        if (n != 12) return false;

                        // do not increment bytesRead - it is for entry header only.
                        // the data we have just read is a footer (falls after the file data)
                        //bytesRead += n; 

                        i = 0;
                        ze._Crc32 = (Int32)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
                        ze._CompressedSize = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
                        ze._UncompressedSize = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
                        ze._LengthOfTrailer += 16;  // bytes including sig, CRC, Comp and Uncomp sizes

                    }

                    wantMore = (SizeOfDataRead != ze._CompressedSize);
                    if (wantMore)
                    {
                        // Seek back to un-read the last 12 bytes  - maybe THEY contain 
                        // the ZipEntryDataDescriptorSignature.
                        // (12 bytes for the CRC, Comp and Uncomp size.)
                        ze.ArchiveStream.Seek(-12, System.IO.SeekOrigin.Current);

                        // Adjust the size to account for the false signature read in 
                        // FindSignature().
                        SizeOfDataRead += 4;
                    }
                }

                //if (SizeOfDataRead != ze._CompressedSize)
                //    throw new BadReadException("Data format error (bit 3 is set)");

                // seek back to previous position, to prepare to read file data
                ze.ArchiveStream.Seek(posn, System.IO.SeekOrigin.Begin);
            }

            ze._CompressedFileDataSize = ze._CompressedSize;


            // bit 0 set indicates that some kind of encryption is in use
            if ((ze._BitField & 0x01) == 0x01)
            {

#if AESCRYPTO
                if (ze.Encryption == EncryptionAlgorithm.WinZipAes128 ||
                    ze.Encryption == EncryptionAlgorithm.WinZipAes256)
                {
                    // read in the WinZip AES metadata
                    ze._aesCrypto = WinZipAesCrypto.ReadFromStream(null, ze._KeyStrengthInBits, ze.ArchiveStream);
                    bytesRead += ze._aesCrypto.SizeOfEncryptionMetadata - 10;
                    ze._CompressedFileDataSize = ze.CompressedSize - ze._aesCrypto.SizeOfEncryptionMetadata;
                    ze._LengthOfTrailer += 10;
                }
                else
#endif
                {
                    // read in the header data for "weak" encryption
                    ze._WeakEncryptionHeader = new byte[12];
                    bytesRead += ZipEntry.ReadWeakEncryptionHeader(ze._archiveStream, ze._WeakEncryptionHeader);
                    // decrease the filedata size by 12 bytes
                    ze._CompressedFileDataSize -= 12;
                }
            }

            // Remember the size of the blob for this entry. 
            // We also have the starting position in the stream for this entry. 
            ze._LengthOfHeader = bytesRead;
            ze._TotalEntrySize = ze._LengthOfHeader + ze._CompressedSize + ze._LengthOfTrailer;


            // We've read in the regular entry header, the extra field, and any encryption
            // header.  The pointer in the file is now at the start of the filedata, which is
            // potentially compressed and encrypted.  Just ahead in the file, there are
            // _CompressedFileDataSize bytes of data, followed by potentially a non-zero
            // length trailer, consisting of optionally, some encryption stuff (10 byte MAC for AES), and 
            // the bit-3 trailer (16 or 24 bytes).

            return true;
        }



        internal static int ReadWeakEncryptionHeader(Stream s, byte[] buffer)
        {
            // PKZIP encrypts the compressed data stream.  Encrypted files must
            // be decrypted before they can be extracted.

            // Each PKZIP-encrypted file has an extra 12 bytes stored at the start of the data
            // area defining the encryption header for that file.  The encryption header is
            // originally set to random values, and then itself encrypted, using three, 32-bit
            // keys.  The key values are initialized using the supplied encryption password.
            // After each byte is encrypted, the keys are then updated using pseudo-random
            // number generation techniques in combination with the same CRC-32 algorithm used
            // in PKZIP and implemented in the CRC32.cs module in this project.

            // read the 12-byte encryption header
            int additionalBytesRead = s.Read(buffer, 0, 12);
            if (additionalBytesRead != 12)
                throw new ZipException(String.Format("Unexpected end of data at position 0x{0:X8}", s.Position));

            return additionalBytesRead;
        }



        private static bool IsNotValidSig(int signature)
        {
            return (signature != ZipConstants.ZipEntrySignature);
        }


        /// <summary>
        /// Reads one ZipEntry from the given stream.  If the entry is encrypted, we don't
        /// decrypt at this point.  We also do not decompress.  Mostly we read metadata.
        /// </summary>
        /// <param name="zf">the zipfile this entry belongs to.</param>
        /// <param name="first">true of this is the first entry being read from the stream.</param>
        /// <returns>the ZipEntry read from the stream.</returns>
        internal static ZipEntry Read(ZipFile zf, bool first)
        {
            System.IO.Stream s = zf.ReadStream;

            System.Text.Encoding defaultEncoding = zf.ProvisionalAlternateEncoding;
            ZipEntry entry = new ZipEntry();
            entry._Source = EntrySource.Zipfile;
            entry._zipfile = zf;
            entry._archiveStream = s;

            if (first) HandlePK00Prefix(s);

            if (!ReadHeader(entry, defaultEncoding)) return null;

            // store the position in the stream for this entry
            entry.__FileDataPosition = entry.ArchiveStream.Position;

            // seek past the data without reading it. We will read on Extract()
            s.Seek(entry._CompressedFileDataSize, System.IO.SeekOrigin.Current);

            // workitem 6607 - don't seek for directories
            // finally, seek past the (already read) Data descriptor if necessary
            if (((entry._BitField & 0x0008) == 0x0008) && !entry.FileName.EndsWith("/"))
            {
                // _InputUsesZip64 is set in ReadHeader()
                int DescriptorSize = (entry._InputUsesZip64) ? 24 : 16;
                s.Seek(DescriptorSize, System.IO.SeekOrigin.Current);
            }

            // workitem 5306
            // http://www.codeplex.com/DotNetZip/WorkItem/View.aspx?WorkItemId=5306
            HandleUnexpectedDataDescriptor(entry);

            return entry;
        }


        internal static void HandlePK00Prefix(Stream s)
        {
            // in some cases, the zip file begins with "PK00".  This is a throwback and is rare,
            // but we handle it anyway. We do not change behavior based on it.
            uint datum = (uint)ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.SharedUtilities.ReadInt(s);
            if (datum != ZipConstants.PackedToRemovableMedia)
            {
                s.Seek(-4, System.IO.SeekOrigin.Current); // unread the block
            }
        }



        private static void HandleUnexpectedDataDescriptor(ZipEntry entry)
        {
            System.IO.Stream s = entry.ArchiveStream;
            // In some cases, the "data descriptor" is present, without a signature, even when bit 3 of the BitField is NOT SET.  
            // This is the CRC, followed
            //    by the compressed length and the uncompressed length (4 bytes for each 
            //    of those three elements).  Need to check that here.             
            //
            uint datum = (uint)ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.SharedUtilities.ReadInt(s);
            if (datum == entry._Crc32)
            {
                int sz = ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.SharedUtilities.ReadInt(s);
                if (sz == entry._CompressedSize)
                {
                    sz = ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.SharedUtilities.ReadInt(s);
                    if (sz == entry._UncompressedSize)
                    {
                        // ignore everything and discard it.
                    }
                    else
                        s.Seek(-12, System.IO.SeekOrigin.Current); // unread the three blocks
                }
                else
                    s.Seek(-8, System.IO.SeekOrigin.Current); // unread the two blocks
            }
            else
                s.Seek(-4, System.IO.SeekOrigin.Current); // unread the block

        }


        #region Extract methods
       
        /// <summary>
        /// Extracts the entry to the specified stream. 
        /// </summary>
        /// 
        /// <remarks>
        /// 
        /// <para>
        /// For example, the caller could specify Console.Out, or a MemoryStream.
        /// </para>
        /// 
        /// </remarks>
        /// 
        /// <param name="stream">the stream to which the entry should be extracted.  </param>
        /// 
        public void Extract(System.IO.Stream stream)
        {
            InternalExtract(stream);
        }


        internal System.IO.Stream ArchiveStream
        {
            get
            {
                if (_archiveStream == null)
                {
                    if (_zipfile != null)
                    {
                        _archiveStream = _zipfile.ReadStream;
                    }
                }
                return _archiveStream;
            }
        }
        #endregion


        // Pass in either basedir or s, but not both. 
        // In other words, you can extract to a stream or to a directory (filesystem), but not both!
        // The Password param is required for encrypted entries.
		private void InternalExtract(System.IO.Stream output)
        {
            try
            {
                ValidateCompression();
                ValidateEncryption();

				if (ValidateOutput(output))
                {
                    // if true, then the entry was a directory and has been created.
                    // We need to fire the Extract Event.
                    return;
                }
				
                Int32 ActualCrc32 = _ExtractOne(output);

                // After extracting, Validate the CRC32
                if (ActualCrc32 != _Crc32)
                {
#if AESCRYPTO
                    // CRC is not meaningful with WinZipAES and AES method 2 (AE-2)
                    if ((Encryption != EncryptionAlgorithm.WinZipAes128 &&
                        Encryption != EncryptionAlgorithm.WinZipAes256)
                        || _WinZipAesMethod != 0x02)
#endif

                        throw new BadCrcException("CRC error: the file being extracted appears to be corrupted. " +
                              String.Format("Expected 0x{0:X8}, Actual 0x{1:X8}", _Crc32, ActualCrc32));
                }


            }
            catch
            {
                

                // re-raise the original exception
                throw;
            }
        }



        private void ValidateEncryption()
        {
#if AESCRYPTO
            if (Encryption != EncryptionAlgorithm.PkzipWeak &&
                Encryption != EncryptionAlgorithm.WinZipAes128 &&
                Encryption != EncryptionAlgorithm.WinZipAes256 &&
                Encryption != EncryptionAlgorithm.None)
                throw new ArgumentException(String.Format("Unsupported Encryption algorithm ({0:X2})",
                              Encryption));
#else
            if (Encryption != EncryptionAlgorithm.PkzipWeak &&
                Encryption != EncryptionAlgorithm.None)
                throw new ArgumentException(String.Format("Unsupported Encryption algorithm ({0:X2})",
                              Encryption));

#endif

        }

        private void ValidateCompression()
        {
            if ((CompressionMethod != 0) && (CompressionMethod != 0x08))  // deflate
                throw new ArgumentException(String.Format("Unsupported Compression method (0x{0:X2})",
                              CompressionMethod));
        }

        private bool ValidateOutput(Stream outstream)
        {
            if (outstream != null)
            {
                if ((IsDirectory) || (FileName.EndsWith("/")))
                {
                    // extract a directory to streamwriter?  nothing to do!
                    return true;  // true == all done!  caller can return
                }
                return false;
            }

            throw new ZipException("Cannot extract.", new ArgumentException("Invalid input.", "outstream | basedir"));
        }



        private void _CheckRead(int nbytes)
        {
            if (nbytes == 0)
                throw new BadReadException(String.Format("bad read of entry {0} from compressed archive.",
                             this.FileName));

        }


        private Int32 _ExtractOne(System.IO.Stream output)
        {
            System.IO.Stream input = this.ArchiveStream;

            input.Seek(this.FileDataPosition, System.IO.SeekOrigin.Begin);

            // to validate the CRC. 
            Int32 CrcResult = 0;

            byte[] bytes = new byte[WORKING_BUFFER_SIZE];

            // The extraction process varies depending on how the entry was stored.
            // It could have been encrypted, and it coould have been compressed, or both, or
            // neither. So we need to check both the encryption flag and the compression flag,
            // and take the proper action in all cases.  

            Int64 LeftToRead = (CompressionMethod == 0x08) ? this.UncompressedSize : this._CompressedFileDataSize;

            // Get a stream that either decrypts or not.
            Stream input2 = null;
            if (Encryption != EncryptionAlgorithm.None)
				throw new NotSupportedException("Encrypted ZIP files not supported");

			input2 = new CrcCalculatorStream(input, _CompressedFileDataSize);


            //Stream input2a = new TraceStream(input2);

            // Using the above, now we get a stream that either decompresses or not.
            Stream input3 = (CompressionMethod == 0x08)
                ? new ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.DeflateStream(input2, ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip.CompressionMode.Decompress, true)
                : input2;

            Int64 bytesWritten = 0;
            // As we read, we maybe decrypt, and then we maybe decompress. Then we write.
            using (var s1 = new CrcCalculatorStream(input3))
            {
                while (LeftToRead > 0)
                {
                    //Console.WriteLine("ExtractOne: LeftToRead {0}", LeftToRead);

                    // Casting LeftToRead down to an int is ok here in the else clause, because 
                    // that only happens when it is less than bytes.Length, which is much less
                    // than MAX_INT.
                    int len = (LeftToRead > bytes.Length) ? bytes.Length : (int)LeftToRead;
                    int n = s1.Read(bytes, 0, len);

                    //Console.WriteLine("ExtractOne: Read {0} bytes\n{1}", n, Util.FormatByteArray(bytes,n));

                    _CheckRead(n);
                    output.Write(bytes, 0, n);
                    LeftToRead -= n;
                    bytesWritten += n;
                }

                CrcResult = s1.Crc32;


#if AESCRYPTO
                // Read the MAC if appropriate
                if (Encryption == EncryptionAlgorithm.WinZipAes128 ||
                    Encryption == EncryptionAlgorithm.WinZipAes256)
                {
                    var wzs = input2 as WinZipAesCipherStream;
                    _aesCrypto.CalculatedMac = wzs.FinalAuthentication;
                }
#endif
            }

            return CrcResult;
        }




        internal void MarkAsDirectory()
        {
            _IsDirectory = true;
            // workitem 6279
            if (!_FileNameInArchive.EndsWith("/"))
                _FileNameInArchive += "/";
        }



     

#if INFOZIP_UTF8
	static private bool FileNameIsUtf8(char[] FileNameChars)
	{
	    bool isUTF8 = false;
	    bool isUnicode = false;
	    for (int j = 0; j < FileNameChars.Length; j++)
	    {
		byte[] b = System.BitConverter.GetBytes(FileNameChars[j]);
		isUnicode |= (b.Length != 2);
		isUnicode |= (b[1] != 0);
		isUTF8 |= ((b[0] & 0x80) != 0);
	    }

	    return isUTF8;
	}
#endif


        private byte[] ConsExtraField()
        {
            byte[] blockZip64 = null;
            byte[] blockWinZipAes = null;

            // Always emit an extra field with zip64 information.
            // Later, if we don't need it, we'll set the header ID to rubbish and
            // the data will be ignored.  This results in additional overhead metadata
            // in the zip file, but it will be small in comparison to the entry data.
            if (_zipfile._zip64 != Zip64Option.Never)
            {
                // add extra field for zip64 here
                blockZip64 = new byte[4 + 28];
                int i = 0;

                // HeaderId = dummy data now, maybe set to 0x0001 (ZIP64) later.
                //blockZip64[i++] = 0x99;
                //blockZip64[i++] = 0x99;

                // HeaderId = dummy data now, maybe set to 0x0001 (ZIP64) later.
               blockZip64[i++] = 0x99;
               blockZip64[i++] = 0x99;
 
                // DataSize
                blockZip64[i++] = 0x1c;  // decimal 28 - this is important
                blockZip64[i++] = 0x00;

                // The actual metadata - we may or may not have real values yet...

                // uncompressed size
                Array.Copy(BitConverter.GetBytes(_UncompressedSize), 0, blockZip64, i, 8);
                i += 8;
                // compressed size
                Array.Copy(BitConverter.GetBytes(_CompressedSize), 0, blockZip64, i, 8);
                i += 8;
                // relative offset
                Array.Copy(BitConverter.GetBytes(_RelativeOffsetOfLocalHeader), 0, blockZip64, i, 8);
                i += 8;
                // starting disk number
                Array.Copy(BitConverter.GetBytes(0), 0, blockZip64, i, 4);
            }


#if AESCRYPTO
            if (Encryption == EncryptionAlgorithm.WinZipAes128 ||
            Encryption == EncryptionAlgorithm.WinZipAes256)
            {
                blockWinZipAes = new byte[4 + 7];
                int i = 0;
                // extra field for WinZip AES 
                // header id
                blockWinZipAes[i++] = 0x01;
                blockWinZipAes[i++] = 0x99;

                // data size
                blockWinZipAes[i++] = 0x07;
                blockWinZipAes[i++] = 0x00;

                // vendor number
                blockWinZipAes[i++] = 0x01;  // AE-1 - means "Verify CRC"
                blockWinZipAes[i++] = 0x00;

                // vendor id "AE"
                blockWinZipAes[i++] = 0x41;
                blockWinZipAes[i++] = 0x45;

                // key strength
                blockWinZipAes[i] = 0xFF;
                if (_KeyStrengthInBits == 128)
                    blockWinZipAes[i] = 1;
                if (_KeyStrengthInBits == 256)
                    blockWinZipAes[i] = 3;
                i++;

                // actual compression method
                blockWinZipAes[i++] = (byte)(_CompressionMethod & 0x00FF);
                blockWinZipAes[i++] = (byte)(_CompressionMethod & 0xFF00);
            }
#endif

            // could inject other blocks here...


            // concatenate any blocks we've got: 
            byte[] block = null;
            int totalLength = 0;
            if (blockZip64 != null)
                totalLength += blockZip64.Length;

            if (blockWinZipAes != null)
                totalLength += blockWinZipAes.Length;

            if (totalLength > 0)
            {
                block = new byte[totalLength];
                int current = 0;
                if (blockZip64 != null)
                {
                    System.Array.Copy(blockZip64, 0, block, current,
                              blockZip64.Length);
                    current += blockZip64.Length;
                }

                if (blockWinZipAes != null)
                {
                    System.Array.Copy(blockWinZipAes, 0, block, current,
                              blockWinZipAes.Length);
                    current += blockWinZipAes.Length;
                }

            }

            return block;
        }



        // workitem 6513: when writing, use alt encoding only when ibm437 will not do
        private System.Text.Encoding GenerateCommentBytes()
        {
            _CommentBytes = ibm437.GetBytes(_Comment);
            // need to use this form of GetString() for .NET CF
            string s1 = ibm437.GetString(_CommentBytes, 0, _CommentBytes.Length);
            if (s1 == _Comment)
                return ibm437;
            else
            {
                _CommentBytes = _provisionalAlternateEncoding.GetBytes(_Comment);
                return _provisionalAlternateEncoding;
            }
        }


        // workitem 6513
        private byte[] _GetEncodedFileNameBytes()
        {
            // here, we need to flip the backslashes to forward-slashes, 
            // also, we need to trim the \\server\share syntax from any UNC path.
            // and finally, we need to remove any leading .\

            string SlashFixed = FileName.Replace("\\", "/");
            string s1 = null;
            if ((_TrimVolumeFromFullyQualifiedPaths) && (FileName.Length >= 3)
                && (FileName[1] == ':') && (SlashFixed[2] == '/'))
            {
                // trim off volume letter, colon, and slash
                s1 = SlashFixed.Substring(3);
            }
            else if ((FileName.Length >= 4)
             && ((SlashFixed[0] == '/') && (SlashFixed[1] == '/')))
            {
                int n = SlashFixed.IndexOf('/', 2);
                //System.Console.WriteLine("input Path '{0}'", FileName);
                //System.Console.WriteLine("xformed: '{0}'", SlashFixed);
                //System.Console.WriteLine("third slash: {0}\n", n);
                if (n == -1)
                    throw new ArgumentException("The path for that entry appears to be badly formatted");
                s1 = SlashFixed.Substring(n + 1);
            }
            else if ((FileName.Length >= 3)
             && ((SlashFixed[0] == '.') && (SlashFixed[1] == '/')))
            {
                // trim off dot and slash
                s1 = SlashFixed.Substring(2);
            }
            else
            {
                s1 = SlashFixed;
            }

            // workitem 6513: when writing, use the alternative encoding only when ibm437 will not do.
            byte[] result = ibm437.GetBytes(s1);
            // need to use this form of GetString() for .NET CF
            string s2 = ibm437.GetString(result, 0, result.Length);
            _CommentBytes = null;
            if (s2 == s1)
            {
                // file can be encoded with ibm437, now try comment

                // case 1: no comment.  use ibm437
                if (_Comment == null || _Comment.Length == 0)
                {
                    _actualEncoding = ibm437;
                    return result;
                }

                // there is a comment.  Get the encoded form.
                System.Text.Encoding commentEncoding = GenerateCommentBytes();
#if !SILVERLIGHT
                // case 2: if the comment also uses 437, we're good. 
                if (commentEncoding.CodePage == 437)
                {
                    _actualEncoding = ibm437;
                    return result;
                }
#endif
                // case 3: comment requires non-437 code page.  Use the same
                // code page for the filename.
                _actualEncoding = commentEncoding;
                result = commentEncoding.GetBytes(s1);
                return result;
            }
            else
            {
                // Cannot encode with ibm437 safely.
                // Therefore, use the provisional encoding
                result = _provisionalAlternateEncoding.GetBytes(s1);
                if (_Comment != null && _Comment.Length != 0)
                {
                    _CommentBytes = _provisionalAlternateEncoding.GetBytes(_Comment);
                }

                _actualEncoding = _provisionalAlternateEncoding;
                return result;
            }
        }


        // At current cursor position in the stream, read the extra field,
        // and set the properties on the ZipEntry instance appropriately. 
        // This can be called when processing the Extra field in the Central Directory, 
        // or in the local header.
        internal int ProcessExtraField(Int16 extraFieldLength)
        {
            int additionalBytesRead = 0;

            System.IO.Stream s = ArchiveStream;

            if (extraFieldLength > 0)
            {
                byte[] Buffer = this._Extra = new byte[extraFieldLength];
                additionalBytesRead = s.Read(Buffer, 0, Buffer.Length);

                int j = 0;
                while (j < Buffer.Length)
                {
                    int start = j;

                    UInt16 HeaderId = (UInt16)(Buffer[j] + Buffer[j + 1] * 256);
                    Int16 DataSize = (short)(Buffer[j + 2] + Buffer[j + 3] * 256);

                    j += 4;

                    switch (HeaderId)
                    {
                        case 0x0001: // ZIP64
                            {
                                // The _IsZip64Format flag is true IFF the prior compressed/uncompressed size values were 0xFFFFFFFF.
                                // But we don't need to be rigid about this.  Some zip archives don't behave this way.

                                //if (!ze._IsZip64Format)
                                //throw new BadReadException(String.Format("  Found zip64 metadata when none expected at position 0x{0:X16}", s.Position - additionalBytesRead));

                                this._InputUsesZip64 = true;

                                if (DataSize > 28)
                                    throw new BadReadException(String.Format("  Inconsistent datasize (0x{0:X4}) for ZIP64 extra field at position 0x{1:X16}", DataSize, s.Position - additionalBytesRead));

                                if (this._UncompressedSize == 0xFFFFFFFF)
                                {
                                    this._UncompressedSize = BitConverter.ToInt64(Buffer, j);
                                    j += 8;
                                }
                                if (this._CompressedSize == 0xFFFFFFFF)
                                {
                                    this._CompressedSize = BitConverter.ToInt64(Buffer, j);
                                    j += 8;
                                }
                                if (this._RelativeOffsetOfLocalHeader == 0xFFFFFFFF)
                                {
                                    this._RelativeOffsetOfLocalHeader = BitConverter.ToInt64(Buffer, j);
                                    j += 8;
                                }
                                // ignore the potential last 4 bytes - I don't know what to do with them anyway.
                            }
                            break;

#if AESCRYPTO
                        case 0x9901: // WinZip AES encryption is in use.  (workitem 6834)
                            // we will handle this extra field only  if compressionmethod is 0x63
                            //Console.WriteLine("Found WinZip AES Encryption header (compression:0x{0:X2})", this._CompressionMethod);
                            if (this._CompressionMethod == 0x0063)
                            {
                                if ((this._BitField & 0x01) != 0x01)
                                    throw new BadReadException(String.Format("  Inconsistent metadata at position 0x{0:X16}", s.Position - additionalBytesRead));

                                this._sourceIsEncrypted = true;

                                //this._aesCrypto = new WinZipAesCrypto(this);
                                // see spec at http://www.winzip.com/aes_info.htm
                                if (DataSize != 7)
                                    throw new BadReadException(String.Format("  Inconsistent WinZip AES datasize (0x{0:X4}) at position 0x{1:X16}", DataSize, s.Position - additionalBytesRead));

                                this._WinZipAesMethod = BitConverter.ToInt16(Buffer, j);
                                j += 2;
                                if (this._WinZipAesMethod != 0x01 && this._WinZipAesMethod != 0x02)
                                    throw new BadReadException(String.Format("  Unexpected vendor version number (0x{0:X4}) for WinZip AES metadata at position 0x{1:X16}",
                                        this._WinZipAesMethod, s.Position - additionalBytesRead));

                                Int16 vendorId = BitConverter.ToInt16(Buffer, j);
                                j += 2;
                                if (vendorId != 0x4541)
                                    throw new BadReadException(String.Format("  Unexpected vendor ID (0x{0:X4}) for WinZip AES metadata at position 0x{1:X16}", vendorId, s.Position - additionalBytesRead));

                                this._KeyStrengthInBits = -1;
                                if (Buffer[j] == 1) _KeyStrengthInBits = 128;
                                if (Buffer[j] == 3) _KeyStrengthInBits = 256;

                                if (this._KeyStrengthInBits < 0)
                                    throw new Exception(String.Format("Invalid key strength ({0})", this._KeyStrengthInBits));

                                this.Encryption = (this._KeyStrengthInBits == 128)
                                    ? EncryptionAlgorithm.WinZipAes128
                                    : EncryptionAlgorithm.WinZipAes256;

                                j++;

                                // set the actual compression method
                                this._CompressionMethod = BitConverter.ToInt16(Buffer, j);
                                j += 2; // a formality
                            }
                            break;
#endif
                    }

                    // move to the next Header in the extra field
                    j = start + DataSize + 4;
                }
            }
            return additionalBytesRead;
        }




        private void SetFdpLoh()
        {
            // Indicates that the value has not yet been set. 
            // Therefore, seek to the local header, figure the start of file data.
            long origPosition = this.ArchiveStream.Position;
            this.ArchiveStream.Seek(this._RelativeOffsetOfLocalHeader, System.IO.SeekOrigin.Begin);

            byte[] block = new byte[30];
            this.ArchiveStream.Read(block, 0, block.Length);

            // At this point we could verify the contents read from the local header
            // with the contents read from the central header.  We could, but don't need to. 
            // So we won't.

            Int16 filenameLength = (short)(block[26] + block[27] * 256);
            Int16 extraFieldLength = (short)(block[28] + block[29] * 256);

            this.ArchiveStream.Seek(filenameLength + extraFieldLength, System.IO.SeekOrigin.Current);
            this._LengthOfHeader = 30 + extraFieldLength + filenameLength;
            this.__FileDataPosition = _RelativeOffsetOfLocalHeader + 30 + filenameLength + extraFieldLength;

            if (this._Encryption == EncryptionAlgorithm.PkzipWeak)
            {
                this.__FileDataPosition += 12;
            }
#if AESCRYPTO
            else if (this.Encryption == EncryptionAlgorithm.WinZipAes128 ||
                    this.Encryption == EncryptionAlgorithm.WinZipAes256)
            {
                this.__FileDataPosition += ((this._KeyStrengthInBits / 8 / 2) + 2);// _aesCrypto.SizeOfEncryptionMetadata;
            }
#endif

            // restore file position:
            this.ArchiveStream.Seek(origPosition, System.IO.SeekOrigin.Begin);
        }



        internal long FileDataPosition
        {
            get
            {
                if (__FileDataPosition == -1)
                    SetFdpLoh();

                return __FileDataPosition;
            }
        }

        private int LengthOfHeader
        {
            get
            {
                if (_LengthOfHeader == 0)
                    SetFdpLoh();

                return _LengthOfHeader;
            }
        }

        internal DateTime _LastModified;
        private bool _TrimVolumeFromFullyQualifiedPaths = true;  // by default, trim them.
        internal string _LocalFileName;
        private string _FileNameInArchive;
        internal Int16 _VersionNeeded;
        internal Int16 _BitField;
        internal Int16 _CompressionMethod;
        internal string _Comment;
        private bool _IsDirectory;
        private byte[] _CommentBytes;
        internal Int64 _CompressedSize;
        internal Int64 _CompressedFileDataSize; // CompressedSize less 12 bytes for the encryption header, if any
        internal Int64 _UncompressedSize;
        internal Int32 _TimeBlob;
        internal Int32 _Crc32;
        internal byte[] _Extra;
        private long _cdrPosition;

		private static System.Text.Encoding ibm437 = System.Text.Encoding.Unicode; //WAS: IBM437
		private System.Text.Encoding _provisionalAlternateEncoding = System.Text.Encoding.UTF8; //IBM437
        private System.Text.Encoding _actualEncoding = null;

        internal ZipFile _zipfile;
        internal long __FileDataPosition = -1;
       internal Int64 _RelativeOffsetOfLocalHeader;
        private Int64 _TotalEntrySize;
        internal int _LengthOfHeader;
        internal int _LengthOfTrailer;
        private bool _InputUsesZip64;

        internal EntrySource _Source = EntrySource.None;
        internal EncryptionAlgorithm _Encryption = EncryptionAlgorithm.None;
        internal byte[] _WeakEncryptionHeader;
        internal System.IO.Stream _archiveStream;
        private object LOCK = new object();

        private const int WORKING_BUFFER_SIZE = 0x4400;
        private const int Rfc2898KeygenIterations = 1000;
    }

}