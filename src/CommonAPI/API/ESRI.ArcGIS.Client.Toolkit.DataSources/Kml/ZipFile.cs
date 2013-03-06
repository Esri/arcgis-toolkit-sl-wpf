// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Resources;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources.Kml
{
	/// <summary>
	/// Simple unzip utility leveraging Silverlight's GetResourceStream
	/// </summary>
	internal class ZipFile : IDisposable
	{
		private Stream stream;
		private ZipFile(Stream zipFileStream)
		{
			this.stream = zipFileStream;
		}

		public static ZipFile Read(Stream zipFileStream)
		{
			return new ZipFile(zipFileStream);
		}

		/// <summary>
		/// Gets the file stream for the specified filename.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <returns></returns>
		public Stream GetFileStream(string filename)
		{
			if (entries == null)
				entries = ParseCentralDirectory();
			long position = this.stream.Position;
			this.stream.Seek(0, SeekOrigin.Begin);
			Uri fileUri = new Uri(filename, UriKind.Relative);
			StreamResourceInfo info = new StreamResourceInfo(this.stream, null);
			StreamResourceInfo stream = System.Windows.Application.GetResourceStream(info, fileUri);
			this.stream.Position = position;
			if (stream != null)
				return stream.Stream;
			return null;
		}

		/// <summary>
		/// Gets the name of the files in zip file.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> EntryFileNames
		{
			get
			{
				if(entries == null)
					entries = ParseCentralDirectory();
				foreach (FileEntry entry in entries)
					//Ignore folders and special MacOS folders
					if(!entry.Filename.EndsWith("/") && !entry.Filename.StartsWith("__MACOSX/"))
						yield return entry.Filename;
			}
		}
		private class FileEntry
		{
			public string Filename;
			public int FileStart;
			public int CompressedSize;
			public int UncompressedSize;
			public int CRC32;
		}
		private List<FileEntry> entries;
		private List<FileEntry> ParseCentralDirectory()
		{
			BinaryReader reader = new BinaryReader(this.stream);
			List<FileEntry> entries = new List<FileEntry>();
			reader.BaseStream.Seek(-4, SeekOrigin.End);
			while (reader.ReadInt32() != 101010256)
			{
				reader.BaseStream.Seek(-5, SeekOrigin.Current);
			}
			reader.BaseStream.Seek(6, SeekOrigin.Current);
			short entryCount = reader.ReadInt16();
			int directorySize = reader.ReadInt32();
			int directoryStart = reader.ReadInt32();
			reader.BaseStream.Seek(directoryStart, SeekOrigin.Begin);
			bool needsFixing = false;
			for (int i = 0; i < entryCount; i++)
			{
				int headerSignature = reader.ReadInt32();
				if (headerSignature == 33639248) //Central directory file header signature 
				{
					reader.BaseStream.Seek(4, SeekOrigin.Current);
					short flag = reader.ReadInt16();
					if (flag == 8) //Silverlight doesn't like this format
					{
						needsFixing = true;
					}
					reader.BaseStream.Seek(6, SeekOrigin.Current);
					int crc32 = reader.ReadInt32();
					int compressedSize = reader.ReadInt32();
					int unCompressedSize = reader.ReadInt32();
					short fileNameLenght = reader.ReadInt16();
					short extraFieldLength = reader.ReadInt16();
					short fileCommentLength = reader.ReadInt16();
					reader.BaseStream.Seek(8, SeekOrigin.Current);
					int fileStart = reader.ReadInt32();
					string filename = new string(reader.ReadChars(fileNameLenght));
					entries.Add(new FileEntry()
					{
						Filename = filename, FileStart = fileStart, CRC32 = crc32,
						CompressedSize = compressedSize, UncompressedSize = unCompressedSize
					});
					reader.BaseStream.Seek(extraFieldLength + fileCommentLength, SeekOrigin.Current);
				}
			}
			if (needsFixing) 
			{
				//We are using a zipformat that Silverlight doesn't like. 
				//Zipfiles where the file size is reported after the compressed data
				//is a no-go, so we rebuild the header and report the information there.
				MemoryStream newZip = new MemoryStream();
				BinaryWriter writer = new BinaryWriter(newZip);
				//Rebuild file entries
				foreach (FileEntry entry in entries)
				{
					FileEntry e = entry;
					reader.BaseStream.Seek(entry.FileStart, SeekOrigin.Begin);
					e.FileStart = (int)writer.BaseStream.Position;
					CopyBytes(reader, writer, 6);
					writer.Write((short)2); //This flag needs to be replaced to indicate the new format
					reader.BaseStream.Seek(2, SeekOrigin.Current);
					CopyBytes(reader, writer, 6);
					writer.Write(entry.CRC32); //Update CRC
					writer.Write(entry.CompressedSize); //Update Compressed size
					writer.Write(entry.UncompressedSize); //Update Uncompressed size
					writer.Write((short)entry.Filename.Length);
					reader.BaseStream.Seek(14, SeekOrigin.Current);
					short fieldLength = reader.ReadInt16();
					writer.Write(fieldLength);
					CopyBytes(reader, writer, entry.Filename.Length + fieldLength + entry.CompressedSize);
				}
				//Rebuild directory
				reader.BaseStream.Seek(directoryStart, SeekOrigin.Begin);
				for (int i = 0; i < entryCount; i++)
				{
					CopyBytes(reader, writer, 8);
					writer.Write((short)2);  //This flag needs to be replaced to indicate the new format
					reader.BaseStream.Seek(2, SeekOrigin.Current);
					CopyBytes(reader, writer, 18);
					short filenamelength = reader.ReadInt16();
					writer.Write(filenamelength);
					short extrafieldlength = reader.ReadInt16();
					writer.Write(extrafieldlength);
					short filecommentlength = reader.ReadInt16();
					writer.Write(filecommentlength);
					CopyBytes(reader, writer, 8);
					writer.Write(entries[i].FileStart);
					reader.BaseStream.Seek(4, SeekOrigin.Current);
					CopyBytes(reader, writer, filenamelength + extrafieldlength + filecommentlength);
				}
				CopyBytes(reader, writer, (int)(reader.BaseStream.Length - reader.BaseStream.Position));
				this.stream = newZip;
			}
			return entries;
		}
		private static void CopyBytes(BinaryReader input, BinaryWriter output, int count)
		{
			for (int i = 0; i < count; i++)
			{
				output.Write(input.ReadByte());
			}
		}

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing,
		/// or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (stream != null)
				stream.Dispose();
		}

		#endregion
	}
}
