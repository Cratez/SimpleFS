/* 
 * John Harrison
 * CST352 Assignment 3
 */
using System;

namespace FileSystemCSharp
{
    public class SlowDisk : DiskDriver
    {
        #region private
        private const int BYTES_PER_SECTOR = 256;
        private const int SECTOR_COUNT = 1024;
        private const int AVERAGE_LATENCY = 100;

        private bool mPowerOn;
        private int mSerialNumber;
        private DiskSector[] mSectors;

        private class DiskSector
        {
            public int LBA;

            public byte[] Data;

            public DiskSector(int lba, int length)
            {
                LBA = lba;
                Data = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    Data[i] = 255;
                }
            }
        }

        private void CheckReady()
        {
            if (!mPowerOn)
            {
                throw new Exception("Disk currently powered off");
            }
        }

        private void CheckLBA(int lba)
        {
            if (lba < 0 || lba >= SECTOR_COUNT)
            {
                throw new Exception("LBA " + lba.ToString() + " invalid, expected less than " + SECTOR_COUNT.ToString());
            }
        }

        private void CheckSectorLength(int len)
        {
            if (len != BYTES_PER_SECTOR)
            {
                throw new Exception("Invalid sector size " + len.ToString() + ", expected " + BYTES_PER_SECTOR.ToString());
            }
        }
        private void SimulateLatency(int lba)
        {
            //todo, probably never
        }
        #endregion

        // getters
        public int SerialNumber => mSerialNumber;
        public int BytesPerSector => BYTES_PER_SECTOR;
        public int SectorCount => SECTOR_COUNT;
        public int AvgLatency => AVERAGE_LATENCY;
        public bool Ready => mPowerOn;

         
        public SlowDisk(int serialnumber)
        {
            mSerialNumber = serialnumber;
            mPowerOn = false;
            mSectors = new DiskSector[SECTOR_COUNT];
            for (int i = 0; i < SECTOR_COUNT; i++)
            {
                mSectors[i] = new DiskSector(i, BYTES_PER_SECTOR);
            }
        }

        public void TurnOn()
        {
            mPowerOn = true;
        }

        public void TurnOff()
        {
            mPowerOn = false;
        }

        public byte[] ReadSector(int lba)
        {
            CheckReady();
            CheckLBA(lba);

            //todo?
            SimulateLatency(lba);

            //return data
            return mSectors[lba].Data;
        }

        public void WriteSector(int lba, byte[] data)
        {
            CheckReady();
            CheckLBA(lba);
            CheckSectorLength(data.Length);

            //todo?
            SimulateLatency(lba);

            //write data to sector
            mSectors[lba].Data = data;
        }

    }
}
