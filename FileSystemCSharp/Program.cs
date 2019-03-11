/* Student: John Harrison
 * class :  CST352 Operating Systems
 * instructor: Pete Myers
 * 
 * NOTE: I started this assignment with C++. Which I is included in the zip with the progress I made.
 * Ultimately I found the work I had done, apart from really watching the videos was proving difficult.
 * Instead of trying to suffer through mapping the work you did to my c++ code to finish the assignment, I decided to just write along with the work you did
 * on a CSharp project instead. Since time is running short. It was sooooooooooooooooooooo much easier. Thank you so much for the video/walkthrough. Probably wouldnt have
 * complete the assignment without that help.
 */

using System;

namespace FileSystemCSharp
{
    class Program
    {

        static void TestfileSystem()
        {
            try
            {
                Random r = new Random();
                SlowDisk disk = new SlowDisk(1);
                disk.TurnOn();

                FileSystem fs = new SimpleFS();
                fs.Format(disk);
                fs.Mount(disk, "/");

                Directory root = fs.GetRootDirectory();
                root.CreateDirectory("Testing123");

                Directory dir = root.CreateDirectory("nested");

                File file = dir.CreateFile("nestedfile");

                FileStream filestream = file.Open();
                filestream.Write(0, CreateTestBytes(r, 142));
                filestream.Close();

                root.CreateFile("rootfile");

                fs.Find("/rootfile");
                FSEntry f = fs.Find("/nested/nestedfile");
                string fullname = f.FullPathName;

                RescursivelyPrintDirectories(root);

                fs.Unmount("/");
                disk.TurnOff();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Test File System failed. Ex: " + ex.Message);
            }
        }
        static void RescursivelyPrintDirectories(Directory dir, string indent = "")
        {           
            Console.WriteLine(indent + $"{dir.Name} <Dir> ");

            foreach (File file in dir.GetFiles())
            {
                Console.WriteLine(indent + "  " + $"{file.Name} <File> ");
            }
            foreach(Directory d in dir.GetSubDirectories())
            {
                RescursivelyPrintDirectories(d, indent + "  ");
            }
        }

        static void TestVirtualFileSystem()
        {
            try
            {
                Random r = new Random();

                SlowDisk disk = new SlowDisk(1);
                disk.TurnOn();

                VirtualFS vfs = new VirtualFS();

                vfs.Format(disk);
                vfs.Mount(disk, "/");
                VirtualNode root = vfs.RootNode;

                VirtualNode dir1 = root.CreateDirectoryNode("dir1");
                VirtualNode dir2 = root.CreateDirectoryNode("dir2");

                VirtualNode file1 = dir1.CreateFileNode("file1");
                TestFileWriteRead(file1, r, 0, 100);
                TestFileWriteRead(file1, r, 0, 500);
                TestFileWriteRead(file1, r, 250, 100);

                vfs.Unmount("/");

                vfs.Mount(disk,"/");
                RescursivelyPrintNodes(vfs.RootNode);

                disk.TurnOff();
            }
            catch(Exception ex)
            {
                Console.WriteLine("VFS test failed: " + ex.Message);
            }
        }

