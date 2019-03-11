/* 
 * John Harrison
 * CST352 Assignment 3
 */

namespace FileSystemCSharp
{
        public interface DiskDriver
        {
            // unique serial number for this disk
            int SerialNumber { get; }

            // storage properties of disk
            int SectorCount { get; }            // Logical Block Access (LBA) addressing
            int BytesPerSector { get; }         // expect 256
            int AvgLatency { get; }             // average latency for read/write operations in msec

            // status of disk
            void TurnOn();
            void TurnOff();
            bool Ready { get; }

            // read and write a single sector
            // byte[] must be exactly BytesPerSector long
            // uses LBA addressing
            byte[] ReadSector(int lba);
            void WriteSector(int lba, byte[] data);
        }
   }
