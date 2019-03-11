/* 
 * John Harrison
 * CST352 Assignment 3
 */
using System;

namespace FileSystemCSharp
{
    class DIR_NODE : NODE
    {
        private int mEntryCount;

        public int EntryCount
        {
            get { return mEntryCount; }
            set
            {
                mEntryCount = value;
                BitConverter.GetBytes(mEntryCount).CopyTo(mRaw, Constants.ENTRY_COUNT_AT);
            }
        }

        public DIR_NODE(int bytesPerSector, int firstDataAt, string name, int entryCount) : 
            base(bytesPerSector, SectorType.DIR_NODE, firstDataAt, name)
        {
            EntryCount = entryCount;
        }

        private DIR_NODE(byte[] raw) : base(raw)
        {
            mEntryCount = BitConverter.ToInt32(raw, Constants.ENTRY_COUNT_AT);
        }

        public new static DIR_NODE CreateFromBytes(byte[] raw)
        {
            if (GetTypeFromBytes(raw) != SectorType.DIR_NODE)
            {
                throw new Exception("Expected a DIR_NODE!");
            }
            return new DIR_NODE(raw);
        }
    }
}