        static void TestPhysicalFileSystem()
        {
            SlowDisk disk = new SlowDisk(1);
            disk.TurnOn();

            int SECTOR_SIZE = disk.BytesPerSector;

            //free sector
            { 
                FREE_SECTOR freeWrite = new FREE_SECTOR(disk.BytesPerSector);
                disk.WriteSector(0, freeWrite.RawBytes);
                FREE_SECTOR freeRead = FREE_SECTOR.CreateFromBytes(disk.ReadSector(0));
                CheckSectorBytes(freeWrite, freeRead);
            }

            //drive info
            { 
                DRIVE_INFO driveWrite = new DRIVE_INFO(disk.BytesPerSector,101);
                disk.WriteSector(1, driveWrite.RawBytes);
                DRIVE_INFO driveRead = DRIVE_INFO.CreateFromBytes(disk.ReadSector(1));
                CheckSectorBytes(driveWrite, driveRead);               
            }

            //dir node
            {
                DIR_NODE dirWrite = new DIR_NODE(disk.BytesPerSector, 103, "dir1", 10);
                disk.WriteSector(2, dirWrite.RawBytes);
                DIR_NODE dirRead = DIR_NODE.CreateFromBytes(disk.ReadSector(2));
                CheckSectorBytes(dirWrite, dirRead);
            }

            //file node
            {
                FILE_NODE fileWrite = new FILE_NODE(disk.BytesPerSector, 104, "file1", 100);
                disk.WriteSector(3, fileWrite.RawBytes);
                FILE_NODE fileRead = FILE_NODE.CreateFromBytes(disk.ReadSector(3));
                CheckSectorBytes(fileWrite, fileRead);
            }

            //data sector
            {
                byte[] testData = new byte[DATA_SECTOR.MaxDataLength(disk.BytesPerSector)];
                for(int i = 0; i < testData.Length; i++)
                {
                    testData[i] = (byte)(i + 1);
                }

                DATA_SECTOR dataWrite = new DATA_SECTOR(disk.BytesPerSector, 105, testData);
                disk.WriteSector(4, dataWrite.RawBytes);
                DATA_SECTOR dataRead = DATA_SECTOR.CreateFromBytes(disk.ReadSector(4));
                CheckSectorBytes(dataWrite, dataRead);
            }

            disk.TurnOff();
        }

        static void CheckSectorBytes(SECTOR lhs, SECTOR rhs)
        {
            if (!Compare(lhs.RawBytes, rhs.RawBytes))
                Console.WriteLine($"{lhs.Type.ToString()} sectors are not equal!");
            else
                Console.WriteLine($"{lhs.Type.ToString()} sectors are equal!");
        }

        private static void RescursivelyPrintNodes(VirtualNode node, string indent = "")
        {
            Console.Write(indent + node.Name);
            if (node.IsFile)
            {
                Console.WriteLine(" <file, len=" + node.FileLength.ToString() + ">");
            }
            else if (node.IsDirectory)
            {
                Console.WriteLine(" <directory>");
                foreach(VirtualNode child in node.GetChildren())
                {
                    RescursivelyPrintNodes(child, indent + "  ");
                }
            }
        }

        private static void TestFileWriteRead(VirtualNode file, Random r, int index, int length)
        {
            byte[] towrite = CreateTestBytes(r, length);
            file.Write(index, towrite);
            byte[] toread = file.Read(index, length);
            if (!Compare(towrite, toread))
                throw new Exception("File read/write at " + index + " for " + length + " bytes, failed for file " + file.Name);
        }

        private static bool Compare(byte[] data1, byte[] data2)
        {
            if (data1.Length != data2.Length)
                return false;

            for(int i = 0; i < data1.Length; i++)
            {
                if (data1[i] != data2[i])
                    return false;
            }

            return true;
        }

        private static byte[] CreateTestBytes(Random r, int length)
        {
            byte[] result = new byte[length];
            r.NextBytes(result);
            return result;
        }

        static void CheckBytes(string name1, SECTOR s1, string name2, SECTOR s2)
        {
            if (!Compare(s1.RawBytes, s2.RawBytes))
                throw new Exception($"Sectors {name1} and {name2} are not equal");
        }

        static void TestSeveralSectors()
        {
            SlowDisk disk = new SlowDisk(1);
            disk.TurnOn();

            byte[] testdata = new byte[disk.BytesPerSector];

            for(int i = 0; i < disk.BytesPerSector; i++)
            {
                testdata[i] = (byte)(i % 256);
            }

            TestSector(disk, 0, testdata);
            TestSector(disk, 1, testdata);
            TestSector(disk, disk.SectorCount - 1, testdata);
        }

        static bool TestSector(DiskDriver disk, int lba, byte[] testdata)
        {
            disk.WriteSector(lba, testdata);
            byte[] s = disk.ReadSector(lba);
            bool success = Compare(testdata, s);
            Console.WriteLine("Compare " + success.ToString());

            return success;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("**Test Several Test**");
            TestSeveralSectors();

            Console.WriteLine("\n**Test Physical File System Test**");
            TestPhysicalFileSystem();

            Console.WriteLine("\n**Test Virtual File System Test**");
            TestVirtualFileSystem();

            Console.WriteLine("\n**Test File System Test**");
            TestfileSystem();
        }
    }
}
