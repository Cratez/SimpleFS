/* 
 * John Harrison
 * CST352 Assignment 3
 */
using System;

namespace FileSystemCSharp
{
    class FREE_SECTOR : SECTOR
    {  
        private FREE_SECTOR(byte[] raw) : base(raw){}

        public FREE_SECTOR(int bytesPerSector) : base(bytesPerSector, SectorType.FREE_SECTOR, 0) { }

        public static FREE_SECTOR CreateFromBytes(byte[] raw)
        {
            //this feels like such a cheap way to check this...
            if (GetTypeFromBytes(raw) > SectorType.FREE_SECTOR)
            {
                throw new Exception("Expected a FREE_SECTOR!");
            }

            return new FREE_SECTOR(raw);
        }
    }
}
