/* 
 * John Harrison
 * CST352 Assignment 3
 */
using System;

namespace FileSystemCSharp
{
    class FILE_NODE : NODE
    {
        private int mFileSize;

        public int FileSize
        {
            get
            {
                return mFileSize;
            }
            set
            {
                mFileSize = value;
                BitConverter.GetBytes(mFileSize).CopyTo(mRaw, Constants.FILE_SIZE_AT);
            }
        }

        public FILE_NODE(int bytesPerSector, int firstDataAt, string name, int fileSize) : base(bytesPerSector, SectorType.FILE_NODE, firstDataAt, name)
        {
            FileSize = fileSize;
        }

        private FILE_NODE(byte[] raw) : base(raw)
        {
            mFileSize = BitConverter.ToInt32(raw, Constants.FILE_SIZE_AT);
        }

        public new static FILE_NODE CreateFromBytes(byte[] raw)
        {
            if (GetTypeFromBytes(raw) != SectorType.FILE_NODE)
            {
                throw new Exception("Expected a FILE_NODE!");
            }
            return new FILE_NODE(raw);
        }
    }
}
