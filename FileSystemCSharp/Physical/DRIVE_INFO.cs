/* 
 * John Harrison
 * CST352 Assignment 3
 */
using System;

namespace FileSystemCSharp
{
    class DRIVE_INFO : SECTOR
    {
        public int RootNodeAt => NextSectorAt;

        private DRIVE_INFO(byte[] raw) : base(raw)
        {
        }

        public DRIVE_INFO(int bytesPerSector, int rootNodeAt) : base(bytesPerSector, SectorType.DRIVE_INFO, rootNodeAt)
        {
        }
   
        public static DRIVE_INFO CreateFromBytes(byte[] raw)
        {
            if (GetTypeFromBytes(raw) != SectorType.DRIVE_INFO)
            {
                throw new Exception("Expected a DRIVE_INFO!");
            }

            return new DRIVE_INFO(raw);
        }
    }
}
