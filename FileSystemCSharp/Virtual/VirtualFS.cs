/* 
 * John Harrison
 * CST352 Assignment 3
 */

using System;
using System.Collections.Generic;

namespace FileSystemCSharp
{
    class VirtualFS
    {  
        private Dictionary<string, VirtualDrive> mDrives;
        private VirtualNode mRootNode;

        public VirtualNode RootNode => mRootNode;

        public VirtualFS()
        {
            mDrives = new Dictionary<string, VirtualDrive>();
            mRootNode = null;
        }

        public void Format(DiskDriver disk)
        {
            //wipe the disk
            FREE_SECTOR freeSector = new FREE_SECTOR(disk.BytesPerSector);
            for (int i = 0; i < disk.SectorCount; i++)
            {
                disk.WriteSector(i, freeSector.RawBytes);
            }

            //format it
            DRIVE_INFO driveInfo = new DRIVE_INFO(disk.BytesPerSector, Constants.ROOT_DIR_SECTOR);
            disk.WriteSector(Constants.DRIVE_INFO_SECTOR, driveInfo.RawBytes);

            
            DIR_NODE rootNode = new DIR_NODE(disk.BytesPerSector, Constants.ROOT_DATA_SECTOR, "/", 0);
            disk.WriteSector(Constants.ROOT_DIR_SECTOR, rootNode.RawBytes);


            DATA_SECTOR rootData = new DATA_SECTOR(disk.BytesPerSector, 0, new byte[1]);
            disk.WriteSector(Constants.ROOT_DATA_SECTOR, rootData.RawBytes);
        }

        public void Mount(DiskDriver disk, string mountPoint)
        {
            if (mDrives.Count == 0 && mountPoint.Length != 1 && mountPoint[0] != '/')
            {
                throw new Exception("First disk must be mounted at the root");
            }

            //add virtual drive
            DRIVE_INFO driveInfo = DRIVE_INFO.CreateFromBytes(disk.ReadSector(Constants.DRIVE_INFO_SECTOR));
            VirtualDrive newDrive = new VirtualDrive(disk, Constants.DRIVE_INFO_SECTOR, driveInfo);
            mDrives.Add(mountPoint, newDrive);

            //add root dir, make node, add it
            int rootNodeAt = driveInfo.RootNodeAt;
            DIR_NODE newDriveRootDir = DIR_NODE.CreateFromBytes(disk.ReadSector(rootNodeAt));
            VirtualNode virtualNode = new VirtualNode(newDrive, rootNodeAt, newDriveRootDir, null);

            if (mDrives.Count == 1)
            {
                mRootNode = virtualNode;
            }
        }

        public void Unmount(string mountPoint)
        {
            if (!mDrives.ContainsKey(mountPoint))
            {
                throw new Exception("No drive mounted at mountpoint " + mountPoint);
            }

            VirtualDrive virtualDrive = mDrives[mountPoint];
            if (mRootNode.Drive == virtualDrive)
            {
                mRootNode = null;
            }
            mDrives.Remove(mountPoint);
        }
    }
}
