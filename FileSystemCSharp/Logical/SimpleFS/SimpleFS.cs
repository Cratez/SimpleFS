/* 
 * John Harrison
 * CST352 Assignment 3
 */

using System.Linq;

namespace FileSystemCSharp
{
    public class SimpleFS : FileSystem
    {
        private VirtualFS mVirtualFileSystem;

        public char PathSeparator => Constants.PATH_SEPARATOR;
        public int MaxNameLength => Constants.MAX_FILE_NAME;

        public SimpleFS()
        {
            mVirtualFileSystem = new VirtualFS();
        }

        public void Mount(DiskDriver disk, string mountPoint)
        {
            mVirtualFileSystem.Mount(disk, mountPoint);
        }

        public void Unmount(string mountPoint)
        {
            mVirtualFileSystem.Unmount(mountPoint);
        }

        public void Format(DiskDriver disk)
        {
            mVirtualFileSystem.Format(disk);
        }

        public Directory GetRootDirectory()
        {
            return new SimpleDirectory(mVirtualFileSystem.RootNode);
        }

        public FSEntry Find(string path)
        {
            //parse the path
            string[] source = path.TrimEnd(new char[] { PathSeparator }).Split(new char[] { PathSeparator });

            //get root node
            VirtualNode matchNode = mVirtualFileSystem.RootNode;
      
            //loop over each path element searching for it in each virtualnode. Return null if not found at one step.
            foreach (string current in source.Skip(1))
            {
                matchNode = matchNode.GetChild(current);
                if (matchNode == null)
                {
                    //doesnt exist
                    return null;
                }
            }

            FSEntry result;
            if (matchNode.IsDirectory)
                result = new SimpleDirectory(matchNode);
            else
                result = new SimpleFile(matchNode);

            return result;
        }
    }
}
