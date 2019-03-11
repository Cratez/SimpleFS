/* 
 * John Harrison
 * CST352 Assignment 3
 */
using System;

namespace FileSystemCSharp
{
    class VirtualDrive
    {
        private DiskDriver mDisk;
        private DRIVE_INFO mSector;
        private int mBytesPerDataSector;     
        private int mDriveInfoSector;

        public DiskDriver Disk
        {
            get
            {
                return mDisk;
            }
        }

        public int BytesPerDataSector
        {
            get
            {
                return mBytesPerDataSector;
            }
        }

        public VirtualDrive(DiskDriver disk, int driveInfoSector, DRIVE_INFO sector)
        {
            mDisk = disk;
            mDriveInfoSector = driveInfoSector;
            mBytesPerDataSector = DATA_SECTOR.MaxDataLength(disk.BytesPerSector);
            mSector = sector;
        }

        public int[] GetNextFreeSectors(int count)
        {
            //array for our free sectors...
            int[] freeSectors = new int[count];

            //search fof free sectors...
            for (int i = 0; i < mDisk.SectorCount; i++)
            {
                byte[] raw = mDisk.ReadSector(i);
                if (SECTOR.GetTypeFromBytes(raw) == SECTOR.SectorType.FREE_SECTOR)
                {
                    //add the free sector
                    freeSectors[--count] = i;

                    //enough?
                    if (count <= 0)
                        return freeSectors;
                }
            }
            throw new Exception("Disk is full!");
        }
    }
}
