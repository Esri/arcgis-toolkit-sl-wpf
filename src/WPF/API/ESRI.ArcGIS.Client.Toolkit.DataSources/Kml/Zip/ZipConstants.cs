// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see https://opensource.org/licenses/ms-pl for details.
// All other rights reserved.

using System;

namespace ESRI.ArcGIS.Client.Toolkit.DataSources.Kml.Zip
{
  static class ZipConstants
  {      
    public const UInt32 PackedToRemovableMedia = 0x30304b50;
    public const UInt32 Zip64EndOfCentralDirectoryRecordSignature = 0x06064b50;
    public const UInt32 Zip64EndOfCentralDirectoryLocatorSignature = 0x07064b50;
    public const UInt32 EndOfCentralDirectorySignature = 0x06054b50;
    public const int ZipEntrySignature                 = 0x04034b50;
    public const int ZipEntryDataDescriptorSignature   = 0x08074b50;
    public const int ZipDirEntrySignature              = 0x02014b50;

      
    // These are dictated by the Zip Spec.See APPNOTE.txt
    public const int AesKeySize = 192;  // 128, 192, 256
    public const int AesBlockSize = 128;  // ???

    public const UInt16 AesAlgId128 = 0x660E; 
    public const UInt16 AesAlgId192 = 0x660F; 
    public const UInt16 AesAlgId256 = 0x6610; 

  }
}
