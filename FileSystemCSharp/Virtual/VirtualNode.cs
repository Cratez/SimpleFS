/* 
 * John Harrison
 * CST352 Assignment 3
 */
using System;
using System.Collections.Generic;

namespace FileSystemCSharp
{
    class VirtualNode
    {
        private VirtualDrive mDrive;
        private int mNodeSector;
        private NODE mSector;
        private VirtualNode mParent;
        private Dictionary<string, VirtualNode> mChildren;
        private List<VirtualBlock> mBlocks;

        public VirtualDrive Drive => mDrive;
        public string Name => mSector.Name;
        public VirtualNode Parent => mParent;
        public bool IsDirectory => mSector.Type == SECTOR.SectorType.DIR_NODE;
        public bool IsFile => mSector.Type == SECTOR.SectorType.FILE_NODE;
        public int FileLength => (mSector as FILE_NODE).FileSize;

        public VirtualNode(VirtualDrive drive, int nodeSector, NODE sector, VirtualNode parent)
        {
            mDrive = drive;
            mNodeSector = nodeSector;
            mSector = sector;
            mParent = parent;
            mChildren = null;
            mBlocks = null;
        }

        public void Rename(string name)
        {
            if (mParent == null)
                throw new Exception("Cannot rename the root node");

            if(mParent.mChildren != null && mParent.mChildren.Count > 0)
            {
                //only do this if we see we are there
                if (mParent.mChildren.ContainsKey(this.Name))
                {
                    //remove old
                    mParent.mChildren.Remove(this.Name);

                    //add new
                    mParent.mChildren[name] = this;
                }
                else
                {
                    throw new Exception("Node not referenced in parent");
                }

                //rename
                mSector.Name = name;
                mDrive.Disk.WriteSector(mNodeSector, mSector.RawBytes);
            }
        }

        public void Move(VirtualNode destination)
        {
            if(destination != null)
            {
                if(destination.mParent == null)
                {
                    throw new Exception("Can't move root node");
                }

                if(destination.mSector.Type == SECTOR.SectorType.DIR_NODE)
                {
                    //move this node to destination
                    destination.LoadChildren();
                    destination.mChildren[Name] = this;
                    destination.CommitChildren();

                    //remove this node from old
                    mParent.LoadChildren();
                    mParent.mChildren.Remove(Name);
                    mParent.CommitChildren();

                    //set new parent
                    this.mParent = destination;
                }
                else
                {
                    throw new Exception("Cannot move to not dir node");
                }
            }
            else
            {
                throw new Exception("Cannot move to null VirtualNode");
            }
           
        }

        public void Delete()
        {
            if(mParent == null)
            {
                throw new Exception("Cannot delete root node");
            }

            //we need to do cleanup if we are a dir
            if (IsDirectory)
            {
                LoadChildren();
                foreach(var child in mChildren)
                {
                    //recursive delete call to free
                    child.Value.Delete();
                }
            }

            mParent.LoadChildren();
            mParent.mChildren.Remove(this.Name);
            mParent.CommitChildren();

            //free sector to wipe with


            FREE_SECTOR freeSector = new FREE_SECTOR(mDrive.Disk.BytesPerSector);
            //start at first data
            int sectorIndex = mSector.FirstDataAt;
            while (sectorIndex != 0)
            {
                //store what will be next sector...
                int nextsector = 0;

                //get sector
                DATA_SECTOR currentSector = DATA_SECTOR.CreateFromBytes(mDrive.Disk.ReadSector(sectorIndex));
                nextsector = currentSector.NextSectorAt;

                //wipe sector
                mDrive.Disk.WriteSector(sectorIndex, freeSector.RawBytes);

                //point to next sector
                sectorIndex = nextsector;
            };

            //wipe main sector
            mDrive.Disk.WriteSector(mNodeSector, freeSector.RawBytes);
        }

        private void LoadChildren()
        {
            if (mChildren == null)
            {
                mChildren = new Dictionary<string, VirtualNode>();

                DATA_SECTOR dirData = DATA_SECTOR.CreateFromBytes(mDrive.Disk.ReadSector(mSector.FirstDataAt));
      
                int nChildren = (mSector as DIR_NODE).EntryCount;
                for (int i = 0; i < nChildren; i++)
                {
                    //fetch address
                    int childNodeAddress = BitConverter.ToInt32(dirData.DataBytes, i * 4);

                    //read in child
                    byte[] childRaw = Drive.Disk.ReadSector(childNodeAddress);
                    NODE childNode = NODE.CreateFromBytes(childRaw);

                    //store the node as virutal node child
                    VirtualNode virtualNode = new VirtualNode(mDrive, childNodeAddress, childNode, this);
                    mChildren[virtualNode.Name] = virtualNode;
                }
            }
        }

