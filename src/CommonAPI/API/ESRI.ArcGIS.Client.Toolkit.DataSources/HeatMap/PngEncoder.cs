// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see https://opensource.org/licenses/ms-pl for details.
// All other rights reserved.

using System;
using System.IO;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources
{
	/// <summary>
	/// PngEncoder class courtesy of Joe Stegman and opmized by Nikola:
	/// http://blogs.msdn.com/nikola/archive/2009/03/04/silverlight-super-fast-dymanic-image-generation-code-revisited.aspx
	/// </summary>
	internal class PngEncoder
	{
		private const int _MAXBLOCK = 0xFFFF;
		private static byte[] _HEADER = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
		private static byte[] _IHDR = { (byte)'I', (byte)'H', (byte)'D', (byte)'R' };
		private static byte[] _GAMA = { (byte)'g', (byte)'A', (byte)'M', (byte)'A' };
		private static byte[] _IDAT = { (byte)'I', (byte)'D', (byte)'A', (byte)'T' };
		private static byte[] _IEND = { (byte)'I', (byte)'E', (byte)'N', (byte)'D' };

		public PngEncoder(int width, int height)
		{
			PrepareBuffer(width, height);
		}

		public Stream GetImageStream()
		{
			MemoryStream ms = new MemoryStream();
			ms.Write(_buffer, 0, _buffer.Length);
			ms.Seek(0, SeekOrigin.Begin);
			return ms;
		}

		public void SetPixelSlow(int col, int row, byte red, byte green, byte blue, byte alpha)
		{
			int start = _rowLength * row + col * 4 + 1;
			int blockNum = start / _blockSize;
			start += ((blockNum + 1) * 5);
			start += _dataStart;

			_buffer[start] = red;
			_buffer[start + 1] = green;
			_buffer[start + 2] = blue;
			_buffer[start + 3] = alpha;
		}

		public void SetPixelAtRowStart(int col, int rowStart, byte red, byte green, byte blue, byte alpha)
		{
			int start = rowStart + (col << 2);

			_buffer[start] = red;
			_buffer[start + 1] = green;
			_buffer[start + 2] = blue;
			_buffer[start + 3] = alpha;
		}

		public int GetRowStart(int row)
		{
			int start = _rowLength * row + 1;
			int blockNum = start / _blockSize;
			start += ((blockNum + 1) * 5);
			start += _dataStart;
			return start;
		}

		byte[] _buffer;
		int _rowLength;
		int _blockSize;
		int _dataStart;
		private void PrepareBuffer(int width, int height)
		{
			uint widthLength = (uint)(width * 4) + 1;
			_rowLength = (int)widthLength;
			uint dcSize = widthLength * (uint)height;

			uint rowsPerBlock = _MAXBLOCK / widthLength;
			uint blockSize = rowsPerBlock * widthLength;
			_blockSize = (int)blockSize;
			uint blockCount;
			ushort length;
			uint remainder = dcSize;

			if ((dcSize % blockSize) == 0)
			{
				blockCount = dcSize / blockSize;
			}
			else
			{
				blockCount = (dcSize / blockSize) + 1;
			}

			uint totalSize = 51 + (dcSize + 12 + 4 + blockCount * 5) + 12; // header, (data), end

			_buffer = new byte[totalSize];

			int currIndex = 0;

			// ********************************
			// ******* write png header ******* 
			_HEADER.CopyTo(_buffer, (int)currIndex);
			currIndex += _HEADER.Length;

			// ********************************
			// ******* Write IHDR ******* 
			//  Width:              4 bytes
			//  Height:             4 bytes
			//  Bit depth:          1 byte
			//  Color type:         1 byte
			//  Compression method: 1 byte
			//  Filter method:      1 byte
			//  Interlace method:   1 byte

			byte[] size;
			size = BitConverter.GetBytes((uint)13); // sizeof(data(IHDR))
			_buffer[currIndex] = size[3]; currIndex++;
			_buffer[currIndex] = size[2]; currIndex++;
			_buffer[currIndex] = size[1]; currIndex++;
			_buffer[currIndex] = size[0]; currIndex++;

			// "IHDR"
			_IHDR.CopyTo(_buffer, (int)currIndex);
			currIndex += _IHDR.Length;

			size = BitConverter.GetBytes(width);
			_buffer[currIndex] = size[3]; currIndex++;
			_buffer[currIndex] = size[2]; currIndex++;
			_buffer[currIndex] = size[1]; currIndex++;
			_buffer[currIndex] = size[0]; currIndex++;

			size = BitConverter.GetBytes(height);
			_buffer[currIndex] = size[3]; currIndex++;
			_buffer[currIndex] = size[2]; currIndex++;
			_buffer[currIndex] = size[1]; currIndex++;
			_buffer[currIndex] = size[0]; currIndex++;

			_buffer[currIndex] = 8; currIndex++; // 8 bits
			_buffer[currIndex] = 6; currIndex++; // RGBA format
			currIndex += 3; // skip to end of IHDR

			currIndex += 4; // skip CRC, assume 0

			// ********************************
			// ******* Write gAMA chunk ******* 
			size = BitConverter.GetBytes((uint)4); // sizeof(data(gAMA))
			_buffer[currIndex] = size[3]; currIndex++;
			_buffer[currIndex] = size[2]; currIndex++;
			_buffer[currIndex] = size[1]; currIndex++;
			_buffer[currIndex] = size[0]; currIndex++;

			// "GAMA"
			_GAMA.CopyTo(_buffer, (int)currIndex);
			currIndex += _GAMA.Length;

			// Set gamma = 1
			size = BitConverter.GetBytes(1 * 100000);
			_buffer[currIndex] = size[3]; currIndex++;
			_buffer[currIndex] = size[2]; currIndex++;
			_buffer[currIndex] = size[1]; currIndex++;
			_buffer[currIndex] = size[0]; currIndex++;

			currIndex += 4; // skip CRC, assume 0

			// ***************************************
			// ******* Write IDAT (data) chunk ******* 
			size = BitConverter.GetBytes(dcSize + 2 + 4 + blockCount * 5); // image data size + 2 bytes for compression header + 4 bytes for adler checksum + blocks overhead
			_buffer[currIndex] = size[3]; currIndex++;
			_buffer[currIndex] = size[2]; currIndex++;
			_buffer[currIndex] = size[1]; currIndex++;
			_buffer[currIndex] = size[0]; currIndex++;

			// "IDAT"
			_IDAT.CopyTo(_buffer, (int)currIndex);
			currIndex += _IDAT.Length;

			// write compression header
			_buffer[currIndex] = 0x78; currIndex++;
			_buffer[currIndex] = 0xDA; currIndex++;

			_dataStart = currIndex;

			// write image data
			//currIndex += (int)dcSize; // !!!
			for (uint blocks = 0; blocks < blockCount; blocks++)
			{
				// Write LEN
				length = (ushort)((remainder < blockSize) ? remainder : blockSize);

				if (length == remainder)
				{
					_buffer[currIndex] = 1;
				}
				else
				{
					_buffer[currIndex] = 0;

				}
				currIndex++;

				size = BitConverter.GetBytes(length);
				_buffer[currIndex] = size[0]; currIndex++;
				_buffer[currIndex] = size[1]; currIndex++;

				// Write one's compliment of LEN
				size = BitConverter.GetBytes((ushort)~length);
				_buffer[currIndex] = size[0]; currIndex++;
				_buffer[currIndex] = size[1]; currIndex++;

				// Write blocks
				//for (int i = currIndex; i < currIndex + length; i++)
				//{
				//    _buffer[i] = 200;
				//}
				currIndex += length;

				// Next block
				remainder -= blockSize;
			}

			currIndex += 4; // skip adler32 checksum, assume 0
			currIndex += 4; // skip CRC, assume 0

			// ********************************
			// ******* Write IEND chunk ******* 
			currIndex += 4; // sizeof(data(IEND)) is 0

			// "IEND"
			_IEND.CopyTo(_buffer, (int)currIndex);
			currIndex += _IEND.Length;
			_buffer[currIndex] = 81; currIndex++; // CRC
			_buffer[currIndex] = 189; currIndex++; // CRC
			_buffer[currIndex] = 159; currIndex++; // CRC
			_buffer[currIndex] = 125; currIndex++; // CRC

		}
	}
}
