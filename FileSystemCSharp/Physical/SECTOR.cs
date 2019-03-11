/* 
 * John Harrison
 * CST352 Assignment 3
 */
using System;

namespace FileSystemCSharp
{
    abstract class SECTOR
    {
        public enum SectorType : byte { FREE_SECTOR, DRIVE_INFO, DIR_NODE, FILE_NODE, DATA_SECTOR }

        private int mBytesPerSector;
        private int mNextSectorAt;
        private SectorType mType;     
        protected byte[] mRaw;

        public int BytesPerSector => mBytesPerSector;
        public SectorType Type => mType;
        public byte[] RawBytes => mRaw;

        public int NextSectorAt
        {
            get
            {
                return mNextSectorAt;
            }
            set
            {
                mNextSectorAt = value;
                BitConverter.GetBytes(mNextSectorAt).CopyTo(mRaw, Constants.NEXT_SECTOR_AT);
            }
        }

        protected SECTOR(int bytesPerSector, SectorType type, int nextSectorAt)
        {
            mRaw = new byte[bytesPerSector];

            mBytesPerSector = bytesPerSector;
            mType = type;      
            mRaw[Constants.TYPE_AT] = (byte)type;
            NextSectorAt = nextSectorAt;
        }

        protected SECTOR(byte[] raw)
        {
            mRaw = raw;
            mBytesPerSector = raw.Length;        
            mType = (SectorType)raw[Constants.TYPE_AT];
            mNextSectorAt = BitConverter.ToInt32(raw, Constants.NEXT_SECTOR_AT);
        }

        public static SectorType GetTypeFromBytes(byte[] raw)
        {
            return (SectorType)raw[Constants.TYPE_AT];
        }
    }
}
