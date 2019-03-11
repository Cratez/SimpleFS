/* 
 * John Harrison
 * CST352 Assignment 3
 */
using System;
using System.Linq;

namespace FileSystemCSharp
{
    class DATA_SECTOR : SECTOR
    {
        private DATA_SECTOR(byte[] raw) : base(raw)
        {
        }

        public byte[] DataBytes
        {
            get
            {
                return mRaw.Skip(Constants.SECTOR_DATA_LEN).ToArray();
            }
            set
            {
                if (value != null)
                {
                    if (value.Length > MaxDataLength(BytesPerSector))
                        throw new Exception("Too much ddata for sector");

                    value.CopyTo(mRaw, Constants.SECTOR_DATA_LEN);
                }
                else
                {
                    Array.Clear(mRaw, Constants.SECTOR_DATA_LEN, MaxDataLength(BytesPerSector));
                }
            }
        }

        public static int MaxDataLength(int bytesPerSector)
        {
            return bytesPerSector - Constants.SECTOR_DATA_LEN;
        }

        public DATA_SECTOR(int bytesPerSector, int nextDataAt, byte[] data) : base(bytesPerSector, SectorType.DATA_SECTOR, nextDataAt)
        {
            DataBytes = data;
        }
  
        public static DATA_SECTOR CreateFromBytes(byte[] raw)
        {
            if (GetTypeFromBytes(raw) != SectorType.DATA_SECTOR)
            {
                throw new Exception("Expected a DATA_SECTOR!");
            }
            return new DATA_SECTOR(raw);
        }
    }
}