        private void CommitChildren()
        {

            DATA_SECTOR dirData = DATA_SECTOR.CreateFromBytes(mDrive.Disk.ReadSector(mSector.FirstDataAt));

            byte[] data = null;
            if (mChildren.Count > 0)
            {
                //get data for children
                data = new byte[mChildren.Count * 4];
                int byteIndex = 0;
                foreach (VirtualNode child in mChildren.Values)
                {
                    //copy child address
                    BitConverter.GetBytes(child.mNodeSector).CopyTo(data, byteIndex);
                    byteIndex += 4;
                }
            }

            //add to dirData, write back to disk
            dirData.DataBytes = data;
            mDrive.Disk.WriteSector(mSector.FirstDataAt, dirData.RawBytes);

            //write entry count to disk
            (mSector as DIR_NODE).EntryCount = mChildren.Count;
            mDrive.Disk.WriteSector(mNodeSector, mSector.RawBytes);
        }

        public VirtualNode CreateDirectoryNode(string name)
        {
            bool makeFile = false; //make dir
            return CommonMake(name, makeFile);
        }

        public VirtualNode CreateFileNode(string name)
        {
            bool makeFile = true;
            return CommonMake(name, makeFile);
        }

        private VirtualNode CommonMake(string name, bool makeFile = true )
        {
            if(mSector.Type != SECTOR.SectorType.DIR_NODE)
            {
                throw new Exception($"Cannot create Dir/File under node type " + mSector.Type.ToString());
            }
            LoadChildren();

            if (mChildren.ContainsKey(name))
            {
                throw new Exception("Name already in use!");
            }

            int[] nextFreeSectors = mDrive.GetNextFreeSectors(2);
            int nodeAddr = nextFreeSectors[0];
            int dataAddr = nextFreeSectors[1];

            NODE newNode = null;
            if (makeFile)
                newNode = new FILE_NODE(mDrive.Disk.BytesPerSector, dataAddr, name, 0);
            else
                newNode = new DIR_NODE(mDrive.Disk.BytesPerSector, dataAddr, name, 0);

            DATA_SECTOR dirdata = new DATA_SECTOR(mDrive.Disk.BytesPerSector, 0, null);

            mDrive.Disk.WriteSector(nodeAddr, newNode.RawBytes);
            mDrive.Disk.WriteSector(dataAddr, dirdata.RawBytes);

            VirtualNode child = new VirtualNode(mDrive, nodeAddr, newNode, this);
            mChildren[name] = child;
            CommitChildren();
            return child;
        }

        public IEnumerable<VirtualNode> GetChildren()
        {
            if (mSector.Type != SECTOR.SectorType.DIR_NODE)
            {
                throw new Exception("Cannot get children for node type " + mSector.Type.ToString());
            }

            LoadChildren();

            return mChildren.Values;
        }

        public VirtualNode GetChild(string name)
        {
            if (mSector.Type != SECTOR.SectorType.DIR_NODE)
            {
                throw new Exception("Cannot get children for node type " + mSector.Type.ToString());
            }

            LoadChildren();

            VirtualNode result;
            if (mChildren.ContainsKey(name))
            {
                result = mChildren[name];
            }
            else
            {
                result = null;
            }

            return result;
        }

        private void LoadBlocks()
        {
            if (mBlocks == null)
            {
                //make list
                mBlocks = new List<VirtualBlock>();

                //load all blocks into memory
                int currentSector = mSector.FirstDataAt;
                do
                {
                    DATA_SECTOR dataSector = DATA_SECTOR.CreateFromBytes(mDrive.Disk.ReadSector(currentSector));
                    mBlocks.Add(new VirtualBlock(mDrive, currentSector, dataSector, false));
                    currentSector = dataSector.NextSectorAt;
                }
                while (currentSector != 0);
            }
        }

        private void CommitBlocks()
        {
            //write out all blocks to memory
            foreach (VirtualBlock current in mBlocks)
            {
                current.CommitBlock();
            }
        }

        public byte[] Read(int index, int length)
        {
            if (mSector.Type != SECTOR.SectorType.FILE_NODE)
            {
                throw new Exception("Cannot read data to node type " + mSector.Type.ToString());
            }

            LoadBlocks();
            return VirtualBlock.ReadBlockData(mDrive, mBlocks, index, length);
        }

        public void Write(int index, byte[] data)
        {
            if (mSector.Type != SECTOR.SectorType.FILE_NODE)
            {
                throw new Exception("Cannot write data to node type " + mSector.Type.ToString());
            }

            //load blocks
            LoadBlocks();


            // extend file with more sectors if needed
            FILE_NODE fileSector = mSector as FILE_NODE;
            int fileFileLength = Math.Max(fileSector.FileSize, index + data.Length);
            VirtualBlock.ExtendBlocks(mDrive, mBlocks, fileSector.FileSize, fileFileLength);

            //write data to the blocks
            VirtualBlock.WriteBlockData(mDrive, mBlocks, index, data);

            //wrte out data
            CommitBlocks();

            // write out new file size to disk
            if (fileFileLength != fileSector.FileSize)
            {
                fileSector.FileSize = fileFileLength;
                Drive.Disk.WriteSector(mNodeSector, fileSector.RawBytes);
            }
        }
    }
}
