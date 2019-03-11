/* 
 * John Harrison
 * CST352 Assignment 3
 * 
 * Yeah. This was REALLY confusing. I pretty much just wrote what you wrote. Thanks for the videos.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileSystemCSharp
{
    class VirtualBlock
    {
        private VirtualDrive drive;
        private DATA_SECTOR sector;
        private int sectorAddress;
        private bool dirty;

        public int SectorAddress => sectorAddress;
        public DATA_SECTOR Sector => sector;
        public bool Dirty => dirty;

        public byte[] Data
        {
            get
            {
                return (byte[])sector.DataBytes.Clone();
            }
            set
            {
                sector.DataBytes = value;
                dirty = true;
            }
        }

        public VirtualBlock(VirtualDrive drive, int sectorAddress, DATA_SECTOR sector, bool dirty = false)
        {
            this.drive = drive;
            this.sector = sector;
            this.sectorAddress = sectorAddress;
            this.dirty = dirty;
        }

        public void CommitBlock()
        {
            if (dirty)
            {
                drive.Disk.WriteSector(sectorAddress, sector.RawBytes);
                dirty = false;
            }
        }

        public static byte[] ReadBlockData(VirtualDrive drive, List<VirtualBlock> blocks, int startIndex, int length)
        {
            int blockSize = drive.BytesPerDataSector;

            if (startIndex + length > blocks.Count * blockSize)
            {
                throw new Exception("Not enough blocks to read data");
            }

            byte[] resultData = new byte[length];

            //start and end block
            int startBlock = startIndex / blockSize;
            int endBlock = (startIndex + length) / blockSize;


            if (startBlock == endBlock)
            {
                byte[] dataBytes = blocks[startBlock].sector.DataBytes;
                int fromStart = startIndex % blockSize;

                CopyBytes(length, dataBytes, fromStart, resultData, 0);
            }
            else
            {
                int resultNdx = 0;

                byte[] currentSectorBytes = blocks[startBlock].sector.DataBytes;
                CopyBytes(blockSize - startIndex, currentSectorBytes, startIndex % blockSize, resultData, resultNdx);
                resultNdx += blockSize - startIndex;

                for (int currentBlock = startBlock + 1; currentBlock < endBlock; currentBlock++)
                {
                    currentSectorBytes = blocks[currentBlock].sector.DataBytes;
                    CopyBytes(blockSize, currentSectorBytes, 0, resultData, resultNdx);
                    resultNdx += blockSize;
                }

                currentSectorBytes = blocks[endBlock].sector.DataBytes;
                CopyBytes((startIndex + length) % blockSize, currentSectorBytes, 0, resultData, resultNdx);
            }
            return resultData;
        }

        public static void WriteBlockData(VirtualDrive drive, List<VirtualBlock> blocks, int startIndex, byte[] data)
        {
            int blockSize = drive.BytesPerDataSector;

            if (startIndex + data.Length > blocks.Count * blockSize)
                throw new Exception("Not enough blocks to write data");

            //start and end block
            int startBlock = startIndex / blockSize;
            int endBlock = (startIndex + data.Length) / blockSize;
 
            //just one block?
            if (startBlock == endBlock)
            {
                byte[] currentSectorBytes = blocks[startBlock].Data;
                int toStart = startIndex % blockSize;
                CopyBytes(data.Length, data, 0, currentSectorBytes, toStart);
                blocks[startBlock].Data = currentSectorBytes;
            }
            else
            {
                int resultIndex = 0;

                //first
                byte[] currentSectorBytes = blocks[startBlock].Data;
                CopyBytes(blockSize - startIndex, data, resultIndex, currentSectorBytes, startIndex % blockSize);
                blocks[startBlock].Data = currentSectorBytes;
                resultIndex += blockSize - startIndex;

                //middle blocks
                for (int i = startBlock + 1; i < endBlock; i++)
                {
                    currentSectorBytes = blocks[i].Data;
                    CopyBytes(blockSize, data, resultIndex, currentSectorBytes, 0);
                    blocks[i].Data = currentSectorBytes;
                    resultIndex += blockSize;
                }

                //last block
                currentSectorBytes = blocks[endBlock].Data;
                CopyBytes((startIndex + data.Length) % blockSize, data, resultIndex, currentSectorBytes, 0);
                blocks[endBlock].Data = currentSectorBytes;
            }
        }

        public static void ExtendBlocks(VirtualDrive drive, List<VirtualBlock> blocks, int initialFileLength, int finalFileLength)
        {
            int initialSectorcount = BlocksNeeded(drive, initialFileLength);
            int finalSectorcount = BlocksNeeded(drive, finalFileLength);

            if (finalSectorcount > initialSectorcount)
            {
                int[] freeSectors = drive.GetNextFreeSectors(finalSectorcount - initialSectorcount);


                DATA_SECTOR dataSector = blocks.Last().Sector;
                foreach (int freeSector in freeSectors)
                {
                    int currentSector = dataSector.NextSectorAt = freeSector;
                    dataSector = new DATA_SECTOR(drive.Disk.BytesPerSector, 0, null);
                    blocks.Add(new VirtualBlock(drive, currentSector, dataSector, true));
                }               
            }
        }

        private static int BlocksNeeded(VirtualDrive drive, int numBytes)
        {
            return Math.Max(1, (int)Math.Ceiling((double)numBytes / (double)drive.BytesPerDataSector));
        }

        private static void CopyBytes(int copyCount, byte[] from, int fromStart, byte[] to, int toStart)
        {
            for (int i = 0; i < copyCount; i++)
            {
                to[toStart + i] = from[fromStart + i];
            }
        }
    }
}
